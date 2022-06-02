using Serilog;
using System;

namespace Planum.Models.BuisnessLogic.Managers
{
    [Serializable]
    public class TagDoesNotExistException : Exception
    {
        public TagDoesNotExistException() { Log.Error("Tag does not exist exception was thrown"); }
        public TagDoesNotExistException(string message) : base(message) { Log.Error("Tag does not exist exception was thrown"); }
        public TagDoesNotExistException(string message, Exception innerException) : base(message, innerException) { Log.Error("Tag does not exist exception was thrown"); }
    }

    [Serializable]
    public class TagDoesNotHaveCorrectUser : Exception
    {
        public TagDoesNotHaveCorrectUser() { Log.Error("Tag does not have correct user exception was thrown"); }
        public TagDoesNotHaveCorrectUser(string message) : base(message) { Log.Error("Tag does not have correct user exception was thrown"); }
        public TagDoesNotHaveCorrectUser(string message, Exception innerException) : base(message, innerException) { Log.Error("Tag does not have correct user exception was thrown"); }
    }
}
