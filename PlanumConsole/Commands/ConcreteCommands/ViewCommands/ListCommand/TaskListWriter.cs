using System;
using System.Collections.Generic;
using System.Linq;
using Planum.Config;
using Planum.Logger;
using Planum.Model.Entities;

namespace Planum.Console.Commands.View
{
    public class TaskListWriter
    {
        RepoConfig RepoConfig { get; set; }
        ILoggerWrapper Logger { get; set; }

        public TaskListWriter(ILoggerWrapper logger, RepoConfig repoConfig)
        {
            Logger = logger;
            RepoConfig = repoConfig;
        }

        protected string AddStatusColor(string line, PlanumTask task, IEnumerable<Guid> overdue, IEnumerable<Guid> inProgress, IEnumerable<Guid> warning)
        {
            if (task.Tags.Contains(DefaultTags.Complete))
                return ConsoleSpecial.AddStyle(line, style: TextStyle.Strikethrough, foregroundColor: ConsoleInfoColors.Success);
            else if (overdue.Contains(task.Id))
                return ConsoleSpecial.AddStyle(line, style: TextStyle.Bold, foregroundColor: ConsoleInfoColors.Error);
            else if (inProgress.Contains(task.Id))
                return ConsoleSpecial.AddStyle(line, style: TextStyle.Bold, foregroundColor: ConsoleInfoColors.Warning);
            else if (warning.Contains(task.Id))
                return ConsoleSpecial.AddStyle(line, style: TextStyle.Dim, foregroundColor: ConsoleInfoColors.Warning);
            return line;
        }

        protected string GetTaskNameMarkerSymbol(PlanumTask task, IEnumerable<Guid> overdue, IEnumerable<Guid> inProgress, IEnumerable<Guid> warning)
        {
            if (task.Tags.Contains(DefaultTags.Complete))
                return ConsoleSpecial.AddStyle(RepoConfig.TaskCompleteMarkerSymbol, style: TextStyle.Strikethrough, foregroundColor: ConsoleInfoColors.Success);
            else if (overdue.Contains(task.Id))
                return ConsoleSpecial.AddStyle(RepoConfig.TaskOverdueMarkerSymbol, style: TextStyle.Bold, foregroundColor: ConsoleInfoColors.Error);
            else if (inProgress.Contains(task.Id))
                return ConsoleSpecial.AddStyle(RepoConfig.TaskInProgressMarkerSymbol, style: TextStyle.Bold, foregroundColor: ConsoleInfoColors.Warning);
            else if (warning.Contains(task.Id))
                return ConsoleSpecial.AddStyle(RepoConfig.TaskWarningMarkerSymbol, style: TextStyle.Dim, foregroundColor: ConsoleInfoColors.Warning);
            return RepoConfig.TaskNotCompleteMarkerSymbol;
        }

        protected string GetDeadlineMarkerSymbol(Deadline deadline)
        {
            if (!deadline.enabled)
                return ConsoleSpecial.AddStyle(RepoConfig.TaskCompleteMarkerSymbol, style: TextStyle.Dim, foregroundColor: TextForegroundColor.White);
            else if (deadline.Overdue())
                return ConsoleSpecial.AddStyle(RepoConfig.TaskOverdueMarkerSymbol, style: TextStyle.Bold, foregroundColor: ConsoleInfoColors.Error);
            else if (deadline.InProgress())
                return ConsoleSpecial.AddStyle(RepoConfig.TaskInProgressMarkerSymbol, style: TextStyle.Bold, foregroundColor: ConsoleInfoColors.Warning);
            else if (deadline.Warning())
                return ConsoleSpecial.AddStyle(RepoConfig.TaskWarningMarkerSymbol, style: TextStyle.Dim, foregroundColor: ConsoleInfoColors.Warning);
            return RepoConfig.TaskNotCompleteMarkerSymbol;
        }

        protected string GetEnabledSymbol(bool enabled)
        {
            if (enabled)
                return RepoConfig.TaskCompleteMarkerSymbol;
            return RepoConfig.TaskNotCompleteMarkerSymbol;
        }

        protected string GetTaskName(PlanumTask task, IEnumerable<PlanumTask> tasks, IEnumerable<Guid> overdue, IEnumerable<Guid> inProgress, IEnumerable<Guid> warning)
        {
            var line = "";
            if (tasks.Where(x => x.Id != task.Id && x.Name == task.Name).Any())
                line = RepoConfig.AddMarkdownLink(task.Name +
                       RepoConfig.TaskNameIdDelimiter +
                       task.Id.ToString(), task.SaveFile);
            line = RepoConfig.AddMarkdownLink(task.Name, task.SaveFile);
            line = AddStatusColor(line, task, overdue, inProgress, warning);
            return line;
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

        protected void WriteTaskHeader(IList<string> lines, PlanumTask task) => lines.Add(
                ConsoleSpecial.AddStyle(RepoConfig.TaskMarkerStartSymbol + task.Id.ToString() + RepoConfig.TaskMarkerEndSymbol, style: TextStyle.Dim, foregroundColor: TextForegroundColor.White)
                );

        protected void WriteName(IList<string> lines, PlanumTask task, IEnumerable<PlanumTask> tasks, IEnumerable<Guid> overdue, IEnumerable<Guid> inProgress, IEnumerable<Guid> warning, int level = 0)
        {
            lines.Add(AddLineTabs(level) +
                RepoConfig.TaskItemSymbol +
                RepoConfig.AddCheckbox(GetTaskNameMarkerSymbol(task, overdue, inProgress, warning)) +
                RepoConfig.TaskNameSymbol +
                RepoConfig.TaskHeaderDelimeterSymbol +
                GetTaskName(task, tasks, overdue, inProgress, warning));
        }

        protected void WriteChecklistName(IList<string> lines, PlanumTask task, IEnumerable<PlanumTask> tasks, IEnumerable<Guid> overdue, IEnumerable<Guid> inProgress, IEnumerable<Guid> warning, int level = 0)
        {
            lines.Add(
                AddLineTabs(level) +
                RepoConfig.TaskItemSymbol +
                RepoConfig.AddCheckbox(GetTaskNameMarkerSymbol(task, overdue, inProgress, warning)) +
                GetTaskName(task, tasks, overdue, inProgress, warning));
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
                        GetTaskName(child, tasks, overdue, inProgress, warning)
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
                        GetTaskName(parent, tasks, overdue, inProgress, warning)
                        );
            }
        }

