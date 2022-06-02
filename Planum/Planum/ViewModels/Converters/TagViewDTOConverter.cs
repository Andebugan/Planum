using Planum.Models.BuisnessLogic.Entities;

namespace Planum.ViewModels
{
    public class TagViewDTOConverter : ITagViewDTOConverter
    {
        public TagViewDTO ConvertToViewDTO(Tag tag)
        {
            TagViewDTO tagViewDTO = new TagViewDTO(tag.Id, tag.UserId, tag.Category, tag.Name, tag.Description);
            return tagViewDTO;
        }

        public Tag ConvertFromViewDTO(TagViewDTO tagViewDTO)
        {
            Tag tag = new Tag(tagViewDTO.Id, tagViewDTO.UserId, tagViewDTO.Category, tagViewDTO.Name, tagViewDTO.Description);
            return tag;
        }
    }
}
