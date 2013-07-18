
namespace VoiceRecorder
{
    using System.Linq;
    using System.Windows;

    public static class Extensions
    {
        // TODO : No longer used here but needs to be extracted to a library
        public static bool TryFindResource(this Application app, object key, out object resource)
        {
            var resourceKeyValue = app.Resources.FirstOrDefault(r => r.Key.Equals(key));
            if (resourceKeyValue.Value != null)
            {
                resource = resourceKeyValue.Value;
                return true;
            }

            foreach(var dictionary in app.Resources.MergedDictionaries)
            {
                resourceKeyValue = dictionary.FirstOrDefault(r => r.Key.Equals(key));
                if (resourceKeyValue.Value != null)
                {
                    resource = resourceKeyValue.Value;
                    return true;
                }
            }

            resource = null;
            return false;
        }
    }
}
