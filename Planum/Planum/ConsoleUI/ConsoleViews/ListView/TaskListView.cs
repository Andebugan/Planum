using Alba.CsConsoleFormat;
using Planum.Models.BuisnessLogic.Entities;
using System;
using System.Collections.Generic;
using static System.ConsoleColor;
using System.Linq;
using System.Text;

namespace Planum.ConsoleUI.ConsoleViews.ListView
{
    public class TaskListView
    {
        public int NameWidth = 50;

        public void RenderTask(Task task, bool showCategory = false, bool showDescription = false)
        {
            /*
            var doc = new Document();

            Grid grid = new Grid();
            grid.Color = DarkGray;

            grid.Columns.Add(GridLength.Auto);
            grid.Columns.Add(NameWidth);
            if (showCategory)
                grid.Columns.Add(CategoryWidth);

            grid.Children.Add(new Cell(tag.Id) { Align = Align.Center, Color = Cyan });
            grid.Children.Add(new Cell(tag.Name) { Align = Align.Center, Color = Blue });
            if (showCategory)
                grid.Children.Add(new Cell(tag.Category) { Align = Align.Center, Color = Cyan });
            if (showDescription)
            {
                if (showCategory)
                    grid.Children.Add(new Cell(tag.Description) { ColumnSpan = 3, Align = Align.Stretch, Color = White });
                else
                    grid.Children.Add(new Cell(tag.Description) { ColumnSpan = 2, Align = Align.Stretch, Color = White });
            }

            if (showCategory)
                grid.Children.Add(new Cell() { ColumnSpan = 3, Stroke = LineThickness.Double });
            else
                grid.Children.Add(new Cell() { ColumnSpan = 2, Stroke = LineThickness.Double });

            doc.Children.Add(grid);

            ConsoleRenderer.RenderDocument(doc);
            */
        }

        public void RenderTasks(List<Tag> tasks, bool showCategory = false, bool showDescription = false)
        {
            
        }
    }
}
