using Alba.CsConsoleFormat;
using Planum.Models.BuisnessLogic.Managers;
using System.Collections.Generic;
using static System.ConsoleColor;
using System;
using Planum.Models.BuisnessLogic.Entities;
using System.Linq;

namespace Planum.ConsoleUI
{
    public class TaskMonthView
    {
        List<Task> tasks;
        Dictionary<string, bool> boolParams;
        DateTime renderedMonth;
        int columnWidth;

        struct CellInformation
        {
            public int taskId;
            public string cellText;
            public ConsoleColor cellColor;

            public CellInformation(int taskId, string cellText, ConsoleColor cellColor) : this()
            {
                this.taskId = taskId;
                this.cellText = cellText;
                this.cellColor = cellColor;
            }
        }

        List<CellInformation> cells = new List<CellInformation>();

        Document doc;
        Grid grid;

        public TaskMonthView(List<Task> tasks)
        {
            this.tasks = tasks;
            this.boolParams = boolParams;
        }

        protected bool IsSameDay(DateTime firstDate, DateTime secondDate)
        {
            if (firstDate.Day == secondDate.Day &&
                firstDate.Month == secondDate.Month &&
                firstDate.Year == secondDate.Year)
                return true;
            else
                return false;
        }

        protected void AddTaskNameCell(Task task, ConsoleColor cellColor, string symbol)
        {
            string cellText = "[" + task.Id.ToString() + "] " + task.Name.ToString();
            if (cellText.Length > columnWidth)
            {
                cellText = cellText.Substring(0, columnWidth - 3);
                cellText += "...";
            }
            else
            {
                while (cellText.Length < columnWidth)
                    cellText += symbol;
            }

            if (task.Deadline != DateTime.MinValue)
            {
                // today inside 24h period of deadline
                if (Math.Abs((task.Deadline - DateTime.Now).TotalDays) < 1)
                    cellColor = Yellow;
                // deadline is ded
                if (task.Deadline < DateTime.Now)
                    cellColor = Red;
            }

            if (!cells.Any(x => x.taskId == task.Id))
                cells.Add(new CellInformation(task.Id, cellText, cellColor));
            else
            {
                var cell = cells.Find(x => x.taskId == task.Id);
                cell.cellText = cellText;
                cell.cellColor = cellColor;
                cells[cells.IndexOf(cells.Find(x => x.taskId == task.Id))] = cell;
            }
        }

        protected void AddTodayProgressCell(Task task, ConsoleColor cellColor)
        {
            string cellText = "";
            float increment = columnWidth / 24.0f;
            float hourCount = 0;
            while ((int)hourCount < task.Deadline.Hour && cellText.Length < columnWidth - 1)
            {
                hourCount += increment;
                if ((int)hourCount > cellText.Length)
                    cellText += "━";
            }
            while (cellText.Length < columnWidth)
                cellText += "╌";
            if (!cells.Any(x => x.taskId == task.Id))
                cells.Add(new CellInformation(task.Id, cellText, cellColor));
            else
            {
                var cell = cells.Find(x => x.taskId == task.Id);
                cell.cellText = cellText;
                cell.cellColor = cellColor;
                cells[cells.IndexOf(cells.Find(x => x.taskId == task.Id))] = cell;
            }
        }

        protected void AddBetweenStartAndDeadlineCell(Task task, ConsoleColor cellColor)
        {
            string cellText = "";
            for (int i = 0; i < columnWidth; i++)
                cellText += "╌";
            if (!cells.Any(x => x.taskId == task.Id))
                cells.Add(new CellInformation(task.Id, cellText, cellColor));
            else
            {
                var cell = cells.Find(x => x.taskId == task.Id);
                cell.cellText = cellText;
                cell.cellColor = cellColor;
                cells[cells.IndexOf(cells.Find(x => x.taskId == task.Id))] = cell;
            }
        }

        protected void AddProgressCell(Task task, ConsoleColor cellColor)
        {
            string cellText = "";
            for (int i = 0; i < columnWidth; i++)
                cellText += "━";
            if (!cells.Any(x => x.taskId == task.Id))
                cells.Add(new CellInformation(task.Id, cellText, DarkYellow));
            else
            {
                var cell = cells.Find(x => x.taskId == task.Id);
                cell.cellText = cellText;
                cell.cellColor = cellColor;
                cells[cells.IndexOf(cells.Find(x => x.taskId == task.Id))] = cell;
            }
        }

