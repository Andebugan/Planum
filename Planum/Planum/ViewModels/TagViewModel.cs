using Planum.Models.BuisnessLogic.Entities;
using Planum.Models.BuisnessLogic.Managers;
using ReactiveUI;

namespace Planum.ViewModels
{

    public class TagViewModel : ViewModelBase
    {
        protected ITagManager _tagManager;

        protected TagViewDTO _tag;
        public TagViewDTO Tag
        {
            get => _tag;
            set => this.RaiseAndSetIfChanged(ref _tag, value);
        }

        TagClickHandler _tagClickHandler;

        public TagViewModel(ITagManager tagManager, TagViewDTO tag, TagClickHandler tagClickHandler)
        {
            _tagManager = tagManager;
            Tag = tag;
            _tagClickHandler = tagClickHandler;
        }

        public void EditTag()
        {
            bool ErrorPopupOpen = false;
            string ErrorText = "";

            string? name = Tag.Name;
            if (string.IsNullOrEmpty(name))
            {
                ErrorPopupOpen = true;
                ErrorText = "Name can't be null or empty";
                _tagClickHandler.Invoke(ErrorPopupOpen, ErrorText);
                return;
            }

            string description = Tag.Description;

            int category = Tag.Category;

            _tagManager.UpdateTag(Tag.Id, name, category, description);
            _tagClickHandler.Invoke(ErrorPopupOpen, ErrorText);
        }

        public void DeleteTag()
        {
            _tagManager.DeleteTag(Tag.Id);
            _tagClickHandler.Invoke(false, "");
        }
    }
}
