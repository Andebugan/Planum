using Planum.Logger;
using Planum.Model;
using Planum.Model.Entities;
using Planum.Parser;

namespace Planum.Repository
{
    public class TaskMarkdownReader
    {
        ModelConfig ModelConfig { get; set; }
        ILoggerWrapper Logger { get; set; }

        public TaskMarkdownReader(ILoggerWrapper logger, ModelConfig modelConfig)
        {
            ModelConfig = modelConfig;
            Logger = logger;
        }

        public bool CheckTaskMarker(string line) => line.StartsWith(ModelConfig.TaskMarkerStartSymbol) && line.EndsWith(ModelConfig.TaskMarkerEndSymbol);
        public Guid ReadTask(ref IEnumerator<string> enumerator, ref List<PlanumTaskDTO> tasks)
        {
            Logger.Log("Starting task read", LogLevel.INFO);

            // parse task ID
            PlanumTaskDTO task = new PlanumTaskDTO();
            if (!TryParseTaskMarker(enumerator.Current, ref task))
                throw new TaskRepoException($"Unable to parse guid from line: \"{enumerator.Current}\"");
            else
                enumerator.MoveNext();
            if (task.Id == Guid.Empty)
                task.Id = Guid.NewGuid();

            HashSet<Tuple<string, string>> parentRefs = new HashSet<Tuple<string, string>>();
            HashSet<Tuple<string, string>> childRefs = new HashSet<Tuple<string, string>>();

            while (enumerator.Current != null && enumerator.Current.Trim(' ', '\n') != "")
            {
                if (TryParseTaskName(enumerator.Current, ref task))
                    enumerator.MoveNext();
                else if (TryParseTags(enumerator.Current, ref task))
                    enumerator.MoveNext();
                else if (TryParseDescription(enumerator.Current, ref task))
                    enumerator.MoveNext();
                else if (TryParseTaskReference(enumerator.Current, ref parentRefs, ModelConfig.TaskParentSymbol))
                    enumerator.MoveNext();
                else if (TryParseTaskReference(enumerator.Current, ref childRefs, ModelConfig.TaskChildSymbol))
                    enumerator.MoveNext();
                else if (TryParseDeadline(ref enumerator, ref task)) { }
                else if (TryParseChecklist(ref enumerator, ref task, ref tasks)) { }
                else
                    throw new TaskRepoException($"Unable to parse task at line: \"{enumerator.Current}\"");
            }

            task.Parents = parentRefs;
            task.Children = childRefs;

            tasks.Add(task);
            Logger.Log($"Read finished, task: {task.Id} | {task.Name}", LogLevel.INFO);
            return task.Id;
        }

        protected string GetTabs(int level)
        {
            var tabs = "";
            for (int i = 0; i < level; i++)
                tabs += ModelConfig.TaskItemTabSymbol;
            return tabs;
        }

        protected string RemoveMarkdownLink(string line)
        {
            line = line.Split("](").First();
            if (line.Length > 0)
                return line.Remove(0, 1);
            return line;
        }

        protected string GetItemMark(string line, string symbol, int level = 0)
        {
            var markerSymbols = new string[] {
                ModelConfig.TaskWarningMarkerSymbol,
                ModelConfig.TaskInProgressMarkerSymbol,
                ModelConfig.TaskOverdueMarkerSymbol,
                ModelConfig.TaskCompleteMarkerSymbol,
                ModelConfig.TaskNotCompleteMarkerSymbol
            };

            var checklistStart = GetTabs(level) +
                        ModelConfig.TaskItemSymbol +
                        ModelConfig.TaskCheckboxStart;

            var checklistEnd = ModelConfig.TaskCheckboxEnd + symbol;
            if (symbol != "")
                checklistEnd += ModelConfig.TaskHeaderDelimeterSymbol;

            foreach (var marker in markerSymbols)
                if (line.StartsWith(
                        checklistStart +
                        marker +
                        checklistEnd))
                    return marker;
            return "";
        }

        protected bool CheckMarkedItemLine(string line, string symbol, int level = 0) => GetItemMark(line, symbol, level).Any();

        protected bool CheckItemLine(string line, string symbol, int level = 0)
        {
            if (line.StartsWith(
                        GetTabs(level) +
                        ModelConfig.TaskItemSymbol +
                        symbol +
                        ModelConfig.TaskHeaderDelimeterSymbol))
                return true;
            return false;
        }

        protected bool TryParseTaskMarker(string line, ref PlanumTaskDTO task)
        {
            line = line.Trim(' ', '\n');
            if (!(line.Contains(ModelConfig.TaskMarkerStartSymbol) && line.Contains(ModelConfig.TaskMarkerEndSymbol)))
                return false;
            line = line.Remove(0, ModelConfig.TaskMarkerStartSymbol.Length);
            line = line.Remove(line.Length - ModelConfig.TaskMarkerEndSymbol.Length, ModelConfig.TaskMarkerEndSymbol.Length);
            if (line == "")
                return true;
            Guid id;
            if (!Guid.TryParse(line, out id))
                return false;
            task.Id = id;
            return true;
        }

