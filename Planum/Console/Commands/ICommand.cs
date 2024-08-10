namespace Planum.Console.Commands
{
    public interface ICommand
    {
        public bool CheckCommandMatch(string[] command);
        public string[] Execute(string[] args);
    }
}
