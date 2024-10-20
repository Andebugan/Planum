namespace Planum.Console.Commands
{
    public interface ICommand
    {
        public CommandInfo CommandInfo { get; set; }
        public IEnumerable<IOption> CommandOptions { get; }
        public bool CheckMatch(string value);
        public abstract List<string> Execute(ref IEnumerator<string> args);
        public bool WasExecuted { get; set; }
    }
}
