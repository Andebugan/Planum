using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Planum.Models.BuisnessLayer.Entities;
using Planum.Models.DTO.ModelData;

namespace Planum.Models.BuisnessLayer.Managers.TagManagement
{
    public interface ITagConverter
    {
        TagDTO ConvertToDTO(Tag tag);

        Tag ConvertFromDTO(TagDTO tagDTO);
    }
}
