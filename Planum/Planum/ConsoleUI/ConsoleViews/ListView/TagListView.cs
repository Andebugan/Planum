using Alba.CsConsoleFormat;
using Planum.Models.BuisnessLogic.Entities;
using System.Collections.Generic;
using static System.ConsoleColor;

namespace Planum.ConsoleUI.ConsoleViews
{
    public class TagListView
    {
        public int NameWidth = 25;
        public int CategoryWidth = 25;

        public void RenderTag(Tag tag, bool showCategory = false, bool showDescription = false)
        {
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
        }

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

            grid.Children.RemoveAt(grid.Children.Count - 1);
            doc.Children.Add(grid);

            ConsoleRenderer.RenderDocument(doc);
        }
    }
}
