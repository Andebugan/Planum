using Planum.Models.BuisnessLogic.Entities;
using System.Collections.Generic;

namespace Planum.Models.BuisnessLogic.Managers
{
    public interface ITagManager
    {
        int CreateTag(int category, string name, string description);
        void DeleteConnectedToUser(int userId);
        void DeleteTag(int tagId);
        Tag? FindTag(int tagId);
        List<Tag> GetAllTags();
        Tag GetTag(int tagId);
        void UpdateTag(int id, string name, int category, string description);
    }
}