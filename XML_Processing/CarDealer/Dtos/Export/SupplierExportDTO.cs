﻿using System.Xml.Serialization;

namespace CarDealer.Dtos.Export
{
    [XmlType("suplier")]
    public class SupplierExportDTO
    {
        [XmlAttribute("id")]
        public int Id { get; set; }

        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("travelled-distance")]
        public int PartsCount { get; set; }
    }
}
