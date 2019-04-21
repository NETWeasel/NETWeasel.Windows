using System.Threading.Tasks;
using NETWeasel.Updater.Providers;

namespace NETWeasel.Updater
{
    public class Updater
    {
        private readonly IUpdateProvider _updateProvider;

        public Updater(IUpdateProvider updateProvider)
        {
            _updateProvider = updateProvider;
        }

        public Task<UpdateMeta> CheckForUpdate() => _updateProvider.CheckForUpdate();

        public Task Update() => _updateProvider.Update();
    }
}