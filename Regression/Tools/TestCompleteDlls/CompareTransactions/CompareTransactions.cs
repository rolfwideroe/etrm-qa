using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Xml;
using ElvizTestUtils;
using ElvizTestUtils.LookUpServiceReference;
using Microsoft.XmlDiffPatch;
using NUnit.Framework;

namespace CompareTransactions
{
    //public class LookUpServiceFunctions
    //{

    //    private static string LookUpServiceUrl = "http://{0}:8009/LookupService";

    //    public static ILookupService GetLookUpServiceServiceProxy()
    //    {
    //      //  string appServerName = WCFClientUtil.GetLookUpServiceServiceProxy();

    //        BasicHttpBinding binding = WCFClientUtil.GetDefaultBasicHttpBinding();

    //        EndpointAddress address = WCFClientUtil.GetDefaultEndPointAdresss(appServerName);

    //        Console.WriteLine("Connecting to : " + address.Uri);

    //        return new LookupServiceClient(binding, address);
    //    }
    //}

    public class CompareUpdatedTransaction
    {
        public static string path = @"\\Berpc-in7\Shared\DealManager\DealManager\TestFiles\";

        [Test]
        public void CreateXMLwithOriginalTransactionDTO()
        {
            int origId = 75;
           // ILookupService serviceLookup = LookUpServiceFunctions.GetLookUpServiceServiceProxy();
            ILookupService serviceLookup = WCFClientUtil.GetLookUpServiceServiceProxy();
            string[] results = {"no error"};
            int[] orIds = {origId};
            TransactionDTO[] origTransaction = serviceLookup.GetTransactionsByIds(orIds);

            if (origTransaction.Length > 0)
            {
                DateTime modificationDate = new DateTime();
                origTransaction[0].ModificationDate = modificationDate;
                //origTransaction[0].TransactionId = 0;

                System.Xml.Serialization.XmlSerializer writer =
                    new System.Xml.Serialization.XmlSerializer(origTransaction[0].GetType());
                //writer.Serialize(Console.Out, origTransaction);
                //Console.WriteLine();
                //Console.ReadLine();Original
                string filename = path + "tmp\\UpdatedTransactionDTO-" + origId + ".xml";
                System.IO.StreamWriter file = new System.IO.StreamWriter(filename);
                Console.WriteLine(filename);
                writer.Serialize(file, origTransaction[0]);
                file.Close();
                string actualfilename = path + +origId + ".xml";
                //XMLDiffCompare(filename, actualfilename, origId);

            }
            else results[0] = "Error retrieving original transaction by ID";

        }

        [Test]
        public static List<string> XMLAfterUpdate(string version, int updId)
        {
            //int origId = 1;
           // int updId = 145;
            //string version = "ExpectedResults_2014_2";
            var results = new List<string>();
            var resfromcomparenodes = new List<string>();

            string expectedResults = path + version+ "\\UpdatedTransactionDTO-" + updId + ".xml"; //= path + "ExpectedResults\\UpdatedTransactionDTO-" + updId + ".xml";
            if (!File.Exists(expectedResults))
            {
                results.Add("False");
                results.Add("File with expected results: " + expectedResults + "does not exist");
                return results;
            }
            // ********************
            //string expectedResults = path + "tmp\\OriginalTransactionDTO-" + updId + ".xml";

            ILookupService serviceLookup = WCFClientUtil.GetLookUpServiceServiceProxy();

            int[] upIds = {updId};
            TransactionDTO[] updTransactions = serviceLookup.GetTransactionsByIds(upIds);

            if (updTransactions.Length != 1)
            {
                results.Add("Error retrieving updated transaction by ID: expected only one transaction");
                return results;
            }

            TransactionDTO dto = updTransactions[0];

            DateTime modificationDate = new DateTime();
            dto.ModificationDate = modificationDate;

            if (dto.InstrumentType.Name == "Capacity" || dto.InstrumentType.Name == "Capacity Spot" ||
                dto.InstrumentType.Name == "Capacity Structured Deal" || dto.InstrumentType.Name == "Vanilla VPP")
                dto.ExpiryDate = new DateTime(2000, 1, 1, dto.ExpiryDate.Hour, dto.ExpiryDate.Minute, 0);


            System.Xml.Serialization.XmlSerializer writer =
                new System.Xml.Serialization.XmlSerializer(updTransactions[0].GetType());

            string filename = path + "TestInfo\\UpdatedTransactionDTO-" + updId + ".xml";
            System.IO.StreamWriter file = new System.IO.StreamWriter(filename);
            writer.Serialize(file, dto);
            file.Close();
            if (!File.Exists(filename))
            {
                results.Add("False");
                results.Add("File with actual results: " + filename + "does not exist");
                return results;
            }

            bool res = XMLDiffCompare(expectedResults, filename, updId);
            results.Add(res.ToString());

            if (!res)
            {
                resfromcomparenodes = Compare(expectedResults, filename);
                foreach (string item in resfromcomparenodes)
                    results.Add(item);
            }

            foreach (string item in results) Console.WriteLine(item);
            return results;
        }

