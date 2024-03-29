﻿using System.Xml.Serialization;

namespace PetClinic.DataProcessor.Export
{
    [XmlType("Procedure")]
    public class ProcedureExportDTO
    {
        [XmlElement("Passport")]
        public string Passport { get; set; }

        [XmlElement("OwnerNumber")]
        public string OwnerNumber { get; set; }

        [XmlElement("DateTime")]
        public string DateTime { get; set; }

        [XmlArray("AnimalAids")]
        public AnimalAidExportDTO[] AnimalAids { get; set; }

        [XmlElement("TotalPrice")]
        public decimal TotalPrice { get; set; }
    }

    [XmlType("AnimalAid")]
    public class AnimalAidExportDTO
    {
        [XmlElement("Name")]
        public string Name { get; set; }

        [XmlElement("Price")]
        public decimal Price { get; set; }
    }
}
