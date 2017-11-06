using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ElvizTestUtils.BatchTests;
using ElvizTestUtils.LookUpServiceReference;
using NUnit.Framework;

namespace ElvizTestUtils.QaLookUp
{
    [TestFixture]
    public class Test
    {
        [Test]
        public void SortTest()
        {

            string s= @"<LookupMessage><MetaData><Action>PriceBasisLookup</Action><MessageId>1234</MessageId></MetaData><PriceBasisLookup/></LookupMessage>";

            LookupServiceClient client = WCFClientUtil.GetLookUpServiceServiceProxy();

            string r = client.LookUp(s);

            Console.WriteLine(r);

            //string[] s = {"A", "B", "D", "C"};
            //string[] prefStrings = {"B", "A"};

            //string[] sorted=s.OrderBy(x => prefStrings.ToList().IndexOf(x)).ToArray();

            //foreach (string s1 in sorted)
            //{
            //    Console.WriteLine(s1);
            //}

            //BatchTestFile t= TestXmlTool.Deserialize<BatchTestFile>(
            //    @"C:\Users\daniel.watanabe\Documents\TFS\Elviz\Development\QA\Regression\Bin\ErmBatch\TestFiles\El-Asian-Profile-EUR-Exposure-EUR-VolSurf.xml");

            //TestXmlTool.SerializeToXml(t, @"C:\Users\daniel.watanabe\Documents\TFS\Elviz\Development\QA\Regression\Bin\ErmBatch\TestFiles\00a.xml");
            //Console.WriteLine(TestXmlTool.Serialize(t));

        }

        [Test]
        public void Testit()
        {
            //int id = 381;

            //QaLookUpClient c = new QaLookUpClient();

            ////  QaTransactionDTO dto = c.GetQaTransactionDTO(id);
            ////    Assert.AreEqual(dto.TransactionId,id);
            ////Assert.AreEqual(dto.BuySell,QaBuySell.Buy);
            ////PropertyInfo[] info = dto.GetType().GetProperties();

            //int[] tr = QaLookUpClient.GetTransactionIdByExternalId(new string[] { "Gas FSD Hourly-MW-h InsertUpdate Bulk Excel", "Gas FSD Daily InsertUpdate Excel" });

            //int[] rd;

            //foreach (int i in tr)
            //{
            //    Console.WriteLine(i);
            //}

            //foreach (PropertyInfo propertyInfo in info)
            //{
            //    Console.WriteLine(propertyInfo.Name);
            //}

            //System.Xml.Serialization.XmlSerializer x = new System.Xml.Serialization.XmlSerializer(dto.GetType());

            //StringWriter textWriter = new StringWriter();

            //x.Serialize(textWriter, dto);

            //Console.WriteLine(textWriter.ToString());
        }

        [Test]
        public void Testen()
        {
            int id = 159;

            QaLookUpClient client=new QaLookUpClient();

            QaTransactionDTO dto= client.GetQaTransactionDTO(id);

            System.Xml.Serialization.XmlSerializer x = new System.Xml.Serialization.XmlSerializer(dto.GetType());

            StringWriter textWriter = new StringWriter();

            x.Serialize(textWriter, dto);

            Console.WriteLine(textWriter.ToString());
        }

        [Test]
        public void T()
        {
            //InstrumentData e=new InstrumentData(){BalanceArea = "A",PutCall = QaPutCall.Call};
            //InstrumentData a=new InstrumentData(){BalanceArea ="A",PutCall = QaPutCall.Put};

            //QaTransactionDtoAssert.AssertInstrumentData(e,a,new string[]{"InstrumentData.BalanceArea"});

            SettlementData es=new SettlementData(){CurrencySource = "Vizz",MarketPriceMultiplicator = 1};
            SettlementData aa=new SettlementData(){CurrencySource = "Viz",MarketPriceMultiplicator = 11};

           // QaTransactionDtoAssert.AssertSettlementData(es, aa, new string[] { "SettlementData.CurrencySource" },false);


            QaLookUpClient c = new QaLookUpClient();

            QaTransactionDTO exp = c.GetQaTransactionDTO(157);
            QaTransactionDTO act = c.GetQaTransactionDTO(158);

            QaTransactionDtoAssert.AreEqual(exp, act, new string[] { "TransactionId","BuySell" }, false);

            string[] exclude=new string[]{"InstrumentData","SettlementData"};

            //t.ToList().Where(x => exclude.Contains(x.Name));

            //IList<PropertyInfo> t = typeof(QaTransactionDTO).GetProperties();

            //IList<PropertyInfo> excludeList = t.ToList().Where(x => exclude.Contains(x.Name)).ToList();
            //    //t.ToList().Where(x => exclude.Contains(x.Name)).ToArray();

            //t = t.Except(excludeList).ToList();

            //foreach (PropertyInfo propertyInfo in t)
            //{
            //    Console.WriteLine(propertyInfo.Name);
            //}

        }

