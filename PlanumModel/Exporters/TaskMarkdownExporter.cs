using Planum.Logger;
using Planum.Model.Entities;

namespace Planum.Model.Exporters
{
    public class TaskMarkdownExporter
    {
        protected ModelConfig ModelConfig { get; set; }
        protected ILoggerWrapper Logger { get; set; }

        public TaskMarkdownExporter(ModelConfig modelConfig, ILoggerWrapper logger)
        {
            ModelConfig = modelConfig;
            Logger = logger;
        }

        ///<summary>Add checkbox to task marker</summary>
        protected string AddCheckbox(string marker) => ModelConfig.TaskCheckboxStart + marker + ModelConfig.TaskCheckboxEnd;

        protected string AddMarkdownLink(string line, string path) => "[" + line + "](" + path + ")";
        protected string ParseMarkdownLink(string line, out string path)
        {
            path = "";
            var split = line.Trim('[', ')').Split("](");
            if (split.Length == 2)
            {
                path = split[1];
                return split[0];
            }
            else if (split.Length == 1)
                return split[0];
            return "";
        }

        protected string ParseMarkdownLink(string line)
        {
            var split = line.Trim('[', ')').Split("](");
            if (split.Length == 2)
            {
                return split[0];
            }
            else if (split.Length == 1)
                return split[0];
            return "";
        }

        protected void ParseTaskName(string line, out string id, out string name)
        {
            id = "";
            name = "";
            var split = line.Trim(' ', '\n').Split(ModelConfig.TaskNameIdDelimiter);
            if (split.Length == 1)
                name = split[0];
            else if (split.Length == 2)
            {
                name = split[0];
                id = split[1];
            }
        }

        protected string GetMarkerSymbol()
        {
            if (task.Tags.Contains(DefaultTags.Complete))
                return ModelConfig.TaskCompleteMarkerSymbol;
            else if (overdue.Contains(task.Id))
                return ModelConfig.TaskOverdueMarkerSymbol;
            else if (inProgress.Contains(task.Id))
                return ModelConfig.TaskInProgressMarkerSymbol;
            else if (warning.Contains(task.Id))
                return ModelConfig.TaskWarningMarkerSymbol;
            return ModelConfig.TaskNotCompleteMarkerSymbol;
        }

        protected string GetDeadlineMarkerSymbol(Deadline deadline)
        {
            if (!deadline.enabled)
                return ModelConfig.TaskCompleteMarkerSymbol;
            else if (deadline.Overdue())
                return ModelConfig.TaskOverdueMarkerSymbol;
            else if (deadline.InProgress())
                return ModelConfig.TaskInProgressMarkerSymbol;
            else if (deadline.Warning())
                return ModelConfig.TaskWarningMarkerSymbol;
            return ModelConfig.TaskNotCompleteMarkerSymbol;
        }

        protected string GetEnabledSymbol(bool enabled)
        {
            if (enabled)
                return ModelConfig.TaskCompleteMarkerSymbol;
            return ModelConfig.TaskNotCompleteMarkerSymbol;
        }

        public string GetTaskName(PlanumTask task, IEnumerable<PlanumTask> tasks)
        {
            if (tasks.Where(x => x.Id != task.Id && x.Name == task.Name).Any())
                return RepoConfig.AddMarkdownLink(task.Name +
                       RepoConfig.TaskNameIdDelimiter +
                       task.Id.ToString(), task.SaveFile);
            return RepoConfig.AddMarkdownLink(task.Name, task.SaveFile);
        }

        protected bool CheckIfChecklist(PlanumTask task)
        {
            if (task.Tags.Contains(DefaultTags.Checklist))
                return true;
            return false;
        }

        protected string AddLineTabs(int level = 0)
        {
            var tabs = "";
            for (int i = 0; i < level; i++)
                tabs += RepoConfig.TaskItemTabSymbol;
            return tabs;
        }

        protected void WriteTaskHeader(IList<string> lines, PlanumTask task) => lines.Add(RepoConfig.TaskMarkerStartSymbol + task.Id.ToString() + RepoConfig.TaskMarkerEndSymbol);
        protected void WriteName(IList<string> lines, PlanumTask task, IEnumerable<PlanumTask> tasks, IEnumerable<Guid> overdue, IEnumerable<Guid> inProgress, IEnumerable<Guid> warning, int level = 0)
        {
            var line = AddLineTabs(level) +
                RepoConfig.TaskItemSymbol +
                RepoConfig.AddCheckbox(GetTaskNameMarkerSymbol(task, overdue, inProgress, warning)) +
                RepoConfig.TaskNameSymbol +
                RepoConfig.TaskHeaderDelimeterSymbol +
                task.Name;
            if (tasks.Where(x => x.Id != task.Id && x.Name == task.Name).Any())
                line += RepoConfig.TaskNameIdDelimiter + task.Id.ToString();
            lines.Add(line);
        }

