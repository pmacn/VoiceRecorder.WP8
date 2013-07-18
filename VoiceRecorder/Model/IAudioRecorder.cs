using System.ComponentModel;
using System.Threading.Tasks;

namespace VoiceRecorder.Model
{
    public interface IAudioRecorder : INotifyPropertyChanged
    {
        Task StartRecording(Recording recording);
        Task StopRecording();
    }
}