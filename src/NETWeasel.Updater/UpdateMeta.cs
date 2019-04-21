namespace NETWeasel.Updater
{
    public class UpdateMeta
    {
        internal UpdateMeta(bool isUpdateAvailable, string version)
        {
            IsUpdateAvailable = isUpdateAvailable;
            Version = version;
        }

        public bool IsUpdateAvailable { get; }
        public string Version { get; }
    }
}