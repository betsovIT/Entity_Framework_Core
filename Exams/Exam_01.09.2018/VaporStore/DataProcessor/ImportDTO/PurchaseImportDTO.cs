﻿using System.Xml.Serialization;

namespace VaporStore.DataProcessor.ImportDTO
{
    [XmlType("Purchase")]
    public class PurchaseImportDTO
    {
        [XmlAttribute("title")]
        public string Title { get; set; }

        [XmlElement("Type")]
        public string Type { get; set; }

        [XmlElement("Key")]
        public string Key { get; set; }

        [XmlElement("Card")]
        public string Card { get; set; }

        [XmlElement("Date")]
        public string Date { get; set; }
    }
}
