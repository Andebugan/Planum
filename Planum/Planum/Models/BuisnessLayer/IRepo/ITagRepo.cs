using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Planum.Models.DTO.ModelData;

namespace Planum.Models.BuisnessLayer.RepoInterfaces
{
    public interface ITagRepo
    {
        // TODO: check if already existis -> existance check for repo interface
        public void AddTag(TagDTO tag);
        public void UpdateTag(TagDTO tag);
        public void DeleteTag(int id);
        public TagDTO GetTag(int id);
        public TagDTO? FindTag(int id);
        public List<TagDTO> GetAll();
    }
}
