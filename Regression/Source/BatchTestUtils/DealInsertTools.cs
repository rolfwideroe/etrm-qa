using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using ElvizTestUtils.DatabaseTools;
using ElvizTestUtils.DealServiceReference;
using NUnit.Framework;

namespace ElvizTestUtils
{
    public class DealInsertTools
    {
        //insert deal
        public static Dictionary<string, int> InsertXmlReturnTransId(string insertXml, IDealService dealServiceClient, bool isBulkInsert)
        {
            Dictionary<string, int> insertedExtIdsAndInternalIds = new Dictionary<string, int>();

            try
            {
                string insertResultXml = dealServiceClient.ImportDeal(insertXml);

                XmlDocument resultXml = new XmlDocument();
                resultXml.LoadXml(insertResultXml);

                XmlNodeList resultNode = resultXml.GetElementsByTagName("Result");
                XmlNodeList transIdNode = resultXml.GetElementsByTagName("TransactionId");
                XmlNodeList errormessage = resultXml.GetElementsByTagName("Message");
                XmlNode extIdNode =
                    resultXml.SelectSingleNode(
                        "/DealResult/Details/Transaction/TransactionDetails/ExternalTransactionId");

                if (resultNode[0].InnerXml == "Success")
                {
                    if (isBulkInsert)
                    {
                        if (extIdNode == null)
                            throw new ArgumentException(
                                "Insert was success and Bulk Insert, but returned no external id");

                        string[] extIds = Regex.Split(extIdNode.InnerText, "; ");

                        QaDao qaDao = new QaDao();

                        int[] transIds = qaDao.GetOriginalTransactionIdsFromExternalIds(extIds);

                        for (int i = 0; i < extIds.Length; i++)
                        {
                            string extId = extIds[i];
                            int internalId = transIds[i];

                            insertedExtIdsAndInternalIds.Add(extId, internalId);
                        }
                    }
                    else
                    {
                        string transIdString = (transIdNode[0].InnerText);

                        int transId;
                        int.TryParse(transIdString, out transId);
                        if (transId == 0)
                            throw new ArgumentException(
                                "Insert was sucess and not bulk insert but did not return a transaction id");

                        XmlDocument insertXmlDoc = new XmlDocument();
                        insertXmlDoc.LoadXml(insertXml);

                        XmlNode d = insertXmlDoc.SelectSingleNode("DealInsert/Transaction/ReferenceData/ExternalId");
                        string extId = d.InnerText;

                        insertedExtIdsAndInternalIds.Add(extId, transId);
                    }
                }
                else throw new ArgumentException(
                                "Insert failed, should always pass: " + errormessage[0].InnerXml);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.ToString());
            }

            return insertedExtIdsAndInternalIds;
        }
    }
}
