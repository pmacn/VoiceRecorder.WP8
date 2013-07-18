
namespace VoiceRecorder.Data
{
    using Microsoft.Phone.Data.Linq;
    using System.Data.Linq;
    using System.Threading;
    using Model;
    using System;

    public class RecordingsContext : DataContext
    {
        private readonly static Mutex Mutex = new Mutex();

        public RecordingsContext(string connectionString)
            : base(connectionString)
        {
            if (DatabaseExists()) UpdateDatabase();
            else CreateNewDatabase();

            var options = new DataLoadOptions();
            options.LoadWith<Recording>(r => r.RecordingTags);
            LoadOptions = options;
        }

        private void CreateNewDatabase()
        {
            lock (Mutex)
            {
                if (DatabaseExists()) return;

                CreateDatabase();
                var schemaUpdater = this.CreateDatabaseSchemaUpdater();
                schemaUpdater.DatabaseSchemaVersion = 1;
                schemaUpdater.Execute();
            }
        }

        private void UpdateDatabase()
        {
            var schemaUpdater = this.CreateDatabaseSchemaUpdater();
            var version = schemaUpdater.DatabaseSchemaVersion;

            lock (Mutex)
            {
                switch (version)
                {
                    case 0:
                        UpdateToVersionOne(schemaUpdater);
                        break;
                }
            }
        }

        private static void UpdateToVersionOne(DatabaseSchemaUpdater schemaUpdater)
        {
            schemaUpdater.AddTable<RecordingTag>();
            schemaUpdater.AddTable<Tag>();
            schemaUpdater.DatabaseSchemaVersion = 1;
            schemaUpdater.Execute();
        }

        public Table<Recording> Recordings;

        public Table<Tag> Tags;

        public Table<RecordingTag> RecordingTags;
    }
}