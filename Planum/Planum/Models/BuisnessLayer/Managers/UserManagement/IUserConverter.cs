using Planum.Models.BuisnessLayer.Entities;
using Planum.Models.DTO.ModelData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Planum.Models.BuisnessLayer.Managers.UserManagement
{
    public interface IUserConverter
    {
        UserDTO ConvertToDTO(User user);

        User ConvertFromDTO(UserDTO userDTO);
    }
}
