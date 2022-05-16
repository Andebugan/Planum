using System;
using System.Collections.Generic;
using Planum.Models.BuisnessLayer.RepoInterfaces;
using Planum.Models.DTO.ModelData;

namespace Planum.Models.DataLayer
{
    public class UserRepoFile : IUserRepo
    {
        public int AddUser(UserDTO userDTO)
        {
            throw new NotImplementedException();
        }

        public void DeleteUser(int id)
        {
            throw new NotImplementedException();
        }

        public UserDTO? FindUser(int id)
        {
            throw new NotImplementedException();
        }

        public List<UserDTO> GetAll()
        {
            throw new NotImplementedException();
        }

        public UserDTO GetUser(int id)
        {
            throw new NotImplementedException();
        }

        public void UpdateUser(UserDTO userDTO)
        {
            throw new NotImplementedException();
        }
    }
}
