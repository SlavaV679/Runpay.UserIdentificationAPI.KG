using System.Xml.Serialization;

namespace Runpay.UserIdentification.Domain.Models
{
    public class PersonalData
    {
        [XmlElement(ElementName = "last_name")]
        public string LastName { get; set; }

        [XmlElement(ElementName = "first_name")]
        public string FirstName { get; set; }

        [XmlElement(ElementName = "middle_name")]
        public string MiddleName { get; set; }

        [XmlElement("date_of_birth")]
        public string DateOfBirth { get; set; }

        [XmlElement(ElementName = "passport_number")]
        public string PassportNumber { get; set; }

        [XmlElement(ElementName = "issuing_country")]
        public string IssuingCountry { get; set; }

        [XmlElement(ElementName = "phone_number")]
        public string Phone { get; set; }
    }
}
