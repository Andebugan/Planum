using System;

namespace Planum.Models.BuisnessLogic.Managers
{
    [Serializable]
    public class UserInvalidLoginOrPasswordException : Exception
    {
        public UserInvalidLoginOrPasswordException() { }
        public UserInvalidLoginOrPasswordException(string message) : base(message) { }
        public UserInvalidLoginOrPasswordException(string message, Exception innerException) : base(message, innerException) { }
    }

    [Serializable]
    public class UserLoginAlreadyExistException : Exception
    {
        public UserLoginAlreadyExistException() { }
        public UserLoginAlreadyExistException(string message) : base(message) { }
        public UserLoginAlreadyExistException(string message, Exception innerException) : base(message, innerException) { }
    }

    [Serializable]
    public class UserDoesNotExistException : Exception
    {
        public UserDoesNotExistException() { }
        public UserDoesNotExistException(string message) : base(message) { }
        public UserDoesNotExistException(string message, Exception innerException) : base(message, innerException) { }
    }
}
