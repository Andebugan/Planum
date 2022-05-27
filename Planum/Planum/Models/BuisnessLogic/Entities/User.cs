using System;

namespace Planum.Models.BuisnessLogic.Entities
{
    public class User
    {
        public int Id { get; }

        public string Login { get; }

        public string Password { get; }

        public User(int id, string login, string password)
        {
            if (string.IsNullOrWhiteSpace(login))
                throw new ArgumentException("User name can not be null or empty", nameof(login));

            Id = id;
            Login = login;
            Password = password;
        }
    }
}
