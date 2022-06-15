using Alba.CsConsoleFormat;
using Planum.Models.BuisnessLogic.Entities;
using System.Collections.Generic;
using static System.ConsoleColor;
using System;

namespace Planum.ConsoleUI.ConsoleViews
{
    public class TagListView
    {
        public int NameWidth = 25;
        public int CategoryWidth = 25;

        public void RenderTags(List<Tag> tags, bool showCategory = false, bool showDescription = false)
        {
            var doc = new Document();

            Grid grid = new Grid();
            grid.Color = DarkGray;

            grid.Columns.Add(GridLength.Auto);
            grid.Columns.Add(NameWidth);
            if (showCategory)
                grid.Columns.Add(CategoryWidth);

            foreach (var tag in tags)
            {
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
            }

            if (grid.Children.Count == 0)
            {
                Console.ForegroundColor = Cyan;
                Console.WriteLine("none of the tags match the given paremeters\n");
                Console.ForegroundColor = White;
                return;
            }

            grid.Children.RemoveAt(grid.Children.Count - 1);
            doc.Children.Add(grid);

            ConsoleRenderer.RenderDocument(doc);
        }
    }
}
