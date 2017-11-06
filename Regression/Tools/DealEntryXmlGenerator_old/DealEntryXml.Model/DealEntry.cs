using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace DealEntryXml.Model
{
    [XmlRoot(ElementName = "DealEntry")]
    public class DealEntry
    {
        public DealEntry(){}

        public DealEntry(Transaction transaction)
        {
            Transaction = transaction;
        }

        [XmlElement(ElementName = "Transaction")]
        public Transaction Transaction { get; set; }
    }
}
