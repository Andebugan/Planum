using System.Collections.Generic;

using Planum.Models.BuisnessLogic.Entities;
using Planum.Models.BuisnessLogic.IRepo;
using Planum.Models.DTO;

namespace Planum.Models.BuisnessLogic.Managers
{
    public class TagManager : ITagManager
    {
        protected ITagRepo _tagRepo;
        protected ITaskManager _taskManager;
        protected ITagConverter _tagConverter;

        public TagManager(ITagRepo tagRepo, ITaskManager taskManager, ITagConverter tagConverter)
        {
            _tagRepo = tagRepo;
            _taskManager = taskManager;
            _tagConverter = tagConverter;
        }

        public void UpdateTag(int id, string name, int category, string description)
        {
            Tag? tag = FindTag(id);
            if (tag == null) return;
            Tag newTag = new Tag(tag.Id, tag.UserId, category, name, description);
            TagDTO tagDTO = _tagConverter.ConvertToDTO(newTag);
            _tagRepo.UpdateTag(tagDTO);
        }

        public void DeleteTag(int tagId)
        {
            if (FindTag(tagId) != null)
            {
                _taskManager.RemoveTagFromAll(tagId);
                _tagRepo.DeleteTag(tagId);
            }
        }

        public void DeleteConnectedToUser(int userId)
        {
            List<Tag> tags = GetAllTags();
            foreach (Tag tag in tags)
            {
                if (tag.UserId == userId)
                    DeleteTag(tag.Id);
            }
        }

        public int CreateTag(int user_id, int category, string? name, string? description)
        {
            Tag new_tag = new Tag(0, user_id, category, name, description);
            TagDTO tagDTO = _tagConverter.ConvertToDTO(new_tag);
            return _tagRepo.AddTag(tagDTO);
        }

        public Tag GetTag(int tagId)
        {
            TagDTO tagDTO = _tagRepo.GetTag(tagId);
            Tag tag = _tagConverter.ConvertFromDTO(tagDTO);
            return tag;
        }

        public Tag? FindTag(int tagId)
        {
            TagDTO? tagDTO = _tagRepo.FindTag(tagId);
            if (tagDTO == null)
                return null;
            Tag tag = _tagConverter.ConvertFromDTO(tagDTO);
            return tag;
        }

        public List<Tag> GetAllTags()
        {
            List<TagDTO> tagDTOs = _tagRepo.GetAllTags();
            List<Tag> tagList = new List<Tag>();
            foreach (var tagDTO in tagDTOs)
            {
                tagList.Add(_tagConverter.ConvertFromDTO(tagDTO));
            }
            return tagList;
        }
    }
}
