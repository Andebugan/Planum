using Serilog;
using System;

namespace Planum.Models.BuisnessLogic.Managers
{
    [Serializable]
    public class TaskDoesNotExistException : Exception
    {
        public TaskDoesNotExistException() { Log.Error("Task does not exist exception thrown"); }
        public TaskDoesNotExistException(string message) : base(message) { Log.Error("Task does not exist exception thrown"); }
        public TaskDoesNotExistException(string message, Exception innerException) : base(message, innerException) { Log.Error("Task does not exist exception thrown"); }
    }

    [Serializable]
    public class TaskHasIncorrectUser : Exception
    {
        public TaskHasIncorrectUser() { Log.Error("Task does not have correct user exception thrown"); }
        public TaskHasIncorrectUser(string message) : base(message) { Log.Error("Task does not have correct user exception thrown"); }
        public TaskHasIncorrectUser(string message, Exception innerException) : base(message, innerException) { Log.Error("Task does not have correct user exception thrown"); }
    }
}
