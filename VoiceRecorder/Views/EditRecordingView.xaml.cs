using Microsoft.Phone.Controls;
using System.Globalization;

namespace VoiceRecorder.Views
{
    public partial class EditRecordingView : PhoneApplicationPage
    {
        public EditRecordingView()
        {
            InitializeComponent();
            BannerAd.CountryOrRegion = RegionInfo.CurrentRegion.TwoLetterISORegionName;
        }
    }
}