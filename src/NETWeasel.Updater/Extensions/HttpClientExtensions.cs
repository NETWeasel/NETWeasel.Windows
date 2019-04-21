using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace NETWeasel.Updater.Extensions
{
    // Adapted from https://stackoverflow.com/a/43169927

    internal static class HttpClientExtensions
    {
        internal static async Task DownloadAsync(this HttpClient client, string downloadUrl, string destinationFilePath, IProgress<double> progresser = default)
        {
            using (var response = await client.GetAsync(downloadUrl, HttpCompletionOption.ResponseHeadersRead))
            {
                response.EnsureSuccessStatusCode();

                var totalBytes = response.Content.Headers.ContentLength;

                using (var contentStream = await response.Content.ReadAsStreamAsync())
                {
                    if (totalBytes == null || totalBytes <= 0)
                    {
                        throw new Exception(); // TODO ???
                    }

                    await DownloadToDisk(totalBytes.Value, contentStream, destinationFilePath, progresser);
                }
            }
        }

        private static async Task DownloadToDisk(long totalDownloadSize, Stream contentStream, string destinationFilePath, IProgress<double> progresser)
        {
            var totalBytesRead = 0L;
            var readCount = 0L;
            var buffer = new byte[8192];
            var isMoreToRead = true;

            using (var fileStream = new FileStream(destinationFilePath, FileMode.Create, FileAccess.Write, FileShare.None, 8192, true))
            {
                do
                {
                    var bytesRead = await contentStream.ReadAsync(buffer, 0, buffer.Length);

                    if (bytesRead == 0)
                    {
                        isMoreToRead = false;

                        UpdateProgress();

                        continue;
                    }

                    await fileStream.WriteAsync(buffer, 0, bytesRead);

                    totalBytesRead += bytesRead;

                    readCount += 1;

                    if (readCount % 100 == 0)
                    {
                        UpdateProgress();
                    }

                    void UpdateProgress()
                    {
                        if (progresser == null)
                            return;

                        var progress = Math.Round((double) totalBytesRead / totalDownloadSize * 100, 2);

                        progresser.Report(progress);
                    }
                }
                while (isMoreToRead);
            }
        }
    }
}
