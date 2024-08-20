using System.Collections.Generic;
using System.Linq;
using Planum.Commands.Selector;

namespace Planum.Commands
{
    public abstract class BaseCommand<T> : ICommand<T>
    {
        public CommandInfo CommandInfo { get; set; }
        public List<IOption<T>> CommandOptions { get; set; }

        SelectorParser SelectorParser { get; set; }

        public BaseCommand(SelectorParser selectorParser, CommandInfo commandInfo, List<IOption<T>> commandOptions)
        {
            CommandInfo = commandInfo;
            SelectorParser = selectorParser;
            CommandOptions = commandOptions;
        }

        public abstract List<string> Execute(ref IEnumerator<string> args);
        public bool CheckMatch(ref IEnumerator<string> args) => args.Current.Trim(' ') == CommandInfo.Name;

       protected bool ParseSettings(ref IEnumerator<string> args, ref List<string> lines, ref T commandSettings)
        {
            bool match = false;
            bool parsingError = false;

            do
            {
                var arg = args.Current;
                var optionMatches = CommandOptions.Where(x => x.CheckMatch(arg));
                match = true;
                if (!optionMatches.Any())
                {
                    match = false;
                    break;
                }

                if (!optionMatches.First().TryParseValue(ref args, ref commandSettings))
                {
                    lines.Add(ConsoleSpecial.AddStyle($"Unable to parse option: {arg} {args.Current}", foregroundColor: ConsoleInfoColors.Error));
                    parsingError = false;
                    break;
                }
            }
            while (!match && !parsingError && args.MoveNext());

            return !parsingError;
        }
    }
}
