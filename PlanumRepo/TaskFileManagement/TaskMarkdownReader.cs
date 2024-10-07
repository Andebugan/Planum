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
        public void ReadTask(ref IEnumerator<string> enumerator, ref List<PlanumTaskDTO> tasks)
        {
            Logger.Log("Starting task read", LogLevel.INFO);

            // parse task ID
            PlanumTaskDTO task = new PlanumTaskDTO();
            ParseTaskMarker(enumerator.Current, ref task);

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

            tasks.Add(task);

            Logger.Log($"Read finished, task: {task.Id} | {task.Name}", LogLevel.INFO);
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
            line = line.Trim(' ', '\n');
            var markerSymbols = new string[] {
                ModelConfig.TaskWarningMarkerSymbol,
                ModelConfig.TaskInProgressMarkerSymbol,
                ModelConfig.TaskOverdueMarkerSymbol,
                ModelConfig.TaskCompleteMarkerSymbol,
                ModelConfig.TaskNotCompleteMarkerSymbol
            };

            foreach (var marker in markerSymbols)
                if (line.StartsWith(
                            GetTabs(level) + 
                            ModelConfig.TaskItemSymbol + 
                            ModelConfig.TaskCheckboxStart + 
                            marker + 
                            ModelConfig.TaskCheckboxEnd + 
                            " " +
                            symbol +
                            ModelConfig.TaskHeaderDelimeterSymbol))
                    return marker;
            return "";
        }

        protected bool CheckMarkedItemLine(string line, string symbol, int level = 0) => GetItemMark(line, symbol, level) != "";

        protected bool CheckItemLine(string line, string symbol, int level = 0)
        {
            if (line.StartsWith(
                        GetTabs(level) + 
                        ModelConfig.TaskItemSymbol + 
                        ModelConfig.TaskHeaderDelimeterSymbol))
                return true;
            return false;
        }

        protected void ParseTaskMarker(string line, ref PlanumTaskDTO task)
        {
            line = line.Trim(' ', '\n');
            line = line.Remove(0, ModelConfig.TaskMarkerStartSymbol.Length);
            line = line.Remove(line.Length - ModelConfig.TaskMarkerEndSymbol.Length, ModelConfig.TaskMarkerEndSymbol.Length);
            var id = Guid.NewGuid();
            if (line.Trim() == string.Empty)
                task.Id = id;
            if (!Guid.TryParse(line, out id))
                throw new TaskRepoException($"Unable to parse guid from line: \"{line}\"");
        }

        protected string GetTaskItem(string line, string symbol, int level = 0)
        {
            return line.Replace(
                    GetTabs(level) + 
                    ModelConfig.TaskItemSymbol + 
                    ModelConfig.TaskHeaderDelimeterSymbol,
                    "");
        }

        protected string GetMarkerTaskItem(string line, string symbol, ref string taskMarker, int level = 0)
        {
            taskMarker = GetItemMark(line, symbol, level);
            if (taskMarker == "")
                throw new TaskRepoException($"Unable to parse marked task element: \"{line}\"");
            return line.Replace(
                        GetTabs(level) + 
                        ModelConfig.TaskItemSymbol + 
                        ModelConfig.TaskCheckboxStart + 
                        taskMarker + 
                        ModelConfig.TaskCheckboxEnd + 
                        " " +
                        symbol +
                        ModelConfig.TaskHeaderDelimeterSymbol,
                        "");
        }

        protected string GetMarkerTaskItem(string line, string symbol, int level = 0)
        {
            string marker = "";
            return GetMarkerTaskItem(line, symbol, ref marker, level);
        }

        protected bool TryParseTaskName(string line, ref PlanumTaskDTO task, int currentLevel = 0)
        {
            if (line == null || !CheckMarkedItemLine(line, ModelConfig.TaskNameSymbol, currentLevel))
                return false;

            task.Name = GetMarkerTaskItem(line, ModelConfig.TaskNameSymbol, currentLevel);
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

            task.Description = GetTaskItem(line, ModelConfig.TaskNameSymbol, currentLevel);
            return true;
        }

        protected bool TryParseTaskReference(string line, ref HashSet<Tuple<string, string>> taskRefs, string symbol, int currentLevel = 0)
        {
            if (line == null || !CheckMarkedItemLine(line, symbol, currentLevel))
                return false;
            
            var task = RemoveMarkdownLink(GetMarkerTaskItem(line, symbol, currentLevel)).Split("|");
            if (task.Length < 2)
                throw new TaskRepoException($"Incorrect task reference format: \"{line}\"");

            var taskName = task[0].Trim();
            var taskId = task[1].Trim();

            taskRefs.Add(new Tuple<string, string>(taskId, taskName));
            return true;
        }

        protected bool TryParseDeadline(ref IEnumerator<string> enumerator, ref PlanumTaskDTO task, int currentLevel = 0)
        {
            if (enumerator.Current == null || !CheckMarkedItemLine(enumerator.Current, ModelConfig.TaskDeadlineHeaderSymbol, currentLevel))
                return false;

            // parse header
            var deadline = ParseDeadlineHeader(enumerator.Current, currentLevel);
            HashSet<Tuple<string, string>> next = new HashSet<Tuple<string, string>>();
            
            while (enumerator.Current != null && enumerator.Current.Trim() != "" && enumerator.MoveNext())
            {
                // parse warning
                if (CheckItemLine(enumerator.Current, ModelConfig.TaskWarningTimeSymbol, currentLevel + 1))
                    deadline.warningTime = ParseTimeSpanItem(enumerator.Current, ModelConfig.TaskWarningTimeSymbol, currentLevel + 1);
                // parse duration
                else if (CheckItemLine(enumerator.Current, ModelConfig.TaskDurationTimeSymbol, currentLevel + 1))
                    deadline.duration = ParseTimeSpanItem(enumerator.Current, ModelConfig.TaskDurationTimeSymbol, currentLevel + 1);
                // parse repeat
                else if (CheckItemLine(enumerator.Current, ModelConfig.TaskRepeatTimeSymbol, currentLevel + 1))
                {
                    var repeatSpan = new RepeatSpan();
                    if (!TaskValueParser.TryParseRepeat(ref repeatSpan, enumerator.Current))
                        throw new TaskRepoException($"Unable to parse repeat span from \"{enumerator.Current}\"");
                    deadline.repeatSpan = repeatSpan;
                }
                // parse next
                else if (CheckMarkedItemLine(enumerator.Current, ModelConfig.TaskNextSymbol, currentLevel + 1))
                    TryParseTaskReference(enumerator.Current, ref next, ModelConfig.TaskNextSymbol, currentLevel + 1);
                else
                    break;
            }

            return true;
        }

        protected DeadlineDTO ParseDeadlineHeader(string line, int currentLevel = 0)
        {
            DeadlineDTO deadline = new DeadlineDTO();

            var taskMarker = "";
            var deadlineHeader = GetMarkerTaskItem(line, ModelConfig.TaskDeadlineHeaderSymbol, ref taskMarker, currentLevel); 
            var head = deadlineHeader.Split(ModelConfig.TaskValueIdDelimiter);

            var id = Guid.NewGuid();
            var date = DateTime.Today;

            if (head.Count() == 1)
            {
                if (!ValueParser.TryParse(ref id, head.First().Trim()) && !ValueParser.TryParse(ref date, head.First().Trim()))
                    throw new TaskRepoException($"Unable to parse deadline id or value from \"{line}\"");
            }
            else if (head.Count() == 2)
            {
                if (!ValueParser.TryParse(ref date, head[0].Trim()))
                    throw new TaskRepoException($"Unable to parse deadline date from \"{line}\"");
                if (!ValueParser.TryParse(ref id, head[1].Trim()))
                    throw new TaskRepoException($"Unable to parse deadline id from \"{line}\"");
            }
            
            deadline.Id = id;
            deadline.deadline = date;
            return deadline;
        }

        protected TimeSpan ParseTimeSpanItem(string line, string symbol, int currentLevel = 0)
        {
            var item = GetTaskItem(line, symbol, currentLevel + 1);
            var timeSpan = TimeSpan.Zero;
            if (!ValueParser.TryParse(ref timeSpan, line))
                throw new TaskRepoException($"Unable to parse warning time: \"{line}\"");
            return timeSpan;
        }

        protected bool TryParseChecklist(ref IEnumerator<string> enumerator, ref PlanumTaskDTO task, ref List<PlanumTaskDTO> tasks, int currentLevel = 0)
        {
            if (enumerator.Current == null || !CheckMarkedItemLine(enumerator.Current, "", currentLevel))
                return false;

            PlanumTaskDTO checklistTask = new PlanumTaskDTO();
            checklistTask.Tags.Add(DefaultTags.Checklist);
            checklistTask.Id = Guid.NewGuid();

            // parse header
            if (!TryParseTaskName(enumerator.Current, ref checklistTask, currentLevel))
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
