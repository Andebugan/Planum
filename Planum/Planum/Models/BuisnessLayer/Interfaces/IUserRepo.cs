using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Planum.Models.DTO.ModelData;

namespace Planum.Models.BuisnessLayer.Interfaces
{
    internal interface IUserRepo
    {
        public void Add(UserDTO userDTO);
        public void Update(UserDTO userDTO);
        public void Delete(int id);
        public UserDTO Get(int id);
        public List<UserDTO> GetAll();
        public void Reset();
    }
}
