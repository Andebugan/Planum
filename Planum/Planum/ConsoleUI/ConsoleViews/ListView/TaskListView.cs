using Alba.CsConsoleFormat;
using Planum.Models.BuisnessLogic.Entities;
using System;
using System.Collections.Generic;
using static System.ConsoleColor;
using System.Linq;
using System.Text;
using Planum.Models.BuisnessLogic.Managers;

namespace Planum.ConsoleUI.ConsoleViews
{
    public class TaskListView
    {
        public int NameWidth = 40;
        public int StatusWidth = 30;

        public void RenderTask(Task task,
            ITagManager tagManager, ITaskManager taskManager,
            bool showDescription = false,
            bool showTags = false,
            bool showStatus = false,
            bool showParent = false,
            bool showChildren = false,
            bool showStatusQueue = false,
            bool showStartTime = false,
            bool showDeadline = false,
            bool showRepeatPerion = false)
        {
            var doc = new Document();

            Grid grid = new Grid();
            grid.Color = DarkGray;

            if (showDescription && !showStatus)
                NameWidth = 70;

            grid.Columns.Add(GridLength.Auto);
            grid.Columns.Add(NameWidth);

            Tag? status = null;
            if (task.StatusQueueIds.Count > 0)
                status = tagManager.FindTag(task.StatusQueueIds[task.CurrentStatusIndex]);
            if (status == null)
            {
                showStatus = false;
            }

            if (showStatus)
                grid.Columns.Add(StatusWidth);

            // time left logic here + archived flag, if true

            // id
            grid.Children.Add(new Cell(task.Id) { Align = Align.Center, Color = Cyan });
            // name
            grid.Children.Add(new Cell(task.Name) { Align = Align.Center, Color = Blue });

            // status
            if (showStatus)
                grid.Children.Add(new Cell(status.Id.ToString() + ":" + status.Name) { Align = Align.Center, Color = Cyan });

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
                    grid.Children.Add(new Cell("tags:") { ColumnSpan = 3, Align = Align.Left, Color = Magenta });
                else
                    grid.Children.Add(new Cell("tags:") { ColumnSpan = 2, Align = Align.Left, Color = Magenta });

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

                    tagGrid.Children.Add(new Cell(tag.Id) { Align = Align.Stretch, Color = DarkCyan });
                    tagGrid.Children.Add(new Cell(tag.Name) { Align = Align.Stretch, Color = White });
                }

                if (showStatus)
                {
                    grid.Children.Add(new Cell(tagGrid) { ColumnSpan = 3, Align = Align.Stretch, Color = White });
                }
                else
                {
                    grid.Children.Add(new Cell(tagGrid) { ColumnSpan = 2, Align = Align.Stretch, Color = White });
                }
            }

            // parents
            if (showParent && task.ParentIds.Count != 0)
            {
                if (showStatus)
                    grid.Children.Add(new Cell("parents:") { ColumnSpan = 3, Align = Align.Left, Color = Magenta });
                else
                    grid.Children.Add(new Cell("parents:") { ColumnSpan = 2, Align = Align.Left, Color = Magenta });

                Grid parentGrid = new Grid();
                parentGrid.Color = Black;
                parentGrid.Columns.Add(GridLength.Auto);
                parentGrid.Columns.Add(GridLength.Auto);

                IReadOnlyList<int> parentIds = task.ParentIds;
                foreach(var id in parentIds)
                {
                    Task? parent = taskManager.FindTask(id);
                    if (parent == null)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("error: parent task does not exist!");
                        Console.ForegroundColor = ConsoleColor.White;
                        return;
                    }

                    parentGrid.Children.Add(new Cell(parent.Id) { Align = Align.Stretch, Color = DarkCyan });
                    parentGrid.Children.Add(new Cell(parent.Name) { Align = Align.Stretch, Color = White });
                }

                if (showStatus)
                {
                    grid.Children.Add(new Cell(parentGrid) { ColumnSpan = 3, Align = Align.Stretch, Color = White });
                }
                else
                {
                    grid.Children.Add(new Cell(parentGrid) { ColumnSpan = 2, Align = Align.Stretch, Color = White });
                }
            }

            // children
            if (showChildren && task.ChildIds.Count != 0)
            {
                if (showStatus)
                    grid.Children.Add(new Cell("children:") { ColumnSpan = 3, Align = Align.Left, Color = Magenta });
                else
                    grid.Children.Add(new Cell("children:") { ColumnSpan = 2, Align = Align.Left, Color = Magenta });

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

                    childGrid.Children.Add(new Cell(child.Id) { Align = Align.Stretch, Color = DarkCyan });
                    childGrid.Children.Add(new Cell(child.Name) { Align = Align.Stretch, Color = White });
                }

                if (showStatus)
                {
                    grid.Children.Add(new Cell(childGrid) { ColumnSpan = 3, Align = Align.Stretch, Color = White });
                }
                else
                {
                    grid.Children.Add(new Cell(childGrid) { ColumnSpan = 2, Align = Align.Stretch, Color = White });
                }
            }

            // status queue
            if (showStatusQueue && task.StatusQueueIds.Count != 0)
            {
                if (showStatus)
                    grid.Children.Add(new Cell("status queue:") { ColumnSpan = 3, Align = Align.Left, Color = Magenta });
                else
                    grid.Children.Add(new Cell("status queue:") { ColumnSpan = 2, Align = Align.Left, Color = Magenta });

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
                        statusGrid.Children.Add(new Cell(tag.Id) { Align = Align.Stretch, Color = DarkCyan });
                        statusGrid.Children.Add(new Cell(tag.Name) { Align = Align.Stretch, Color = White });
                    }
                    else
                    {
                        statusGrid.Children.Add(new Cell(tag.Id) { Align = Align.Stretch, Color = DarkYellow });
                        statusGrid.Children.Add(new Cell(tag.Name) { Align = Align.Stretch, Color = Yellow });
                    }
                }

                if (showStatus)
                {
                    grid.Children.Add(new Cell(statusGrid) { ColumnSpan = 3, Align = Align.Stretch, Color = White });
                }
                else
                {
                    grid.Children.Add(new Cell(statusGrid) { ColumnSpan = 2, Align = Align.Stretch, Color = White });
                }
            }

            // start time
            if (showStartTime && task.Timed && task.StartTime != DateTime.MinValue)
            {
                if (showStatus)
                {
                    grid.Children.Add(new Cell("start time: " + task.StartTime.ToString()) 
                    { ColumnSpan = 3, Align = Align.Stretch, Color = White });
                }
                else
                {
                    grid.Children.Add(new Cell("start time: " + task.StartTime.ToString()) 
                    { ColumnSpan = 2, Align = Align.Stretch, Color = White });
                }
            }

            // deadline
            if (showStartTime && task.Timed && task.StartTime != DateTime.MinValue)
            {
                if (showStatus)
                {
                    grid.Children.Add(new Cell("start time: " + task.StartTime.ToString())
                    { ColumnSpan = 3, Align = Align.Stretch, Color = White });
                }
                else
                {
                    grid.Children.Add(new Cell("start time: " + task.StartTime.ToString())
                    { ColumnSpan = 2, Align = Align.Stretch, Color = White });
                }
            }
            // repeat period

            doc.Children.Add(grid);

            ConsoleRenderer.RenderDocument(doc);
        }

        public void RenderTasks(List<Tag> tasks, bool showCategory = false, bool showDescription = false)
        {
            
        }
    }
}
