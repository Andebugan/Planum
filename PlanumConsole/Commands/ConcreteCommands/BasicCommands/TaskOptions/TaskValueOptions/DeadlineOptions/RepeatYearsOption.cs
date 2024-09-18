using Planum.Config;
using Planum.Parser;

namespace Planum.Console.Commands.Task
{
    public class RepeatYearsOption: BaseOption<TaskCommandSettings>
    {
        public RepeatYearsOption(OptionInfo optionInfo, ConsoleConfig commandConfig) : base(optionInfo, commandConfig) { }

        public override bool TryParseValue(ref IEnumerator<string> args, ref List<string> lines, ref TaskCommandSettings result)
        {
            int years = 0;
            if (!ValueParser.TryParse(ref years, args.Current))
            {
                lines.Add(ConsoleSpecial.AddStyle($"Unable to parse deadline repeat years from: \"{args.Current}\"", foregroundColor: ConsoleInfoColors.Error));
                return false;
            }
            else
            {
                foreach (var task in result.Tasks)
                {
                    var filteredDeadlines = result.DeadlineFilter.Filter(task.Deadlines);
                    foreach (var deadline in filteredDeadlines)
                        deadline.repeatYears = years;
                    task.Deadlines = filteredDeadlines.ToHashSet();
                }
            }

            return true;
        }
    }
}
