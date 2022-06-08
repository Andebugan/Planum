using Planum.Models.DTO;

namespace Planum.DataModels
{
    public interface ITagDTOComparator
    {
        bool CompareDTOs(int firstId, TagDTO firstDTO, int secondId, TagDTO secondDTO);
        bool CompareDTOs(TagDTO firstDTO, TagDTO secondDTO);
    }
}
