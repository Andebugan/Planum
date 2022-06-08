using Serilog;
using System;

namespace Planum.Models.DataModels
{
    [Serializable]
    public class UserDoesNotExist : Exception
    {
        public UserDoesNotExist() { Log.Error("User does not exist exception was thrown"); }
        public UserDoesNotExist(string message) : base(message) { Log.Error("User does not exist exception was thrown"); }
        public UserDoesNotExist(string message, Exception innerException) : base(message, innerException) { Log.Error("User does not exist exception was thrown"); }
    }

    [Serializable]
    public class CantAddUserToRepoException : Exception
    {
        public CantAddUserToRepoException() { Log.Error("Can't add user to repo exception was thrown"); }
        public CantAddUserToRepoException(string message) : base(message) { Log.Error("Can't add user to repo exception was thrown"); }
        public CantAddUserToRepoException(string message, Exception innerException) : base(message, innerException) { Log.Error("Can't add user to repo exception was thrown"); }
    }
}
