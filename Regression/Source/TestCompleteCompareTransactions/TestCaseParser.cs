using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Xml.XPath;
using ElvizTestUtils;
using ElvizTestUtils.QaLookUp;
using NUnit.Framework;

namespace TestCompleteCompareTransactions
{
 
    class TestCaseParser
    {

        private const string TestFilesDealEntry = @"C:\TFS\Development\QA\Regression\Source\TestComplete\DealEntry\DealEntry\TestFiles\Electricity\ElectricityVanillaVPP";
        [Test]
        public void MoveDealGroup()
        {
            string[] fileEntries = Directory.GetFiles(TestFilesDealEntry);
            foreach (string filename in fileEntries)
            {
                Console.WriteLine(filename);
                //string filename =
                //    @"C:\TFS\Development\QA\Regression\Source\TestComplete\DealEntry\DealEntry\TestFiles\Currency\CurrencySwap\CurrecncySwap-Test3.xml";
                IList<XmlNode> tmpList = new List<XmlNode>();

                XmlDocument doc = new XmlDocument();
                doc.Load(filename); // input XML
                XmlNodeList transactionNodeItem = doc.GetElementsByTagName("Transaction");
                XmlNode TransactionNode = transactionNodeItem.Item(0);
                if (TransactionNode == null) Assert.Fail("No Transaction node in XML");

                XmlNodeList ReferenceDataNode = doc.GetElementsByTagName("ReferenceData");
                {
                    XmlNode RefData = ReferenceDataNode.Item(0);
                    if (RefData != null)
                    {

                        XmlNodeList childrens = RefData.ChildNodes;
                        for (int i = 0; i < childrens.Count; i++)
                        {
                            Console.WriteLine(childrens[i].Name);
                            if (childrens[i].Name.Contains("DealGroup"))
                            {
                                tmpList.Add(childrens[i]);
                                Console.WriteLine(filename + " will be updated.");

                            }
                        }

                        XmlNodeList existingDealGroupNode = doc.GetElementsByTagName("DealGroup");
                        XmlNode existingDealGroup = existingDealGroupNode.Item(0);
                        if (existingDealGroup == null)
                        {
                            XmlElement groupElement = doc.CreateElement("DealGroup");
                            TransactionNode.AppendChild(groupElement);
                            XmlNodeList dealGroupNode = doc.GetElementsByTagName("DealGroup");
                            XmlNode xmlNode = dealGroupNode.Item(0);
                            if (xmlNode != null)
                            {
                                foreach (XmlNode node in tmpList)
                                {
                                    xmlNode.AppendChild(node);
                                    doc.Save(filename);
                                }
                            }
                        }

                        
                    }

                }
            }
        }
    }
}
