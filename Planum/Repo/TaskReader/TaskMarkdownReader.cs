using System;
using System.Collections.Generic;
using Planum.Config;
using Planum.Logger;
using Planum.Model.Entities;
using Planum.Parser;

namespace Planum.Repository
{
    public class TaskMarkdownReader
    {
        AppConfig AppConfig { get; set; }
        RepoConfig RepoConfig { get; set; }
        ILoggerWrapper Logger { get; set; }

        public TaskMarkdownReader(ILoggerWrapper logger, AppConfig appConfig, RepoConfig repoConfig)
        {
            AppConfig = appConfig;
            RepoConfig = repoConfig;
            Logger = logger;
        }

        protected bool CheckTaskMarker(string line) => line.StartsWith(RepoConfig.TaskMarkerStartSymbol) && line.EndsWith(RepoConfig.TaskMarkerEndSymbol);
        protected Guid ParseTaskMarker(string line, ref IList<TaskReadStatus> statuses)
        {
            line = line.Trim(' ', '\n');
            line = line.Remove(0, RepoConfig.TaskMarkerStartSymbol.Length);
            line = line.Remove(line.Length - RepoConfig.TaskMarkerEndSymbol.Length, RepoConfig.TaskMarkerEndSymbol.Length);
            var id = Guid.Empty;
            if (line == string.Empty)
                return id;
            if (!Guid.TryParse(line, out id))
                statuses.Add(new TaskReadStatus(null, TaskReadStatusType.UNABLE_TO_PARSE_TASK_GUID, line: line));
            return id;
        }

        protected bool CheckName(string line)
        {
            foreach (string statusMarker in RepoConfig.GetTaskStatusMarkerSymbols())
            {
                if (line.StartsWith(RepoConfig.TaskItemSymbol +
                    RepoConfig.AddCheckbox(statusMarker) +
                    RepoConfig.TaskNameSymbol +
                    RepoConfig.TaskHeaderDelimeterSymbol))
                    return true;
            }
            return false;
        }

        protected string ParseTaskName(string line, ref bool complete)
        {
            if (line.StartsWith(RepoConfig.TaskItemSymbol +
                RepoConfig.AddCheckbox(RepoConfig.TaskCompleteMarkerSymbol) +
                RepoConfig.TaskNameSymbol +
                RepoConfig.TaskHeaderDelimeterSymbol))
                complete = true;
            return line.Remove(0, (RepoConfig.TaskItemSymbol +
            RepoConfig.AddCheckbox(" ") +
            RepoConfig.TaskNameSymbol +
            RepoConfig.TaskHeaderDelimeterSymbol).Length);
        }

        protected bool CheckTag(string line, int level = 0) => line.StartsWith(AddLineTabs(level) +
                RepoConfig.TaskItemSymbol +
                RepoConfig.TaskTagSymbol +
                RepoConfig.TaskHeaderDelimeterSymbol);
        protected string ParseTaskTag(string line, int level = 0) => line.Remove(0, (AddLineTabs(level) +
                RepoConfig.TaskItemSymbol +
                RepoConfig.TaskTagSymbol +
                RepoConfig.TaskHeaderDelimeterSymbol).Length);

        protected bool CheckDescription(string line, int level = 0) => line.StartsWith(AddLineTabs(level) +
                RepoConfig.TaskItemSymbol +
                RepoConfig.TaskDescriptionSymbol +
                RepoConfig.TaskHeaderDelimeterSymbol);
        protected string ParseTaskDescription(ref IEnumerator<string> linesEnumerator, ref bool moveNext, int level = 0)
        {
            string descriptionString = linesEnumerator.Current.Remove(0, (AddLineTabs(level) +
                RepoConfig.TaskItemSymbol +
                RepoConfig.TaskDescriptionSymbol +
                RepoConfig.TaskHeaderDelimeterSymbol).Length);

            if (linesEnumerator.Current.EndsWith(RepoConfig.TaskDescriptionNewlineSymbol))
            {
                moveNext = linesEnumerator.MoveNext();
                while (moveNext && linesEnumerator.Current.EndsWith(RepoConfig.TaskDescriptionNewlineSymbol))
                {
                    descriptionString += linesEnumerator.Current.Remove(0, (AddLineTabs(level + 1).Length));
                    moveNext = linesEnumerator.MoveNext();
                }

                if (moveNext)
                {
                    descriptionString += linesEnumerator.Current.Remove(0, (AddLineTabs(level + 1).Length));
                    moveNext = linesEnumerator.MoveNext();
                }
                else
                    descriptionString = descriptionString.TrimEnd('\\');
            }
            else
                moveNext = linesEnumerator.MoveNext();

            return descriptionString;
        }

