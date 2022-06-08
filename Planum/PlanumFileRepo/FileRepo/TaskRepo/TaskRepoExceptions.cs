using System;

namespace Planum.Models.DataModels
{
    [Serializable]
    public class TaskDoesNotExistException : Exception
    {
        public TaskDoesNotExistException() { }
        public TaskDoesNotExistException(string message) : base(message) { }
        public TaskDoesNotExistException(string message, Exception innerException) : base(message, innerException) { }
    }

    [Serializable]
    public class ArchivedTaskDoesNotExistException : Exception
    {
        public ArchivedTaskDoesNotExistException() { }
        public ArchivedTaskDoesNotExistException(string message) : base(message) { }
        public ArchivedTaskDoesNotExistException(string message, Exception innerException) : base(message, innerException) { }
    }
    

    [Serializable]
    public class CantAddTaskToRepoException : Exception
    {
        public CantAddTaskToRepoException() { }
        public CantAddTaskToRepoException(string message) : base(message) { }
        public CantAddTaskToRepoException(string message, Exception innerException) : base(message, innerException) { }
    }
}
