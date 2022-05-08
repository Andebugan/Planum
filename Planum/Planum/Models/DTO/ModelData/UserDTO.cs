using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Planum.Models.DTO.ModelData
{
    struct UserParamsDTO
    {
        public int id;
        public string login;
        public string password;
    }
    internal class UserDTO
    {
        protected int _id;
        public int Id { get { return _id; } set { _id = value; } }

        protected string _login = "user";
        public string Login { get { return _login; } set { _login = value; } }

        protected string _password = "user";
        public string Password { get { return _password; } set { _password = value; } }

        public UserDTO(UserParamsDTO userParams)
        {
            Id = userParams.id;
            Login = userParams.login;
            Password = userParams.password;
        }
    }
}
