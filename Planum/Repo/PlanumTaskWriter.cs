using System;
using System.Collections.Generic;
using System.Linq;
using Planum.Config;
using Planum.Model.Entities;
using Planum.Model.Repository;

namespace Planum.Repository
{
    public class PlanumTaskWriter: IPlanumTaskWriter
    {
        RepoConfig RepoConfig { get; set; }
        AppConfig AppConfig { get; set; }

        public PlanumTaskWriter(AppConfig appConfig, RepoConfig repoConfig)
        {
            AppConfig = appConfig;
            RepoConfig = repoConfig;
        }

        public string GetTaskNameMarkerSymbol(PlanumTask task, IEnumerable<Guid> overdue, IEnumerable<Guid> inProgress, IEnumerable<Guid> warning)
        {
            if (task.Tags.Contains(DefaultTags.Complete))
                return RepoConfig.TaskCompleteMarkerSymbol;
            else if (overdue.Contains(task.Id))
                return RepoConfig.TaskOverdueMarkerSymbol;
            else if (inProgress.Contains(task.Id))
                return RepoConfig.TaskInProgressMarkerSymbol;
            else if (warning.Contains(task.Id))
                return RepoConfig.TaskWarningMarkerSymbol;
            return RepoConfig.TaskNotCompleteMarkerSymbol;
        }

        public string GetDeadlineMarkerSymbol(Deadline deadline)
        {
            if (deadline.Overdue())
                return RepoConfig.TaskOverdueMarkerSymbol;
            else if (deadline.InProgress())
                return RepoConfig.TaskInProgressMarkerSymbol;
            else if (deadline.Warning())
                return RepoConfig.TaskWarningMarkerSymbol;
            else if (!deadline.enabled)
                return RepoConfig.TaskCompleteMarkerSymbol;
            return RepoConfig.TaskNotCompleteMarkerSymbol;
        }

        public string GetEnabledSymbol(bool enabled)
        {
            if (enabled)
                return RepoConfig.TaskCompleteMarkerSymbol;
            return RepoConfig.TaskNotCompleteMarkerSymbol;
        }

        public string GetTaskName(PlanumTask task, IEnumerable<PlanumTask> tasks)
        {
            if (tasks.Where(x => x.Id != task.Id && x.Name == task.Name).Any())
                return RepoConfig.AddMarkdownLink(task.Name +
                       RepoConfig.TaskNameIdDelimiter +
                       task.Id.ToString(), RepoConfig.GetTaskPath(task.Id));
            return RepoConfig.AddMarkdownLink(task.Name,
                   RepoConfig.GetTaskPath(task.Id));
        }

        public bool CheckIfChecklist(PlanumTask task)
        {
            if (task.Tags.Contains(DefaultTags.Checklist))
                return false;
            return true;
        }

        public string AddLineTabs(int level = 0)
        {
            var tabs = "";
            for (int i = 0; i < level; i++)
                tabs += RepoConfig.TaskItemTabSymbol;
            return tabs;
        }

        public void WriteTaskHeader(IList<string> lines, PlanumTask task) => lines.Add(RepoConfig.TaskMarkerStartSymbol + task.Id.ToString() + RepoConfig.TaskMarkerEndSymbol);
        public void WriteName(IList<string> lines, PlanumTask task, IEnumerable<PlanumTask> tasks, IEnumerable<Guid> overdue, IEnumerable<Guid> inProgress, IEnumerable<Guid> warning, int level = 0)
        {
            lines.Add(
                AddLineTabs(level) +
                RepoConfig.TaskItemSymbol +
                GetTaskNameMarkerSymbol(task, overdue, inProgress, warning) +
                RepoConfig.TaskNameSymbol +
                RepoConfig.TaskHeaderDelimeterSymbol +
                task.Name);
        }

        public void WriteDescription(IList<string> lines, PlanumTask task, int level = 0)
        {
            if (task.Description == "")
                lines.Add(
                    AddLineTabs(level) +
                    RepoConfig.TaskItemSymbol +
                    RepoConfig.TaskDescriptionSymbol +
                    RepoConfig.TaskHeaderDelimeterSymbol +
                    task.Description);
        }

        public void WriteChecklists(IList<string> lines, PlanumTask task, IEnumerable<PlanumTask> tasks, IEnumerable<Guid> overdue, IEnumerable<Guid> inProgress, IEnumerable<Guid> warning, int level = 0)
        {
            foreach (var checklist in tasks.Where(x => x.Tags.Contains(DefaultTags.Checklist) && task.Children.Contains(x.Id)))
            {
                // name
                WriteName(lines, checklist, tasks, overdue, inProgress, warning, level);
                // description
                WriteDescription(lines, checklist, level);
                // deadlines
                WriteDeadlines(lines, checklist, tasks, overdue, inProgress, warning, level);
                // checklists
                WriteChecklists(lines, checklist, tasks, overdue, inProgress, warning, level + 1);
            }
        }

