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

        protected string GetTaskName(PlanumTask task) => AddMarkdownLink(task.Name, task.SaveFile);

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
            line += symbol + ModelConfig.TaskHeaderDelimeterSymbol + value;
            return line;
        }

        protected void WriteTaskHeader(IList<string> lines, PlanumTask task) => lines.Add(ModelConfig.TaskMarkerStartSymbol + task.Id.ToString() + ModelConfig.TaskMarkerEndSymbol);

        protected void WriteName(IList<string> lines, PlanumTask task, PlanumTaskStatus status, int level = 0)
        {
            if (level > 0)
                lines.Add(AddTaskItem(ModelConfig.TaskNameSymbol, task.Name + ModelConfig.TaskNameIdDelimiter + task.Id, level, status));
            else
                lines.Add(AddTaskItem(ModelConfig.TaskNameSymbol, task.Name, level, status));
        }

        protected void WriteChecklistName(IList<string> lines, PlanumTask task, PlanumTaskStatus status, int level = 0) => lines.Add(AddTaskItem("", task.Name, level, status));

        protected void WriteTags(IList<string> lines, PlanumTask task, int level = 0)
        {
            foreach (var tag in task.Tags)
                if (tag != DefaultTags.Checklist && tag != DefaultTags.Complete)
                    lines.Add(AddTaskItem(ModelConfig.TaskTagSymbol, tag, level));
        }

        protected void WriteDescription(IList<string> lines, PlanumTask task, int level = 0) => lines.Add(AddTaskItem(ModelConfig.TaskDescriptionSymbol, task.Description, level));

        protected void WriteChecklists(IList<string> lines, PlanumTask task, IEnumerable<PlanumTask> tasks, Dictionary<Guid, PlanumTaskStatus> statuses, int level = 0)
        {
            foreach (var checklist in tasks.Where(x => x.Tags.Contains(DefaultTags.Checklist) && task.Children.Contains(x.Id)))
            {
                // name
                WriteChecklistName(lines, checklist, statuses[checklist.Id], level);
                // tags
                WriteTags(lines, task, level + 1);
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
                    lines.Add(AddTaskItem(ModelConfig.TaskChildSymbol, GetTaskName(child), level, statuses[child.Id]));
        }

        protected void WriteParents(IList<string> lines, PlanumTask task, IEnumerable<PlanumTask> tasks, Dictionary<Guid, PlanumTaskStatus> statuses, int level = 0)
        {
            foreach (var parent in tasks.Where(x => task.Parents.Contains(x.Id)))
                if (CheckIfChecklist(parent))
                    continue;
                else
                    lines.Add(AddTaskItem(ModelConfig.TaskParentSymbol, GetTaskName(parent), level, statuses[parent.Id]));
        }

        protected void WriteNext(IList<string> lines, Deadline deadline, IEnumerable<PlanumTask> tasks, Dictionary<Guid, PlanumTaskStatus> statuses, int level = 0)
        {
            foreach (var next in tasks.Where(x => deadline.next.Contains(x.Id)))
                lines.Add(AddTaskItem(ModelConfig.TaskNextSymbol, GetTaskName(next), level, statuses[next.Id]));
        }

        protected void WriteDeadlines(IList<string> lines, PlanumTask task, IEnumerable<PlanumTask> tasks, Dictionary<Guid, PlanumTaskStatus> statuses, int level = 0)
        {
            foreach (var deadline in task.Deadlines)
            {
                // header
                lines.Add(AddTaskItem(ModelConfig.TaskDeadlineHeaderSymbol, deadline.Id.ToString(), level, deadline.GetDeadlineStatus()));
                // deadline
                lines.Add(AddTaskItem(ModelConfig.TaskDeadlineSymbol, deadline.deadline.ToString(ModelConfig.TaskDateTimeWriteFormat), level + 1, deadline.GetDeadlineStatus()));
                // warning time
                if (deadline.warningTime != TimeSpan.Zero)
                    lines.Add(AddTaskItem(ModelConfig.TaskWarningTimeSymbol, deadline.warningTime.ToString(ModelConfig.TaskTimeSpanWriteFormat), level + 1, deadline.GetDeadlineStatus()));
                // duration
                if (deadline.duration != TimeSpan.Zero)
                    lines.Add(AddTaskItem(ModelConfig.TaskDurationTimeSymbol, deadline.duration.ToString(ModelConfig.TaskTimeSpanWriteFormat), level + 1, deadline.GetDeadlineStatus()));
                // repeat
                if (deadline.repeatSpan != TimeSpan.Zero && deadline.repeatMonths > 0 && deadline.repeatYears > 0)
                    lines.Add(AddTaskItem(ModelConfig.TaskRepeatTimeSymbol, deadline.repeatYears.ToString() + " " + deadline.repeatMonths.ToString() + " " + deadline.repeatSpan.ToString(ModelConfig.TaskTimeSpanWriteFormat), level + 1, deadline.GetDeadlineStatus()));
                // next
                WriteNext(lines, deadline, tasks, statuses, level);
            }
        }

        public void WriteTask(IList<string> lines, PlanumTask task, IEnumerable<PlanumTask> tasks)
        {
            Logger.Log($"Starting task write: {task.Id} | {task.Name}", LogLevel.INFO);
            if (CheckIfChecklist(task))
                return;

            var statuses = PlanumTask.GetTaskStatuses(tasks);

            WriteTaskHeader(lines, task);
            WriteName(lines, task, statuses[task.Id]);
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
    }
}
