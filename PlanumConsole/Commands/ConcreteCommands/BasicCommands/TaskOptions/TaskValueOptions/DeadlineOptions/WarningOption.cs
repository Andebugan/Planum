using Planum.Config;
using Planum.Parser;

namespace Planum.Console.Commands.Task
{
    public class WarningOption: BaseOption<TaskCommandSettings>
    {
        public WarningOption(OptionInfo optionInfo, ConsoleConfig commandConfig) : base(optionInfo, commandConfig) { }

        public override bool TryParseValue(ref IEnumerator<string> args, ref List<string> lines, ref TaskCommandSettings result)
        {
            TimeSpan warning = TimeSpan.Zero;
            if (!ValueParser.TryParse(ref warning, args.Current))
            {
                lines.Add(ConsoleSpecial.AddStyle($"Unable to parse deadline duration from: \"{args.Current}\"", foregroundColor: ConsoleInfoColors.Error));
                return false;
            }
            else
            {
                foreach (var task in result.Tasks)
                {
                    var filteredDeadlines = result.DeadlineFilter.Filter(task.Deadlines);
                    foreach (var deadline in filteredDeadlines)
                        deadline.warningTime = warning;
                    task.Deadlines = filteredDeadlines.ToHashSet();
                }
            }
            return true;
        }
    }
}
