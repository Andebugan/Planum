using System.Collections.Generic;
using System.Linq;
using Planum.Config;
using Planum.Parser;

namespace Planum.Console.Commands.Task
{
    public class RepeatMonthsOption: BaseOption<TaskCommandSettings>
    {
        public RepeatMonthsOption(OptionInfo optionInfo, ConsoleConfig commandConfig) : base(optionInfo, commandConfig) { }

        public override bool TryParseValue(ref IEnumerator<string> args, ref List<string> lines, ref TaskCommandSettings result)
        {
            int months = 0;
            if (!ValueParser.TryParse(ref months, args.Current))
            {
                lines.Add(ConsoleSpecial.AddStyle($"Unable to parse deadline repeat months from: \"{args.Current}\"", foregroundColor: ConsoleInfoColors.Error));
                return false;
            }
            else
            {
                foreach (var task in result.Tasks)
                {
                    var filteredDeadlines = result.DeadlineFilter.Filter(task.Deadlines);
                    foreach (var deadline in filteredDeadlines)
                        deadline.repeatMonths = months;
                    task.Deadlines = filteredDeadlines.ToHashSet();
                }
            }
            return true;
        }
    }
}
