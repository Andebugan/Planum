using Alba.CsConsoleFormat;
using Planum.Models.BuisnessLogic.Entities;
using System;
using System.Collections.Generic;
using static System.ConsoleColor;
using Planum.Models.BuisnessLogic.Managers;

namespace Planum.ConsoleUI.ConsoleViews
{
    public class TaskListView
    {
        public int NameWidth = 40;
        public int StatusWidth = 30;

        public void RenderTasks(List<Task> tasks,
            ITagManager tagManager, ITaskManager taskManager, Dictionary<string, bool> boolParams)
        {
            var doc = new Document();

            Grid grid = new Grid();
            grid.Color = DarkGray;

            bool showDescription = boolParams["showDescription"];
            bool showTags = boolParams["showTags"];
            bool showStatus = boolParams["showStatus"];
            bool showParent = boolParams["showParent"];
            bool showChildren = boolParams["showChildren"];
            bool showStatusQueue = boolParams["showStatusQueue"];
            bool showStartTime = boolParams["showStartTime"];
            bool showDeadline = boolParams["showDeadline"];
            bool showRepeatPeriod = boolParams["showRepeatPeriod"];
            bool showOverdueTasks = boolParams["showOverdueTasks"];
            bool showTodayTasks = boolParams["showTodayTasks"];
            bool showNotCurrentTasks = boolParams["showNotCurrentTasks"];
            bool showCurrentTasks = boolParams["showCurrentTasks"];
            bool showNoParent = boolParams["showNoParent"];
            bool showNoChildren = boolParams["showNoChildren"];
            bool showNoStatuses = boolParams["showNoStatuses"];
            bool showNoTags = boolParams["showNoTags"];


            if (!showStatus)
                NameWidth = 70;

            grid.Columns.Add(GridLength.Auto);
            grid.Columns.Add(NameWidth);

            if (showStatus)
                grid.Columns.Add(StatusWidth);

            bool hasTimeFilters = showTodayTasks || showOverdueTasks
                || showCurrentTasks || showNotCurrentTasks;

            foreach (var task in tasks)
            {
                bool today = false;
                bool current = false;
                bool overdue = false;
                bool notCurrent = false;

                if (task.ParentIds.Count > 0 && showNoParent)
                    continue;
                if (task.ChildIds.Count > 0 && showNoChildren)
                    continue;
                if (task.TagIds.Count > 0 && showNoTags)
                    continue;
                if (task.StatusQueueIds.Count > 0 && showNoStatuses)
                    continue;

                if (hasTimeFilters)
                {
                    if (Math.Abs((DateTime.Now - task.Deadline).TotalDays) < 1 && DateTime.Now <= task.Deadline)
                        today = true;

                    if (Math.Abs((DateTime.Now - task.Deadline).TotalDays) > 1 && DateTime.Now < task.Deadline &&
                        DateTime.Now > task.StartTime)
                        current = true;

                    if (DateTime.Now > task.Deadline)
                        overdue = true;

                    if (DateTime.Now < task.StartTime)
                        notCurrent = true;
                }

                if (hasTimeFilters && !(today == showTodayTasks && current == showCurrentTasks &&
                    overdue == showOverdueTasks && notCurrent == showNotCurrentTasks
                    ))
                    continue;

                if (!task.Timed && hasTimeFilters)
                    continue;

                // if archived
                if (task.Archived)
                {
                    if (showStatus)
                        grid.Children.Add(new Cell("archived") { Align = Align.Left, Color = Magenta, ColumnSpan = 3 });
                    else
                        grid.Children.Add(new Cell("archived") { Align = Align.Left, Color = Magenta, ColumnSpan = 2 });
                }
                else
                // time logic (complete tasks are deleted or archived)
                {
                    DateTime currentDate = DateTime.Now;
                    if (task.Timed && task.Deadline != DateTime.MinValue)
                    {
                        // time before task is activated
                        if (Math.Abs((currentDate - task.Deadline).TotalDays) > 1 && currentDate < task.Deadline &&
                            currentDate < task.StartTime && task.StartTime != DateTime.MinValue)
                        {
                            TimeSpan timeSpan = task.StartTime - currentDate;

                            if (showStatus)
                                grid.Children.Add(new Cell("+ time before start time: " + timeSpan.Days.ToString() + "d " +
                                    timeSpan.Hours.ToString() + "h " + timeSpan.Minutes.ToString() + "m")
                                {
                                    ColumnSpan = 3,
                                    Align = Align.Left,
                                    Color = Cyan
                                });
                            else
                                grid.Children.Add(new Cell($"+ time before start time: " + timeSpan.Days.ToString() + "d " +
                                    timeSpan.Hours.ToString() + "h " + timeSpan.Minutes.ToString() + "m")
                                {
                                    ColumnSpan = 2,
                                    Align = Align.Left,
                                    Color = Cyan
                                });
                        }
                        // task is active
                        else if (Math.Abs((currentDate - task.Deadline).TotalDays) > 1 && currentDate < task.Deadline)
                        {
                            TimeSpan timeSpan = task.Deadline - currentDate;

                            if (showStatus)
                                grid.Children.Add(new Cell("+ time before deadline: " + timeSpan.Days.ToString() + "d " +
                                    timeSpan.Hours.ToString() + "h " + timeSpan.Minutes.ToString() + "m")
                                {
                                    ColumnSpan = 3,
                                    Align = Align.Left,
                                    Color = Green
                                });
                            else
                                grid.Children.Add(new Cell($"+ time before deadline: " + timeSpan.Days.ToString() + "d " +
                                    timeSpan.Hours.ToString() + "h " + timeSpan.Minutes.ToString() + "m")
                                {
                                    ColumnSpan = 2,
                                    Align = Align.Left,
                                    Color = Green
                                });
                        }
                        // today is the day
                        else if (Math.Abs((currentDate - task.Deadline).TotalDays) < 1 && currentDate <= task.Deadline)
                        {
                            TimeSpan timeSpan = task.Deadline - currentDate;

                            if (showStatus)
                                grid.Children.Add(new Cell("~ less than a day before the deadline: " + timeSpan.Days.ToString() + "d " +
                                    timeSpan.Hours.ToString() + "h " + timeSpan.Minutes.ToString() + "m")
                                {
                                    ColumnSpan = 3,
                                    Align = Align.Left,
                                    Color = Yellow
                                });
                            else
                                grid.Children.Add(new Cell("~ less than a day before the deadline: " + timeSpan.Days.ToString() + "d " +
                                    timeSpan.Hours.ToString() + "h " + timeSpan.Minutes.ToString() + "m")
                                {
                                    ColumnSpan = 2,
                                    Align = Align.Left,
                                    Color = Yellow
                                });
                        }
                        // overdue
                        else if (currentDate > task.Deadline)
                        {
                            TimeSpan timeSpan = currentDate - task.Deadline;

                            if (showStatus)
                                grid.Children.Add(new Cell($"! time overdue: " + timeSpan.Days.ToString() + "d " +
                                    timeSpan.Hours.ToString() + "h " + timeSpan.Minutes.ToString() + "m")
                                {
                                    ColumnSpan = 3,
                                    Align = Align.Left,
                                    Color = Red
                                });
                            else
                                grid.Children.Add(new Cell($"! time overdue: " + timeSpan.Days.ToString() + "d " +
                                    timeSpan.Hours.ToString() + "h " + timeSpan.Minutes.ToString() + "m")
                                {
                                    ColumnSpan = 2,
                                    Align = Align.Left,
                                    Color = Red
                                });
                        }
                        // overdue
                    }
                }

                // id
                grid.Children.Add(new Cell(task.Id) { Align = Align.Center, Color = Blue });
                // name
                grid.Children.Add(new Cell(task.Name) { Align = Align.Center, Color = Blue });

                // status
                Tag? status = null;
                if (task.StatusQueueIds.Count > 0)
                    status = tagManager.FindTag(task.StatusQueueIds[task.CurrentStatusIndex]);
                if (showStatus && status != null)
                    grid.Children.Add(new Cell(status.Name) { Align = Align.Center, Color = Cyan });
                else if (showStatus && status == null)
                    grid.Children.Add(new Cell("no status") { Align = Align.Center, Color = Cyan });

                // description
                if (showDescription)
                {
                    if (showStatus)
                        grid.Children.Add(new Cell(task.Description) { ColumnSpan = 3, Align = Align.Stretch, Color = White });
                    else
                        grid.Children.Add(new Cell(task.Description) { ColumnSpan = 2, Align = Align.Stretch, Color = White });
                }

                // tags
                if (showTags && task.TagIds.Count != 0)
                {
                    if (showStatus)
                        grid.Children.Add(new Cell("tags:") { Align = Align.Left, VerticalAlign = VerticalAlign.Center, Color = Cyan });
                    else
                        grid.Children.Add(new Cell("tags:") { Align = Align.Left, VerticalAlign = VerticalAlign.Center, Color = Cyan });

                    Grid tagGrid = new Grid();
                    tagGrid.Color = Black;
                    tagGrid.Columns.Add(GridLength.Auto);
                    tagGrid.Columns.Add(GridLength.Auto);

                    IReadOnlyList<int> tagIds = task.TagIds;
                    foreach (var id in tagIds)
                    {
                        Tag? tag = tagManager.FindTag(id);
                        if (tag == null)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("error: tag does not exist!");
                            Console.ForegroundColor = ConsoleColor.White;
                            return;
                        }

                        tagGrid.Children.Add(new Cell("[" + tag.Id + "]") { Align = Align.Stretch, Color = White });
                        tagGrid.Children.Add(new Cell(tag.Name) { Align = Align.Stretch, Color = White });
                    }

                    if (showStatus)
                    {
                        grid.Children.Add(new Cell(tagGrid) { ColumnSpan = 2, Align = Align.Stretch, Color = White });
                    }
                    else
                    {
                        grid.Children.Add(new Cell(tagGrid) { Align = Align.Stretch, Color = White });
                    }
                }

                // parents
                if (showParent && task.ParentIds.Count != 0)
                {
                    if (showStatus)
                        grid.Children.Add(new Cell("parents:") { Align = Align.Left, VerticalAlign = VerticalAlign.Center, Color = Cyan });
                    else
                        grid.Children.Add(new Cell("parents:") { Align = Align.Left, VerticalAlign = VerticalAlign.Center, Color = Cyan });

                    Grid parentGrid = new Grid();
                    parentGrid.Color = Black;
                    parentGrid.Columns.Add(GridLength.Auto);
                    parentGrid.Columns.Add(GridLength.Auto);

                    IReadOnlyList<int> parentIds = task.ParentIds;
                    foreach (var id in parentIds)
                    {
                        Task? parent = taskManager.FindTask(id);
                        if (parent == null)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("error: parent task does not exist!");
                            Console.ForegroundColor = ConsoleColor.White;
                            return;
                        }

                        parentGrid.Children.Add(new Cell("[" + parent.Id + "]") { Align = Align.Stretch, Color = White });
                        parentGrid.Children.Add(new Cell(parent.Name) { Align = Align.Stretch, Color = White });
                    }

                    if (showStatus)
                    {
                        grid.Children.Add(new Cell(parentGrid) { ColumnSpan = 2, Align = Align.Stretch, Color = White });
                    }
                    else
                    {
                        grid.Children.Add(new Cell(parentGrid) { Align = Align.Stretch, Color = White });
                    }
                }

