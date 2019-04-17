using System;
using System.Xml;
using System.Xml.Serialization;

namespace NETWeasel.Windows
{
    internal static class SpecificationParser
    {
        internal static Specification Deserialize(string path)
        {
            if(string.IsNullOrWhiteSpace(path))
                throw new ArgumentNullException(nameof(path));

            var serializer = new XmlSerializer(typeof(NETWeaselXml));

            using (var reader = XmlReader.Create(path))
            {
                var deserialized = (NETWeaselXml)serializer.Deserialize(reader);

                var spec = deserialized.Specification;

                if (spec == null)
                {
                    throw new InvalidOperationException("Cannot deserialize NETWeasel specification if it does not exist in XML, ensure a spec file has been provided and is valid XML");
                }

                // TODO There needs to be validation or handling of what is given to us from the XML file
                // I.e. What do we do when the upgrade GUID isn't a valid GUID? Throw early.

                return new Specification(
                    SanitizeString(spec.ProductName),
                    SanitizeString(spec.ProductVersion),
                    SanitizeString(spec.ProductManufacturer),
                    SanitizeStringToGuid(spec.ProductUpgradeGuid));
            }
        }

        private static string SanitizeString(string input)
        {
            return input.Trim();
        }

        private static Guid SanitizeStringToGuid(string input)
        {
            var sanitized = SanitizeString(input);

            return Guid.Parse(sanitized);
        }
    }

    [XmlRoot("NETWeasel")]
    public class NETWeaselXml
    {
        [XmlElement("Specification")]
        public SpecificationXml Specification { get; set; }
    }

    public class SpecificationXml
    {
        [XmlElement("ProductName")]
        public string ProductName { get; set; }
        [XmlElement("ProductVersion")]
        public string ProductVersion { get; set; }
        [XmlElement("ProductManufacturer")]
        public string ProductManufacturer { get; set; }
        [XmlElement("ProductUpgradeGuid")]
        public string ProductUpgradeGuid { get; set; }
    }

    internal class Specification
    {
        internal Specification(string productName, string productVersion,
            string productManufacturer, Guid upgradeId)
        {
            ProductName = productName ?? throw new ArgumentNullException(nameof(productName));
            ProductVersion = productVersion ?? throw new ArgumentNullException(nameof(productVersion));
            ProductManufacturer = productManufacturer;
            UpgradeId = upgradeId;
        }

        internal string ProductName { get; }
        internal string ProductVersion { get; }
        internal string ProductManufacturer { get; }
        internal Guid UpgradeId { get; }
    }
}
