
namespace VoiceRecorder.ViewModels
{
    using Caliburn.Micro;
    using Microsoft.Phone.Tasks;

    public class AboutViewModel : Screen
    {
        #region Fields

        private readonly IEventAggregator _eventAggregator;

        #endregion

        #region Constructors

        public AboutViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
        }

        #endregion

        #region Methods

        public void SendFeedback()
        {
            _eventAggregator.RequestTask<EmailComposeTask>(t =>
            {
                t.To = "support@dotnetexperiments.com";
                t.Subject = "VoiceRecorder feedback";
            });
        }

        #endregion
    }
}
