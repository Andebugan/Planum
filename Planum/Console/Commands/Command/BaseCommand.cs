using System.Collections.Generic;
using System.Linq;
using Planum.Commands.Selector;
using Planum.Logger;

namespace Planum.Commands
{
    public abstract class BaseCommand<T> : ICommand
    {
        public CommandInfo CommandInfo { get; set; }

        List<BaseOption<T>> commandOptions;
        public IEnumerable<IOption> CommandOptions
        {
            get => commandOptions.Select(x => (IOption)x);
        }

        protected ILoggerWrapper Logger { get; set; }
        SelectorParser SelectorParser { get; set; }

        public BaseCommand(SelectorParser selectorParser, CommandInfo commandInfo, List<BaseOption<T>> commandOptions, ILoggerWrapper logger)
        {
            CommandInfo = commandInfo;
            SelectorParser = selectorParser;
            this.commandOptions = commandOptions;
            Logger = logger;
        }

        public abstract List<string> Execute(ref IEnumerator<string> args);
        public bool CheckMatch(string value) => value.Trim(' ') == CommandInfo.Name;

        protected bool ParseSettings(ref IEnumerator<string> args, ref List<string> lines, ref T commandSettings)
        {
            Logger.Log(message: "Parsing command settings");
            bool match = false;
            bool parsingError = false;

            do
            {
                var arg = args.Current;
                var optionMatches = commandOptions.Where(x => x.CheckMatch(arg));
                match = true;
                if (!optionMatches.Any())
                {
                    Logger.Log(message: $"Unable to find option: {arg}");
                    lines.Add(ConsoleSpecial.AddStyle($"Unable to find option: {arg}", foregroundColor: ConsoleInfoColors.Error));
                    match = false;
                    break;
                }
                else
                    args.MoveNext();

                if (!optionMatches.First().TryParseValue(ref args, ref commandSettings))
                {
                    Logger.Log(message: $"Unable to parser option: {arg} {args.Current}");
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
