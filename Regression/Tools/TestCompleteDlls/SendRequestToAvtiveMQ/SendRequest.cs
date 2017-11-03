using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Apache.NMS;
using Apache.NMS.Util;
using System.Threading;

namespace SendRequestToAvtiveMQ
{
    public class Message
    {
        
        public static string SendToQueue(string filename, string servername, string queuename)
        {
            //Uri connecturi = new Uri("tcp://qa132a:61616/");
            Uri connecturi = new Uri(servername);
            Console.WriteLine("About to connect to " + connecturi);
            IConnectionFactory factory = new NMSConnectionFactory(connecturi);
            try
            {
                using (IConnection connection = factory.CreateConnection())
                using (ISession session = connection.CreateSession())
                {
                    //IDestination destination = SessionUtil.GetDestination(session, "queue://PBCustomFeedInputQueue");
                    IDestination destination = SessionUtil.GetDestination(session, "queue://PBCustomFeedInputQueue");
                    Console.WriteLine("Using destination: " + destination);
                    // Create a consumer and producer
                    using (IMessageProducer producer = session.CreateProducer(destination))
                    {
                        // Start the connection so that messages will be processed.
                        connection.Start();
                        producer.DeliveryMode = MsgDeliveryMode.Persistent;
                      //  producer.RequestTimeout = receiveTimeout;
                        //string msg = "<?xml version=\"1.0\" encoding=\"utf-8\"?><PriceFeed xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"><param Venue=\"NORDPOOL\" Couterparty=\"NORDPOOL\" Commodity=\"Electricity\" RefArea=\"NPS\" TimeZone=\"(UTC+01:00) Central Europe Standard Time\" Feed=\"Excel Adapter\" IsExchange=\"true\" IsOption=\"false\" IsSwap=\"false\" /><price LoadType=\"Base\" Ticker=\"ENOQ1-14\" From=\"2014-04-01T00:00:00\" To=\"2014-06-30T00:00:00\" PeriodType=\"Quarter\"><Bid xsi:nil=\"true\" /><Ask>200</Ask><Last xsi:nil=\"true\" /><Close>202</Close></price><price LoadType=\"Base\" Ticker=\"ENOQ1-14\" From=\"2014-01-01T00:00:00\" To=\"2014-03-31T00:00:00\" PeriodType=\"Quarter\"><Bid xsi:nil=\"true\" /><Ask>100</Ask><Last xsi:nil=\"true\" /><Close xsi:nil=\"true\" /></price></PriceFeed>";
                        //Load XML document with request to queue
                        XmlDocument doc = new XmlDocument();
                        doc.Load(filename);
                        ITextMessage request = session.CreateTextMessage(doc.InnerXml);
                        Console.WriteLine(doc.InnerXml);
                        request.NMSType = "Viz.Priceboard.Adapter.Interface.PriceFeed";
                        producer.Send(request);
                        return "Request was sent";
                    }
                }
            }
            catch (Exception exception)
            {
                {
                    Console.WriteLine(exception.Message);
                    return exception.Message;
                }
                throw;
            }

        }
    }
}
