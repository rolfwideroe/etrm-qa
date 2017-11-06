using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using ElvizTestUtils;
using ElvizTestUtils.LookUpServiceReference;
using NUnit.Framework;

namespace TestWCFDealInsertUpdate
{
   // [TestFixture]
    class GetTimeSeriesToXml
    {
     

     //   [Test] //Helper: include this test to be able to export the expected time series for a transaction
        public void Go()
        {

                LookupServiceClient lookupServiceClient = WCFClientUtil.GetLookUpServiceServiceProxy();
                int transId = 201;

                TransactionDTO dto= lookupServiceClient.GetTransactionsByIds(new []{transId})[0];

                TimeSeries[] actualTimeSeries = dto.TimeSeriesSet;

                ExpectedTimeSeriesSet set =new ExpectedTimeSeriesSet();
                ExpectedTimeSeries[] series = new ExpectedTimeSeries[actualTimeSeries.Count()];

            for (int i = 0; i < actualTimeSeries.Count(); i++)
            {
                
                TimeSeries actualSerie = actualTimeSeries[i];

                string resolution = actualSerie.Resolution.ToString();
                string timeSeriesTypeName = actualSerie.TimeSeriesType.Name;
                string timezoneName = actualSerie.TimezoneName;

                TimeSeriesValue[] actualValues = actualSerie.TimeSeriesValues;

                ExpectedTimeSeriesValue[] values = new ExpectedTimeSeriesValue[actualValues.Count()];

                for (int j = 0; j < actualValues.Count(); j++)
                {
                    TimeSeriesValue actualValue = actualValues[j];

                    DateTime from = actualValue.FromDateTime;
                    DateTime to = actualValue.UntilDateTime;
                    DateTime utcFrom = actualValue.UtcFromDateTime;
                    DateTime utcTo = actualValue.UtcUntilDateTime;
                    
                    //double value = actualValue.Value.HasValue ? actualValue.Value.Value : 0.0;
                    double? value = actualValue.Value;

                    values[j]=new ExpectedTimeSeriesValue(){FromDateTime = from,UntilDateTime = to,UtcFromDateTime = utcFrom,UtcUntilDateTime = utcTo,Value = value};


                    
                }



                series[i] = new ExpectedTimeSeries()
                {
                    ExpectedTimeSeriesValues = values,
                    Resolution = resolution,
                    TimeSeriesTypeName = timeSeriesTypeName,
                    TimezoneName = timezoneName
                };
            }
            set.ExpectedTimeSeries = series;


                 //   ExpectedTimeSeriesSet set;
                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        //XmlDocument docum = new XmlDocument();
                        //docum.AppendChild(docum.ImportNode(node, true));
                        //docum.Save(memoryStream);
                        //memoryStream.Position = 0;
                        //XmlReader reader = XmlReader.Create(memoryStream);

                        //set=(ExpectedTimeSeriesSet) xmlSerializer.Deserialize(reader);

                         XmlWriterSettings xSettings = new XmlWriterSettings
                         {
                             NewLineChars = Environment.NewLine,
                             NewLineOnAttributes = true,
                             NewLineHandling = NewLineHandling.Replace,
                             CloseOutput = true
                         };


                        XmlSerializer x = new XmlSerializer(set.GetType());

                        //x.Serialize(Console.Out, set); //x.Serialize();


                        XmlDocument doc =new XmlDocument();
                        XmlWriterSettings settings = new XmlWriterSettings();
                        settings.Encoding = Encoding.UTF8;
                        settings.Indent = true;
                        settings.IndentChars = "\t";
                        settings.NewLineChars = Environment.NewLine;
                        settings.ConformanceLevel = ConformanceLevel.Document;

                        using (XmlWriter writer = XmlTextWriter.Create(memoryStream,settings))
                        {
                            x.Serialize(writer,set);
                        }

                        string xml = Encoding.UTF8.GetString(memoryStream.ToArray());

                        const string removeString = "<?xml version=\"1.0\" encoding=\"utf-8\"?>";
                        int index = xml.IndexOf(removeString);
                        int length = removeString.Length;
                        String startOfString = xml.Substring(0, index);
                        String endOfString = xml.Substring(index + length);
                        String cleanXml = startOfString + endOfString;

                        const string removeString2 = " xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"";
                        index = cleanXml.IndexOf(removeString2);
                        length = removeString2.Length;
                        startOfString = cleanXml.Substring(0, index);
                        endOfString = cleanXml.Substring(index + length);
                        cleanXml = startOfString + endOfString;
                        Console.WriteLine(cleanXml);

                        //XmlDocument xmlout = new XmlDocument();
                        //xmlout.LoadXml("<DealResultsTemp" + removeString2 + "> " + cleanXml + "</DealResultsTemp>");
                        //xmlout.PreserveWhitespace = true;
                        //xmlout.Save("c:\\temp\\timeseriesExpectedResults.xml");

                    }
        }
    }
}