        protected bool CheckParent(string line)
        {
            foreach (string statusMarker in RepoConfig.GetTaskStatusMarkerSymbols())
                if (line.StartsWith(RepoConfig.TaskItemSymbol +
                                    RepoConfig.AddCheckbox(statusMarker) +
                                    RepoConfig.TaskParentSymbol +
                                    RepoConfig.TaskHeaderDelimeterSymbol))
                    return true;
            return false;
        }
        protected string ParseParentString(string line) => RepoConfig.ParseMarkdownLink(line.Remove(0, (RepoConfig.TaskItemSymbol +
                        RepoConfig.AddCheckbox(" ") +
                        RepoConfig.TaskParentSymbol +
                        RepoConfig.TaskHeaderDelimeterSymbol).Length));

        protected bool CheckChild(string line)
        {
            foreach (string statusMarker in RepoConfig.GetTaskStatusMarkerSymbols())
                if (line.StartsWith(RepoConfig.TaskItemSymbol +
                                    RepoConfig.AddCheckbox(statusMarker) +
                                    RepoConfig.TaskChildSymbol +
                                    RepoConfig.TaskHeaderDelimeterSymbol))
                    return true;
            return false;
        }
        protected string ParseChildString(string line) => RepoConfig.ParseMarkdownLink(line.Remove(0, (RepoConfig.TaskItemSymbol +
                        RepoConfig.AddCheckbox(" ") +
                        RepoConfig.TaskChildSymbol +
                        RepoConfig.TaskHeaderDelimeterSymbol).Length));

        protected string AddLineTabs(int level = 0)
        {
            var tabs = "";
            for (int i = 0; i < level; i++)
                tabs += RepoConfig.TaskItemTabSymbol;
            return tabs;
        }

        protected bool CheckChecklist(string line, int level = 0)
        {
            foreach (string statusMarker in RepoConfig.GetTaskStatusMarkerSymbols())
                if (line.StartsWith(AddLineTabs(level) +
                            RepoConfig.TaskItemSymbol +
                            statusMarker))
                    return true;
            return false;
        }
        protected void ParseChecklist(ref bool moveNext, ref IList<TaskReadStatus> statuses, ref IEnumerator<string> linesEnumerator, Guid taskId, IList<PlanumTask> tasks, Dictionary<Guid, IList<string>> next, int level = 0)
        {
            var checklistTask = new PlanumTask(id: Guid.NewGuid());
            checklistTask.Parents.Add(taskId);
            checklistTask.Tags.Add(DefaultTags.Checklist);

            // parse checklist name
            if (linesEnumerator.Current.StartsWith(AddLineTabs(level) +
                RepoConfig.TaskItemSymbol +
                RepoConfig.AddCheckbox(RepoConfig.TaskCompleteMarkerSymbol)))
                checklistTask.Tags.Add(DefaultTags.Complete);

            checklistTask.Name = linesEnumerator.Current.Remove(0, (AddLineTabs(level) +
                RepoConfig.TaskItemSymbol +
                RepoConfig.AddCheckbox(RepoConfig.TaskCompleteMarkerSymbol)).Length);

            moveNext = linesEnumerator.MoveNext();

            // finish parsing checklist + recursive part
            while (moveNext)
            {
                // parse description
                if (CheckDescription(linesEnumerator.Current, level + 1))
                    checklistTask.Description = ParseTaskDescription(ref linesEnumerator, ref moveNext, level + 1);
                // parse tag
                else if (CheckTag(linesEnumerator.Current))
                {
                    checklistTask.Tags.Add(ParseTaskTag(linesEnumerator.Current, level + 1));
                    moveNext = linesEnumerator.MoveNext();
                }
                // parse deadline
                else if (CheckDeadline(linesEnumerator.Current, level + 1))
                    checklistTask.Deadlines.Add(ParseDeadline(ref moveNext, ref statuses, ref linesEnumerator, next, level + 1));
                // parse checklist
                else if (CheckChecklist(linesEnumerator.Current, level + 1))
                    ParseChecklist(ref moveNext, ref statuses, ref linesEnumerator, checklistTask.Id, tasks, next, level + 1);
                else
                {
                    moveNext = true;
                    return;
                }
            }
        }