        [Test]
        public void T3()
        {
            double? d = null;

            double? f = d as double?;

          //  QaDeliveryType t1;//QaDeliveryType.Financial;
           // QaDeliveryType t2;// = QaDeliveryType.Financial;

//            Assert.AreEqual(true,t1==t2);

            string[] exc = new[] { "Portfolios.PortfolioExternalId" };

            QaTransactionDTO expected=new QaTransactionDTO(){ContractModelType = "AB",TransactionId = 1,BuySell = QaBuySell.Buy};
            QaTransactionDTO actual=new QaTransactionDTO(){ContractModelType = "AB",TransactionId = 2};

            InstrumentData e=new InstrumentData(){Strike = 4,FromDate = new DateTime(2011,11,1)};
            InstrumentData a=new InstrumentData(){Strike = 4.1,FromDate = new DateTime(2011,11,2)};

            expected.InstrumentData = e;
            actual.InstrumentData = a;

          //  IList<string> list=QaTransactionDtoAssert.AreEqualWithErrorList(expected, actual, exc, false);
          QaTransactionDtoAssert.AreEqual(expected, actual, exc, false);

            IList<string> list=new List<string>();
            foreach (string s in list)
            {
                Console.WriteLine(s);
            }
        }

        [Test]
        public void T223()
        {
            string es = "A";
            string aas="A";

            object e = es;
            object a = aas;

            Assert.AreEqual(true,e==a);
        }

        [Test]
        public void T2()
        {
         

            //PropertyGroup[] expected = { new PropertyGroup { Name = "Hi" } };
            //PropertyGroup[] actual = { new PropertyGroup { Name = "Ho" } };

            Portfolios expected=new Portfolios(){PortfolioExternalId = "a"};
            Portfolios actual=new Portfolios(){PortfolioExternalId = "b"};

            string[] exc = new[] { "Portfolios.PortfolioExternalId" };
            string[] excBullshit = new[] {"bull"};
            try
            {
                QaTransactionDtoAssert.AreEqual(expected, actual, exc, true);
                throw new ArgumentException("wrong");
            }
            catch (AssertionException)
            {
                
            }

            QaTransactionDtoAssert.AreEqual(expected, actual, exc, false);

            QaTransactionDtoAssert.AreEqual(expected, actual, excBullshit, true);

            try
            {
                QaTransactionDtoAssert.AreEqual(expected, actual, excBullshit, false);
                throw new ArgumentException("wrong");
            }
            catch (AssertionException)
            {

            }

        }

        [Test]
        public void Compare()
        {
            int expected = 175;
            int actual = 179;
            QaLookUpClient c=new QaLookUpClient();
            QaTransactionDTO expectedDto = c.GetQaTransactionDTO(expected);
            QaTransactionDTO actualDto = c.GetQaTransactionDTO(actual);

            CompareTransactions(expectedDto,actualDto);

            System.Xml.Serialization.XmlSerializer x = new System.Xml.Serialization.XmlSerializer(actualDto.GetType());

            StringWriter textWriter = new StringWriter();

            x.Serialize(textWriter, actualDto);

            Console.WriteLine(textWriter.ToString());
        }


