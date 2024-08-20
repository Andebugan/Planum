using System;

namespace Planum.Commands.Selector
{
    [System.Serializable]
    /// <summary>
    /// Selector parser exception
    /// </summary>
    public class SelectorException : Exception
    {
        public OptionInfo OptionInfo { get; set; }

        public SelectorException(OptionInfo optionInfo) => OptionInfo = optionInfo;
        public SelectorException(string message, OptionInfo optionInfo) : base(message) => OptionInfo = optionInfo;
        public SelectorException(string message, Exception inner, OptionInfo optionInfo) : base(message, inner) => OptionInfo = optionInfo;
    }
}
