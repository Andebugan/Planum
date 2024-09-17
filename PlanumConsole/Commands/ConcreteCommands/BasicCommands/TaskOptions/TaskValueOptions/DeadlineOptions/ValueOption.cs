using System;
using System.Collections.Generic;
using System.Linq;
using Planum.Config;
using Planum.Parser;

namespace Planum.Console.Commands.Task
{
    public class ValueOption: BaseOption<TaskCommandSettings>
    {
        public ValueOption(OptionInfo optionInfo, CommandConfig commandConfig) : base(optionInfo, commandConfig) { }

        public override bool TryParseValue(ref IEnumerator<string> args, ref List<string> lines, ref TaskCommandSettings result)
        {
            DateTime deadline = new DateTime();
            if (!ValueParser.TryParse(ref deadline, args.Current))
            {
                lines.Add(ConsoleSpecial.AddStyle($"Unable to parse deadline value from: \"{args.Current}\"", foregroundColor: ConsoleInfoColors.Error));
                return false;
            }
            else
            {
                foreach (var task in result.Tasks)
                {
                    var filteredDeadlines = result.DeadlineFilter.Filter(task.Deadlines);
                    foreach (var Deadline in filteredDeadlines)
                        Deadline.deadline = deadline;
                    task.Deadlines = filteredDeadlines.ToHashSet();
                }
            }
            return true;
        }
    }
}
