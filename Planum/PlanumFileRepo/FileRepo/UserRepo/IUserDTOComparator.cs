using Planum.Models.DTO;

namespace Planum.DataModels
{
    public interface IUserDTOComparator
    {
        public bool CompareDTOs(int id_1, UserDTO userDTO_1, int id_2, UserDTO userDTO_2);
        public bool CompareDTOs(UserDTO userDTO_1, UserDTO userDTO_2);
    }
}
