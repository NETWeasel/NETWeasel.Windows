using System;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using NETWeasel.Common;
using NETWeasel.Updater.Extensions;

namespace NETWeasel.Updater.Providers
{
    public class HostedXmlUpdateProvider : IUpdateProvider
    {
        private readonly HttpClient _httpClient;
        private readonly string _updateUrl;

        public HostedXmlUpdateProvider(string updateUrl) : this(updateUrl, new HttpClient())
        {

        }

        public HostedXmlUpdateProvider(string updateUrl, HttpClient httpClient)
        {
            _updateUrl = updateUrl;
            _httpClient = httpClient;
        }

        public async Task<UpdateMeta> CheckForUpdate()
        {
            var url = BuildUrl("spec.xml");

            var response = await _httpClient.GetStringAsync(url);

            var updateFilePath = Guid.NewGuid() + ".xml";

            // To be able to deserialize XML, we need a "temporary" file
            // so save the response to disk, deserialize and then
            // delete the file from the user's machine
            File.WriteAllText(updateFilePath, response);

            var deserialized = SpecificationParser.Deserialize(updateFilePath);

            File.Delete(updateFilePath);

            var currentVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();

            var currentParsedVersion = new Version(currentVersion);
            var remoteParsedVersion = new Version(deserialized.ProductVersion);

            var isUpdateAvailable = remoteParsedVersion > currentParsedVersion;

            return new UpdateMeta(isUpdateAvailable, deserialized.ProductVersion);
        }

        public async Task Update(IProgress<double> progress = default)
        {
            var updateCheck = await CheckForUpdate();

            if (!updateCheck.IsUpdateAvailable)
                return;

            var expectedRemoteFileName = updateCheck.Version + ".tar.lz";

            var url = BuildUrl(expectedRemoteFileName);

            await _httpClient.DownloadAsync(url, expectedRemoteFileName, progress);
        }

        private string BuildUrl(string path)
        {
            var baseUri = _updateUrl;

            if (!baseUri.EndsWith("/"))
            {
                baseUri += "/";
            }

            return baseUri + path;
        }
    }
}