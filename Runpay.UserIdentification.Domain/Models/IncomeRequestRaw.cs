using Newtonsoft.Json.Converters;
using System.Xml.Serialization;

namespace Runpay.UserIdentification.Domain.Models
{
    [XmlRoot(ElementName = "request")]
    public class IncomeRequestRaw
    {
        [XmlElement(ElementName = "signwmid")]
        public string Signwmid { get; set; } = string.Empty;

        [XmlElement(ElementName = "sign")]
        public string Sign { get; set; } = string.Empty;

        [XmlElement(ElementName = "personaldata")]
        public PersonalData PersonalData { get; set; }

        public static IncomeRequestRaw Deserialize(string xml)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(IncomeRequestRaw));

            using (StringReader reader = new StringReader(xml))
            {
                return (IncomeRequestRaw)serializer.Deserialize(reader);
            }
        }
    }
}