                // children
                if (showChildren && task.ChildIds.Count != 0)
                {
                    if (showStatus)
                        grid.Children.Add(new Cell("children:") { Align = Align.Left, VerticalAlign = VerticalAlign.Center, Color = Cyan });
                    else
                        grid.Children.Add(new Cell("children:") { Align = Align.Left, VerticalAlign = VerticalAlign.Center, Color = Cyan });

                    Grid childGrid = new Grid();
                    childGrid.Color = Black;
                    childGrid.Columns.Add(GridLength.Auto);
                    childGrid.Columns.Add(GridLength.Auto);

                    IReadOnlyList<int> childIds = task.ChildIds;
                    foreach (var id in childIds)
                    {
                        Task? child = taskManager.FindTask(id);
                        if (child == null)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("error: child task does not exist!");
                            Console.ForegroundColor = ConsoleColor.White;
                            return;
                        }

                        childGrid.Children.Add(new Cell("[" + child.Id + "]") { Align = Align.Stretch, Color = White });
                        childGrid.Children.Add(new Cell(child.Name) { Align = Align.Stretch, Color = White });
                    }

                    if (showStatus)
                    {
                        grid.Children.Add(new Cell(childGrid) { ColumnSpan = 2, Align = Align.Stretch, Color = White });
                    }
                    else
                    {
                        grid.Children.Add(new Cell(childGrid) { Align = Align.Stretch, Color = White });
                    }
                }