        public static bool XMLDiffCompare(string expectedfile, string actualfile, int Id)
        {
            string file1 = expectedfile;
            string file2 = actualfile;
            string diffile = path + @"TestInfo\\diff.xml";
            string htmlfile = path + @"TestInfo\\errors-" + Id + ".html";

            System.Xml.XmlWriter writer = System.Xml.XmlWriter.Create(diffile);

            XmlDiff diff =
                new XmlDiff(XmlDiffOptions.IgnoreChildOrder | XmlDiffOptions.IgnoreComments |
                            XmlDiffOptions.IgnoreWhitespace);

            bool isEqual = false;
            isEqual = diff.Compare(file1, file2, false, writer);
            writer.Close();

            if (isEqual)
            {
                return isEqual;

            }
            else
            {
                XmlDiffView diffView = new XmlDiffView();

                //Load the original file again and the diff file.
                XmlTextReader original = new XmlTextReader(file1);
                XmlTextReader diffGram = new XmlTextReader(diffile);
                diffView.Load(original, diffGram);

                //Wrap the HTML file with necessary html and 
                //body tags and prepare it before passing it to 
                //the GetHtml method.

                StreamWriter sw1 = new StreamWriter(htmlfile);
                //Wrapping
                sw1.Write("<html><body><table>");
                sw1.Write("<tr><td><b>Legend:</b> <font style='background-color: yellow'" +
                          " color='black'>added</font>&nbsp;&nbsp;<font style='background-color: red'" +
                          "color='black'>removed</font>&nbsp;&nbsp;<font style='background-color: " +
                          "lightgreen' color='black'>changed</font>&nbsp;&nbsp;" +
                          "<font style='background-color: red' color='blue'>moved from</font>" +
                          "&nbsp;&nbsp;<font style='background-color: yellow' color='blue'>moved to" +
                          "</font>&nbsp;&nbsp;<font style='background-color: white' color='#AAAAAA'>" +
                          "ignored</font></td></tr>");
                sw1.Write("<tr><td><b>");
                sw1.Write(file1);
                sw1.Write("</b></td><td><b>");
                sw1.Write(file2);
                sw1.Write("</b></td></tr>");

                //This gets the differences but just has the 
                //rows and columns of an HTML table
                diffView.GetHtml(sw1);

                sw1.Write("</table></body></html>");

                //HouseKeeping...close everything we dont want to lock.
                sw1.Close();
                diffView = null;
                original.Close();
                diffGram.Close();

                return isEqual;
            }
        }


        public static List<string> Compare(string expectedfile, string actualfile)
        {
            XmlDocument doc1 = new XmlDocument();
            doc1.Load(expectedfile);
            XmlDocument doc2 = new XmlDocument();
            doc2.Load(actualfile);

            XmlNode inDoc = doc1.GetElementsByTagName("TransactionDTO")[0];
            XmlNode outDoc = doc2.GetElementsByTagName("TransactionDTO")[0];

            var errorlist = new List<string>();
            errorlist = CompareNodes(inDoc, outDoc, "/", errorlist);

            XmlNodeList inPriceVolumeNode = doc1.GetElementsByTagName("TimeSeriesSet");
            if (inPriceVolumeNode.Count > 0)
            {
                XmlNodeList outPriceVolumeNode = doc2.GetElementsByTagName("TimeSeriesSet");
                if (outPriceVolumeNode.Count > 0)
                {
                    if (inPriceVolumeNode[0].InnerXml == outPriceVolumeNode[0].InnerXml)
                        Console.WriteLine("1 == 2");
                    else
                    {
                        Console.WriteLine("/TransactionDTO/" + inPriceVolumeNode[0].Name + " != " +
                                          outPriceVolumeNode[0].Name + "(" + outPriceVolumeNode[0].InnerXml + ")");
                        errorlist.Add("/TransactionDTO/" + inPriceVolumeNode[0].Name + " != " +
                                      outPriceVolumeNode[0].Name + "(" + outPriceVolumeNode[0].InnerXml + ")");
                    }

                }
            }

            //foreach (var item in errorlist) Console.WriteLine(item);
            return errorlist;

        }

