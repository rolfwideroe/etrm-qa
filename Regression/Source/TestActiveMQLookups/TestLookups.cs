using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using Apache.NMS;
using Apache.NMS.Util;
using ElvizTestUtils;
using NUnit.Framework;


namespace TestActiveMQLookups
{
    [TestFixture]
    public class TestLookups
    {

        [OneTimeSetUp]
        public void RunBeforeAnyTests()
        {
            Environment.CurrentDirectory = TestContext.CurrentContext.TestDirectory;
        }
        private static readonly IEnumerable<string> TestFileLookUp = TestCasesFileEnumeratorByFolder.TestCaseFiles("TestFiles\\TransactionLookup");
       private const string TestFilesTransactionLookup = "Testfiles\\TransactionLookup\\";

       private static readonly IEnumerable<string> TestFileAllLookUps = TestCasesFileEnumeratorByFolder.TestCaseFiles("TestFiles\\AllLookups");
       private const string TestFiles = "TestFiles\\AllLookups\\";
       public string  CorrelationID;

       [Test, Timeout(1000 * 1500), TestCaseSource("TestFileLookUp")]
       public void TransactionLookup(string testFile)
        {
          
            string testFilepath = Path.Combine(Directory.GetCurrentDirectory(), TestFilesTransactionLookup + testFile);
            string resultFilepath = Path.Combine(Directory.GetCurrentDirectory(), TestFilesTransactionLookup +"Results\\" + testFile);
            string appServerName = ElvizInstallationUtility.GetAppServerName();
            string servername = "tcp://" + appServerName + ":61616/";
            CorrelationID = Guid.NewGuid().ToString();

            if (!File.Exists(resultFilepath)) Assert.Fail("File with expected values does not exist");

           try
            {
               Uri connecturi = new Uri(servername);
               IConnectionFactory factory = new NMSConnectionFactory(connecturi);
               IConnection connection = factory.CreateConnection();
               ISession session = connection.CreateSession();
               IDestination destination = session.GetQueue("response.queue");
               IMessageConsumer consumer = session.CreateConsumer(destination);
               connection.Start();

               SendRequest(servername, testFilepath, CorrelationID);
               TimeSpan.FromSeconds(2);

               //Read response from response.queue
               IMessage response = consumer.Receive(TimeSpan.FromSeconds(5));
               Assert.IsNotNull (response, "Can not read responce from the queue.");
               if (response.NMSCorrelationID != CorrelationID) Assert.Fail("Test got wrong response.");

               consumer.Close();
               session.Close();
               connection.Close();

               ITextMessage textMessage = response as ITextMessage;

               if (textMessage != null)
               {
                  // Console.WriteLine(textMessage.Text);
                   AnalizeResult(textMessage.Text,"Transactions", resultFilepath);
               }
            
               else Assert.Fail("Empty response from queue.");
            }

            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }

        }