        protected void AddDeadlineCell(Task task, ConsoleColor cellColor)
        {
            string cellText = "";
            float increment = columnWidth / 24.0f;
            float hourCount = 0;
            while ((int)hourCount < task.Deadline.Hour && cellText.Length < columnWidth - 1)
            {
                hourCount += increment;
                if ((int)hourCount > cellText.Length)
                    cellText += "╌";
            }
            cellText += "#";
            if (!cells.Any(x => x.taskId == task.Id))
                cells.Add(new CellInformation(task.Id, cellText, cellColor));
            else
            {
                var cell = cells.Find(x => x.taskId == task.Id);
                cell.cellText = cellText;
                cell.cellColor = cellColor;
                cells[cells.IndexOf(cells.Find(x => x.taskId == task.Id))] = cell;
            }
        }

        protected void AddProgressDeadlineCell(Task task, ConsoleColor cellColor)
        {
            string cellText = "";
            float increment = columnWidth / 24.0f;
            float hourCount = 0;
            while ((int)hourCount < task.Deadline.Hour && cellText.Length < columnWidth - 1)
            {
                hourCount += increment;
                if ((int)hourCount > cellText.Length)
                    cellText += "━";
            }
            cellText += "#";
            if (!cells.Any(x => x.taskId == task.Id))
                cells.Add(new CellInformation(task.Id, cellText, cellColor));
            else
            {
                var cell = cells.Find(x => x.taskId == task.Id);
                cell.cellText = cellText;
                cell.cellColor = cellColor;
                cells[cells.IndexOf(cells.Find(x => x.taskId == task.Id))] = cell;
            }
        }

        protected void AddStartOverdueDeadlineCell(Task task, ConsoleColor cellColor)
        {
            string cellText = "";
            float increment = columnWidth / 24.0f;
            float hourCount = 0;
            while ((int)hourCount < task.Deadline.Hour && cellText.Length < columnWidth - 1)
            {
                hourCount += increment;
                if ((int)hourCount > cellText.Length)
                    cellText += " ";
            }
            cellText += "#";
            while (cellText.Length < columnWidth)
                cellText += "═";
            if (!cells.Any(x => x.taskId == task.Id))
                cells.Add(new CellInformation(task.Id, cellText, cellColor));
            else
            {
                var cell = cells.Find(x => x.taskId == task.Id);
                cell.cellText = cellText;
                cell.cellColor = cellColor;
                cells[cells.IndexOf(cells.Find(x => x.taskId == task.Id))] = cell;
            }
        }

        protected void AddOverdueDeadlineCell(Task task, ConsoleColor cellColor)
        {
            string cellText = "";
            float increment = columnWidth / 24.0f;
            float hourCount = 0;
            while ((int)hourCount < task.Deadline.Hour && cellText.Length < columnWidth - 1)
            {
                hourCount += increment;
                if ((int)hourCount > cellText.Length)
                    cellText += "━";
            }
            cellText += "#";
            while (cellText.Length < columnWidth)
                cellText += "═";
            if (!cells.Any(x => x.taskId == task.Id))
                cells.Add(new CellInformation(task.Id, cellText, cellColor));
            else
            {
                var cell = cells.Find(x => x.taskId == task.Id);
                cell.cellText = cellText;
                cell.cellColor = cellColor;
                cells[cells.IndexOf(cells.Find(x => x.taskId == task.Id))] = cell;
            }
        }

        protected void AddOverdueDeadlineEndCell(Task task, ConsoleColor cellColor)
        {
            string cellText = "";
            float increment = columnWidth / 24.0f;
            float hourCount = 0;
            while ((int)hourCount < task.Deadline.Hour && cellText.Length < columnWidth - 3)
            {
                hourCount += increment;
                if ((int)hourCount > cellText.Length)
                    cellText += "═";
            }
            cellText += "<!>";
            while (cellText.Length < columnWidth)
                cellText += " ";
            if (!cells.Any(x => x.taskId == task.Id))
                cells.Add(new CellInformation(task.Id, cellText, cellColor));
            else
            {
                var cell = cells.Find(x => x.taskId == task.Id);
                cell.cellText = cellText;
                cell.cellColor = cellColor;
                cells[cells.IndexOf(cells.Find(x => x.taskId == task.Id))] = cell;
            }
        }

