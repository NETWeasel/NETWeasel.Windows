using System;
using System.Threading.Tasks;

namespace NETWeasel.Updater.Providers
{
    public interface IUpdateProvider
    {
        Task<UpdateMeta> CheckForUpdate();
        Task Update(IProgress<double> progress = default);
    }
}