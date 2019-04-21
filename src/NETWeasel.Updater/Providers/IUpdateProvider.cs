using System;
using System.Threading.Tasks;

namespace NETWeasel.Updater.Providers
{
    /// <summary>
    /// Interface for all providers to implement, providers
    /// feed the Updater
    /// </summary>
    public interface IUpdateProvider
    {
        /// <summary>
        /// Checks if there is an update waiting to be applied
        /// to the running application's version.
        /// </summary>
        /// <returns>Update meta</returns>
        Task<UpdateMeta> CheckForUpdate();

        /// <summary>
        /// Downloads the update from the provider's
        /// implemented source
        /// </summary>
        /// <param name="progress">Progress reporter</param>
        /// <returns>Filepath of the downloaded update</returns>
        Task<string> DownloadUpdate(IProgress<double> progress = default);
    }
}