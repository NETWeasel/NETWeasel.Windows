using System.IO;

namespace NETWeasel.Common
{
    public static class FileHelper
    {
        public static void CopyDirectory(string sourceDirectory, string targetDirectory)
        {
            var sourceDirInfo = new DirectoryInfo(sourceDirectory);
            var targetDirInfo = new DirectoryInfo(targetDirectory);

            CopyAll(sourceDirInfo, targetDirInfo);
        }

        private static void CopyAll(DirectoryInfo source, DirectoryInfo target)
        {
            Directory.CreateDirectory(target.FullName);

            foreach (var file in source.GetFiles())
            {
                file.CopyTo(Path.Combine(target.FullName, file.Name), true);
            }

            foreach (var subDir in source.GetDirectories())
            {
                var nextTargetSubDir = target.CreateSubdirectory(subDir.Name);
                CopyAll(subDir, nextTargetSubDir);
            }
        }
    }
}
