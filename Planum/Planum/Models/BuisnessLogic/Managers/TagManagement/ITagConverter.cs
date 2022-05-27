using Planum.Models.BuisnessLogic.Entities;
using Planum.Models.DTO;

namespace Planum.Models.BuisnessLogic.Managers
{
    public interface ITagConverter
    {
        TagDTO ConvertToDTO(Tag tag);

        Tag ConvertFromDTO(TagDTO tagDTO);
    }
}
