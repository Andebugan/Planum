using Planum.Models.BuisnessLogic.Entities;

namespace Planum.ViewModels
{
    public interface ITagViewDTOConverter
    {
        TagViewDTO ConvertToViewDTO(Tag tag);

        Tag ConvertFromViewDTO(TagViewDTO tagViewDTO);
    }
}