                // status queue
                if (showStatusQueue && task.StatusQueueIds.Count != 0)
                {
                    if (showStatus)
                        grid.Children.Add(new Cell("status queue:") { Align = Align.Center, VerticalAlign = VerticalAlign.Center, Color = Cyan });
                    else
                        grid.Children.Add(new Cell("status queue:") { Align = Align.Center, VerticalAlign = VerticalAlign.Center, Color = Cyan });

                    Grid statusGrid = new Grid();
                    statusGrid.Color = Black;
                    statusGrid.Columns.Add(GridLength.Auto);
                    statusGrid.Columns.Add(GridLength.Auto);

                    IReadOnlyList<int> statusIds = task.StatusQueueIds;
                    for (int i = 0; i < statusIds.Count; i++)
                    {
                        Tag? tag = tagManager.FindTag(statusIds[i]);
                        if (tag == null)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("error: tag does not exist!");
                            Console.ForegroundColor = ConsoleColor.White;
                            return;
                        }

                        if (i != task.CurrentStatusIndex)
                        {
                            statusGrid.Children.Add(new Cell("[" + tag.Id + "]") { Align = Align.Stretch, Color = White });
                            statusGrid.Children.Add(new Cell(tag.Name) { Align = Align.Stretch, Color = White });
                        }
                        else
                        {
                            statusGrid.Children.Add(new Cell("[" + tag.Id + "]") { Align = Align.Stretch, Color = DarkYellow });
                            statusGrid.Children.Add(new Cell(tag.Name) { Align = Align.Stretch, Color = Yellow });
                        }
                    }

