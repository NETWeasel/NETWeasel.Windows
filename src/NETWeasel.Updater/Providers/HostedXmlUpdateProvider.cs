using System;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using NETWeasel.Common;

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

            File.WriteAllText(updateFilePath, response);

            var deserialized = SpecificationParser.Deserialize(updateFilePath);

            File.Delete(updateFilePath);

            var currentVersion = Assembly.GetEntryAssembly().GetName().Version.ToString();

            var currentParsedVersion = new Version(currentVersion);
            var remoteParsedVersion = new Version(deserialized.ProductVersion);

            var isUpdateAvailable = remoteParsedVersion > currentParsedVersion;

            return new UpdateMeta(isUpdateAvailable, deserialized.ProductVersion);
        }

        public async Task Update(IProgress<int> progress = default)
        {
            var updateCheck = await CheckForUpdate();

            if (updateCheck.IsUpdateAvailable)
                return;

            var expectedRemoteFileName = updateCheck.Version + ".tar.lz";

            var url = BuildUrl(expectedRemoteFileName);

            var response = await _httpClient.GetAsync(url);

            using (var file = new FileStream(expectedRemoteFileName, FileMode.Create))
            {
                await response.Content.CopyToAsync(file);
            }

            progress?.Report(100);
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