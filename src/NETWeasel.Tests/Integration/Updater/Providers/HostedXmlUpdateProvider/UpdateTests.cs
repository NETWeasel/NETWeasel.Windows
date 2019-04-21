using System;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace NETWeasel.Tests.Integration.Updater.Providers.HostedXmlUpdateProvider
{
    public class UpdateTests
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public UpdateTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public async Task Update()
        {
            var progresser = new Progress<double>(pro => _testOutputHelper.WriteLine($"Downloaded {pro}"));

            var updater = new NETWeasel.Updater.Updater(
                new NETWeasel.Updater.Providers.HostedXmlUpdateProvider("http://localhost"));

            await updater.Update(progress:progresser);
        }
    }
}
