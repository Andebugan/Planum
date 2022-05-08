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
    internal class TagManager
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
            TagParamsDTO tagParamsDTO = new TagParamsDTO();

            tagParamsDTO.id = tag.Id;
            tagParamsDTO.userId = tag.UserId;
            tagParamsDTO.name = tag.Name;
            tagParamsDTO.description = tag.Description;
            tagParamsDTO.category = tag.Category;

            return new TagDTO(tagParamsDTO);
        }

        protected Tag ConvertFromDTO(TagDTO tagDTO)
        {
            TagParams tagParams = new TagParams();

            tagParams.id = tagDTO.Id;
            tagParams.userId = tagDTO.UserId;
            tagParams.category = tagDTO.Category;
            tagParams.name = tagDTO.Name;
            tagParams.description = tagDTO.Description;

            return new Tag(tagParams);
        }

        public void UpdateTag(ref Tag tag, TagParams tagParams)
        {
            tag.Update(tagParams);
            TagDTO tagDTO = ConvertToDTO(tag);
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

        public void CreateTag(TagParams tagParams)
        {
            Tag tag = new Tag(tagParams);
            TagDTO tagDTO = ConvertToDTO(tag);
            tagRepo.Add(tagDTO);
        }

        public Tag? GetTag(int tagId)
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
