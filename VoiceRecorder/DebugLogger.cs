namespace VoiceRecorder
{
    using Caliburn.Micro;
    using System;
    using System.Diagnostics;

    public class DebugLogger : ILog
    {
        public void Error(Exception exception)
        {
            Debug.WriteLine("Error: {0}", exception.Message);
        }

        public void Info(string format, params object[] args)
        {
            Debug.WriteLine(format, args);
        }

        public void Warn(string format, params object[] args)
        {
            Debug.WriteLine(format, args);
        }
    }
}