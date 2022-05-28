using System;

namespace Planum.Models.BuisnessLogic.Managers
{
    [Serializable]
    public class TaskDoesNotExistException : Exception
    {
        public TaskDoesNotExistException() { }
        public TaskDoesNotExistException(string message) : base(message) { }
        public TaskDoesNotExistException(string message, Exception innerException) : base(message, innerException) { }
    }
}
