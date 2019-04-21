namespace NETWeasel.Updater
{
    /// <summary>
    /// Contains information about the latest update
    /// </summary>
    public class UpdateMeta
    {
        internal UpdateMeta(bool isUpdateAvailable, string version)
        {
            IsUpdateAvailable = isUpdateAvailable;
            Version = version;
        }

        /// <summary>
        /// If true, there is an update to be applied,
        /// if false, the running application is on the
        /// latest version
        /// </summary>
        public bool IsUpdateAvailable { get; }

        /// <summary>
        /// The latest version available/pulled from
        /// the update check
        /// </summary>
        public string Version { get; }
    }
}