        protected void WriteChecklistName(IList<string> lines, PlanumTask task, IEnumerable<PlanumTask> tasks, IEnumerable<Guid> overdue, IEnumerable<Guid> inProgress, IEnumerable<Guid> warning, int level = 0)
        {
            lines.Add(
                AddLineTabs(level) +
                RepoConfig.TaskItemSymbol +
                RepoConfig.AddCheckbox(GetTaskNameMarkerSymbol(task, overdue, inProgress, warning)) +
                task.Name);
        }

        protected void WriteTags(IList<string> lines, PlanumTask task, int level = 0)
        {
            foreach (var tag in task.Tags)
            {
                if (tag != DefaultTags.Checklist && tag != DefaultTags.Complete)
                    lines.Add(
                            AddLineTabs(level) +
                            RepoConfig.TaskItemSymbol +
                            RepoConfig.TaskTagSymbol +
                            RepoConfig.TaskHeaderDelimeterSymbol +
                            tag);
            }
        }

        protected void WriteDescription(IList<string> lines, PlanumTask task, int level = 0)
        {
            if (task.Description != string.Empty)
            {
                var descriptionLines = task.Description.Split(RepoConfig.TaskDescriptionNewlineSymbol);

                if (descriptionLines.Count() <= 0)
                    return;

                var tmpLine = AddLineTabs(level) +
                    RepoConfig.TaskItemSymbol +
                    RepoConfig.TaskDescriptionSymbol +
                    RepoConfig.TaskHeaderDelimeterSymbol +
                    descriptionLines[0];

                if (descriptionLines.Count() > 1)
                    tmpLine = tmpLine + RepoConfig.TaskDescriptionNewlineSymbol;
                lines.Add(tmpLine);
                if (descriptionLines.Count() == 1)
                    return;

                for (int i = 1; i < descriptionLines.Count() - 1; i++)
                    lines.Add(AddLineTabs(level + 1) +
                        descriptionLines[i] +
                        RepoConfig.TaskDescriptionNewlineSymbol
                    );

                lines.Add(AddLineTabs(level + 1) +
                    descriptionLines[descriptionLines.Length - 1]
                );
            }
        }

        protected void WriteChecklists(IList<string> lines, PlanumTask task, IEnumerable<PlanumTask> tasks, IEnumerable<Guid> overdue, IEnumerable<Guid> inProgress, IEnumerable<Guid> warning, int level = 0)
        {
            foreach (var checklist in tasks.Where(x => x.Tags.Contains(DefaultTags.Checklist) && task.Children.Contains(x.Id)))
            {
                // name
                WriteChecklistName(lines, checklist, tasks, overdue, inProgress, warning, level);
                // tags
                WriteTags(lines, task, level + 1);
                // description
                WriteDescription(lines, checklist, level + 1);
                // deadlines
                WriteDeadlines(lines, checklist, tasks, overdue, inProgress, warning, level + 1);
                // checklists
                WriteChecklists(lines, checklist, tasks, overdue, inProgress, warning, level + 1);
            }
        }

        protected void WriteChildren(IList<string> lines, PlanumTask task, IEnumerable<PlanumTask> tasks, IEnumerable<Guid> overdue, IEnumerable<Guid> inProgress, IEnumerable<Guid> warning)
        {
            foreach (var child in tasks.Where(x => task.Children.Contains(x.Id)))
            {
                if (CheckIfChecklist(child))
                    continue;
                else
                    lines.Add(
                        RepoConfig.TaskItemSymbol +
                        RepoConfig.AddCheckbox(GetTaskNameMarkerSymbol(child, overdue, inProgress, warning)) +
                        RepoConfig.TaskChildSymbol +
                        RepoConfig.TaskHeaderDelimeterSymbol +
                        GetTaskName(child, tasks)
                        );
            }
        }

        protected void WriteParents(IList<string> lines, PlanumTask task, IEnumerable<PlanumTask> tasks, IEnumerable<Guid> overdue, IEnumerable<Guid> inProgress, IEnumerable<Guid> warning)
        {
            foreach (var parent in tasks.Where(x => task.Parents.Contains(x.Id)))
            {
                if (CheckIfChecklist(parent))
                    continue;
                else
                    lines.Add(
                        RepoConfig.TaskItemSymbol +
                        RepoConfig.AddCheckbox(GetTaskNameMarkerSymbol(parent, overdue, inProgress, warning)) +
                        RepoConfig.TaskParentSymbol +
                        RepoConfig.TaskHeaderDelimeterSymbol +
                        GetTaskName(parent, tasks)
                        );
            }
        }