        protected bool CheckDeadline(string line, int level = 0)
        {
            foreach (string statusMarker in RepoConfig.GetTaskStatusMarkerSymbols())
                if (line.StartsWith(RepoConfig.TaskItemSymbol +
                                    RepoConfig.AddCheckbox(statusMarker) +
                                    RepoConfig.TaskDeadlineHeaderSymbol +
                                    RepoConfig.TaskHeaderDelimeterSymbol))
                    return true;
            return false;
        }

        protected bool CheckDeadlineDate(string line, int level = 0) => line.StartsWith(AddLineTabs(level) +
                RepoConfig.TaskItemSymbol +
                RepoConfig.TaskDeadlineSymbol +
                RepoConfig.TaskHeaderDelimeterSymbol);
        protected DateTime ParseDeadlineDate(string line, ref IList<TaskReadStatus> statuses, int level = 0)
        {
            line = line.Remove(0, (AddLineTabs(level) +
                RepoConfig.TaskItemSymbol +
                RepoConfig.TaskDeadlineSymbol +
                RepoConfig.TaskHeaderDelimeterSymbol).Length);
            DateTime deadline = DateTime.Today;
            if (!ValueParser.TryParse(ref deadline, line))
                statuses.Add(new TaskReadStatus(null, TaskReadStatusType.UNABLE_TO_PARSE_DEADLINE, line: line));
            return deadline;
        }

        protected bool CheckWarningTime(string line, int level = 0) => line.StartsWith(AddLineTabs(level) +
                RepoConfig.TaskItemSymbol +
                RepoConfig.TaskWarningTimeSymbol +
                RepoConfig.TaskHeaderDelimeterSymbol);
        protected TimeSpan ParseWarningTime(string line, ref IList<TaskReadStatus> statuses, int level = 0)
        {
            line = line.Remove(0, (AddLineTabs(level) +
                RepoConfig.TaskItemSymbol +
                RepoConfig.TaskWarningTimeSymbol +
                RepoConfig.TaskHeaderDelimeterSymbol).Length);
            TimeSpan warningTime = TimeSpan.Zero;
            if (!ValueParser.TryParse(ref warningTime, line))
                statuses.Add(new TaskReadStatus(null, TaskReadStatusType.UNABLE_TO_PARSE_WARNING_TIME, line: line));
            return warningTime;
        }

        protected bool CheckDuration(string line, int level = 0) => line.StartsWith(AddLineTabs(level) +
                RepoConfig.TaskItemSymbol +
                RepoConfig.TaskDurationTimeSymbol +
                RepoConfig.TaskHeaderDelimeterSymbol);
        protected TimeSpan ParseDuration(string line, ref IList<TaskReadStatus> statuses, int level = 0)
        {
            line = line.Remove(0, (AddLineTabs(level) +
                RepoConfig.TaskItemSymbol +
                RepoConfig.TaskDurationTimeSymbol +
                RepoConfig.TaskHeaderDelimeterSymbol).Length);
            TimeSpan duration = TimeSpan.Zero;
            if (!ValueParser.TryParse(ref duration, line))
                statuses.Add(new TaskReadStatus(null, TaskReadStatusType.UNABLE_TO_PARSE_WARNING_TIME, line: line));
            return duration;
        }

        protected bool CheckRepeated(string line, int level = 0)
        {
            foreach (string statusMarker in RepoConfig.GetTaskStatusMarkerSymbols())
                if (line.StartsWith(AddLineTabs(level) +
                            RepoConfig.TaskItemSymbol +
                            RepoConfig.AddCheckbox(statusMarker) +
                            RepoConfig.TaskRepeatTimeSymbol +
                            RepoConfig.TaskHeaderDelimeterSymbol))
                    return true;
            return false;
        }
        protected void ParseRepeat(string line, ref IList<TaskReadStatus> statuses, ref Deadline deadline, int level = 0)
        {
            // parse header
            if (line.StartsWith(AddLineTabs(level) +
                        RepoConfig.TaskItemSymbol +
                        RepoConfig.AddCheckbox(RepoConfig.TaskNotCompleteMarkerSymbol) +
                        RepoConfig.TaskRepeatTimeSymbol +
                        RepoConfig.TaskHeaderDelimeterSymbol))
                deadline.repeated = false;
            else
                deadline.repeated = true;

            line = line.Remove(0, (AddLineTabs(level) +
                        RepoConfig.TaskItemSymbol +
                        RepoConfig.AddCheckbox(" ") +
                        RepoConfig.TaskRepeatTimeSymbol +
                        RepoConfig.TaskHeaderDelimeterSymbol).Length);

            if (!ValueParser.TryParse(ref deadline.repeatSpan, ref deadline.repeatMonths, ref deadline.repeatYears, line))
                statuses.Add(new TaskReadStatus(null, TaskReadStatusType.UNABLE_TO_PARSE_REPEAT_PERIOD, line: line));
        }

