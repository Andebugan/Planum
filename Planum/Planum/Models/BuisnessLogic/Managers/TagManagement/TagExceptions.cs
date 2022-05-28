using System;

namespace Planum.Models.BuisnessLogic.Managers
{
    [Serializable]
    public class TagDoesNotExistException : Exception
    {
        public TagDoesNotExistException() { }
        public TagDoesNotExistException(string message) : base(message) { }
        public TagDoesNotExistException(string message, Exception innerException) : base(message, innerException) { }
    }
}
