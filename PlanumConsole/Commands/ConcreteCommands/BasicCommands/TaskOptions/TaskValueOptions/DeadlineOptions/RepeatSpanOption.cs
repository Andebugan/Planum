using Planum.Config;
using Planum.Parser;

namespace Planum.Console.Commands.Task
{
    public class RepeatSpanOption: BaseOption<TaskCommandSettings>
    {
        public RepeatSpanOption(OptionInfo optionInfo, ConsoleConfig commandConfig) : base(optionInfo, commandConfig) { }

        public override bool TryParseValue(ref IEnumerator<string> args, ref List<string> lines, ref TaskCommandSettings result)
        {
            TimeSpan repeatSpan = TimeSpan.Zero;
            if (!ValueParser.TryParse(ref repeatSpan, args.Current))
            {
                lines.Add(ConsoleSpecial.AddStyle($"Unable to parse deadline repeat span from: \"{args.Current}\"", foregroundColor: ConsoleInfoColors.Error));
                return false;
            }
            else
            {
                foreach (var task in result.Tasks)
                {
                    var filteredDeadlines = result.DeadlineFilter.Filter(task.Deadlines);
                    foreach (var deadline in filteredDeadlines)
                        deadline.repeatSpan = repeatSpan;
                    task.Deadlines = filteredDeadlines.ToHashSet();
                }
            }

            return true;
        }
    }
}
