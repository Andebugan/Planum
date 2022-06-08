namespace Planum.ConsoleUI
{
    public interface ICommand
    {
        void Execute();
        string GetName();
        string GetDescription();
        bool IsAvaliable();
    }
}
