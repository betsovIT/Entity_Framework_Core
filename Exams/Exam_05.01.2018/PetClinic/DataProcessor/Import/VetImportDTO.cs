using System.Xml.Serialization;

namespace PetClinic.DataProcessor.Import
{
    [XmlType("Vet")]
    public class VetImportDTO
    {
        [XmlElement("Name")]
        public string Name { get; set; }

        [XmlElement("Profession")]
        public string Profession { get; set; }

        [XmlElement("Age")]
        public int Age { get; set; }

        [XmlElement("PhoneNumber")]
        public string PhoneNumber { get; set; }
    }
}