        protected bool CheckNext(string line, int level = 0)
        {
            foreach (string statusMarker in RepoConfig.GetTaskStatusMarkerSymbols())
                if (line.StartsWith(AddLineTabs(level) +
                            RepoConfig.TaskItemSymbol +
                            RepoConfig.AddCheckbox(statusMarker) +
                            RepoConfig.TaskNextSymbol +
                            RepoConfig.TaskHeaderDelimeterSymbol))
                    return true;
            return false;
        }
        protected string ParseNextTaskString(string line, int level = 0) => RepoConfig.ParseMarkdownLink(line.Remove(0, (AddLineTabs(level) +
                    RepoConfig.TaskItemSymbol +
                    RepoConfig.AddCheckbox(" ") +
                    RepoConfig.TaskNextSymbol +
                    RepoConfig.TaskHeaderDelimeterSymbol).Length));

        protected Deadline ParseDeadline(ref bool moveNext, ref IList<TaskReadStatus> statuses, ref IEnumerator<string> linesEnumerator, Dictionary<Guid, IList<string>> next, int level = 0)
        {
            Deadline deadline = new Deadline();

            // parse header
            var line = linesEnumerator.Current.Remove(0, (AddLineTabs(level) +
                        RepoConfig.TaskItemSymbol +
                        RepoConfig.AddCheckbox(" ") +
                        RepoConfig.TaskDeadlineHeaderSymbol +
                        RepoConfig.TaskHeaderDelimeterSymbol).Length).Trim(' ', '\n');

            if (linesEnumerator.Current.StartsWith(AddLineTabs(level) +
                        RepoConfig.TaskItemSymbol +
                        RepoConfig.AddCheckbox(RepoConfig.TaskCompleteMarkerSymbol) +
                        RepoConfig.TaskDeadlineHeaderSymbol +
                        RepoConfig.TaskHeaderDelimeterSymbol))
                deadline.enabled = false;
            else
                deadline.enabled = true;

            var deadlineId = Guid.NewGuid();

            if (line != string.Empty && !ValueParser.TryParse(ref deadlineId, line))
            {
                statuses.Add(new TaskReadStatus(null, TaskReadStatusType.UNABLE_TO_PARSE_REPEAT_PERIOD, line: line));
                deadline.Id = Guid.Empty;
                return deadline;
            }
            deadline.Id = deadlineId;

            moveNext = linesEnumerator.MoveNext();

            while (moveNext)
            {
                // parse deadline
                if (CheckDeadlineDate(linesEnumerator.Current, level + 1))
                {
                    deadline.deadline = ParseDeadlineDate(linesEnumerator.Current, ref statuses, level + 1);
                    moveNext = linesEnumerator.MoveNext();
                }
                // parse warning
                else if (CheckWarningTime(linesEnumerator.Current, level + 1))
                {
                    deadline.warningTime = ParseWarningTime(linesEnumerator.Current, ref statuses, level + 1);
                    moveNext = linesEnumerator.MoveNext();
                }
                // parse duration
                else if (CheckDuration(linesEnumerator.Current, level + 1))
                {
                    deadline.duration = ParseDuration(linesEnumerator.Current, ref statuses, level + 1);
                    moveNext = linesEnumerator.MoveNext();
                }
                // parse repeat duration
                else if (CheckRepeated(linesEnumerator.Current, level + 1))
                {
                    ParseRepeat(linesEnumerator.Current, ref statuses, ref deadline, level + 1);
                    moveNext = linesEnumerator.MoveNext();
                }
                // parse next tasks 
                else if (CheckNext(linesEnumerator.Current, level + 1))
                {
                    if (!next.ContainsKey(deadline.Id))
                        next[deadline.Id] = new List<string>();
                    next[deadline.Id].Add(ParseNextTaskString(linesEnumerator.Current, level + 1));
                    moveNext = linesEnumerator.MoveNext();
                }
                else
                {
                    moveNext = true;
                    break;
                }
            }

            return deadline;
        }

