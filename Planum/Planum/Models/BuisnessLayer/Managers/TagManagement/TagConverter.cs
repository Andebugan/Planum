using Planum.Models.BuisnessLayer.Entities;
using Planum.Models.DTO.ModelData;

namespace Planum.Models.BuisnessLayer.Managers.TagManagement
{
    public class TagConverter
    {
        protected TagDTO ConvertToDTO(Tag tag)
        {
            TagDTO tagDTO = new TagDTO(tag.Id, tag.UserId, tag.Category, tag.Name, tag.Description);
            return tagDTO;
        }

        protected Tag ConvertFromDTO(TagDTO tagDTO)
        {
            Tag tag = new Tag(tagDTO.Id, tagDTO.UserId, tagDTO.Category, tagDTO.Name, tagDTO.Description);
            return tag;
        }
    }
}
