using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Planum.Models.DTO.ModelData;

namespace Planum.Models.BuisnessLayer.Interfaces
{
    internal interface ITagRepo
    {
        // TODO: check if already existis -> existance check for repo interface
        public void Add(TagDTO tag);
        public void Update(TagDTO tag);
        public void Delete(int id);
        public TagDTO Get(int id);
        public List<TagDTO> GetAll();
        public void Reset();
    }
}
