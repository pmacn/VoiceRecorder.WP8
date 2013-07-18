
namespace VoiceRecorder.ViewModels
{
    using Caliburn.Micro;
    using System;
    using System.Windows;
    using VoiceRecorder.Model;

    public class NewTagViewModel : Screen
    {
        private readonly ITagManager _tagManager;

        public NewTagViewModel(ITagManager tagManager)
        {
            _tagManager = tagManager;            
        }

        public string DesiredTagName { get; set; }

        public async void Create()
        {
            if (String.IsNullOrWhiteSpace(DesiredTagName))
                return;

            var result = await _tagManager.Create(DesiredTagName);
            if (result == CreateTagResult.TagWithNameAlreadyExists)
            {
                MessageBox.Show("A tag with this name already exists.");
            }
            else
            {
                TryClose();
            }
        }

        public void Cancel()
        {
            TryClose();
        }
    }
}
