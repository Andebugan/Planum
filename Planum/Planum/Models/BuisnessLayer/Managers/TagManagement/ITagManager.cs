using Planum.Models.BuisnessLayer.Entities;
using System.Collections.Generic;

namespace Planum.Models.BuisnessLayer.Managers
{
    public interface ITagManager
    {
        void CreateTag(int user_id, int category, string name, string description);
        void DeleteConnectedToUser(int userId);
        void DeleteTag(int tagId);
        List<Tag> GetAll();
        Tag GetTag(int tagId);
        void UpdateTag(int id, string name, int category, string description);
    }
}