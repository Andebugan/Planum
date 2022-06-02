using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Planum.ViewModels
{
    public class UserViewDTO
    {
        public int Id { get; set; }

        public string Login { get; set; }

        public string Password { get; set; }

        public UserViewDTO(int id, string login, string password)
        {
            if (string.IsNullOrWhiteSpace(login))
                throw new ArgumentException("User name can not be null or empty", nameof(login));

            Id = id;
            Login = login;
            Password = password;
        }
    }
}
