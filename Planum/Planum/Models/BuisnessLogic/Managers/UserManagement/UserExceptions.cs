using Serilog;
using System;

namespace Planum.Models.BuisnessLogic.Managers
{
    [Serializable]
    public class UserInvalidLoginOrPasswordException : Exception
    {
        public UserInvalidLoginOrPasswordException() { Log.Warning("Invalid login of password exception thrown"); }
        public UserInvalidLoginOrPasswordException(string message) : base(message) { Log.Warning("Invalid login of password exception thrown"); }
        public UserInvalidLoginOrPasswordException(string message, Exception innerException) : base(message, innerException) { Log.Warning("Invalid login of password exception thrown"); }
    }

    [Serializable]
    public class UserLoginAlreadyExistException : Exception
    {
        public UserLoginAlreadyExistException() { Log.Warning("User already exist exception thrown"); }
        public UserLoginAlreadyExistException(string message) : base(message) { Log.Warning("User already exist exception thrown"); }
        public UserLoginAlreadyExistException(string message, Exception innerException) : base(message, innerException) { Log.Warning("User already exist exception thrown"); }
    }

    [Serializable]
    public class UserDoesNotExistException : Exception
    {
        public UserDoesNotExistException() { Log.Error("User does not exist exception thrown");  }
        public UserDoesNotExistException(string message) : base(message) { Log.Error("User does not exist exception thrown"); }
        public UserDoesNotExistException(string message, Exception innerException) : base(message, innerException) { Log.Error("User does not exist exception thrown"); }
    }

    [Serializable]
    public class CurrentUserIsNullException : Exception
    {
        public CurrentUserIsNullException() { Log.Warning("Current user is null exception thrown"); }
        public CurrentUserIsNullException(string message) : base(message) { Log.Warning("Current user is null exception thrown"); }
        public CurrentUserIsNullException(string message, Exception innerException) : base(message, innerException) { Log.Warning("Current user is null exception thrown"); }
    }

    [Serializable]
    public class IncorrectUserException : Exception
    {
        public IncorrectUserException() { Log.Error("Incorrect user exception thrown"); }
        public IncorrectUserException(string message) : base(message) { Log.Error("Incorrect user exception thrown"); }
        public IncorrectUserException(string message, Exception innerException) : base(message, innerException) { Log.Error("Incorrect user exception thrown"); }
    }
}