        public void CompareTransactions(QaTransactionDTO expectedTransactionDTO, QaTransactionDTO dto)
        {
            Assert.AreEqual(expectedTransactionDTO.DealType, dto.DealType,
                "Values for property 'DealType' are not equal:");
            Assert.AreEqual(expectedTransactionDTO.BuySell, dto.BuySell, "Values for property 'BuySell' are not equal:");
            Assert.AreEqual(expectedTransactionDTO.DeliveryType, dto.DeliveryType,
                "Values for property 'DeliveryType' are not equal:");
            ////Portfolio
            Assert.AreEqual(expectedTransactionDTO.Portfolios.PortfolioName, dto.Portfolios.PortfolioName,
                "Values for property 'PortfolioName' are not equal:");
            Assert.AreEqual(expectedTransactionDTO.Portfolios.PortfolioExternalId, dto.Portfolios.PortfolioExternalId,
                "Values for property 'PortfolioExternalId' are not equal:");
            Assert.AreEqual(expectedTransactionDTO.Portfolios.CounterpartyPortfolioExternalId,
                dto.Portfolios.CounterpartyPortfolioExternalId,
                "Values for property 'CounterpartyPortfolioExternalId' are not equal:");
            Assert.AreEqual(expectedTransactionDTO.Portfolios.CounterpartyPortfolioName,
                dto.Portfolios.CounterpartyPortfolioName,
                "Values for property 'CounterpartyPortfolioName'  are not equal:");
            Assert.AreEqual(expectedTransactionDTO.ContractModelType, dto.ContractModelType,
                "Values for property 'ContractModelType' are not equal");
            //Console.WriteLine(expectedTransactionDTO.TransactionId);
            //Console.WriteLine(expectedTransactionDTO.DealType);
            //Console.WriteLine(expectedTransactionDTO.Portfolios.PortfolioName);

            ////InstrumentData
            Assert.AreEqual(expectedTransactionDTO.InstrumentData.ExecutionVenue, dto.InstrumentData.ExecutionVenue,
                "Values for property 'ExecutionVenue' are not equal:");
            Assert.AreEqual(expectedTransactionDTO.InstrumentData.InstrumentName, dto.InstrumentData.InstrumentName,
                "Values for property 'InstrumentName' are not equal:");
            Assert.AreEqual(expectedTransactionDTO.InstrumentData.PriceBasis, dto.InstrumentData.PriceBasis,
                "Values for property 'PriceBasis'  are not equal:");
            Assert.AreEqual(expectedTransactionDTO.InstrumentData.PriceBasisToArea, dto.InstrumentData.PriceBasisToArea,
                "Values for property 'PriceBasisToArea'  are not equal:");
            Assert.AreEqual(expectedTransactionDTO.InstrumentData.PriceType, dto.InstrumentData.PriceType,
                "Values for property 'PriceType'  are not equal:");
            Assert.AreEqual(expectedTransactionDTO.InstrumentData.FromDate, dto.InstrumentData.FromDate,
                "Values for property 'FromDate' are not equal:");
            Assert.AreEqual(expectedTransactionDTO.InstrumentData.ToDate, dto.InstrumentData.ToDate,
                "Values for property 'ToDate' are not equal:");
            Assert.AreEqual(expectedTransactionDTO.InstrumentData.LoadProfile, dto.InstrumentData.LoadProfile,
                "Values for property 'LoadProfile' are not equal:");
            Assert.AreEqual(expectedTransactionDTO.InstrumentData.TimeZone, dto.InstrumentData.TimeZone,
                "Values for property 'TimeZone' are not equal:");
            Assert.AreEqual(expectedTransactionDTO.InstrumentData.PutCall, dto.InstrumentData.PutCall,
                "Values for property 'PutCall' are not equal:");
            Assert.AreEqual(expectedTransactionDTO.InstrumentData.BalanceArea, dto.InstrumentData.BalanceArea,
                "Values for property 'BalanceArea' are not equal:");
            Assert.AreEqual(expectedTransactionDTO.InstrumentData.Strike, dto.InstrumentData.Strike,
                "Values for property 'Strike' are not equal:");
            Assert.AreEqual(expectedTransactionDTO.InstrumentData.ExpiryTime, dto.InstrumentData.ExpiryTime,
                "Values for property 'ExpiryTime' are not equal:");
            Assert.AreEqual(expectedTransactionDTO.InstrumentData.ExpiryDate, dto.InstrumentData.ExpiryDate,
                "Values for property 'ExpiryDate' are not equal:");
            Assert.AreEqual(expectedTransactionDTO.InstrumentData.IndexFormula, dto.InstrumentData.IndexFormula,
                "Values for property 'IndexFormula' are not equal:");
            Assert.AreEqual(expectedTransactionDTO.InstrumentData.BaseIsoCurrency, dto.InstrumentData.BaseIsoCurrency,
                "Values for property 'BaseIsoCurrency' are not equal:");
            Assert.AreEqual(expectedTransactionDTO.InstrumentData.CrossIsoCurrency, dto.InstrumentData.CrossIsoCurrency,
                "Values for property 'CrossIsoCurrency' are not equal:");
            Assert.AreEqual(expectedTransactionDTO.InstrumentData.TransferDate, dto.InstrumentData.TransferDate,
                "Values for property 'TransferDate' are not equal:");
            Assert.AreEqual(expectedTransactionDTO.InstrumentData.CertificateType, dto.InstrumentData.CertificateType,
                "Values for property 'CertificateType' are not equal:");
            Assert.AreEqual(expectedTransactionDTO.InstrumentData.ProductionFacility,
                dto.InstrumentData.ProductionFacility, "Values for property 'ProductionFacility' are not equal:");
            Assert.AreEqual(expectedTransactionDTO.InstrumentData.EnvironmentLabel, dto.InstrumentData.EnvironmentLabel,
                "Values for property 'EnvironmentLabel' are not equal:");
            Assert.AreEqual(expectedTransactionDTO.InstrumentData.FromCountry, dto.InstrumentData.FromCountry,
                "Values for property 'FromCountry' are not equal:");
            Assert.AreEqual(expectedTransactionDTO.InstrumentData.ToCountry, dto.InstrumentData.ToCountry,
                "Values for property 'ToCountry' are not equal:");
            Assert.AreEqual(expectedTransactionDTO.InstrumentData.SamplingPeriod, dto.InstrumentData.SamplingPeriod,
                "Values for property 'SamplingPeriod' are not equal:");
            Assert.AreEqual(expectedTransactionDTO.InstrumentData.Interconnector, dto.InstrumentData.Interconnector,
                "Values for property 'Interconnector' are not equal:");
            Assert.AreEqual(expectedTransactionDTO.InstrumentData.ExpiryOffset, dto.InstrumentData.ExpiryOffset,
                "Values for property 'ExpiryOffset' are not equal:");
            Assert.AreEqual(expectedTransactionDTO.InstrumentData.Priority, dto.InstrumentData.Priority,
                "Values for property 'Priority' are not equal:");
            Assert.AreEqual(expectedTransactionDTO.InstrumentData.MinVol, dto.InstrumentData.MinVol,
                "Values for property 'MinVol' are not equal:");
            Assert.AreEqual(expectedTransactionDTO.InstrumentData.MaxVol, dto.InstrumentData.MaxVol,
                "Values for property 'MaxVol' are not equal:");
            Assert.AreEqual(expectedTransactionDTO.InstrumentData.CapacityId, dto.InstrumentData.CapacityId,
                "Values for property 'CapacityId' are not equal:");

            Assert.AreEqual(expectedTransactionDTO.InstrumentData.UnderlyingExecutionVenue,
                dto.InstrumentData.UnderlyingExecutionVenue,
                "Values for property 'UnderlyingExecutionVenue' are not equal:");
            Assert.AreEqual(expectedTransactionDTO.InstrumentData.UnderlyingDeliveryType,
                dto.InstrumentData.UnderlyingDeliveryType, "Values for property 'UnderlyingDeliveryType' are not equal:");
            Assert.AreEqual(expectedTransactionDTO.InstrumentData.UnderlyingInstrumentType,
                dto.InstrumentData.UnderlyingInstrumentType,
                "Values for property 'UnderlyingInstrumentType' are not equal:");
            Assert.AreEqual(expectedTransactionDTO.InstrumentData.SamplingFrom, dto.InstrumentData.SamplingFrom,
                "Values for property 'SamplingFrom' are not equal:");
            Assert.AreEqual(expectedTransactionDTO.InstrumentData.SamplingTo, dto.InstrumentData.SamplingTo,
                "Values for property 'SamplingTo' are not equal:");

            ////SettlementData
            Assert.AreEqual(expectedTransactionDTO.SettlementData.Quantity, dto.SettlementData.Quantity,
                "Values for property 'Quantity' are not equal:");
            Assert.AreEqual(expectedTransactionDTO.SettlementData.QuantityUnit, dto.SettlementData.QuantityUnit,
                "Values for property 'QuantityUnit are not equal:");
            Assert.AreEqual(expectedTransactionDTO.SettlementData.Price, dto.SettlementData.Price,
                "Values for property 'Price' are not equal:");
            Assert.AreEqual(expectedTransactionDTO.SettlementData.PriceIsoCurrency, dto.SettlementData.PriceIsoCurrency,
                "Values for property 'PriceIsoCurrency' are not equal:");
            Assert.AreEqual(expectedTransactionDTO.SettlementData.PriceVolumeUnit, dto.SettlementData.PriceVolumeUnit,
                "Values for property 'PriceVolumeUnit' are not equal:");
            Assert.AreEqual(expectedTransactionDTO.SettlementData.CurrencySource, dto.SettlementData.CurrencySource,
                "Values for property 'CurrencySource' are not equal:");
            Assert.AreEqual(expectedTransactionDTO.SettlementData.MarketPriceMultiplicator,
                dto.SettlementData.MarketPriceMultiplicator,
                "Values for property 'MarketPriceMultiplicator' are not equal:");
            Assert.AreEqual(expectedTransactionDTO.SettlementData.MasterAgreement, dto.SettlementData.MasterAgreement,
                "Values for property 'MasterAgreement' are not equal:");
            Assert.AreEqual(expectedTransactionDTO.SettlementData.SettlementRule, dto.SettlementData.SettlementRule,
                "Values for property 'SettlementRule' are not equal:");
            Assert.AreEqual(expectedTransactionDTO.SettlementData.Resolution, dto.SettlementData.Resolution,
                "Values for property 'Resolution' are not equal:");
            Assert.AreEqual(expectedTransactionDTO.SettlementData.NominationsHourly,
                dto.SettlementData.NominationsHourly, "Values for property 'NominationsHourly' are not equal:");

            if (expectedTransactionDTO.SettlementData.TimeSeriesSet != null)
            {
                for (int i = 0; i < expectedTransactionDTO.SettlementData.TimeSeriesSet.TimeSeries.Count(); i++)
                {
                    Assert.AreEqual(expectedTransactionDTO.SettlementData.TimeSeriesSet.TimeSeries[i].Resolution,
                        dto.SettlementData.TimeSeriesSet.TimeSeries[i].Resolution,
                        "Values for property 'TimeSeriesSet.TimeSeries.Resolution' are not equal at index = " + i);

                    Assert.AreEqual(
                        expectedTransactionDTO.SettlementData.TimeSeriesSet.TimeSeries[i].TimeSeriesTypeName,
                        dto.SettlementData.TimeSeriesSet.TimeSeries[i].TimeSeriesTypeName,
                        "Values for property 'TimeSeriesSet.TimeSeries.TimeSeriesTypeName' are not equal at index = " +
                        i);

                    Assert.AreEqual(expectedTransactionDTO.SettlementData.TimeSeriesSet.TimeSeries[i].TimezoneName,
                        dto.SettlementData.TimeSeriesSet.TimeSeries[i].TimezoneName,
                        "Values for property 'TimeSeriesSet.TimeSeries.TimezoneName' are not equal at index = " + i);

                    TimeSeriesValue[] expectedTSV =
                        expectedTransactionDTO.SettlementData.TimeSeriesSet.TimeSeries[i].TimeSeriesValues;
                    TimeSeriesValue[] actualTSV = dto.SettlementData.TimeSeriesSet.TimeSeries[i].TimeSeriesValues;

                    for (int j = 0; j < expectedTSV.Length; j++)
                    {
                        Assert.AreEqual(expectedTSV[j].FromDateTime, actualTSV[j].FromDateTime,
                            "Values for property 'FromDateTime' for '" +
                            expectedTransactionDTO.SettlementData.TimeSeriesSet.TimeSeries[i].TimeSeriesTypeName +
                            "' are not equal at index = " + j);
                        Assert.AreEqual(expectedTSV[j].ToDateTime, actualTSV[j].ToDateTime,
                            "Values for property 'ToDateTime' for '" +
                            expectedTransactionDTO.SettlementData.TimeSeriesSet.TimeSeries[i].TimeSeriesTypeName +
                            "' are not equal at index = " + j);
                        Assert.AreEqual(expectedTSV[j].Value, actualTSV[j].Value,
                            "Values for property 'FeeValue' for '" +
                            expectedTransactionDTO.SettlementData.TimeSeriesSet.TimeSeries[i].TimeSeriesTypeName +
                            "' are not equal at index = " + j);
                    }
                }
            }

            // add PriceVolumeTimeSeries
            if (expectedTransactionDTO.SettlementData.PriceVolumeTimeSeriesDetails != null)
            {
                Assert.AreEqual((int) expectedTransactionDTO.SettlementData.PriceVolumeTimeSeriesDetails.Count(),
                    (int) dto.SettlementData.PriceVolumeTimeSeriesDetails.Count(),
                    "Values for property 'PriceVolumeTimeSeriesDetails' are not equal:");
                for (int i = 0; i < expectedTransactionDTO.SettlementData.PriceVolumeTimeSeriesDetails.Count(); i++)
                {
                    Assert.AreEqual(expectedTransactionDTO.SettlementData.PriceVolumeTimeSeriesDetails[i].FromDateTime,
                        dto.SettlementData.PriceVolumeTimeSeriesDetails[i].FromDateTime,
                        "'PriceVolumeTimeSeriesDetails[" + i + "].FromDate' properties are not equal:");
                    Assert.AreEqual(expectedTransactionDTO.SettlementData.PriceVolumeTimeSeriesDetails[i].ToDateTime,
                        dto.SettlementData.PriceVolumeTimeSeriesDetails[i].ToDateTime,
                        "'PriceVolumeTimeSeriesDetails[" + i + "].ToDate' properties are not equal:");
                    Assert.AreEqual(expectedTransactionDTO.SettlementData.PriceVolumeTimeSeriesDetails[i].Price,
                        dto.SettlementData.PriceVolumeTimeSeriesDetails[i].Price,
                        "'PriceVolumeTimeSeriesDetails[" + i + "].Price' properties are not equal:");
                    Assert.AreEqual(expectedTransactionDTO.SettlementData.PriceVolumeTimeSeriesDetails[i].Volume,
                        dto.SettlementData.PriceVolumeTimeSeriesDetails[i].Volume,
                        "'PriceVolumeTimeSeriesDetails[" + i + "].Volume' properties are not equal:");
                }
            }

            ////DealDetails
            Assert.AreEqual(expectedTransactionDTO.DealDetails.Trader, dto.DealDetails.Trader,
                "Values for property 'Trader' are not equal:");
            Assert.AreEqual(expectedTransactionDTO.DealDetails.Status, dto.DealDetails.Status,
                "Values for property 'Status' are not equal:");
            Assert.AreEqual(expectedTransactionDTO.DealDetails.TradeDateTimeUtc, dto.DealDetails.TradeDateTimeUtc,
                "Values for property 'TradeDateTimeUtc' are not equal:");
            ////FeesData
            Assert.AreEqual(expectedTransactionDTO.FeesData.Broker, dto.FeesData.Broker,
                "Values for property 'Broker' are not equal:");
            Assert.AreEqual(expectedTransactionDTO.FeesData.ClearingType, dto.FeesData.ClearingType,
                "Values for property 'ClearingType' are not equal:");
            if (expectedTransactionDTO.FeesData.Fees != null)
            {
                Assert.AreEqual((int) expectedTransactionDTO.FeesData.Fees.Count(), (int) dto.FeesData.Fees.Count(),
                    "Values for property 'Fees' are not equal:");
                for (int i = 0; i < expectedTransactionDTO.FeesData.Fees.Count(); i++)
                {
                    Assert.AreEqual(expectedTransactionDTO.FeesData.Fees[i].FeeType, dto.FeesData.Fees[i].FeeType,
                        "'Fees." + expectedTransactionDTO.FeesData.Fees[i].FeeType + ".ValueType' properties at index= [" + i +
                        "] are not equal:");
                    Assert.AreEqual(expectedTransactionDTO.FeesData.Fees[i].FeeValue, dto.FeesData.Fees[i].FeeValue,
                        "'Fees." + expectedTransactionDTO.FeesData.Fees[i].FeeType + ".FeeValue' properties at index= [" + i +
                        "] are not equal:");
                    Assert.AreEqual(expectedTransactionDTO.FeesData.Fees[i].FeeUnit,
                        dto.FeesData.Fees[i].FeeUnit,
                        "'Fees." + expectedTransactionDTO.FeesData.Fees[i].FeeType + ".FeeUnit' properties at index= [" +
                        i + "] are not equal:");
                }
            }

            //ReferenceData
         //   Assert.AreEqual(expectedTransactionDTO.ReferenceData.ExternalId, dto.ReferenceData.ExternalId,
         //       "Values for property 'ExternalId' are not equal:");
         //   Assert.AreEqual(expectedTransactionDTO.ReferenceData.ExternalSource, dto.ReferenceData.ExternalSource,
         //       "Values for property 'ExternalSource' are not equal:");
            Assert.AreEqual(expectedTransactionDTO.ReferenceData.TicketNumber, dto.ReferenceData.TicketNumber,
                "Values for property 'TicketNumber' are not equal:");

            if (expectedTransactionDTO.ReferenceData.DealGroups != null)
            {
                Assert.AreEqual((int) expectedTransactionDTO.ReferenceData.DealGroups.Count(),
                    (int) dto.ReferenceData.DealGroups.Count(), "Values for property 'DealGroups' are not equal:");
                for (int i = 0; i < expectedTransactionDTO.ReferenceData.DealGroups.Count(); i++)
                {
                    Assert.AreEqual(expectedTransactionDTO.ReferenceData.DealGroups[i].Name,
                        dto.ReferenceData.DealGroups[i].Name,
                        "'DealGroups." + expectedTransactionDTO.ReferenceData.DealGroups[i].Name +
                        ".Name' properties at index= [" + i + "] are not equal:");
                    Assert.AreEqual(expectedTransactionDTO.ReferenceData.DealGroups[i].Value,
                        dto.ReferenceData.DealGroups[i].Value,
                        "'DealGroups." + expectedTransactionDTO.ReferenceData.DealGroups[i].Name +
                        ".FeeValue' properties at index= [" + i + "] are not equal:");
                }
            }
            else
                Assert.AreEqual(expectedTransactionDTO.ReferenceData.DealGroups, dto.ReferenceData.DealGroups,
                    "Values for property 'DealGroups' are not equal:");
            string exp=null;
            if (expectedTransactionDTO.ReferenceData.Comment != string.Empty)
                 exp = expectedTransactionDTO.ReferenceData.Comment;
          

            Assert.AreEqual(exp, dto.ReferenceData.Comment,"Values for property 'Comment' are not equal:");
            // Assert.AreEqual(expectedTransactionDTO.ReferenceData.ModificationDateTimeUtc, dto.ReferenceData.ModificationDateTimeUtc, "Values for property 'ModificationDateTimeUtc' are not equal:");
            Assert.AreEqual(expectedTransactionDTO.ReferenceData.QuotaRegion, dto.ReferenceData.QuotaRegion,
                "Values for property 'QuotaRegion' are not equal:");
            Assert.AreEqual(expectedTransactionDTO.ReferenceData.RiskValue, dto.ReferenceData.RiskValue,
                "Values for property 'RiskValue' are not equal:");
            Assert.AreEqual(expectedTransactionDTO.ReferenceData.Originator, dto.ReferenceData.Originator,
                "Values for property 'Originator' are not equal:");
            Assert.AreEqual(expectedTransactionDTO.ReferenceData.Deliveries, dto.ReferenceData.Deliveries,
                "Values for property 'Deliveries' are not equal:");
            Assert.AreEqual(expectedTransactionDTO.ReferenceData.DeclareId, dto.ReferenceData.DeclareId,
                "Values for property 'DeclareId' are not equal:");
            Assert.AreEqual(expectedTransactionDTO.ReferenceData.ContractSplitId, dto.ReferenceData.ContractSplitId,
                "Values for property 'ContractSplitId' are not equal:");

            Assert.AreEqual(expectedTransactionDTO.ReferenceData.DistributionParentTransactionId,
                dto.ReferenceData.DistributionParentTransactionId,
                "Values for property 'DistributionParentTransactionId' are not equal:");
            Assert.AreEqual(expectedTransactionDTO.ReferenceData.DistributedQuantity,
                dto.ReferenceData.DistributedQuantity, "Values for property 'DistributedQuantity' are not equal:");
            Assert.AreEqual(expectedTransactionDTO.ReferenceData.OriginalQuantity, dto.ReferenceData.OriginalQuantity,
                "Values for property 'OriginalQuantity' are not equal:");
            // Assert.AreEqual(expectedTransactionDTO.ReferenceData.ReferringId, dto.ReferenceData.ReferringId, "Values for property 'ReferringId' are not equal:");
            Assert.AreEqual(expectedTransactionDTO.ReferenceData.CascadingOriginIds,
                dto.ReferenceData.CascadingOriginIds, "Values for property 'CascadingOriginIds' are not equal:");

            //TransactionWorkFlowDetails
            Assert.AreEqual(expectedTransactionDTO.TransactionWorkFlowDetails.Authorised,
                dto.TransactionWorkFlowDetails.Authorised, "Values for property 'Authorised' are not equal:");
            Assert.AreEqual(expectedTransactionDTO.TransactionWorkFlowDetails.Audited,
                dto.TransactionWorkFlowDetails.Audited, "Values for property 'Audited' are not equal:");
            Assert.AreEqual(expectedTransactionDTO.TransactionWorkFlowDetails.Paid, dto.TransactionWorkFlowDetails.Paid,
                "Values for property 'Paid' are not equal:");
            Assert.AreEqual(expectedTransactionDTO.TransactionWorkFlowDetails.ConfirmedByBroker,
                dto.TransactionWorkFlowDetails.ConfirmedByBroker,
                "Values for property 'ConfirmedByBroker' are not equal:");
            Assert.AreEqual(expectedTransactionDTO.TransactionWorkFlowDetails.ConfirmedByCounterparty,
                dto.TransactionWorkFlowDetails.ConfirmedByCounterparty,
                "Values for property 'ConfirmedByCounterparty' are not equal:");

            Assert.That(dto.TransactionWorkFlowDetails.TimeStampClearedUtc,
                Is.EqualTo(expectedTransactionDTO.TransactionWorkFlowDetails.TimeStampClearedUtc).Within(1).Seconds,
                "Values for property 'TimeStampClearedUTC' are not equal:");
            Assert.That(dto.TransactionWorkFlowDetails.TimeStampConfirmationBrokerUtc,
                Is.EqualTo(expectedTransactionDTO.TransactionWorkFlowDetails.TimeStampConfirmationBrokerUtc)
                    .Within(1)
                    .Seconds, "Values for property 'TimeStampConfirmationBrokerUTC' are not equal: ");
            Assert.That(dto.TransactionWorkFlowDetails.TimeStampConfirmationCounterPartyUtc,
                Is.EqualTo(expectedTransactionDTO.TransactionWorkFlowDetails.TimeStampConfirmationCounterPartyUtc)
                    .Within(1)
                    .Seconds, "Values for property 'TimeStampConfirmationCounterPartyUTC' are not equal:");

            //Assert.AreEqual(expectedTransactionDTO.TransactionWorkFlowDetails.TimeStampCounterpartyAuthorised, dto.TransactionWorkFlowDetails.TimeStampCounterpartyAuthorised, "Values for property 'TimeStampCounterpartyAuthorised' are not equal:");

            if (expectedTransactionDTO.PropertyGroups != null)
            {
                Assert.AreEqual(expectedTransactionDTO.PropertyGroups.Length, dto.PropertyGroups.Length);

                for (int i = 0; i < expectedTransactionDTO.PropertyGroups.Length; i++)
                {
                    Assert.AreEqual(expectedTransactionDTO.PropertyGroups[i].Name, dto.PropertyGroups[i].Name,
                        "PropertyGroup Name was not equal");

                    Assert.AreEqual(expectedTransactionDTO.PropertyGroups[i].Properties.Length,
                        dto.PropertyGroups[i].Properties.Length,
                        "Differnt number of Properites in : " + expectedTransactionDTO.PropertyGroups[i].Name);

                    if (expectedTransactionDTO.PropertyGroups[i].Properties != null)
                    {
                        for (int j = 0; j < expectedTransactionDTO.PropertyGroups[i].Properties.Length; j++)
                        {
                            Assert.AreEqual(expectedTransactionDTO.PropertyGroups[i].Properties[j].Name,
                                dto.PropertyGroups[i].Properties[j].Name,
                                "Property Name is not equal in PropertyGroup" +
                                expectedTransactionDTO.PropertyGroups[i].Name);
                            Assert.AreEqual(expectedTransactionDTO.PropertyGroups[i].Properties[j].Value,
                                dto.PropertyGroups[i].Properties[j].Value,
                                "Property FeeValue is not equal in PropertyGroup" +
                                expectedTransactionDTO.PropertyGroups[i].Name);
                            Assert.AreEqual(expectedTransactionDTO.PropertyGroups[i].Properties[j].ValueType,
                                dto.PropertyGroups[i].Properties[j].ValueType,
                                "Property ValueType is not equal in PropertyGroup" +
                                expectedTransactionDTO.PropertyGroups[i].Name);
                        }
                    }
                }
            }
        }
    }
}
