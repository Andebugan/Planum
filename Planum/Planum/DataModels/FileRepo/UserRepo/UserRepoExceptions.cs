using System;

namespace Planum.Models.DataModels
{
    [Serializable]
    public class UserDoesNotExist : Exception
    {
        public UserDoesNotExist() { }
        public UserDoesNotExist(string message) : base(message) { }
        public UserDoesNotExist(string message, Exception innerException) : base(message, innerException) { }
    }

    [Serializable]
    public class CantAddUserToRepoException : Exception
    {
        public CantAddUserToRepoException() { }
        public CantAddUserToRepoException(string message) : base(message) { }
        public CantAddUserToRepoException(string message, Exception innerException) : base(message, innerException) { }
    }
}
