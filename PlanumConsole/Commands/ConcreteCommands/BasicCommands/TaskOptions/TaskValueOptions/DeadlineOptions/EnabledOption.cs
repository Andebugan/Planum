using Planum.Config;
using Planum.Parser;

namespace Planum.Console.Commands.Task
{
    public class EnabledOption: BaseOption<TaskCommandSettings>
    {
        public EnabledOption(OptionInfo optionInfo, ConsoleConfig commandConfig) : base(optionInfo, commandConfig) { }

        public override bool TryParseValue(ref IEnumerator<string> args, ref List<string> lines, ref TaskCommandSettings result)
        {
            bool enabled = false;
            if (!ValueParser.TryParse(ref enabled, args.Current))
            {
                lines.Add(ConsoleSpecial.AddStyle($"Unable to parse deadline enabled from: \"{args.Current}\"", foregroundColor: ConsoleInfoColors.Error));
                return false;
            }
            else
            {
                foreach (var task in result.Tasks)
                {
                    var filteredDeadlines = result.DeadlineFilter.Filter(task.Deadlines);
                    foreach (var deadline in filteredDeadlines)
                        deadline.enabled = enabled;
                    task.Deadlines = filteredDeadlines.ToHashSet();
                }
            }

            return true;
        }
    }
}
