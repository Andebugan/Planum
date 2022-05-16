using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Planum.Models.BuisnessLayer.Entities
{
    public class User
    {
        public int Id { get; protected set; }

        public string? Login { get; protected set; }

        public string? Password { get; protected set; }

        public User(int id, string? login, string? password)
        {
            Id = id;
            Login = login;
            Password = password;
        }

        public User(User user)
        {
            Id = user.Id;
            Login = user.Login;
            Password = user.Password;
        }
    }
}
