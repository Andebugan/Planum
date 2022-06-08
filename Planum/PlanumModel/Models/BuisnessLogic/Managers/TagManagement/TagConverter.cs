using Planum.Models.BuisnessLogic.Entities;
using Planum.Models.DTO;

namespace Planum.Models.BuisnessLogic.Managers
{
    public class TagConverter : ITagConverter
    {
        public TagDTO ConvertToDTO(Tag tag)
        {
            TagDTO tagDTO = new TagDTO(tag.Id, tag.UserId, tag.Category, tag.Name, tag.Description);
            return tagDTO;
        }

        public Tag ConvertFromDTO(TagDTO tagDTO)
        {
            Tag tag = new Tag(tagDTO.Id, tagDTO.UserId, tagDTO.Category, tagDTO.Name, tagDTO.Description);
            return tag;
        }
    }
}
