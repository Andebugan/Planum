using Planum.Models.BuisnessLogic.Entities;
using Planum.Models.DTO;

namespace Planum.ViewModels
{
    public interface IUserViewDTOConverter
    {
        UserViewDTO ConvertToViewDTO(User user);

        User ConvertFromViewDTO(UserViewDTO userViewDTO);
    }
}