        public Guid ReadTask(ref IEnumerator<string> linesEnumerator, ref IList<TaskReadStatus> statuses, IList<PlanumTask> tasks, Dictionary<Guid, IList<string>> children, Dictionary<Guid, IList<string>> parents, Dictionary<Guid, IList<string>> next)
        {
            Logger.Log(LogLevel.INFO, "Starting task read");
            if (!CheckTaskMarker(linesEnumerator.Current))
            {
                Logger.Log(LogLevel.WARN, $"Unable to read task marker at {linesEnumerator.Current}");
                return Guid.Empty;
            }
            // parse task ID
            PlanumTask task = new PlanumTask();
            task.Id = ParseTaskMarker(linesEnumerator.Current, ref statuses);
            bool moveNext = linesEnumerator.MoveNext();

            while (moveNext)
            {
                // parse name
                if (CheckName(linesEnumerator.Current))
                {
                    var complete = false;
                    task.Name = ParseTaskName(linesEnumerator.Current, ref complete);
                    if (complete)
                        task.Tags.Add(DefaultTags.Complete);
                    moveNext = linesEnumerator.MoveNext();
                }
                // parse tag
                else if (CheckTag(linesEnumerator.Current))
                {
                    task.Tags.Add(ParseTaskTag(linesEnumerator.Current));
                    moveNext = linesEnumerator.MoveNext();
                }
                // parse description
                else if (CheckDescription(linesEnumerator.Current))
                    task.Description = ParseTaskDescription(ref linesEnumerator, ref moveNext);
                // parse child
                else if (CheckChild(linesEnumerator.Current))
                {
                    if (!children.ContainsKey(task.Id))
                        children[task.Id] = new List<string>();
                    children[task.Id].Add(ParseChildString(linesEnumerator.Current));
                    moveNext = linesEnumerator.MoveNext();
                }
                // parse parent
                else if (CheckParent(linesEnumerator.Current))
                {
                    if (!parents.ContainsKey(task.Id))
                        parents[task.Id] = new List<string>();
                    parents[task.Id].Add(ParseParentString(linesEnumerator.Current));
                    moveNext = linesEnumerator.MoveNext();
                }
                // parse deadlines
                else if (CheckDeadline(linesEnumerator.Current))
                    task.Deadlines.Add(ParseDeadline(ref moveNext, ref statuses, ref linesEnumerator, next, 0));
                // parse checklists
                else if (CheckChecklist(linesEnumerator.Current))
                    ParseChecklist(ref moveNext, ref statuses, ref linesEnumerator, task.Id, tasks, next, 0);
                else
                    break;
            }
            tasks.Add(task);
            Logger.Log(LogLevel.INFO, $"Read finished, task: {task.Id} | {task.Name}");
            return task.Id;
        }

        public Guid ReadSkipTask(ref IEnumerator<string> linesEnumerator, ref IList<TaskReadStatus> statuses)
        {
            var tasks = new List<PlanumTask>();
            var children = new Dictionary<Guid, IList<string>>();
            var parents = new Dictionary<Guid, IList<string>>();
            var next = new Dictionary<Guid, IList<string>>();
            return ReadTask(ref linesEnumerator, ref statuses, tasks, children, parents, next);
        }

        protected void ParseTaskIdentitesFromString(HashSet<Guid> identites, IEnumerable<string> stringValues, IEnumerable<PlanumTask> tasks)
        {
            foreach (var stringValue in stringValues)
            {
                string id;
                string name;
                RepoConfig.ParseTaskName(stringValue, out id, out name);
                var identified = TaskValueParser.ParseIdentity(id, name, tasks);
                foreach (var task in identified)
                    identites.Add(task.Id);
            }
        }

        public void ParseIdentities(IList<PlanumTask> tasks, Dictionary<Guid, IList<string>> children, Dictionary<Guid, IList<string>> parents, Dictionary<Guid, IList<string>> next)
        {
            foreach (var task in tasks)
            {
                if (children.ContainsKey(task.Id))
                    ParseTaskIdentitesFromString(task.Children, children[task.Id], tasks);
                if (parents.ContainsKey(task.Id))
                    ParseTaskIdentitesFromString(task.Parents, parents[task.Id], tasks);
                foreach (var deadline in task.Deadlines)
                {
                    if (next.ContainsKey(deadline.Id))
                        ParseTaskIdentitesFromString(deadline.next, next[deadline.Id], tasks);
                }
            }
        }
    }
}
