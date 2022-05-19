using Planum.Models.BuisnessLayer.Entities;
using Planum.Models.DTO;

namespace Planum.Models.BuisnessLayer.Managers
{
    public interface ITagConverter
    {
        TagDTO ConvertToDTO(Tag tag);

        Tag ConvertFromDTO(TagDTO tagDTO);
    }
}