                    if (showStatus)
                    {
                        grid.Children.Add(new Cell(statusGrid) { ColumnSpan = 2, Align = Align.Stretch, Color = White });
                    }
                    else
                    {
                        grid.Children.Add(new Cell(statusGrid) { Align = Align.Stretch, Color = White });
                    }
                }

                // start time
                if (showStartTime && task.Timed && task.StartTime != DateTime.MinValue)
                {
                    if (showStatus)
                    {
                        grid.Children.Add(new Cell("start time: " + 
                            task.StartTime.Year.ToString() + "-" +
                            task.StartTime.Month.ToString() + "-" +
                            task.StartTime.Day.ToString() + " " +
                            task.StartTime.Hour.ToString() + ":" +
                            task.StartTime.Minute.ToString())
                        { ColumnSpan = 3, Align = Align.Stretch, Color = White });
                    }
                    else
                    {
                        grid.Children.Add(new Cell("start time: " +
                            task.StartTime.Year.ToString() + "-" +
                            task.StartTime.Month.ToString() + "-" +
                            task.StartTime.Day.ToString() + " " +
                            task.StartTime.Hour.ToString() + ":" +
                            task.StartTime.Minute.ToString())
                        { ColumnSpan = 2, Align = Align.Stretch, Color = White });
                    }
                }

                // deadline
                if (showDeadline && task.Timed && task.Deadline != DateTime.MinValue)
                {
                    if (showStatus)
                    {
                        grid.Children.Add(new Cell("deadline: " +
                            task.Deadline.Year.ToString() + "-" +
                            task.Deadline.Month.ToString() + "-" +
                            task.Deadline.Day.ToString() + " " +
                            task.Deadline.Hour.ToString() + ":" +
                            task.Deadline.Minute.ToString())
                        { ColumnSpan = 3, Align = Align.Stretch, Color = White });
                    }
                    else
                    {
                        grid.Children.Add(new Cell("deadline: " +
                            task.Deadline.Year.ToString() + "-" +
                            task.Deadline.Month.ToString() + "-" +
                            task.Deadline.Day.ToString() + " " +
                            task.Deadline.Hour.ToString() + ":" +
                            task.Deadline.Minute.ToString())
                        { ColumnSpan = 2, Align = Align.Stretch, Color = White });
                    }
                }

                // repeat period
                if (showRepeatPeriod && task.IsRepeated && task.RepeatPeriod != TimeSpan.Zero)
                {
                    if (showStatus)
                    {
                        grid.Children.Add(new Cell("repeat period: " + task.RepeatPeriod.Days.ToString() + "d " +
                                    task.RepeatPeriod.Hours.ToString() + "h " + task.RepeatPeriod.Minutes.ToString() + "m")
                        { ColumnSpan = 3, Align = Align.Stretch, Color = White });
                    }
                    else
                    {
                        grid.Children.Add(new Cell("repeat period: " + task.RepeatPeriod.Days.ToString() + "d " +
                                    task.RepeatPeriod.Hours.ToString() + "h " + task.RepeatPeriod.Minutes.ToString() + "m")
                        { ColumnSpan = 2, Align = Align.Stretch, Color = White });
                    }
                }

                if (showStatus)
                    grid.Children.Add(new Cell() { ColumnSpan = 3, Stroke = LineThickness.Double });
                else
                    grid.Children.Add(new Cell() { ColumnSpan = 2, Stroke = LineThickness.Double });
            }

            if (grid.Children.Count == 0)
            {
                Console.ForegroundColor = Cyan;
                Console.WriteLine("none of the tasks match the given paremeters\n");
                Console.ForegroundColor = White;
                return;
            }

            grid.Children.RemoveAt(grid.Children.Count - 1);
            doc.Children.Add(grid);

            ConsoleRenderer.RenderDocument(doc);
        }
    }
}
