using Planum.Models.BuisnessLogic.Entities;
using Planum.Models.BuisnessLogic.Managers;
using ReactiveUI;
using Serilog;

namespace Planum.ViewModels
{

    public class TagViewModel : ViewModelBase
    {
        protected ITagManager _tagManager;
        protected ITagViewDTOConverter _tagConverter;

        protected TagViewDTO _tag;
        public TagViewDTO Tag
        {
            get => _tag;
            set => this.RaiseAndSetIfChanged(ref _tag, value);
        }

        TagClickHandler _tagClickHandler;

        public TagViewModel(ITagManager tagManager, ITagViewDTOConverter tagConverter, TagViewDTO tag, TagClickHandler tagClickHandler)
        {
            _tagManager = tagManager;
            _tagConverter = tagConverter;
            Tag = tag;
            _tagClickHandler = tagClickHandler;
        }

        public void EditTag()
        {
            Log.Information("Edit tag button clicked");
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

            string category = Tag.Category;
            

            _tagManager.UpdateTag(_tagConverter.ConvertFromViewDTO(Tag));
            _tagClickHandler.Invoke(ErrorPopupOpen, ErrorText);
        }

        public void DeleteTag()
        {
            Log.Information("Delete tag button clicked");
            _tagManager.DeleteTag(Tag.Id);
            _tagClickHandler.Invoke(false, "");
        }
    }
}
