using Alba.CsConsoleFormat;
using Planum.Models.BuisnessLogic.Entities;
using System;
using System.Collections.Generic;
using static System.ConsoleColor;
using System.Linq;
using System.Text;
using Planum.Models.BuisnessLogic.Managers;

namespace Planum.ConsoleUI.ConsoleViews.ListView
{
    public class TaskListView
    {
        public int NameWidth = 25;
        public int StatusWidth = 25;

        public void RenderTask(Task task,
            TagManager tagManager, TaskManager taskManager,
            bool showDescription = false,
            bool showTags = false,
            bool showStatus = false,
            bool showParent = false,
            bool showChildren = false,
            bool showStartTime = false,
            bool showDeadline = false,
            bool showRepeatPerion = false)
        {
            var doc = new Document();

            Grid grid = new Grid();
            grid.Color = DarkGray;

            if (showDescription && !showStatus)
                NameWidth = 50;

            grid.Columns.Add(GridLength.Auto);
            grid.Columns.Add(NameWidth);
            if (showStatus)
                grid.Columns.Add(StatusWidth);

            grid.Children.Add(new Cell(task.Id) { Align = Align.Center, Color = Cyan });
            grid.Children.Add(new Cell(task.Name) { Align = Align.Center, Color = Blue });

            Tag? status = tagManager.FindTag(task.StatusQueueIds[task.CurrentStatusIndex]);

            if (status == null)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("error: status tag does not exist!");
                Console.ForegroundColor = ConsoleColor.White;
                return;
            }

            if (showStatus)
                grid.Children.Add(new Cell(status.Id.ToString() + "|" + status.Name) { Align = Align.Center, Color = Cyan });

            if (showDescription)
            {
                if (showStatus)
                    grid.Children.Add(new Cell(task.Description) { ColumnSpan = 3, Align = Align.Stretch, Color = White });
                else
                    grid.Children.Add(new Cell(task.Description) { ColumnSpan = 2, Align = Align.Stretch, Color = White });
            }

            // ended here
            // work: finish task view
            if (showParent && task.ParentIds.Count != 0)
            {
                if (showStatus)
                {
                    grid.Children.Add(new Cell(task.Description) { ColumnSpan = 3, Align = Align.Stretch, Color = White });
                }
                else
                {
                    grid.Children.Add(new Cell(task.Description) { ColumnSpan = 2, Align = Align.Stretch, Color = White });
                }
            }

            if (showStatus)
                grid.Children.Add(new Cell() { ColumnSpan = 3, Stroke = LineThickness.Double });
            else
                grid.Children.Add(new Cell() { ColumnSpan = 2, Stroke = LineThickness.Double });

            doc.Children.Add(grid);

            ConsoleRenderer.RenderDocument(doc);
        }

        public void RenderTasks(List<Tag> tasks, bool showCategory = false, bool showDescription = false)
        {
            
        }
    }
}
