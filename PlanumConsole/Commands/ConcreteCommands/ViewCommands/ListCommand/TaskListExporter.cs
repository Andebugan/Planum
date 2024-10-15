using Planum.Logger;
using Planum.Model;
using Planum.Model.Entities;
using Planum.Model.Exporters;

namespace Planum.Console.Commands.View
{
    public class TaskListExporter : TaskMarkdownExporter
    {
        public TaskListExporter(ModelConfig modelConfig, ILoggerWrapper logger) : base(modelConfig, logger) { }

        protected string ColorByStatus(string item, PlanumTaskStatus? status = null)
        {
            if (status == null)
                return item;
            else if (status == PlanumTaskStatus.NOT_STARTED)
                return ConsoleSpecial.AddStyle(item, TextStyle.Italic);
            else if (status == PlanumTaskStatus.WARNING)
                return ConsoleSpecial.AddStyle(item, foregroundColor: TextForegroundColor.Cyan);
            else if (status == PlanumTaskStatus.IN_PROGRESS)
                return ConsoleSpecial.AddStyle(item, foregroundColor: TextForegroundColor.BrightYellow);
            else if (status == PlanumTaskStatus.OVERDUE)
                return ConsoleSpecial.AddStyle(item, TextStyle.Bold, TextForegroundColor.Red);
            else if (status == PlanumTaskStatus.COMPLETE)
                return ConsoleSpecial.AddStyle(item, TextStyle.Strikethrough, TextForegroundColor.Green);
            else if (status == PlanumTaskStatus.DISABLED)
                return ConsoleSpecial.AddStyle(item, TextStyle.Dim, TextForegroundColor.White);
            else
                return item;
        }

        protected override string AddTaskItem(string symbol, string value, int level = 0, PlanumTaskStatus? status = null)
        {
            return ColorByStatus(base.AddTaskItem(symbol, value, level, status), status);
        }
    }
}
