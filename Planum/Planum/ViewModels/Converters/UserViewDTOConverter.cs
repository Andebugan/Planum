using Planum.Models.BuisnessLogic.Entities;
using Planum.Models.DTO;

namespace Planum.ViewModels
{
    public class UserViewDTOConverter : IUserViewDTOConverter
    {
        public UserViewDTO ConvertToViewDTO(User user)
        {
            UserViewDTO userViewDTO = new UserViewDTO(user.Id, user.Login, user.Password);
            return userViewDTO;
        }

        public User ConvertFromViewDTO(UserViewDTO userViewDTO)
        {
            User user = new User(userViewDTO.Id, userViewDTO.Login, userViewDTO.Password);
            return user;
        }
    }
}
