using Serilog;
using System;

namespace Planum.Models.DataModels
{
    [Serializable]
    public class TagDoesNotExistException : Exception
    {
        public TagDoesNotExistException() { Log.Error("Tag does not exist exception was thrown"); }
        public TagDoesNotExistException(string message) : base(message) { Log.Error("Tag does not exist exception was thrown"); }
        public TagDoesNotExistException(string message, Exception innerException) : base(message, innerException) { Log.Error("Tag does not exist exception was thrown"); }
    }
}
