using Planum.Models.DTO;

namespace Planum.DataModels
{
    public class TagDTOComparator: ITagDTOComparator
    {
        public bool CompareDTOs(int firstId, TagDTO firstDTO, int secondId, TagDTO secondDTO)
        {
            if (firstId != secondId)
                return false;
            if (firstDTO.UserId != secondDTO.UserId)
                return false;
            if (firstDTO.Category != secondDTO.Category)
                return false;
            if (firstDTO.Description != secondDTO.Description)
                return false;
            if (firstDTO.Name != secondDTO.Name)
                return false;
            return true;
        }

        public bool CompareDTOs(TagDTO firstDTO, TagDTO secondDTO)
        {
            if (firstDTO.Id != secondDTO.Id)
                return false;
            if (firstDTO.UserId != secondDTO.UserId)
                return false;
            if (firstDTO.Category != secondDTO.Category)
                return false;
            if (firstDTO.Description != secondDTO.Description)
                return false;
            if (firstDTO.Name != secondDTO.Name)
                return false;
            return true;
        }
    }
}
