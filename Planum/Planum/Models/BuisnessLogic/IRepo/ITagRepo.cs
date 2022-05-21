using Planum.Models.DTO;
using System.Collections.Generic;


namespace Planum.Models.BuisnessLogic.IRepo
{
    public interface ITagRepo
    {
        // TODO: check if already existis -> existance check for repo interface
        public int AddTag(TagDTO tag);
        public void UpdateTag(TagDTO tag);
        public void DeleteTag(int id);
        public void DeleteTag(int id, int userId);
        public TagDTO GetTag(int id);
        public TagDTO GetTag(int id, int userId);
        public TagDTO? FindTag(int id);
        public TagDTO? FindTag(int id, int userId);
        public List<TagDTO> GetAllTags();
        public List<TagDTO> GetAllTags(int userId);
    }
}