        protected string GetTaskItem(string line, string symbol, int level = 0)
        {
            return line.Replace(
                    GetTabs(level) +
                    ModelConfig.TaskItemSymbol +
                    symbol +
                    ModelConfig.TaskHeaderDelimeterSymbol,
                    "");
        }

        protected string GetMarkerTaskItem(string line, string symbol, ref string taskMarker, int level = 0)
        {
            taskMarker = GetItemMark(line, symbol, level);
            if (!taskMarker.Any())
                throw new TaskRepoException($"Unable to parse marked task element: \"{line}\"");
            var replace = GetTabs(level) +
                        ModelConfig.TaskItemSymbol +
                        ModelConfig.TaskCheckboxStart +
                        taskMarker +
                        ModelConfig.TaskCheckboxEnd +
                        symbol;
            if (symbol != "")
                replace += ModelConfig.TaskHeaderDelimeterSymbol;
            return line.Replace(replace, "");
        }

        protected string GetMarkerTaskItem(string line, string symbol, int level = 0)
        {
            string marker = "";
            return GetMarkerTaskItem(line, symbol, ref marker, level);
        }

        protected bool TryParseChecklistName(string line, ref PlanumTaskDTO task, int currentLevel = 0)
        {
            if (line == null || !CheckMarkedItemLine(line, "", currentLevel))
                return false;

            task.Name = GetMarkerTaskItem(line, "", currentLevel);
            return true;
        }

        protected bool TryParseTaskName(string line, ref PlanumTaskDTO task, int currentLevel = 0)
        {
            if (line == null || !CheckMarkedItemLine(line, ModelConfig.TaskNameSymbol, currentLevel))
                return false;

            var name = GetMarkerTaskItem(line, ModelConfig.TaskNameSymbol, currentLevel);
            task.Name = name.Split(ModelConfig.TaskLinkSymbol).First().Trim();
            return true;
        }

        protected bool TryParseTags(string line, ref PlanumTaskDTO task, int currentLevel = 0)
        {
            if (line == null || !CheckItemLine(line, ModelConfig.TaskTagSymbol, currentLevel))
                return false;

            var tags = GetTaskItem(line, ModelConfig.TaskTagSymbol, currentLevel).Split(',');
            foreach (var tag in tags)
                task.Tags.Add(tag.Trim());
            return true;
        }

        protected bool TryParseDescription(string line, ref PlanumTaskDTO task, int currentLevel = 0)
        {
            if (line == null || !CheckItemLine(line, ModelConfig.TaskDescriptionSymbol, currentLevel))
                return false;

            task.Description = GetTaskItem(line, ModelConfig.TaskDescriptionSymbol, currentLevel);
            return true;
        }

        protected bool TryParseTaskReference(string line, ref HashSet<Tuple<string, string>> taskRefs, string symbol, int currentLevel = 0)
        {
            if (line == null || !CheckMarkedItemLine(line, symbol, currentLevel))
                return false;

            var task = GetMarkerTaskItem(line, symbol, currentLevel);

            var taskValue = task.Split(ModelConfig.TaskLinkSymbol).First();
            var taskIdentifiers = taskValue.Split(ModelConfig.TaskValueIdDelimiter);

            var taskId = "";
            var taskName = "";

            if (taskIdentifiers.Count() == 2)
            {
                taskId = taskIdentifiers[1].Trim();
                taskName = taskIdentifiers[0].Trim();
            }
            else if (taskIdentifiers.Count() == 1)
                taskName = taskIdentifiers[0].Trim();

            taskRefs.Add(new Tuple<string, string>(taskId, taskName));
            return true;
        }

        protected bool TryParseDeadline(ref IEnumerator<string> enumerator, ref PlanumTaskDTO task, int currentLevel = 0)
        {
            if (enumerator.Current == null || !CheckMarkedItemLine(enumerator.Current, ModelConfig.TaskDeadlineHeaderSymbol, currentLevel))
                return false;

            // parse header
            var deadline = new DeadlineDTO();
            deadline.Id = Guid.NewGuid();
            deadline.deadline = DateTime.Today;
            ParseDeadlineHeader(ref deadline, enumerator.Current, currentLevel);

            TimeSpan warningTime = TimeSpan.Zero;
            TimeSpan durationTime = TimeSpan.Zero;

            RepeatSpan repeat = new RepeatSpan();
            bool repeated = false;

            HashSet<Tuple<string, string>> next = new HashSet<Tuple<string, string>>();

            while (enumerator.Current != null && enumerator.Current.Trim() != "" && enumerator.MoveNext())
            {
                if (!(TryParseTimeSpanItem(ref warningTime, enumerator.Current, ModelConfig.TaskWarningTimeSymbol, currentLevel + 1) ||
                    TryParseTimeSpanItem(ref durationTime, enumerator.Current, ModelConfig.TaskDurationTimeSymbol, currentLevel + 1) ||
                    TryParseRepeatSpan(ref repeat, ref repeated, enumerator.Current, ModelConfig.TaskRepeatTimeSymbol, currentLevel + 1) ||
                    TryParseTaskReference(enumerator.Current, ref next, ModelConfig.TaskNextSymbol, currentLevel + 1)))
                    break;
            }
            
            deadline.warningTime = warningTime;
            deadline.duration = durationTime;
            deadline.repeatSpan = repeat;
            deadline.repeated = repeated;

            task.Deadlines.Add(deadline);

            return true;
        }

