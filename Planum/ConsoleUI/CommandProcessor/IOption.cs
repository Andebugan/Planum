using System;
using System.Collections.Generic;
using System.Text;

namespace Planum.ConsoleUI.CommandProcessor
{
    public interface IOption
    {
        bool Optional { get; }
        bool Used { get; }
        string Name { get; }
        string Description { get; }
        string Usage { get; }

        public void Reset();
        public bool GetValue(ref List<string> args, ref string error);
        public string GetDefault();
    }
}
