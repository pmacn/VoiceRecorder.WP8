
namespace VoiceRecorder.ViewModels
{
    using Caliburn.Micro;
    using Microsoft.Phone.Shell;
    using System;
    using System.Linq;

    public sealed class TileViewModel : Screen
    {
        #region Fields

        private const string UriQuery = "StartAction=Record";

        private bool _isTilePinned;

        #endregion

        #region Constructors

        public TileViewModel()
        {
            DisplayName = "tile";
        }

        #endregion

        #region Properties

        public bool IsTilePinned
        {
            get
            {
                return _isTilePinned;
            }
            set
            {
                if (_isTilePinned == value)
                    return;

                _isTilePinned = value;
                NotifyOfPropertyChange(() => IsTilePinned);
            }
        }

        #endregion

        #region Methods

        protected override void OnActivate()
        {
            base.OnActivate();
            IsTilePinned = GetTile() != null;
        }

        private static ShellTile GetTile()
        {
            return ShellTile.ActiveTiles.SingleOrDefault(t =>
            {
                try
                {
                    return t.NavigationUri.OriginalString.Contains(UriQuery);
                }
                catch
                {
                    return false;
                }
            });
        }

        private static IconicTileData GetIconicTileData()
        {
            return new IconicTileData
            {
                IconImage = new Uri("/Assets/Tiles/StartRecordingMediumLarge.png", UriKind.Relative),
                SmallIconImage = new Uri("/Assets/Tiles/StartRecordingSmall.png", UriKind.Relative)
            };
        }

        public void PinTileToStart()
        {
            if (IsTilePinned)
                return;

            var uriString = String.Format(@"/Views/MainPage.xaml?{0}", UriQuery);
            ShellTile.Create(new Uri(uriString, UriKind.Relative), GetIconicTileData(), false);
        }

        #endregion
    }
}