        protected void WriteNext(IList<string> lines, Deadline deadline, IEnumerable<PlanumTask> tasks, IEnumerable<Guid> overdue, IEnumerable<Guid> inProgress, IEnumerable<Guid> warning, int level = 0)
        {
            var nextTasks = tasks.Where(x => deadline.next.Contains(x.Id));
            foreach (var next in nextTasks)
            {
                string nextStr = GetTaskName(next, tasks, overdue, inProgress, warning);
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
                var deadlineLines = new List<string>();

                // header
                deadlineLines.Add(
                    AddLineTabs(level) +
                    RepoConfig.TaskItemSymbol +
                    RepoConfig.AddCheckbox(GetDeadlineMarkerSymbol(deadline)) +
                    RepoConfig.TaskDeadlineHeaderSymbol +
                    RepoConfig.TaskHeaderDelimeterSymbol +
                    deadline.Id.ToString()
                    );
                // deadline
                deadlineLines.Add(
                    AddLineTabs(level + 1) +
                    RepoConfig.TaskItemSymbol +
                    RepoConfig.TaskDeadlineSymbol +
                    RepoConfig.TaskHeaderDelimeterSymbol +
                    deadline.deadline.ToString(RepoConfig.TaskDateTimeWriteFormat)
                    );
                // warning time
                if (deadline.warningTime != TimeSpan.Zero)
                    deadlineLines.Add(
                        AddLineTabs(level + 1) +
                        RepoConfig.TaskItemSymbol +
                        RepoConfig.TaskWarningTimeSymbol +
                        RepoConfig.TaskHeaderDelimeterSymbol +
                        deadline.warningTime.ToString(RepoConfig.TaskTimeSpanWriteFormat)
                        );
                // duration
                if (deadline.duration != TimeSpan.Zero)
                    deadlineLines.Add(
                        AddLineTabs(level + 1) +
                        RepoConfig.TaskItemSymbol +
                        RepoConfig.TaskDurationTimeSymbol +
                        RepoConfig.TaskHeaderDelimeterSymbol +
                        deadline.duration.ToString(RepoConfig.TaskTimeSpanWriteFormat)
                        );
                // repeat
                if (deadline.repeatSpan != TimeSpan.Zero && deadline.repeatMonths > 0 && deadline.repeatYears > 0)
                {
                    var repeatLine = AddLineTabs(level + 1) +
                        RepoConfig.TaskItemSymbol +
                        RepoConfig.AddCheckbox(GetEnabledSymbol(deadline.repeated)) +
                        RepoConfig.TaskRepeatTimeSymbol +
                        RepoConfig.TaskHeaderDelimeterSymbol +
                        deadline.repeatYears.ToString() + " " +
                        deadline.repeatMonths.ToString() + " " +
                        deadline.repeatSpan.ToString(RepoConfig.TaskTimeSpanWriteFormat);
                    deadlineLines.Add(repeatLine);
                }
                // next
                WriteNext(deadlineLines, deadline, tasks, overdue, inProgress, warning, level);

                foreach (var line in deadlineLines)
                {
                    if (!deadline.enabled)
                        lines.Add(ConsoleSpecial.AddStyle(line, style: TextStyle.Dim, foregroundColor: TextForegroundColor.White));
                    else if (deadline.Overdue())
                        lines.Add(ConsoleSpecial.AddStyle(line, style: TextStyle.Bold, foregroundColor: ConsoleInfoColors.Error));
                    else if (!deadline.InProgress())
                        lines.Add(ConsoleSpecial.AddStyle(line, style: TextStyle.Bold, foregroundColor: ConsoleInfoColors.Warning));
                    else if (!deadline.Warning())
                        lines.Add(ConsoleSpecial.AddStyle(line, style: TextStyle.Dim, foregroundColor: ConsoleInfoColors.Warning));
                    else
                        lines.Add(line);
                }
            }
        }

        public void WriteTask(IList<string> lines, PlanumTask task, IEnumerable<PlanumTask> tasks, ListCommandSettings listSettings)
        {
            Logger.Log($"Starting task list write: {task.Id} | {task.Name}", LogLevel.INFO);
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
