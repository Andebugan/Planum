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

        public void WriteTask(IList<string> lines, PlanumTask task, IEnumerable<PlanumTask> tasks)
        {
            Logger.Log($"Starting task write: {task.Id} | {task.Name}", LogLevel.INFO);
            if (CheckIfChecklist(task))
                return;

            var statuses = PlanumTask.GetTaskStatuses(tasks);

            WriteTaskHeader(lines, task);
            WriteName(lines, task, tasks, statuses[task.Id]);
            WriteTags(lines, task);
            WriteDescription(lines, task);
            WriteParents(lines, task, tasks, statuses);
            WriteChildren(lines, task, tasks, statuses);
            WriteDeadlines(lines, task, tasks, statuses);
            WriteChecklists(lines, task, tasks, statuses);
            lines.Add("");

            Logger.Log($"Task write finished", LogLevel.INFO);
            return;
        }

        ///<summary>Add checkbox to task marker</summary>
        protected string AddCheckbox(string marker) => ModelConfig.TaskCheckboxStart + marker + ModelConfig.TaskCheckboxEnd;

        protected string AddMarkdownLink(string path) => ModelConfig.TaskLinkSymbol + "(" + path + ")";

        protected string GetMarkerSymbol(PlanumTaskStatus status)
        {
            if (status == PlanumTaskStatus.COMPLETE)
                return ModelConfig.TaskCompleteMarkerSymbol;
            else if (status == PlanumTaskStatus.OVERDUE)
                return ModelConfig.TaskOverdueMarkerSymbol;
            else if (status == PlanumTaskStatus.IN_PROGRESS)
                return ModelConfig.TaskInProgressMarkerSymbol;
            else if (status == PlanumTaskStatus.WARNING)
                return ModelConfig.TaskWarningMarkerSymbol;
            return ModelConfig.TaskNotCompleteMarkerSymbol;
        }

        protected string GetTaskName(PlanumTask task, IEnumerable<PlanumTask> tasks, bool uniqueName = true)
        {
            var name = task.Name;
            if (tasks.Any(x => x.Id != task.Id && x.Name == task.Name))
                name += ModelConfig.TaskValueIdDelimiter + task.Id.ToString();
            name += " " + AddMarkdownLink(task.SaveFile);
            return name;
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
                tabs += ModelConfig.TaskItemTabSymbol;
            return tabs;
        }

        protected string AddTaskItem(string symbol, string value, int level = 0, PlanumTaskStatus? status = null)
        {
            var line = AddLineTabs(level) + ModelConfig.TaskItemSymbol;
            if (status is not null)
                line += AddCheckbox(GetMarkerSymbol((PlanumTaskStatus)status));
            if (symbol != "")
                line += symbol + ModelConfig.TaskHeaderDelimeterSymbol;
            line += value;
            return line;
        }

        protected void WriteTaskHeader(IList<string> lines, PlanumTask task) => lines.Add(ModelConfig.TaskMarkerStartSymbol + task.Id.ToString() + ModelConfig.TaskMarkerEndSymbol);

        protected void WriteName(IList<string> lines, PlanumTask task, IEnumerable<PlanumTask> tasks, PlanumTaskStatus status, int level = 0)
        {
            lines.Add(AddTaskItem(ModelConfig.TaskNameSymbol, GetTaskName(task, tasks), level, status));
        }

        protected void WriteChecklistName(IList<string> lines, PlanumTask task, PlanumTaskStatus status, int level = 0)
        {
            lines.Add(AddTaskItem("", task.Name, level, status));
        }

        protected void WriteTags(IList<string> lines, PlanumTask task, int level = 0)
        {
            var tags = task.Tags.Except(new string[] { DefaultTags.Checklist });
            if (tags.Any())
                lines.Add(AddTaskItem(ModelConfig.TaskTagSymbol, string.Join(", ", tags), level));
        }

        protected void WriteDescription(IList<string> lines, PlanumTask task, int level = 0)
        {
            if (task.Description.Trim() != "")
                lines.Add(AddTaskItem(ModelConfig.TaskDescriptionSymbol, task.Description, level));
        }

        protected void WriteChecklists(IList<string> lines, PlanumTask task, IEnumerable<PlanumTask> tasks, Dictionary<Guid, PlanumTaskStatus> statuses, int level = 0)
        {
            foreach (var checklist in tasks.Where(x => x.Tags.Contains(DefaultTags.Checklist) && task.Children.Contains(x.Id)))
            {
                // name
                WriteChecklistName(lines, checklist, statuses[checklist.Id], level);
                // description
                WriteDescription(lines, checklist, level + 1);
                // deadlines
                WriteDeadlines(lines, checklist, tasks, statuses, level + 1);
                // checklists
                WriteChecklists(lines, checklist, tasks, statuses, level + 1);
            }
        }

        protected void WriteChildren(IList<string> lines, PlanumTask task, IEnumerable<PlanumTask> tasks, Dictionary<Guid, PlanumTaskStatus> statuses, int level = 0)
        {
            foreach (var child in tasks.Where(x => task.Children.Contains(x.Id)))
                if (CheckIfChecklist(child))
                    continue;
                else
                    lines.Add(AddTaskItem(ModelConfig.TaskChildSymbol, GetTaskName(child, tasks), level, statuses[child.Id]));
        }

        protected void WriteParents(IList<string> lines, PlanumTask task, IEnumerable<PlanumTask> tasks, Dictionary<Guid, PlanumTaskStatus> statuses, int level = 0)
        {
            foreach (var parent in tasks.Where(x => task.Parents.Contains(x.Id)))
                if (CheckIfChecklist(parent))
                    continue;
                else
                    lines.Add(AddTaskItem(ModelConfig.TaskParentSymbol, GetTaskName(parent, tasks), level, statuses[parent.Id]));
        }

        protected void WriteNext(IList<string> lines, Deadline deadline, IEnumerable<PlanumTask> tasks, Dictionary<Guid, PlanumTaskStatus> statuses, int level = 0)
        {
            foreach (var next in tasks.Where(x => deadline.next.Contains(x.Id)))
                lines.Add(AddTaskItem(ModelConfig.TaskNextSymbol, GetTaskName(next, tasks), level, statuses[next.Id]));
        }

        protected void WriteDeadlines(IList<string> lines, PlanumTask task, IEnumerable<PlanumTask> tasks, Dictionary<Guid, PlanumTaskStatus> statuses, int level = 0)
        {
            foreach (var deadline in task.Deadlines)
            {
                // header
                lines.Add(AddTaskItem(
                            ModelConfig.TaskDeadlineHeaderSymbol,
                            $"{deadline.deadline.ToString(ModelConfig.TaskDateTimeWriteFormat)}{ModelConfig.TaskValueIdDelimiter}{deadline.Id.ToString()}",
                            level, deadline.GetDeadlineStatus()));
                // warning time
                if (deadline.warningTime != TimeSpan.Zero)
                    lines.Add(AddTaskItem(ModelConfig.TaskWarningTimeSymbol, deadline.warningTime.ToString(ModelConfig.TaskTimeSpanWriteFormat), level + 1));
                // duration
                if (deadline.duration != TimeSpan.Zero)
                    lines.Add(AddTaskItem(ModelConfig.TaskDurationTimeSymbol, deadline.duration.ToString(ModelConfig.TaskTimeSpanWriteFormat), level + 1));
                // repeat
                if (deadline.repeatSpan.Span != TimeSpan.Zero && deadline.repeatSpan.Months > 0 && deadline.repeatSpan.Months > 0)
                {
                    var status = PlanumTaskStatus.DISABLED;
                    if (deadline.repeated)
                        status = PlanumTaskStatus.COMPLETE;
                    string repeat = deadline.repeatSpan.ToString();
                    lines.Add(AddTaskItem(ModelConfig.TaskRepeatTimeSymbol, repeat, level + 1, status));
                }
                // next
                WriteNext(lines, deadline, tasks, statuses, level + 1);
            }
        }
    }
}
