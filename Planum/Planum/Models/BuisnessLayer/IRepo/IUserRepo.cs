using System.Collections.Generic;
using Planum.Models.DTO;

namespace Planum.Models.BuisnessLayer.RepoInterfaces
{
    public interface IUserRepo
    {
        public int AddUser(UserDTO userDTO);
        public void UpdateUser(UserDTO userDTO);
        public void DeleteUser(int id);
        public UserDTO GetUser(int id);
        public UserDTO? FindUser(int id);
        public List<UserDTO> GetAllUsers();
    }
}
