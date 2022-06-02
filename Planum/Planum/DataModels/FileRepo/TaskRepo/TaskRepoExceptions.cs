using Serilog;
using System;

namespace Planum.Models.DataModels
{
    [Serializable]
    public class TaskDoesNotExistException : Exception
    {
        public TaskDoesNotExistException() { Log.Error("Task does not exist exception was thrown"); }
        public TaskDoesNotExistException(string message) : base(message) { Log.Error("Task does not exist exception was thrown"); }
        public TaskDoesNotExistException(string message, Exception innerException) : base(message, innerException) { Log.Error("Task does not exist exception was thrown"); }
    }

    [Serializable]
    public class ArchivedTaskDoesNotExistException : Exception
    {
        public ArchivedTaskDoesNotExistException() { Log.Error("Archived task does not exist exception was thrown"); }
        public ArchivedTaskDoesNotExistException(string message) : base(message) { Log.Error("Archived task does not exist exception was thrown"); }
        public ArchivedTaskDoesNotExistException(string message, Exception innerException) : base(message, innerException) { Log.Error("Archived task does not exist exception was thrown"); }
    }
    

    [Serializable]
    public class CantAddTaskToRepoException : Exception
    {
        public CantAddTaskToRepoException() { Log.Error("Can't add task to repo exception was thrown"); }
        public CantAddTaskToRepoException(string message) : base(message) { Log.Error("Can't add task to repo exception was thrown"); }
        public CantAddTaskToRepoException(string message, Exception innerException) : base(message, innerException) { Log.Error("Can't add task to repo exception was thrown"); }
    }
}
