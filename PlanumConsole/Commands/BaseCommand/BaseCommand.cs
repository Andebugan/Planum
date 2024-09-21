using Planum.Logger;

namespace Planum.Console.Commands
{
    public abstract class BaseCommand<T> : ICommand
    {
        public CommandInfo CommandInfo { get; set; }

        protected List<BaseOption<T>> commandOptions;
        public virtual IEnumerable<IOption> CommandOptions
        {
            get => commandOptions.Select(x => (IOption)x);
        }

        protected ILoggerWrapper Logger { get; set; }

        public BaseCommand(CommandInfo commandInfo, List<BaseOption<T>> commandOptions, ILoggerWrapper logger)
        {
            CommandInfo = commandInfo;
            this.commandOptions = commandOptions;
            Logger = logger;
        }

        public abstract List<string> Execute(ref IEnumerator<string> args);
        public bool CheckMatch(string value) => value.Trim(' ') == CommandInfo.Name;

        protected bool ParseSettings(ref IEnumerator<string> args, ref List<string> lines, ref T commandSettings)
        {
            Logger.Log(message: "Parsing command settings");

            bool match = true;
            bool parsingError = false;

            while (match && !parsingError && args.MoveNext())
            {
                var arg = args.Current;
                var optionMatches = commandOptions.Where(x => x.CheckMatch(arg));
                match = optionMatches.Any();
                if (!optionMatches.Any())
                {
                    Logger.Log(message: $"Unable to find option: {arg}");
                    lines.Add(ConsoleSpecial.AddStyle($"Unable to find option: {arg}", foregroundColor: ConsoleInfoColors.Error));
                    break;
                }

                if (!optionMatches.First().TryParseValue(ref args, ref lines, ref commandSettings))
                {
                    Logger.Log(message: $"Unable to parser option: {arg} {args.Current}");
                    lines.Add(ConsoleSpecial.AddStyle($"Unable to parse option: {arg} {args.Current}", foregroundColor: ConsoleInfoColors.Error));
                    parsingError = true;
                    break;
                }
            }

            return !parsingError;
        }
    }
}
