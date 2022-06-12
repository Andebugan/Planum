namespace Planum.ConsoleUI
{
    public interface ICommand
    {
        void Execute(string command);
        string GetName();
        string GetDescription();
        bool IsAvaliable();
        bool IsCommand(string command);
    }
}
