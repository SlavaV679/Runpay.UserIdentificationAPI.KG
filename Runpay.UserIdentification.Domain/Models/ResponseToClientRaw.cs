using System.Xml;
using System.Xml.Serialization;

namespace Runpay.UserIdentification.Domain.Models
{

    [XmlRoot(ElementName = "response")]
    public class ResponseToClientRaw
    {
        [XmlElement(ElementName = "retval")]
        public string ReturnedValue { get; set; } = string.Empty;

        [XmlElement(ElementName = "retdesc")]
        public string ReturnedDescription { get; set; } = string.Empty;


        public static string Serialize(ResponseToClientRaw request)
        {
            var settings = new XmlWriterSettings
            {
                Indent = true,
                OmitXmlDeclaration = true
            };

            var namespaces = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });

            using (var stream = new StringWriter())
            using (var textWriter = XmlWriter.Create(stream, settings))
            {
                var serializer = new XmlSerializer(typeof(ResponseToClientRaw));
                serializer.Serialize(textWriter, request, namespaces);
                return stream.ToString();
            }
        }

        public static ResponseToClientRaw Deserialize(string xml)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(ResponseToClientRaw));

            using (StringReader reader = new StringReader(xml))
            {
                return (ResponseToClientRaw)serializer.Deserialize(reader);
            }
        }
    }
}