        public void WriteChildren(IList<string> lines, PlanumTask task, IEnumerable<PlanumTask> tasks, IEnumerable<Guid> overdue, IEnumerable<Guid> inProgress, IEnumerable<Guid> warning)
        {
            foreach (var child in tasks.Where(x => task.Children.Contains(x.Id)))
            {
                if (!CheckIfChecklist(child))
                    continue;
                else
                    lines.Add(
                        RepoConfig.TaskItemSymbol +
                        GetTaskNameMarkerSymbol(child, overdue, inProgress, warning) +
                        RepoConfig.TaskParentSymbol +
                        RepoConfig.TaskHeaderDelimeterSymbol +
                        GetTaskName(child, tasks)
                        );
            }
        }

        public void WriteParents(IList<string> lines, PlanumTask task, IEnumerable<PlanumTask> tasks, IEnumerable<Guid> overdue, IEnumerable<Guid> inProgress, IEnumerable<Guid> warning)
        {
            foreach (var parent in tasks.Where(x => task.Parents.Contains(x.Id)))
            {
                if (!CheckIfChecklist(parent))
                    continue;
                else
                    lines.Add(
                        RepoConfig.TaskItemSymbol +
                        GetTaskNameMarkerSymbol(parent, overdue, inProgress, warning) +
                        RepoConfig.TaskParentSymbol +
                        RepoConfig.TaskHeaderDelimeterSymbol +
                        GetTaskName(parent, tasks)
                        );
            }
        }

        public void WriteNext(IList<string> lines, Deadline deadline, IEnumerable<PlanumTask> tasks, IEnumerable<Guid> overdue, IEnumerable<Guid> inProgress, IEnumerable<Guid> warning, int level = 0)
        {
            var nextTasks = tasks.Where(x => deadline.next.Contains(x.Id));
            foreach (var next in nextTasks)
            {
                string nextStr = GetTaskName(next, tasks);
                lines.Add(AddLineTabs(level + 1) +
                    RepoConfig.TaskItemSymbol +
                    GetTaskNameMarkerSymbol(next, overdue, inProgress, warning) +
                    RepoConfig.TaskNextSymbol + 
                    RepoConfig.TaskHeaderDelimeterSymbol +
                    nextStr);
            }
        }

        public void WriteDeadlines(IList<string> lines, PlanumTask task, IEnumerable<PlanumTask> tasks, IEnumerable<Guid> overdue, IEnumerable<Guid> inProgress, IEnumerable<Guid> warning, int level = 0)
        {
            foreach (var deadline in task.Deadlines)
            {
                // header
                lines.Append(
                    AddLineTabs(level) +
                    RepoConfig.TaskItemSymbol +
                    GetDeadlineMarkerSymbol(deadline) +
                    RepoConfig.TaskDeadlineHeaderSymbol +
                    RepoConfig.TaskHeaderDelimeterSymbol +
                    deadline.Id.ToString()
                    );
                // deadline
                lines.Append(
                    AddLineTabs(level + 1) +
                    RepoConfig.TaskItemSymbol +
                    RepoConfig.TaskDeadlineSymbol +
                    RepoConfig.TaskHeaderDelimeterSymbol +
                    deadline.deadline.ToString("H:m d.M.y")
                    );
                // warning time
                if (deadline.warningTime != TimeSpan.Zero)
                    lines.Append(
                        AddLineTabs(level + 1) +
                        RepoConfig.TaskItemSymbol +
                        RepoConfig.TaskWarningTimeSymbol +
                        RepoConfig.TaskHeaderDelimeterSymbol +
                        deadline.warningTime.ToString(@"d\.h\:m")
                        );
                // duration
                if (deadline.duration != TimeSpan.Zero)
                    lines.Append(
                        AddLineTabs(level + 1) +
                        RepoConfig.TaskItemSymbol +
                        RepoConfig.TaskDurationTimeSymbol +
                        RepoConfig.TaskHeaderDelimeterSymbol +
                        deadline.duration.ToString(@"d\.h\:m")
                        );
                // repeat
                if (deadline.repeatSpan != TimeSpan.Zero && deadline.repeatMonths > 0 && deadline.repeatYears > 0)
                    lines.Append(
                        AddLineTabs(level + 1) +
                        RepoConfig.TaskItemSymbol +
                        GetEnabledSymbol(deadline.repeated) +
                        RepoConfig.TaskRepeatTimeSymbol +
                        RepoConfig.TaskHeaderDelimeterSymbol +
                        deadline.repeatYears.ToString() + " " +
                        deadline.repeatMonths.ToString() + " " +
                        deadline.repeatSpan.ToString(@"d\.h\:m")
                        );
                // next
                WriteNext(lines, deadline, tasks, overdue, inProgress, warning, level);
            }
        }

        public void WriteTask(IList<string> lines, PlanumTask task, IEnumerable<PlanumTask> tasks)
        {
            if (!CheckIfChecklist(task))
                return;

            IEnumerable<Guid> overdue;
            IEnumerable<Guid> inProgress;
            IEnumerable<Guid> warning;

            PlanumTask.CalculateTimeConstraints(tasks, out overdue, out inProgress, out warning);

            WriteTaskHeader(lines, task);
            WriteName(lines, task, tasks, overdue, inProgress, warning);
            WriteDescription(lines, task);
            WriteParents(lines, task, tasks, overdue, inProgress, warning);
            WriteChildren(lines, task, tasks, overdue, inProgress, warning);
            WriteDeadlines(lines, task, tasks, overdue, inProgress, warning);
            WriteChecklists(lines, task, tasks, overdue, inProgress, warning);

            return;
        }
    }
}