        public static List<string> CompareNodes(XmlNode outNode, XmlNode inNode, string path, List<string> errorlist)
        {

            if (inNode.HasChildNodes)
            {
                path = path + inNode.Name + "/";
                XmlNodeList inChildNodes = inNode.ChildNodes;
                if (inNode.FirstChild.NodeType == XmlNodeType.Element)
                {
                    //Console.WriteLine(inNode.FirstChild.NodeType);

                    for (int n = 0; n < inChildNodes.Count; n++)
                    {
                        XmlNode inChildNode = inChildNodes.Item(n);
                        XmlNodeList outChildNodes = outNode.SelectNodes(inChildNode.Name);

                        switch (outChildNodes.Count)
                        {
                            case 0:
                                //Console.WriteLine(inChildNode.Name + "doesn't exist in the second XML file");
                                errorlist.Add(inChildNode.Name + " doesn't exist in the second XML file");
                                break;
                            case 1:
                                {
                                    XmlNode outChildNode = outChildNodes.Item(0);
                                    errorlist = CompareNodes(outChildNode, inChildNode, path, errorlist);
                                    break;
                                }
                        }
                    }
                }

                else
                {
                    if (inNode.InnerText != outNode.InnerText)
                        //Console.Write(inNode.Name + "(" + inNode.Value + ")" + " = " + outNode + "(" +
                        //              outNode.Value + ")");

                        //else
                    {
                        //Console.WriteLine(path + inNode.Name + "(" + inNode.InnerText + ")" + " != " + outNode.Name +
                        //                  "(" + outNode.InnerText + ")");
                        errorlist.Add(path + inNode.Name + "(" + inNode.InnerText + ")" + " != " + outNode.Name +
                                      "(" + outNode.InnerText + ")");
                    }
                    return errorlist;
                }
                return errorlist;
            }

            return errorlist;
        }

        // For debug
        [Test]
        public static List<string> CompareDebug() //string expectedfile, string actualfile)
        {
            XmlDocument doc1 = new XmlDocument();
            //doc1.Load(expectedfile);
            doc1.Load(@"E:\Shared\DealManager\DealManager\TestFiles\ExpectedResults\UpdatedTransactionDTO-15.xml");
            XmlDocument doc2 = new XmlDocument();
            //doc2.Load(actualfile);
            doc2.Load(@"E:\Shared\DealManager\DealManager\TestFiles\TestInfo\UpdatedTransactionDTO-15.xml");

            XmlNode inDoc = doc1.GetElementsByTagName("TransactionDTO")[0];
            XmlNode outDoc = doc2.GetElementsByTagName("TransactionDTO")[0];

            var errorlist = new List<string>();
            errorlist = CompareNodes(inDoc, outDoc, "/", errorlist);

            XmlNodeList inPriceVolumeNode = doc1.GetElementsByTagName("PriceVolumeTimeSeriesDetails");
            if (inPriceVolumeNode.Count > 0)
            {
                XmlNodeList outPriceVolumeNode = doc2.GetElementsByTagName("PriceVolumeTimeSeriesDetails");
                if (outPriceVolumeNode.Count > 0)
                {
                    if (inPriceVolumeNode[0].InnerXml == outPriceVolumeNode[0].InnerXml)
                        Console.WriteLine("1 == 2");
                    else
                    {
                        Console.WriteLine("/TransactionDTO/" + inPriceVolumeNode[0].Name + " != " +
                                          outPriceVolumeNode[0].Name + "(" + outPriceVolumeNode[0].InnerXml + ")");
                        errorlist.Add("/TransactionDTO/" + inPriceVolumeNode[0].Name + " != " +
                                      outPriceVolumeNode[0].Name + "(" + outPriceVolumeNode[0].InnerXml + ")");
                    }

                }
            }

            foreach (var item in errorlist) Console.WriteLine(item);
            return errorlist;

        }

