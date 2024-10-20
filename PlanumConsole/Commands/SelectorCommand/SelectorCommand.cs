using Planum.Logger;
using Planum.Model.Filters;

namespace Planum.Console.Commands.Selector
{
    public abstract class SelectorCommand<T> : BaseCommand<T>
    {
        List<SelectorBaseOption> selectorOptions;

        public override IEnumerable<IOption> CommandOptions
        {
            get => selectorOptions.Select(x => (IOption)x)
                .Concat(commandOptions.Select(x => (IOption)x));
        }

        public SelectorCommand(List<SelectorBaseOption> selectorOptions, CommandInfo commandInfo, List<BaseOption<T>> commandOptions, ILoggerWrapper logger) : base(commandInfo, commandOptions, logger)
        {
            this.selectorOptions = selectorOptions;
            commandInfo.Description += " (match type modifiers: `+` for AND, `*` for OR, `!` for NOT, value type modifiers: `` for substring, `<` for lesser, `<=` for lesser and equal, `==` for equal, `>=` for greater and equal, `>` for greater)";
        }

        protected bool ParseSelectorSettings(ref IEnumerator<string> args, ref List<string> lines, ref TaskFilter taskFilter, ref bool match)
        {
            Logger.Log(message: "Parsing command settings");

            bool parsingError = false;

            while (!match && !parsingError && args.MoveNext())
            {
                match = false;
                string arg = args.Current;

                var matchedOptions = selectorOptions.Where(x => x.CheckMatch(x.ExtractOptionParams(arg)));
                if (!matchedOptions.Any() || !args.MoveNext())
                    break;
                else
                    match = true;

                ValueMatchType matchType;
                MatchFilterType filterType;
                SelectorBaseOption selectorOption = matchedOptions.First();
                selectorOption.ExtractOptionParams(arg, out matchType, out filterType);

                if (!selectorOption.TryParseValue(ref args, ref lines, ref taskFilter, matchType, filterType))
                {
                    Logger.Log(message: $"Unable to parse selector option: {arg} {args.Current}");
                    lines.Add(ConsoleSpecial.AddStyle($"Unable to parse selector option: {arg} {args.Current}", foregroundColor: ConsoleInfoColors.Error));
                    parsingError = true;
                    break;
                }
            }

            return !parsingError;
        }
    }
}
