using System;
using System.Collections.Generic;

using Planum.Models.BuisnessLayer.Entities;
using Planum.Models.BuisnessLayer.Managers.TagManagement;
using Planum.Models.BuisnessLayer.RepoInterfaces;
using Planum.Models.DTO.ModelData;

namespace Planum.Models.BuisnessLayer.Managers
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
            Tag? tag = GetTag(id); // ! change to find tag
            Tag newTag = new Tag(tag.Id, tag.UserId, category, name, description);
            TagDTO tagDTO = _tagConverter.ConvertToDTO(newTag);
            _tagRepo.UpdateTask(tagDTO);
        }

        public void DeleteTag(int tagId)
        {
            _taskManager.RemoveTagFromAll(tagId);
            _tagRepo.Delete(tagId);
        }

        public void DeleteConnectedToUser(int userId)
        {
            List<Tag> tags = GetAll();
            foreach (Tag tag in tags)
            {
                if (tag.UserId == userId)
                    DeleteTag(tag.Id);
            }
        }

        public void CreateTag(int user_id, int category, string? name, string? description)
        {
            int id = 0;
            List<Tag> tags = GetAll();

            Tag new_tag = new Tag(id, user_id, category, name, description);
            TagDTO tagDTO = _tagConverter.ConvertToDTO(new_tag);
            _tagRepo.Add(tagDTO);
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
            Tag tag = _tagConverter.ConvertFromDTO(tagDTO);
            return tag;
        }

        public List<Tag> GetAll()
        {
            List<TagDTO> tagDTOs = _tagRepo.GetAll();
            List<Tag> tagList = new List<Tag>();
            foreach (var tagDTO in tagDTOs)
            {
                tagList.Add(_tagConverter.ConvertFromDTO(tagDTO));
            }
            return tagList;
        }
    }
}
