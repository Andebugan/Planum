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

        public override bool Equals(Object? obj)
        {
            //Check for null and compare run-time types.
            if ((obj == null) || !this.GetType().Equals(obj.GetType()))
            {
                return false;
            }
            else
            {
                User user = (User)obj;
                return user.Id == Id && user.Login == Login && user.Password == Password;
            }
        }

        public override string ToString()
        {
            return String.Format("Point({0}, {1}, {2})", Id, Login, Password);
        }

        public override int GetHashCode()
        {
            throw new NotImplementedException();
        }
    }
}
