using System.Collections.Generic;

using Planum.Models.BuisnessLogic.Entities;
using Planum.Models.BuisnessLogic.IRepo;
using Planum.Models.DTO;
using Serilog;

namespace Planum.Models.BuisnessLogic.Managers
{
    public class TagManager : ITagManager
    {
        protected ITagRepo _tagRepo;
        protected ITaskManager _taskManager;
        protected IUserManager _userManager;
        protected ITagConverter _tagConverter;

        public TagManager(ITagRepo tagRepo, ITaskManager taskManager, ITagConverter tagConverter, IUserManager userManager)
        {
            _tagRepo = tagRepo;
            _taskManager = taskManager;
            _tagConverter = tagConverter;
            _userManager = userManager;
        }

        public int CreateTag(string category, string name, string description)
        {
            Log.Debug("Create tag");
            if (_userManager.CurrentUser == null)
                throw new CurrentUserIsNullException("Can't create tag while current user is null");
            Tag newTag = new Tag(-1, _userManager.CurrentUser.Id, category, name, description);
            TagDTO tagDTO = _tagConverter.ConvertToDTO(newTag);
            return _tagRepo.AddTag(tagDTO);
        }

        public void UpdateTag(Tag tag)
        {
            Log.Debug($"Update tag with id={tag.Id}");
            if (_userManager.CurrentUser == null)
                throw new CurrentUserIsNullException("Can't update tag while current user is null");
            if (_userManager.CurrentUser.Id != tag.UserId)
                throw new TagDoesNotHaveCorrectUser();
            if (FindTag(tag.Id) == null) return;
            TagDTO tagDTO = _tagConverter.ConvertToDTO(tag);
            _tagRepo.UpdateTag(tagDTO);
        }

        public void DeleteTag(int tagId)
        {
            Log.Debug($"Delete tag with id={tagId}");
            if (_userManager.CurrentUser == null)
                throw new CurrentUserIsNullException("Can't delete tag while current user is null");
            if (FindTag(tagId) != null)
            {
                _taskManager.RemoveTagFromAll(tagId);
                _tagRepo.DeleteTag(tagId);
            }
        }

        public void DeleteConnectedToUser(int userId)
        {
            Log.Debug($"Delete tag with id={userId}");
            List<Tag> tags = GetAllTags();
            foreach (Tag tag in tags)
            {
                if (tag.UserId == userId)
                    DeleteTag(tag.Id);
            }
        }

        public Tag GetTag(int tagId)
        {
            Log.Debug($"Get tag with id={tagId}");
            if (_userManager.CurrentUser == null)
                throw new CurrentUserIsNullException("Can't get tag while current user is null");
            TagDTO tagDTO = _tagRepo.GetTag(tagId);
            if (tagDTO.UserId != _userManager.CurrentUser.Id)
                throw new TagDoesNotExistException("Tag with such id and current user id does not exist");
            Tag tag = _tagConverter.ConvertFromDTO(tagDTO);
            return tag;
        }

        public Tag? FindTag(int tagId)
        {
            Log.Debug($"Find tag with id={tagId}");
            if (_userManager.CurrentUser == null)
                throw new CurrentUserIsNullException("Can't find tag while current user is null");
            TagDTO? tagDTO = _tagRepo.FindTag(tagId);
            if (tagDTO == null)
                return null;
            if (tagDTO.UserId != _userManager.CurrentUser.Id)
                return null;
            Tag tag = _tagConverter.ConvertFromDTO(tagDTO);
            return tag;
        }

        public List<Tag> GetAllTags()
        {
            Log.Debug($"Get all tags");
            if (_userManager.CurrentUser == null)
                throw new CurrentUserIsNullException("Can't get all tags while current user is null");
            List<TagDTO> tagDTOs = _tagRepo.GetAllTags();
            List<Tag> tagList = new List<Tag>();
            foreach (var tagDTO in tagDTOs)
            {
                if (tagDTO.UserId == _userManager.CurrentUser.Id)
                    tagList.Add(_tagConverter.ConvertFromDTO(tagDTO));
            }
            return tagList;
        }
    }
}
