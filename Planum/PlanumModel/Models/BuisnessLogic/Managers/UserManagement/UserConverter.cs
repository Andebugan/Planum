using Planum.Models.BuisnessLogic.Entities;
using Planum.Models.DTO;

namespace Planum.Models.BuisnessLogic.Managers
{
    public class UserConverter : IUserConverter
    {
        public UserDTO ConvertToDTO(User user)
        {
            UserDTO userDTO = new UserDTO(user.Id, user.Login, user.Password);
            return userDTO;
        }

        public User ConvertFromDTO(UserDTO userDTO)
        {
            User user = new User(userDTO.Id, userDTO.Login, userDTO.Password);
            return user;
        }
    }
}
