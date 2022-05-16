using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Planum.Models.DTO.ModelData
{
    public class UserDTO
    {
        public int Id { get; protected set; }
        public string? Login { get; protected set; }
        public string? Password { get; protected set; }

        public UserDTO(int id, string? login, string? password)
        {
            Id = id;
            Login = login;
            Password = password;
        }
    }
}