        protected void WriteNext(IList<string> lines, Deadline deadline, IEnumerable<PlanumTask> tasks, IEnumerable<Guid> overdue, IEnumerable<Guid> inProgress, IEnumerable<Guid> warning, int level = 0)
        {
            var nextTasks = tasks.Where(x => deadline.next.Contains(x.Id));
            foreach (var next in nextTasks)
            {
                string nextStr = GetTaskName(next, tasks);
                lines.Add(AddLineTabs(level + 1) +
                    RepoConfig.TaskItemSymbol +
                    RepoConfig.AddCheckbox(GetTaskNameMarkerSymbol(next, overdue, inProgress, warning)) +
                    RepoConfig.TaskNextSymbol +
                    RepoConfig.TaskHeaderDelimeterSymbol +
                    nextStr);
            }
        }

        protected void WriteDeadlines(IList<string> lines, PlanumTask task, IEnumerable<PlanumTask> tasks, IEnumerable<Guid> overdue, IEnumerable<Guid> inProgress, IEnumerable<Guid> warning, int level = 0)
        {
            foreach (var deadline in task.Deadlines)
            {
                // header
                lines.Add(
                    AddLineTabs(level) +
                    RepoConfig.TaskItemSymbol +
                    RepoConfig.AddCheckbox(GetDeadlineMarkerSymbol(deadline)) +
                    RepoConfig.TaskDeadlineHeaderSymbol +
                    RepoConfig.TaskHeaderDelimeterSymbol +
                    deadline.Id.ToString()
                    );
                // deadline
                lines.Add(
                    AddLineTabs(level + 1) +
                    RepoConfig.TaskItemSymbol +
                    RepoConfig.TaskDeadlineSymbol +
                    RepoConfig.TaskHeaderDelimeterSymbol +
                    deadline.deadline.ToString(RepoConfig.TaskDateTimeWriteFormat)
                    );
                // warning time
                if (deadline.warningTime != TimeSpan.Zero)
                    lines.Add(
                        AddLineTabs(level + 1) +
                        RepoConfig.TaskItemSymbol +
                        RepoConfig.TaskWarningTimeSymbol +
                        RepoConfig.TaskHeaderDelimeterSymbol +
                        deadline.warningTime.ToString(RepoConfig.TaskTimeSpanWriteFormat)
                        );
                // duration
                if (deadline.duration != TimeSpan.Zero)
                    lines.Add(
                        AddLineTabs(level + 1) +
                        RepoConfig.TaskItemSymbol +
                        RepoConfig.TaskDurationTimeSymbol +
                        RepoConfig.TaskHeaderDelimeterSymbol +
                        deadline.duration.ToString(RepoConfig.TaskTimeSpanWriteFormat)
                        );
                // repeat
                if (deadline.repeatSpan != TimeSpan.Zero && deadline.repeatMonths > 0 && deadline.repeatYears > 0)
                    lines.Add(
                        AddLineTabs(level + 1) +
                        RepoConfig.TaskItemSymbol +
                        RepoConfig.AddCheckbox(GetEnabledSymbol(deadline.repeated)) +
                        RepoConfig.TaskRepeatTimeSymbol +
                        RepoConfig.TaskHeaderDelimeterSymbol +
                        deadline.repeatYears.ToString() + " " +
                        deadline.repeatMonths.ToString() + " " +
                        deadline.repeatSpan.ToString(RepoConfig.TaskTimeSpanWriteFormat)
                        );
                // next
                WriteNext(lines, deadline, tasks, overdue, inProgress, warning, level);
            }
        }

        public void WriteTask(IList<string> lines, PlanumTask task, IEnumerable<PlanumTask> tasks)
        {
            Logger.Log($"Starting task write: {task.Id} | {task.Name}", LogLevel.INFO);
            if (CheckIfChecklist(task))
                return;

            IEnumerable<Guid> overdue;
            IEnumerable<Guid> inProgress;
            IEnumerable<Guid> warning;

            PlanumTask.CalculateTimeConstraints(tasks, out overdue, out inProgress, out warning);

            WriteTaskHeader(lines, task);
            WriteName(lines, task, tasks, overdue, inProgress, warning);
            WriteTags(lines, task);
            WriteDescription(lines, task);
            WriteParents(lines, task, tasks, overdue, inProgress, warning);
            WriteChildren(lines, task, tasks, overdue, inProgress, warning);
            WriteDeadlines(lines, task, tasks, overdue, inProgress, warning);
            WriteChecklists(lines, task, tasks, overdue, inProgress, warning);
            lines.Add("");

            Logger.Log($"Task write finished", LogLevel.INFO);
            return;
        }
    }
}
