using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace NETWeasel.Windows
{
    internal class Program
    {
        private const string ASCII = @"
  __   __  ______ ______ __     __  ______  ______  ______  ______  __        
/\ ""-.\ \/\  ___/\__  _/\ \  _ \ \/\  ___\/\  __ \/\  ___\/\  ___\/\ \       
\ \ \-.  \ \  __\/_/\ \\ \ \/ "".\ \ \  __\\ \  __ \ \___  \ \  __\\ \ \____  
 \ \_\\""\_\ \_____\\ \_\\ \__/"".~\_\ \_____\ \_\ \_\/\_____\ \_____\ \_____\ 
  \/_/ \/_/\/_____/ \/_/ \/_/   \/_/\/_____/\/_/\/_/\/_____/\/_____/\/_____/ ";

        private const string STRAPLINE = "NETWeasel Windows CLI - Version {0}";

        private const string VERSION = "0.1";

        private static readonly string Introduction = $"{ASCII}\n                   {string.Format(STRAPLINE, VERSION)}";

        private static int Main(string[] args)
        {
            var parsedArguments = ArgumentParser.Parse(args);

            if (!parsedArguments.Any())
            {
                Console.WriteLine("No arguments were supplied");
                return 1;
            }

            if (!parsedArguments.Any(x => x.Command == ArgumentCommand.NoLogo))
            {
                Console.WriteLine(Introduction);

                Console.WriteLine();
                Console.WriteLine();
            }

            if (!parsedArguments.Any(x => x.Command == ArgumentCommand.Path))
            {
                Console.WriteLine("No valid path for packaging was specified");
                return 1;
            }

            if (!parsedArguments.Any(x => x.Command == ArgumentCommand.Output))
            {
                Console.WriteLine("No valid output path for packaging was specified");
                return 1;
            }

            var path = parsedArguments.Single(x => x.Command == ArgumentCommand.Path);

            if (!Directory.Exists(path.Parameter))
            {
                Console.WriteLine("Path not found");
                return 2;
            }

            var outputPath = parsedArguments.Single(x => x.Command == ArgumentCommand.Output);

            if (!Directory.Exists(outputPath.Parameter))
            {
                Console.WriteLine("Path not found");
                return 2;
            }

            try
            {
                Package(path.Parameter, outputPath.Parameter);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return 0;
        }

        private static void Package(string artifactsPath, string desiredOutputDir)
        {
            // Get location for NETWeasel
            var weaselDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var outputDir = Path.Combine(desiredOutputDir, "RELEASE");

            if (!Directory.Exists(outputDir))
            {
                Directory.CreateDirectory(outputDir);
            }

            // Run heat do we get a file for all files that we need
            // to include in the WIX installer
            var sourceFilesWxsPath = RunHeat(weaselDir, artifactsPath, outputDir);

            // Compose the WXS for Wix to pass to candle
            var productWxsPath = ComposeProductWxs(weaselDir, outputDir);

            RunCandle(weaselDir, productWxsPath, sourceFilesWxsPath, outputDir);

            CleanUpFiles(productWxsPath, sourceFilesWxsPath);
        }

        private static string RunHeat(string weaselDir, string artifactsPath, string outputDir)
        {
            // Traverse to the tools folder
            var heatPath = Path.Combine(weaselDir, "tools", "heat.exe");

            var generatedFilePath = Path.Combine(outputDir, "SourceFiles.wxs");

            // Start heat, and generate the wxs for WIX
            // ARGS:
            // dir Harvests an entire directory
            // gg Generates guids for components
            // sfrag Suppress generation of fragments for directories and components.
            // template Template for the generated output
            // out Target directory/file
            // nologo Prevents printing heat logo/info to console
            Process.Start(heatPath, $"dir \"{artifactsPath}\" -gg -sfrag -template fragment -out \"{generatedFilePath}\" -nologo");

            return generatedFilePath;
        }

        private static string ComposeProductWxs(string weaselDir, string outputDir)
        {
            const string TEMPLATE_FILE = "Product.wxs";

            var specPath = Path.Combine(outputDir, "spec.xml");

            if (!File.Exists(specPath))
            {
                throw new InvalidOperationException("Cannot compose WXS with missing NETWeasel Spec file, ensure spec.xml exists");
            }

            var specification = SpecificationParser.Deserialize(specPath);

            var wxsTemplatePath = Path.Combine(weaselDir, "template", TEMPLATE_FILE);

            var composedWxs = new WxsComposer(wxsTemplatePath)
                .Replace("#{NAME}#", specification.ProductName)
                .Replace("#{VERSION}#", specification.ProductVersion)
                .Replace("#{MANUFACTURER}#", specification.ProductManufacturer)
                .Replace("#{UPGRADE_GUID}#", "{" + specification.UpgradeId + "}")
                .Compose();

            var generatedFilePath = Path.Combine(outputDir, "Product.wxs");

            File.WriteAllText(generatedFilePath, composedWxs);

            return generatedFilePath;
        }

        private static void RunCandle(string weaselDir, 
            string productWxsPath,
            string sourceFilesWxsPath,
            string outputDir)
        {
            var candlePath = Path.Combine(weaselDir, "tools", "candle.exe");

            var productFileOutputPath = Path.Combine(outputDir, "Product.wixobj");
            Process.Start(candlePath, $"-out \"{productFileOutputPath}\" \"{productWxsPath}\"");

            var sourceFilesFileOutputPath = Path.Combine(outputDir, "SourceFiles.wixobj");
            Process.Start(candlePath, $"-out \"{sourceFilesFileOutputPath}\" \"{sourceFilesWxsPath}\"");
        }

        private static void CleanUpFiles(params string[] paths)
        {
            foreach (var path in paths)
            {
                if(string.IsNullOrWhiteSpace(path) || !File.Exists(path))
                    continue;
                
                File.Delete(path);
            }
        }
    }
}