        protected void AddOverdueCell(Task task, ConsoleColor cellColor)
        {
            string cellText = "";
            for (int i = 0; i < columnWidth; i++)
                cellText += "═";
            if (!cells.Any(x => x.taskId == task.Id))
                cells.Add(new CellInformation(task.Id, cellText, Red));
            else
            {
                var cell = cells.Find(x => x.taskId == task.Id);
                cell.cellText = cellText;
                cell.cellColor = cellColor;
                cells[cells.IndexOf(cells.Find(x => x.taskId == task.Id))] = cell;
            }
        }

        protected void AddTodayDeadlinessCell(Task task, ConsoleColor cellColor)
        {
            string cellText = "";
            float increment = columnWidth / 24.0f;
            float hourCount = 0;
            while ((int)hourCount < task.Deadline.Hour && cellText.Length < columnWidth - 1)
            {
                hourCount += increment;
                if ((int)hourCount > cellText.Length)
                    cellText += "━";
            }
            cellText += "○";
            if (!cells.Any(x => x.taskId == task.Id))
                cells.Add(new CellInformation(task.Id, cellText, cellColor));
            else
            {
                var cell = cells.Find(x => x.taskId == task.Id);
                cell.cellText = cellText;
                cell.cellColor = cellColor;
                cells[cells.IndexOf(cells.Find(x => x.taskId == task.Id))] = cell;
            }
        }

