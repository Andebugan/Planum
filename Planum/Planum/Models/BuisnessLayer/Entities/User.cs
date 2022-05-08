using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Planum.Models.BuisnessLayer.Entities
{
    struct UserParams
    {
        public int id;
        public string login;
        public string password;
    }

    internal class User
    {
        protected int _id;
        public int Id { get { return _id; } }

        protected string _login = "user";
        public string Login { get { return _login; } }

        protected string _password = "user";
        public string Password { get { return _password; } }

        public User(UserParams userParams)
        {
            _id = userParams.id;
            _login = userParams.login;
            _password = userParams.password;
        }

        public void Update(UserParams userParams)
        {
            _id = userParams.id;
            _login = userParams.login;
            _password = userParams.password;
        }
    }
}
