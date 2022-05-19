using Planum.Models.BuisnessLayer.Entities;
using Planum.Models.DTO;

namespace Planum.Models.BuisnessLayer.Managers
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