        protected void AddDay(DateTime day)
        {
            ConsoleColor color = DarkGray;
            ConsoleColor textColor = White;
            ConsoleColor numberColor = Black;

            Stack stack = new Stack();
            if (renderedMonth.Month == day.Month)
                color = White;
            if (DateTime.Now.Year == day.Year && DateTime.Now.Month == day.Month && DateTime.Now.Day == day.Day)
            {
                color = DarkCyan;
                numberColor = White;
            }

            if (day.Day.ToString().Length == 1)
                stack.Children.Add(new Cell(" " + day.Day.ToString() + " ") { Align = Align.Left, Color = numberColor, Background = color });
            else
                stack.Children.Add(new Cell(" " + day.Day.ToString() + " ") { Align = Align.Left, Color = numberColor, Background = color });

            // ═════════ - red
            // ━━━━━━━━━ - yellow
            // ╌╌╌╌╌╌╌╌╌ - dark yellow
            // # - deadline - shows relative deadline time
            // ○ - today - shows relative today time
            // ━━╌╌ - shows time as well
            // <!> - current deadline overdue
            // start time - task itself
            // today - line roughly at center
            // |today|        |start time|╌╌╌╌╌╌╌╌|deadline|
            // |start time|━━━━━━━━━|today|╌╌╌╌╌╌╌╌|deadline|
            // |start time|        |deadline|═════|today|

            foreach (var task in tasks)
            {
                if (task.StartTime == DateTime.MinValue && task.Deadline != DateTime.MinValue)
                {
                    // today < deadline
                    if (!IsSameDay(DateTime.Now, task.Deadline) &&
                        DateTime.Now < task.Deadline)
                    {
                        // day == today
                        if (IsSameDay(day, DateTime.Now))
                        {
                            AddTaskNameCell(task, Yellow, "╌");
                        }
                        // today < day < deadline
                        else if (!IsSameDay(day, DateTime.Now) &&
                            !IsSameDay(day, task.Deadline) &&
                            DateTime.Now < day &&
                            day < task.Deadline)
                        {
                            AddBetweenStartAndDeadlineCell(task, Yellow);
                        }
                        // day == deadline
                        else if (IsSameDay(day, task.Deadline))
                        {
                            AddDeadlineCell(task, Yellow);
                        }
                        else
                        {
                            if (cells.Any(x => x.taskId == task.Id))
                            {
                                var cell = cells.Find(x => x.taskId == task.Id);
                                cell.taskId = -1;
                                cells[cells.IndexOf(cells.Find(x => x.taskId == task.Id))] = cell;
                            }
                        }
                    }
                    // today == deadline
                    else if (IsSameDay(DateTime.Now, task.Deadline))
                    {
                        // day == today
                        if (IsSameDay(day, DateTime.Now))
                        {
                            AddTaskNameCell(task, Yellow, " ");
                        }
                        else
                        {
                            if (cells.Any(x => x.taskId == task.Id))
                            {
                                var cell = cells.Find(x => x.taskId == task.Id);
                                cell.taskId = -1;
                                cells[cells.IndexOf(cells.Find(x => x.taskId == task.Id))] = cell;
                            }
                        }
                    }
                    // today > deadline
                    else if (!IsSameDay(DateTime.Now, task.Deadline) &&
                        DateTime.Now > task.Deadline)
                    {
                        // day == today
                        if (IsSameDay(day, DateTime.Now))
                        {
                            AddTaskNameCell(task, Red, " ");
                        }
                        // today < day < deadline
                        else if (!IsSameDay(day, DateTime.Now) &&
                            !IsSameDay(day, task.Deadline) &&
                            day > task.Deadline && day < DateTime.Now)
                        {
                            AddOverdueCell(task, Red);
                        }
                        // day == deadline
                        else if (IsSameDay(day, task.Deadline))
                        {
                            AddStartOverdueDeadlineCell(task, Red);
                        }
                        else
                        {
                            if (cells.Any(x => x.taskId == task.Id))
                            {
                                var cell = cells.Find(x => x.taskId == task.Id);
                                cell.taskId = -1;
                                cells[cells.IndexOf(cells.Find(x => x.taskId == task.Id))] = cell;
                            }
                        }
                    }
                    else
                    {
                        if (cells.Any(x => x.taskId == task.Id))
                        {
                            var cell = cells.Find(x => x.taskId == task.Id);
                            cell.taskId = -1;
                            cells[cells.IndexOf(cells.Find(x => x.taskId == task.Id))] = cell;
                        }
                    }                    
                }
                else if (task.StartTime != DateTime.MinValue && task.Deadline == DateTime.MinValue)
                {
                    // today < start time
                    if (!IsSameDay(task.StartTime, DateTime.Now) &&
                        DateTime.Now < task.StartTime)
                    {
                        // day == start time
                        if (IsSameDay(day, task.StartTime))
                        {
                            AddTaskNameCell(task, DarkYellow, " ");
                        }
                        else
                        {
                            if (cells.Any(x => x.taskId == task.Id))
                            {
                                var cell = cells.Find(x => x.taskId == task.Id);
                                cell.taskId = -1;
                                cells[cells.IndexOf(cells.Find(x => x.taskId == task.Id))] = cell;
                            }
                        }
                    }
                    // today == start time
                    else if (IsSameDay(task.StartTime, DateTime.Now))
                    {
                        // day == start time
                        if (IsSameDay(day, task.StartTime))
                        {
                            AddTaskNameCell(task, Yellow, " ");
                        }
                        else
                        {
                            if (cells.Any(x => x.taskId == task.Id))
                            {
                                var cell = cells.Find(x => x.taskId == task.Id);
                                cell.taskId = -1;
                                cells[cells.IndexOf(cells.Find(x => x.taskId == task.Id))] = cell;
                            }
                        }
                    }
                    // today > start time
                    else if (!IsSameDay(task.StartTime, DateTime.Now) &&
                        DateTime.Now > task.StartTime)
                    {
                        // day == start time
                        if (IsSameDay(day, task.StartTime))
                        {
                            AddTaskNameCell(task, Yellow, "━");
                        }
                        // day < today
                        else if (!IsSameDay(day, DateTime.Now) &&
                            day < DateTime.Now && 
                            !IsSameDay(day, task.StartTime) &&
                            day > task.StartTime)
                        {
                            AddProgressCell(task, Yellow);
                        }
                        // day == today
                        else if (IsSameDay(day, DateTime.Now))
                        {
                            AddTodayDeadlinessCell(task, Yellow);
                        }
                        else
                        {
                            if (cells.Any(x => x.taskId == task.Id))
                            {
                                var cell = cells.Find(x => x.taskId == task.Id);
                                cell.taskId = -1;
                                cells[cells.IndexOf(cells.Find(x => x.taskId == task.Id))] = cell;
                            }
                        }
                    }
                    else
                    {
                        if (cells.Any(x => x.taskId == task.Id))
                        {
                            var cell = cells.Find(x => x.taskId == task.Id);
                            cell.taskId = -1;
                            cells[cells.IndexOf(cells.Find(x => x.taskId == task.Id))] = cell;
                        }
                    }
                }
                else if (task.StartTime != DateTime.MinValue && task.Deadline != DateTime.MinValue)
                {
                    // today < start time < deadline
                    if (!IsSameDay(task.StartTime, DateTime.Now) &&
                        DateTime.Now < task.StartTime)
                    {
                        // day == start time
                        if (IsSameDay(task.StartTime, day))
                        {
                            AddTaskNameCell(task, DarkYellow, "╌");
                        }
                        // start time < day < deadline
                        else if (!IsSameDay(task.StartTime, day) &&
                        !IsSameDay(task.Deadline, day) &&
                        task.StartTime < day &&
                        task.Deadline > day)
                        {
                            AddBetweenStartAndDeadlineCell(task, DarkYellow);
                        }
                        // day == deadline
                        else if (IsSameDay(task.Deadline, day))
                        {
                            AddDeadlineCell(task, DarkYellow);
                        }
                        else
                        {
                            if (cells.Any(x => x.taskId == task.Id))
                            {
                                var cell = cells.Find(x => x.taskId == task.Id);
                                cell.taskId = -1;
                                cells[cells.IndexOf(cells.Find(x => x.taskId == task.Id))] = cell;
                            }
                        }
                    }
                    // today == start time
                    else if (IsSameDay(DateTime.Now, task.StartTime))
                    {
                        // day == start time
                        if (IsSameDay(day, task.StartTime))
                        {
                            AddTaskNameCell(task, Yellow, "━");
                        }
                        // start time < day < deadline
                        else if (!IsSameDay(task.StartTime, day) &&
                            !IsSameDay(task.Deadline, day) &&
                            task.StartTime < day &&
                            task.Deadline > day)
                        {
                            AddBetweenStartAndDeadlineCell(task, Yellow);
                        }
                        // day == deadline
                        else if (IsSameDay(day, task.Deadline))
                        {
                            AddDeadlineCell(task, Yellow);
                        }
                        else
                        {
                            if (cells.Any(x => x.taskId == task.Id))
                            {
                                var cell = cells.Find(x => x.taskId == task.Id);
                                cell.taskId = -1;
                                cells[cells.IndexOf(cells.Find(x => x.taskId == task.Id))] = cell;
                            }
                        }
                    }
                    // start time < today < deadline
                    else if (!IsSameDay(task.StartTime, DateTime.Now) &&
                        !IsSameDay(task.Deadline, DateTime.Now) &&
                        task.StartTime < DateTime.Now &&
                        DateTime.Now < task.Deadline)
                    {
                        // day == start time
                        if (IsSameDay(day, task.StartTime))
                        {
                            AddTaskNameCell(task, Yellow, "━");
                        }
                        // start time < day < today
                        else if (!IsSameDay(task.StartTime, day) &&
                            !IsSameDay(DateTime.Today, day) &&
                            task.StartTime < day &&
                            day < DateTime.Today)
                        {
                            AddProgressCell(task, Yellow);
                        }
                        //
                        else if (IsSameDay(day, DateTime.Now))
                        {
                            AddTodayProgressCell(task, Yellow);
                        }
                        // today < day < deadline
                        else if (!IsSameDay(task.Deadline, day) &&
                            day > DateTime.Today &&
                            day < task.Deadline)
                        {
                            AddBetweenStartAndDeadlineCell(task, Yellow);
                        }
                        // day == deadline
                        else if (IsSameDay(day, task.Deadline))
                        {
                            AddDeadlineCell(task, Yellow);
                        }
                        else
                        {
                            if (cells.Any(x => x.taskId == task.Id))
                            {
                                var cell = cells.Find(x => x.taskId == task.Id);
                                cell.taskId = -1;
                                cells[cells.IndexOf(cells.Find(x => x.taskId == task.Id))] = cell;
                            }
                        }
                    }
                    // today == deadline
                    else if (IsSameDay(DateTime.Now, task.Deadline))
                    {
                        // day == start time
                        if (IsSameDay(day, task.Deadline))
                        {
                            AddTaskNameCell(task, Yellow, "━");
                        }
                        // start time < day < deadline
                        else if (!IsSameDay(task.StartTime, day) &&
                            !IsSameDay(task.Deadline, day) &&
                            task.StartTime < day &&
                            task.Deadline > day)
                        {
                            AddProgressCell(task, Yellow);
                        }
                        //  day == deadline
                        else if (IsSameDay(day, task.Deadline))
                        {
                            AddProgressDeadlineCell(task, Yellow);
                        }
                        else
                        {
                            if (cells.Any(x => x.taskId == task.Id))
                            {
                                var cell = cells.Find(x => x.taskId == task.Id);
                                cell.taskId = -1;
                                cells[cells.IndexOf(cells.Find(x => x.taskId == task.Id))] = cell;
                            }
                        }
                    }
                    // deadline < today
                    else if (task.Deadline < DateTime.Now &&
                        !IsSameDay(task.Deadline, DateTime.Now))
                    {
                        // day == start time
                        if (IsSameDay(day, task.StartTime))
                        {
                            AddTaskNameCell(task, Red, "━");
                        }
                        // start time < day < deadline
                        else if (!IsSameDay(day, task.StartTime) &&
                            !IsSameDay(day, task.Deadline) &&
                            day > task.StartTime &&
                            task.Deadline > day)
                        {
                            AddProgressCell(task, Red);
                        }
                        // day == deadline
                        else if (IsSameDay(day, task.Deadline))
                        {
                            AddOverdueDeadlineCell(task, Red);
                        }
                        // deadline < day < today
                        else if (!IsSameDay(day, task.Deadline) &&
                            !IsSameDay(day, DateTime.Now) &&
                            day < DateTime.Now &&
                            task.Deadline < day)
                        {
                            AddOverdueCell(task, Red);
                        }
                        // day == today
                        else if (IsSameDay(day, DateTime.Now))
                        {
                            AddOverdueDeadlineEndCell(task, Red);
                        }
                        else
                        {
                            if (cells.Any(x => x.taskId == task.Id))
                            {
                                var cell = cells.Find(x => x.taskId == task.Id);
                                cell.taskId = -1;
                                cells[cells.IndexOf(cells.Find(x => x.taskId == task.Id))] = cell;
                            }
                        }
                    }
                    else
                    {
                        if (cells.Any(x => x.taskId == task.Id))
                        {
                            var cell = cells.Find(x => x.taskId == task.Id);
                            cell.taskId = -1;
                            cells[cells.IndexOf(cells.Find(x => x.taskId == task.Id))] = cell;
                        }
                    }
                }
            }

            foreach (var cell in cells)
            {
                if (cell.taskId != -1)
                    stack.Children.Add(new Cell(cell.cellText) { Align = Align.Left, Color = cell.cellColor });
                else
                    stack.Children.Add(new Cell(" ") { Align = Align.Left, Color = cell.cellColor });
            }

            grid.Children.Add(new Cell(stack) 
            { Align = Align.Left, Color = color });
        }

