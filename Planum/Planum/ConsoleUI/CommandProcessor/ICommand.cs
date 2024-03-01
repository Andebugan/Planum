using Planum.ConsoleUI.CommandProcessor;
using System.Collections.Generic;

namespace Planum.ConsoleUI
{
    public interface ICommand
    {
        void Execute(List<string> args);
        string GetName();
        string GetDescription();
        string GetUsage();
        bool IsAvaliable();
        bool IsCommand(List<string> args);
        List<IOption> GetOptions();
    }
}