       [Test, Timeout(1000 * 1000), TestCaseSource("TestFileAllLookUps")]
       public void AllLookups(string testFile)
       {

           string testFilepath = Path.Combine(Directory.GetCurrentDirectory(), TestFiles + testFile);
           string appServerName = ElvizInstallationUtility.GetAppServerName();
           string servername = "tcp://" + appServerName + ":61616/";

           string resultFilepath = Path.Combine(Directory.GetCurrentDirectory(), TestFiles +"Results\\" + testFile);
           CorrelationID = Guid.NewGuid().ToString();

           if (!File.Exists(resultFilepath)) Assert.Fail("File with expected values does not exist");

           try
           {
               Uri connecturi = new Uri(servername);
               IConnectionFactory factory = new NMSConnectionFactory(connecturi);
               IConnection connection = factory.CreateConnection();
               ISession session = connection.CreateSession();
               IDestination destination = session.GetQueue("response.queue");
               IMessageConsumer consumer = session.CreateConsumer(destination);
               connection.Start();

               SendRequest(servername, testFilepath, CorrelationID);
               TimeSpan.FromSeconds(2);

               //Read response from response.queue
               IMessage response = consumer.Receive(TimeSpan.FromSeconds(5));
               Assert.IsNotNull (response, "Can not read responce from the queue.");

               if (response.NMSCorrelationID != CorrelationID) Assert.Fail("Test got wrong response.");

               consumer.Close();
               session.Close();
               connection.Close();

               ITextMessage textMessage = response as ITextMessage;

               string tagName = testFile.Replace(".xml", "");
               if (textMessage != null)
               {
                  // Console.WriteLine(textMessage.Text);
                   AnalizeResult(textMessage.Text,tagName, resultFilepath);
               }
            
               else Assert.Fail("Empty response from queue.");
            }
            
           catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }

       }


        private void SendRequest(string servername, string testFilepath, string corrID)
        {
            //string filename =
            //    @"C:\TFS\Development\QA\Regression\Bin\TestActiveMQLookups\TestFiles\TransactionLookup\129 ExternalID.xml";
            XmlDocument doc = new XmlDocument();
            doc.Load(testFilepath);
            try
            {
                IConnectionFactory factory = new NMSConnectionFactory(servername);
                IConnection connection = factory.CreateConnection();
                ISession session = connection.CreateSession();

                IDestination destination = SessionUtil.GetDestination(session, "queue://Elviz.Lookups");
                IDestination responseDestination = SessionUtil.GetDestination(session, "queue://response.queue");

                IMessageProducer producer = session.CreateProducer(destination);
                ITextMessage request = session.CreateTextMessage(doc.InnerXml);
                request.NMSCorrelationID = corrID;
                request.NMSReplyTo = responseDestination;
                
                producer.Send(request);

                producer.Close();
                session.Close();
                connection.Close();
            }
            catch (Exception ex)
            {
               Assert.Fail(ex.Message);
            }
        }



        private void AnalizeResult(string responce,string tagName, string resultFilepath)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(responce);

        //    Console.WriteLine(responce);

            XmlNodeList result = doc.GetElementsByTagName("Result");
            Assert.AreEqual("Success", result.Item(0).InnerXml, "Failed");
            XmlNodeList idNodes = doc.GetElementsByTagName(tagName);
            XmlNode xmlNode = idNodes.Item(0);
            if (xmlNode != null) Assert.AreNotEqual("", xmlNode.InnerXml, "Result does not contain any transactions");

            ArrayList actualArrayList = ConvertXmLtoArrayList(doc,tagName);

            XmlDocument expectedDoc = new XmlDocument();
            expectedDoc.Load(resultFilepath);
            ArrayList expectedArrayList = ConvertXmLtoArrayList(expectedDoc,tagName);

            for (int i = 0; i < expectedArrayList.Count; i++)
            {
                Assert.AreEqual(expectedArrayList[i], actualArrayList[i], "Elements are not equal: ");
                 Console.WriteLine(expectedArrayList[i] + " = " + actualArrayList[i]);

            }
        }

        public ArrayList ConvertXmLtoArrayList(XmlDocument doc,string tagName)
        {
            ArrayList list = new ArrayList();

            XmlNodeList idNodes = doc.GetElementsByTagName(tagName);
            foreach (XmlNode node in idNodes)
            {
                Recurcive(node, list);
            }

            //foreach (var item in list)
            //{
            //    Console.WriteLine(item);
            //}
            return list;
        }

        private void Recurcive(XmlNode InNode, ArrayList list)
        {
            string[] excludeProps = {    "ElvizId", 
                                         "ModificationDate"
                                    };

            string[] timeSeries = {    "PriceVolumeTimeSeries",
                                       "FlexibleStructuredDealInformation",
                                       "CashFlowPeriods"
                                    };
            foreach (XmlNode node in InNode)
            {
                if (timeSeries.Contains(node.ParentNode.Name)) list.Add(node.OuterXml);
                if (node.HasChildNodes) Recurcive(node, list);
                else
                {
                    if ((node.InnerText != string.Empty) &&
                        !excludeProps.Contains(node.ParentNode.Name)) 
                    {
                        list.Add(node.ParentNode.OuterXml);
                    }
                    else
                    {
                        if ((node.OuterXml.Contains("value=")) && !excludeProps.Contains(node.ParentNode.Name)) 
                            list.Add(node.OuterXml);
                    }
                }
             

            }
        }

      


    }
}

