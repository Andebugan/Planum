using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Planum.Models.DTO.ModelData;

namespace Planum.Models.BuisnessLayer.RepoInterfaces
{
    public interface IUserRepo
    {
        public int AddUser(UserDTO userDTO);
        public void UpdateUser(UserDTO userDTO);
        public void DeleteUser(int id);
        public UserDTO GetUser(int id);
        public UserDTO? FindUser(int id);
        public List<UserDTO> GetAll();
    }
}
