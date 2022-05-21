using Planum.Models.BuisnessLogic.Entities;
using System.Collections.Generic;

namespace Planum.Models.BuisnessLogic.Managers
{
    public interface ITagManager
    {
        int CreateTag(int user_id, int category, string? name, string? description);
        void DeleteConnectedToUser(int userId);
        void DeleteTag(int tagId);
        void DeleteTag(int tagId, int userId);
        Tag? FindTag(int tagId);
        Tag? FindTag(int tagId, int userId);
        List<Tag> GetAllTags();
        List<Tag> GetAllTags(int userId);
        Tag GetTag(int tagId);
        Tag GetTag(int tagId, int userId);
        void UpdateTag(int id, string name, int category, string description);
        void UpdateTag(int id, int userId, string name, int category, string description);
    }
}