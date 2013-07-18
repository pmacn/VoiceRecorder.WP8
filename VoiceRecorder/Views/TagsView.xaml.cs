
namespace VoiceRecorder.Views
{
    using Microsoft.Phone.Controls;
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Linq;
    using VoiceRecorder.ViewModels;

    public partial class TagsView : UserControl
    {
        public TagsView()
        {
            InitializeComponent();
        }

        private void DeleteClicked(object sender, EventArgs e)
        {
            if (!GetViewModel().SelectedTags.Any())
            {
                MessageBox.Show("You need to select the tags you want to delete", "Delete tags", MessageBoxButton.OK);
                return;
            }

            var messageBox = new CustomMessageBox
            {
                Caption = "Delete tags",
                Message = "Are you sure you want to delete the selected tags?",
                LeftButtonContent = "yes",
                RightButtonContent = "no",
                IsFullScreen = false
            };

            messageBox.Dismissed += DeleteDismissed;
            messageBox.Show();
        }

        private void DeleteDismissed(object sender, DismissedEventArgs e)
        {
            if (e.Result == CustomMessageBoxResult.LeftButton)
            {
                GetViewModel().DeleteSelected();
            }
        }

        private void AddDismissed(object sender, DismissedEventArgs e)
        {
            if (e.Result == CustomMessageBoxResult.LeftButton)
            {
                GetViewModel().Add();
            }
        }

        private TagsViewModel GetViewModel()
        {
            var vm = (TagsViewModel)this.DataContext;
            return vm;
        }
    }
}
