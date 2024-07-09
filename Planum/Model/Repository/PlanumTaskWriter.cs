using System;
using System.Collections.Generic;
using System.Linq;
using Planum.Config;
using Planum.Model.Entities;

namespace Planum.Model.Repository
{
    public class PlanumTaskWriter
    {
        AppConfig AppConfig { get; set; }
        RepoConfig RepoConfig { get; set; }

        public PlanumTaskWriter(AppConfig appConfig, RepoConfig repoConfig)
        {
            AppConfig = appConfig;
            RepoConfig = repoConfig;
        }

        public string GetTaskNameMarkerSymbol(PlanumTask task, IEnumerable<PlanumTask> tasks)
        {
            if (tasks.Where(x => x.Name == RepoConfig.OverdueTaskName && x.Children.Contains(x.Id)).Any())
                return RepoConfig.TaskOverdueMarkerSymbol;
            else if (tasks.Where(x => x.Name == RepoConfig.InProgressTaskName && x.Children.Contains(x.Id)).Any())
                return RepoConfig.TaskInProgressMarkerSymbol;
            else if (tasks.Where(x => x.Name == RepoConfig.WarningTaskName && x.Children.Contains(x.Id)).Any())
                return RepoConfig.TaskWarningMarkerSymbol;
            else if (tasks.Where(x => x.Name == RepoConfig.CompleteTaskName && x.Children.Contains(x.Id)).Any())
                return RepoConfig.TaskCompleteMarkerSymbol;
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

        public bool CheckIfNormalTask(PlanumTask task)
        {
            if (task.Name == RepoConfig.CompleteTaskName ||
                    task.Name == RepoConfig.ChecklistTaskName ||
                    task.Name == RepoConfig.WarningTaskName ||
                    task.Name == RepoConfig.InProgressTaskName ||
                    task.Name == RepoConfig.OverdueTaskName)
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

        public IEnumerable<string> WriteTaskHeader(IEnumerable<string> lines, PlanumTask task) => lines.Append(RepoConfig.TaskMarkerStartSymbol + task.Id.ToString() + RepoConfig.TaskMarkerEndSymbol);
        public IEnumerable<string> WriteName(IEnumerable<string> lines, PlanumTask task, IEnumerable<PlanumTask> tasks, int level = 0)
        {
            return lines.Append(
                    AddLineTabs(level) +
                    RepoConfig.TaskItemSymbol +
                    GetTaskNameMarkerSymbol(task, tasks) +
                    RepoConfig.TaskNameSymbol +
                    RepoConfig.TaskHeaderDelimeterSymbol +
                    task.Name);
        }

        public IEnumerable<string> WriteDescription(IEnumerable<string> lines, PlanumTask task, int level = 0)
        {
            return task.Description == "" ? lines.Append(
                    AddLineTabs(level) +
                    RepoConfig.TaskItemSymbol +
                    RepoConfig.TaskDescriptionSymbol +
                    RepoConfig.TaskHeaderDelimeterSymbol +
                    task.Description) : lines;
        }

        public IEnumerable<string> WriteChecklists(IEnumerable<string> lines, PlanumTask task, IEnumerable<PlanumTask> tasks, int level = 0)
        {
            var checklistGroupTasks = tasks.Where(x => x.Name == RepoConfig.ChecklistTaskName && x.Parents.Contains(task.Id));
            foreach (var checklist in tasks.Where(x => checklistGroupTasks.Where(y => y.Children.Contains(x.Id)).Any()))
            {
                // name
                lines = WriteName(lines, checklist, tasks, level);
                // description
                lines = WriteDescription(lines, checklist, level);
                // deadlines
                lines = WriteDeadlines(lines, checklist, level);
                // checklists
                lines = WriteChecklists(lines, checklist, tasks, level + 1);
            }
            return lines;
        }

        public IEnumerable<string> WriteChildren(IEnumerable<string> lines, PlanumTask task, IEnumerable<PlanumTask> tasks)
        {
            foreach (var child in tasks.Where(x => task.Children.Contains(x.Id)))
            {
                if (!CheckIfNormalTask(child))
                    continue;
                else if (tasks.Where(x => x.Id != child.Id && x.Name == child.Name).Any())
                    lines = lines.Append(
                            RepoConfig.TaskItemSymbol +
                            GetTaskNameMarkerSymbol(child, tasks) +
                            RepoConfig.TaskChildSymbol +
                            RepoConfig.TaskHeaderDelimeterSymbol +
                            child.Name +
                            RepoConfig.TaskNameIdDelimiter +
                            child.Id.ToString());
                else
                    lines = lines.Append(
                            RepoConfig.TaskItemSymbol +
                            GetTaskNameMarkerSymbol(child, tasks) +
                            RepoConfig.TaskChildSymbol +
                            RepoConfig.TaskHeaderDelimeterSymbol +
                            child.Name +
                            RepoConfig.TaskNameIdDelimiter +
                            child.Id.ToString());
            }
            return lines;
        }

        public IEnumerable<string> WriteParents(IEnumerable<string> lines, PlanumTask task, IEnumerable<PlanumTask> tasks)
        {
            foreach (var parent in tasks.Where(x => task.Parents.Contains(x.Id)))
            {
                if (!CheckIfNormalTask(parent))
                    continue;
                if (tasks.Where(x => x.Id != parent.Id && x.Name == parent.Name).Any())
                    lines = lines.Append(
                            RepoConfig.TaskItemSymbol +
                            GetTaskNameMarkerSymbol(parent, tasks) +
                            RepoConfig.TaskParentSymbol +
                            RepoConfig.TaskHeaderDelimeterSymbol +
                            parent.Name +
                            RepoConfig.TaskNameIdDelimiter +
                            parent.Id.ToString());
                else
                    lines = lines.Append(
                            RepoConfig.TaskItemSymbol +
                            GetTaskNameMarkerSymbol(parent, tasks) +
                            RepoConfig.TaskParentSymbol +
                            RepoConfig.TaskHeaderDelimeterSymbol +
                            parent.Name +
                            RepoConfig.TaskNameIdDelimiter +
                            parent.Id.ToString());
            }
            return lines;
        }

        public IEnumerable<string> WriteDeadlines(IEnumerable<string> lines, PlanumTask task, int level = 0)
        {
            foreach (var deadline in task.Deadlines)
            {
                // header
                lines = lines.Append(
                        AddLineTabs(level) +
                        RepoConfig.TaskItemSymbol +
                        GetDeadlineMarkerSymbol(deadline) +
                        RepoConfig.TaskDeadlineHeaderSymbol +
                        RepoConfig.TaskHeaderDelimeterSymbol
                        );
                // deadline
                lines = lines.Append(
                        AddLineTabs(level + 1) +
                        RepoConfig.TaskItemSymbol +
                        RepoConfig.TaskDeadlineSymbol +
                        RepoConfig.TaskHeaderDelimeterSymbol +
                        deadline.deadline.ToString("H:m d.M.y")
                        );
                // warning time
                if (deadline.warningTime != TimeSpan.Zero)
                    lines = lines.Append(
                            AddLineTabs(level + 1) +
                            RepoConfig.TaskItemSymbol +
                            RepoConfig.TaskWarningTimeSymbol +
                            RepoConfig.TaskHeaderDelimeterSymbol +
                            deadline.warningTime.ToString(@"d\.h\:m")
                            );
                // duration
                if (deadline.duration != TimeSpan.Zero)
                    lines = lines.Append(
                            AddLineTabs(level + 1) +
                            RepoConfig.TaskItemSymbol +
                            RepoConfig.TaskDurationTimeSymbol +
                            RepoConfig.TaskHeaderDelimeterSymbol +
                            deadline.duration.ToString(@"d\.h\:m")
                            );
                // repeat
                if (deadline.repeatSpan != TimeSpan.Zero && deadline.repeatMonths > 0 && deadline.repeatYears > 0)
                    lines = lines.Append(
                            AddLineTabs(level + 1) +
                            RepoConfig.TaskItemSymbol +
                            GetEnabledSymbol(deadline.repeated) +
                            RepoConfig.TaskRepeatTimeSymbol +
                            RepoConfig.TaskHeaderDelimeterSymbol +
                            deadline.repeatYears.ToString() + " " +
                            deadline.repeatMonths.ToString() + " " +
                            deadline.repeatSpan.ToString(@"d\.h\:m")
                            );
            }
            return lines;
        }

        public IEnumerable<string> WriteTask(IEnumerable<string> lines, PlanumTask task, IEnumerable<PlanumTask> tasks)
        {
            if (!CheckIfNormalTask(task))
                return lines;

            lines = WriteTaskHeader(lines, task);
            lines = WriteName(lines, task, tasks);
            lines = WriteDescription(lines, task);
            lines = WriteParents(lines, task, tasks);
            lines = WriteChildren(lines, task, tasks);
            lines = WriteChecklists(lines, task, tasks);
            lines = WriteDeadlines(lines, task);

            return lines;
        }
    }
}