        [Test]
        public static void Debug_Compare_Two_files() // int updId)
        {
            //int origId = 1;
            int updId = 77;
            var results = new List<string>();
            var resfromcomparenodes = new List<string>();

            string expectedResults = path + "ExpectedResults_2014_3\\UpdatedTransactionDTO-" + updId + ".xml";
            if (!File.Exists(expectedResults))
            {
                results.Add("False");
                results.Add("File with expected results: " + expectedResults + "does not exist");
            }
            // ********************
            //string expectedResults = path + "tmp\\OriginalTransactionDTO-" + updId + ".xml";

            ILookupService serviceLookup = WCFClientUtil.GetLookUpServiceServiceProxy();

            int[] upIds = {updId};
            TransactionDTO[] updTransactions = serviceLookup.GetTransactionsByIds(upIds);

            if (updTransactions.Length != 1)
            {
                results.Add("Error retrieving updated transaction by ID: expected only one transaction");
            }

            TransactionDTO dto = updTransactions[0];

            DateTime modificationDate = new DateTime();
            dto.ModificationDate = modificationDate;
            if (dto.InstrumentType.Name == "Capacity" || dto.InstrumentType.Name == "Capacity Spot" ||
                dto.InstrumentType.Name == "Capacity Structured Deal" || dto.InstrumentType.Name == "Vanilla VPP")
                dto.ExpiryDate = new DateTime(2000, 1, 1, dto.ExpiryDate.Hour, dto.ExpiryDate.Minute, 0);


            System.Xml.Serialization.XmlSerializer writer =
                new System.Xml.Serialization.XmlSerializer(updTransactions[0].GetType());

            string filename = path + "TestInfo\\UpdatedTransactionDTO-" + updId + ".xml";
            System.IO.StreamWriter file = new System.IO.StreamWriter(filename);
            writer.Serialize(file, dto);
            file.Close();
            if (!File.Exists(filename))
            {
                results.Add("False");
                results.Add("File with actual results: " + filename + "does not exist");
            }

            bool res = XMLDiffCompare(expectedResults, filename, updId);
            results.Add(res.ToString());
            foreach (var item in results) Console.WriteLine(item);

        }

        [Test]
        public static bool XMLDiffCompareTwoFilesSetInTest()
        {
            string file1 = @"E:\Shared\DealManager\DealManager\TestFiles\tmp\UpdatedTransactionDTO-129.xml";
            string file2 = @"E:\Shared\DealManager\DealManager\TestFiles\tmp\UpdatedTransactionDTO-130.xml"; ;
            string diffile = @"E:\Shared\DealManager\DealManager\TestFiles\tmp\diff.xml";
            string htmlfile = @"E:\Shared\DealManager\DealManager\TestFiles\tmp\diff.html";

            System.Xml.XmlWriter writer = System.Xml.XmlWriter.Create(diffile);

            XmlDiff diff =
                new XmlDiff(XmlDiffOptions.IgnoreChildOrder | XmlDiffOptions.IgnoreComments |
                            XmlDiffOptions.IgnoreWhitespace);

            bool isEqual = false;
            isEqual = diff.Compare(file1, file2, false, writer);
            writer.Close();

            if (isEqual)
            {
                return isEqual;

            }
            else
            {
                XmlDiffView diffView = new XmlDiffView();

                //Load the original file again and the diff file.
                XmlTextReader original = new XmlTextReader(file1);
                XmlTextReader diffGram = new XmlTextReader(diffile);
                diffView.Load(original, diffGram);

                //Wrap the HTML file with necessary html and 
                //body tags and prepare it before passing it to 
                //the GetHtml method.

                StreamWriter sw1 = new StreamWriter(htmlfile);
                //Wrapping
                sw1.Write("<html><body><table>");
                sw1.Write("<tr><td><b>Legend:</b> <font style='background-color: yellow'" +
                          " color='black'>added</font>&nbsp;&nbsp;<font style='background-color: red'" +
                          "color='black'>removed</font>&nbsp;&nbsp;<font style='background-color: " +
                          "lightgreen' color='black'>changed</font>&nbsp;&nbsp;" +
                          "<font style='background-color: red' color='blue'>moved from</font>" +
                          "&nbsp;&nbsp;<font style='background-color: yellow' color='blue'>moved to" +
                          "</font>&nbsp;&nbsp;<font style='background-color: white' color='#AAAAAA'>" +
                          "ignored</font></td></tr>");
                sw1.Write("<tr><td><b>");
                sw1.Write(file1);
                sw1.Write("</b></td><td><b>");
                sw1.Write(file2);
                sw1.Write("</b></td></tr>");

                //This gets the differences but just has the 
                //rows and columns of an HTML table
                diffView.GetHtml(sw1);

                sw1.Write("</table></body></html>");

                //HouseKeeping...close everything we dont want to lock.
                sw1.Close();
                diffView = null;
                original.Close();
                diffGram.Close();
                Console.WriteLine(isEqual);
                return isEqual;
                
            }
        }
    }
    

}
