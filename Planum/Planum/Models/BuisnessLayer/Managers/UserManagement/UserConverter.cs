using Planum.Models.BuisnessLayer.Entities;
using Planum.Models.DTO.ModelData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Planum.Models.BuisnessLayer.Managers.UserManagement
{
    public class UserConverter
    {
        UserDTO ConvertToDTO(User user)
        {
            UserDTO userDTO = new UserDTO(user.Id, user.Login, user.Password);
            return userDTO;
        }

        User ConvertFromDTO(UserDTO userDTO)
        {
            User user = new User(userDTO.Id, userDTO.Login, userDTO.Password);
            return user;
        }
    }
}
