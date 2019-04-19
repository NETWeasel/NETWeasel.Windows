using System;
using System.IO;

namespace NETWeasel.Windows
{
    internal static class FileHelper
    {
        internal static void CopyDirectory(string sourceDirectory, string targetDirectory)
        {
            var sourceDirInfo = new DirectoryInfo(sourceDirectory);
            var targetDirInfo = new DirectoryInfo(targetDirectory);

            CopyAll(sourceDirInfo, targetDirInfo);
        }

        private static void CopyAll(DirectoryInfo source, DirectoryInfo target)
        {
            Directory.CreateDirectory(target.FullName);

            foreach (var fi in source.GetFiles())
            {
                Console.WriteLine(@"Copying {0}\{1}", target.FullName, fi.Name);

                fi.CopyTo(Path.Combine(target.FullName, fi.Name), true);
            }

            foreach (var subDir in source.GetDirectories())
            {
                var nextTargetSubDir = target.CreateSubdirectory(subDir.Name);
                CopyAll(subDir, nextTargetSubDir);
            }
        }

        // Output will vary based on the contents of the source directory.
    }
}
