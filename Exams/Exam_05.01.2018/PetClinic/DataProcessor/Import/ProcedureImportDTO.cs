using System.Xml.Serialization;

namespace PetClinic.DataProcessor.Import
{
    [XmlType("Procedure")]
    public class ProcedureImportDTO
    {
        [XmlElement("Vet")]
        public string Vet { get; set; }

        [XmlElement("Animal")]
        public string Animal { get; set; }

        [XmlElement("DateTime")]
        public string DateTime { get; set; }

        [XmlArray("AnimalAids")]
        public AidImportDTO[] AnimalAids { get; set; }
    }

    [XmlType("AnimalAid")]
    public class AidImportDTO
    {
        [XmlElement("Name")]
        public string Name { get; set; }
    }
}
