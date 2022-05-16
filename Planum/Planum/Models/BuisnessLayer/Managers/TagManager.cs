using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Planum.Models.BuisnessLayer.Entities;
using Planum.Models.BuisnessLayer.Interfaces;
using Planum.Models.DTO.ModelData;

namespace Planum.Models.BuisnessLayer.Managers
{
    public class TagManager
    {
        protected ITagRepo tagRepo;
        protected TaskManager taskManager;

        public TagManager(ref ITagRepo tagRepo, ref TaskManager taskManager)
        {
            if (tagRepo == null)
                throw new ArgumentNullException(nameof(tagRepo));
            if (taskManager == null)
                throw new ArgumentNullException(nameof(taskManager));
            this.tagRepo = tagRepo;
            this.taskManager = taskManager;
        }

        protected TagDTO ConvertToDTO(Tag tag)
        {
            TagDTO tagDTO = new TagDTO(tag.Id, tag.UserId, tag.Category, tag.Name, tag.Description);
            return tagDTO;
        }

        protected Tag ConvertFromDTO(TagDTO tagDTO)
        {
            Tag tag = new Tag(tagDTO.Id, tagDTO.UserId, tagDTO.Category, tagDTO.Name, tagDTO.Description);
            return tag;
        }

        public void Update(int id, int category = -1, string? name = null, string? description = null)
        {
            Tag tag = GetTag(id);
            if (category == -1)
                category = tag.Category;
            if (name == null)
                name = tag.Name;
            if (description == null)
                description = tag.Description;
            Tag newTag = new Tag(tag.Id, tag.UserId, category, name, description);
            TagDTO tagDTO = ConvertToDTO(newTag);
            tagRepo.Update(tagDTO);
        }

        public void DeleteTag(int tagId)
        {
            taskManager.RemoveTagFromAll(tagId);
            tagRepo.Delete(tagId);
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

            foreach (var tag in tags)
            {
                if (id == tag.Id)
                    id += 1;
            }

            Tag new_tag = new Tag(id, user_id, category, name, description);
            TagDTO tagDTO = ConvertToDTO(new_tag);
            tagRepo.Add(tagDTO);
        }

        public Tag GetTag(int tagId)
        {
            TagDTO tagDTO = tagRepo.Get(tagId);
            Tag tag = ConvertFromDTO(tagDTO);
            return tag;
        }

        public List<Tag> GetAll()
        {
            List<TagDTO> tagDTOs = tagRepo.GetAll();
            List<Tag> tagList = new List<Tag>();
            foreach (var tagDTO in tagDTOs)
            {
                tagList.Add(ConvertFromDTO(tagDTO));
            }
            return tagList;
        }
    }
}