        protected void ParseDeadlineHeader(ref DeadlineDTO deadline, string line, int currentLevel = 0)
        {
            var taskMarker = "";
            var deadlineHeader = GetMarkerTaskItem(line, ModelConfig.TaskDeadlineHeaderSymbol, ref taskMarker, currentLevel).Trim();

            Guid id = Guid.Empty;
            DateTime deadlineValue = DateTime.Today;

            if (taskMarker == ModelConfig.TaskCompleteMarkerSymbol)
                deadline.enabled = false;
            else
                deadline.enabled = true;

            if (!deadlineHeader.Any())
                return;
            else if (!deadlineHeader.Contains(ModelConfig.TaskValueIdDelimiter))
            {
                if (ValueParser.TryParse(ref id, deadlineHeader))
                    deadline.Id = id;
                else if (ValueParser.TryParse(ref deadlineValue, deadlineHeader))
                    deadline.deadline = deadlineValue;
                else
                    throw new TaskRepoException($"Unable to parse deadline id or value from \"{line}\"");
            }
            else
            {
                var dateIdSplit = deadlineHeader.Split(ModelConfig.TaskValueIdDelimiter);
                if (!ValueParser.TryParse(ref deadlineValue, dateIdSplit[0].Trim()))
                    throw new TaskRepoException($"Unable to parse deadline date from \"{line}\"");
                if (!ValueParser.TryParse(ref id, dateIdSplit[1].Trim()))
                    throw new TaskRepoException($"Unable to parse deadline id from \"{line}\"");
                deadline.Id = id;
                deadline.deadline = deadlineValue;
            }
        }

        protected bool TryParseTimeSpanItem(ref TimeSpan span, string line, string symbol, int currentLevel = 0)
        {
            if (line == null || !CheckItemLine(line, symbol, currentLevel))
                return false;
            var item = GetTaskItem(line, symbol, currentLevel);
            if (!ValueParser.TryParse(ref span, item))
                return false;
            return true;
        }

        protected bool TryParseRepeatSpan(ref RepeatSpan repeat, ref bool repeated, string line, string symbol, int currentLevel = 0)
        {
            if (line == null || !CheckMarkedItemLine(line, symbol, currentLevel))
                return false;
            if (GetItemMark(line, symbol, currentLevel) == ModelConfig.TaskCompleteMarkerSymbol)
                repeated = true;
            else
                repeated = false;

            var item = GetMarkerTaskItem(line, symbol, currentLevel);
            if (!TaskValueParser.TryParseRepeat(ref repeat, item))
                throw new TaskRepoException($"Unable to parse repeat span from \"{item}\"");
            return true;
        }

        protected bool TryParseChecklist(ref IEnumerator<string> enumerator, ref PlanumTaskDTO task, ref List<PlanumTaskDTO> tasks, int currentLevel = 0)
        {
            if (enumerator.Current == null || !CheckMarkedItemLine(enumerator.Current, "", currentLevel))
                return false;

            PlanumTaskDTO checklistTask = new PlanumTaskDTO();
            checklistTask.Tags.Add(DefaultTags.Checklist);
            checklistTask.Id = Guid.NewGuid();

            // parse header
            if (!TryParseChecklistName(enumerator.Current, ref checklistTask, currentLevel))
                throw new TaskRepoException($"Unable to parse checklist name: \"{enumerator.Current}\"");
            else
                enumerator.MoveNext();

            while (enumerator.Current != null && enumerator.Current != "")
            {
                // parse description
                if (TryParseDescription(enumerator.Current, ref checklistTask, currentLevel + 1))
                    enumerator.MoveNext();
                // parse deadlines
                else if (TryParseDeadline(ref enumerator, ref checklistTask, currentLevel + 1)) { }
                // parse checklists
                else if (TryParseChecklist(ref enumerator, ref checklistTask, ref tasks, currentLevel + 1)) { }
                else
                    break;
            }

            task.Children.Add(new Tuple<string, string>(checklistTask.Id.ToString(), checklistTask.Name));
            checklistTask.Parents.Add(new Tuple<string, string>(task.Id.ToString(), task.Name));
            tasks.Add(checklistTask);

            return true;
        }
    }
}
