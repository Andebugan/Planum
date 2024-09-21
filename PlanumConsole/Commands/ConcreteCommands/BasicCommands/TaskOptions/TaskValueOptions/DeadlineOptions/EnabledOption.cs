using Planum.Config;
using Planum.Logger;
using Planum.Parser;

namespace Planum.Console.Commands.Task
{
    public class EnabledOption: BaseOption<TaskCommandSettings>
    {
        public EnabledOption(ILoggerWrapper logger, OptionInfo optionInfo, ConsoleConfig commandConfig) : base(logger, optionInfo, commandConfig) { }
        public override bool TryParseValue(ref IEnumerator<string> args, ref List<string> lines, ref TaskCommandSettings result)
        {
            if (!args.MoveNext())
            {
                lines.Add(ConsoleSpecial.AddStyle($"No arguments provided for option: {OptionInfo.Name}", foregroundColor: ConsoleInfoColors.Error));
                return false;
            }

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
