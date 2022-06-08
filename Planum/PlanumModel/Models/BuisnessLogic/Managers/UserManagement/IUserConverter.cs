using Planum.Models.BuisnessLogic.Entities;
using Planum.Models.DTO;

namespace Planum.Models.BuisnessLogic.Managers
{
    public interface IUserConverter
    {
        UserDTO ConvertToDTO(User user);

        User ConvertFromDTO(UserDTO userDTO);
    }
}