        public void RenderTasks(DateTime RenderedMonth)
        {
            renderedMonth = RenderedMonth;
            doc = new Document();

            grid = new Grid();
            grid.Color = White;

            columnWidth = (int)Math.Floor((System.Console.WindowWidth) / 8.0f);

            grid.Columns.Add(columnWidth);
            grid.Columns.Add(columnWidth);
            grid.Columns.Add(columnWidth);
            grid.Columns.Add(columnWidth);
            grid.Columns.Add(columnWidth);
            grid.Columns.Add(columnWidth);
            grid.Columns.Add(columnWidth);

            string[] months =
            {
                "January",
                "February",
                "March",
                "April",
                "May",
                "June",
                "July",
                "August",
                "September",
                "October",
                "November",
                "December"
            };

            ConsoleColor color = Gray;
            if (renderedMonth.Month == DateTime.Now.Month)
                color = DarkCyan;

            grid.Children.Add(new Cell(months[renderedMonth.Month - 1]) { Align = Align.Center, Color = color, ColumnSpan = 7 });

            string[] days = 
            {
                "Monday",
                "Tuesday",
                "Wensday",
                "Thursday",
                "Friday",
                "Saturday",
                "Sunday"
            };

            for (int i = 0; i < days.Length; i++)
            {
                color = White;
                if ((int)DateTime.Now.DayOfWeek == i + 1 && renderedMonth.Month == DateTime.Now.Month)
                    color = DarkCyan;
                grid.Children.Add(new Cell(days[i]) { Align = Align.Center, Color = color });
            }

            doc.Children.Add(grid);

            DateTime renderedDay = new DateTime(renderedMonth.Year, renderedMonth.Month, 1);
            DayOfWeek dayOfWeek = renderedDay.DayOfWeek;

            while (dayOfWeek != DayOfWeek.Monday)
            {
                renderedDay -= TimeSpan.FromDays(1);
                dayOfWeek = renderedDay.DayOfWeek;
            }

            while (renderedDay.Month != renderedMonth.Month + 1)
            {
                AddDay(renderedDay);
                renderedDay = renderedDay.AddDays(1);
            }

            ConsoleRenderer.RenderDocument(doc);
        }
    }
}
