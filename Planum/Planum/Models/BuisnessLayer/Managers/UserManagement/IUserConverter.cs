using Planum.Models.BuisnessLayer.Entities;
using Planum.Models.DTO;

namespace Planum.Models.BuisnessLayer.Managers
{
    public interface IUserConverter
    {
        UserDTO ConvertToDTO(User user);

        User ConvertFromDTO(UserDTO userDTO);
    }
}
