using System;

namespace Planum.Models.DTO
{
    public class UserDTO
    {
        public int Id { get; }
        public string Login { get; }
        public string Password { get; }

        public UserDTO(int id, string login = "", string password = "")
        {
            Id = id;
            Login = login;
            Password = password;
        }
    }
}
