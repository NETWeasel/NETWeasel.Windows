using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using NETWeasel.Updater.Providers;
using SharpCompress.Common;
using SharpCompress.Readers;

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

        public async Task Update(bool restartOnUpdate = true, IProgress<double> progress = default)
        {
            var updateFile = await _updateProvider.DownloadUpdate(progress);

            if (string.IsNullOrWhiteSpace(updateFile))
            {
                // TODO There should probably be an error here?
                return;
            }

            // Get a temporary path to extract the update to, we'll
            // be using this to start the updater and copy the files
            // over
            var updatePath = Path.GetTempPath();

            using (var stream = File.OpenRead(updateFile))
            {
                var readerOptions = new ReaderOptions
                {
                    LeaveStreamOpen = false,
                    ArchiveEncoding = { Default = Encoding.GetEncoding(866) },
                };

                using (var reader = ReaderFactory.Open(stream, readerOptions))
                {
                    while (reader.MoveToNextEntry())
                    {
                        if (!reader.Entry.IsDirectory)
                        {
                            reader.WriteEntryToDirectory(updatePath, new ExtractionOptions()
                            {
                                ExtractFullPath = true,
                                Overwrite = true
                            });
                        }
                    }
                }
            }

            var installLocation = Assembly.GetEntryAssembly().Location;

            var args = new Dictionary<string, string>
            {
                ["mode"] = "update",
                ["path"] = installLocation,
                ["cleanupPath"] = updatePath,
            };

            if (restartOnUpdate)
            {
                args.Add("restartApp", string.Empty);
            }

            UpdateHelper.StartUpdater(updatePath, args);
        }
    }
}