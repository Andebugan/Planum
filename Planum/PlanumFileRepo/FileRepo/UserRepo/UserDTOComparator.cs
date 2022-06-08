using Planum.Models.DTO;

namespace Planum.DataModels
{
    public class UserDTOComparator: IUserDTOComparator
    {
        public bool CompareDTOs(int firstId, UserDTO firstDTO, int secondId, UserDTO secondDTO)
        {
            if (firstId != secondId)
                return false;
            if (firstDTO.Login != secondDTO.Login)
                return false;
            if (firstDTO.Password != secondDTO.Password)
                return false;
            return true;
        }

        public bool CompareDTOs(UserDTO firstDTO, UserDTO secondDTO)
        {
            if (firstDTO.Id != secondDTO.Id)
                return false;
            if (firstDTO.Login != secondDTO.Login)
                return false;
            if (firstDTO.Password != secondDTO.Password)
                return false;
            return true;
        }
    }
}
