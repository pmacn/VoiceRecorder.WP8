
namespace VoiceRecorder.Views
{
    using Microsoft.Phone.Controls;
    using System.Globalization;

    public partial class MainPage : PhoneApplicationPage
    {
        public MainPage()
        {
            InitializeComponent();
            BannerAd.CountryOrRegion = RegionInfo.CurrentRegion.TwoLetterISORegionName;
        }
    }
}