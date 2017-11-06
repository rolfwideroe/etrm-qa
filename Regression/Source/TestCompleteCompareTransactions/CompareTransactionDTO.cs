using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using ElvizTestUtils;
using ElvizTestUtils.LookUpServiceReference;
using ElvizTestUtils.QaLookUp;
using NUnit.Framework;

namespace TestCompleteCompareTransactions
{


    public class CompareUpdatedTransaction
    {
       // public static double TOLERANCE = 0.000000001;
   
        //DEbug
        [Test]
        public void DEBUG_GetQATransactionDTOByID()
        {
            int id = 71;

            QaLookUpClient c = new QaLookUpClient();

            //LookupServiceClient lookup = WCFClientUtil.GetLookUpServiceServiceProxy();
           // TransactionDTO[] dto = lookup.GetTransactionsByIds(new int[] {id});
         
            QaTransactionDTO dto = c.GetQaTransactionDTO(id);
            Assert.AreEqual(dto.TransactionId, id);

            XmlSerializer x = new XmlSerializer(dto.GetType());

            StringWriter textWriter = new StringWriter();

            x.Serialize(textWriter, dto);

            Console.WriteLine(textWriter.ToString());
        }
        
       //compare QATransactionDTO from ExpectedQaTransactionDTO with current transaction after updating
       [Test]
       public IList<string> CompareUpdatedQaTransactionDTO(int id, string file)
       {
           IList<string> errors = new string[]{};
            string relativePath = "\\DealManager\\DealManager\\TestFiles\\ExpectedQaTransactionDTO\\";
        //   string relativePath = "\\Source\\TestComplete\\DealManager\\DealManager\\TestFiles\\ExpectedQaTransactionDTO\\";//debug path
           string baseDirectory = Directory.GetParent(Directory.GetParent(Directory.GetCurrentDirectory()).ToString()).ToString();
           string filename = Path.GetFullPath(baseDirectory + relativePath) + file;
           
           XmlSerializer serializer = new XmlSerializer(typeof(QaTransactionDTO));
           XmlTextReader reader = new XmlTextReader(filename);
           reader.ReadToDescendant("QaTransactionDTO");
           QaTransactionDTO originalDto = (QaTransactionDTO)serializer.Deserialize(reader.ReadSubtree());
           reader.Close();

           QaLookUpClient c = new QaLookUpClient();
           QaTransactionDTO updatedDto = c.GetQaTransactionDTO(id);
          
            if (updatedDto.TransactionId != id)
            {
                errors.Add("QaTransaction.GetQaTransactionDTO() return wrong TransactionId: " + updatedDto.TransactionId + " != " + id);
                return errors;
            }

           //change obviously different properties to the same
           DateTime modificationDate = new DateTime();

           updatedDto.ReferenceData.ModificationDateTimeUtc = modificationDate;
           string[] excludeProps = {   "TransactionId", 
                                        "ReferenceData.ModificationDateTimeUtc", 
                                        "ReferenceData.ReferringId", 
                                        "TransactionWorkFlowDetails.TimeStampAuthorised", 
                                        "TransactionWorkFlowDetails.TimeStampCounterpartyAuthorised",
                                         "TransactionWorkFlowDetails.TimeStampClearedUtc" 
                                    };
           errors = QaTransactionDtoAssert.AreEqualWithErrorList(originalDto, updatedDto, excludeProps, false);
           return errors;

           // //print out alll errors    
           //foreach (var item in errors)
           //{
           //    Console.WriteLine(item);
           //}
          
           // return errors;
        }     

       //DEBUG compare two transactions
       [Test]
       public void DEBUG_CompareTransactions()
        {
            int id = 45;
            int upd_id = 159;
           // IList<string> errors = new List<string>();

            QaLookUpClient c = new QaLookUpClient();

            QaTransactionDTO originalDto = c.GetQaTransactionDTO(id);
            QaTransactionDTO copiedDto = c.GetQaTransactionDTO(upd_id);
            string[] excludeProps = {  "TransactionId", 
                                        "ReferenceData.ExternalId",
                                        "ReferenceData.ReferringId",
                                        "ReferenceData.ModificationDateTimeUtc", 
                                        "ReferenceData.ReferringId", 
                                        "TransactionWorkFlowDetails.TimeStampAuthorised", 
                                        "TransactionWorkFlowDetails.TimeStampCounterpartyAuthorised",
                                         "TransactionWorkFlowDetails.TimeStampClearedUtc" 
                                    };
            IList<string> errors = QaTransactionDtoAssert.AreEqualWithErrorList(originalDto, copiedDto, excludeProps, false);
            foreach (string error in errors)
            {
               Console.WriteLine(error);
            }
        }


       // TestComplete call CompareCopiedTransaction()
        public IList<string> CompareCopiedTransaction(int id, int upd_id)
        {
            QaLookUpClient c = new QaLookUpClient();
            QaTransactionDTO originalDto = c.GetQaTransactionDTO(id);
            QaTransactionDTO copiedDto = c.GetQaTransactionDTO(upd_id);
           // errors = CompareQaTransactionDTOs(originalDto, copiedDto);
            string[] excludeProps = {   "TransactionId", 
                                        "ReferenceData.ExternalId",
                                        "ReferenceData.ReferringId",
                                        "ReferenceData.ModificationDateTimeUtc", 
                                        "ReferenceData.ReferringId", 
                                        "ReferenceData.TicketNumber", 
                                        "TransactionWorkFlowDetails.TimeStampAuthorised", 
                                        "TransactionWorkFlowDetails.TimeStampCounterpartyAuthorised",
                                         "TransactionWorkFlowDetails.TimeStampClearedUtc" 
                                    };
            IList<string> errors = QaTransactionDtoAssert.AreEqualWithErrorList(originalDto, copiedDto, excludeProps, false);
            return errors;
        }

        // TestComplete call CompareUpdatedTransaction()
        //public IList<string> CompareUpdatedWithExpectedDTOs(int id)
        //{
        //    //int id = 75;
        //    //int upd_id = 1197;
        //    QaLookUpClient c = new QaLookUpClient();
        //    QaTransactionDTO originalDto = c.GetQaTransactionDTO(id);

        //    //QaTransactionDTO copiedDto = c.GetQaTransactionDTO(upd_id);
        //    //errors = CompareQaTransactionDTOs(originalDto, copiedDto);
        //    string[] excludeProps = {   "TransactionId", 
        //                                "ReferenceData.ModificationDateTimeUtc", 
        //                                "ReferenceData.ReferringId", 
        //                                "TransactionWorkFlowDetails.TimeStampAuthorised", 
        //                                "TransactionWorkFlowDetails.TimeStampCounterpartyAuthorised",
        //                                 "TransactionWorkFlowDetails.TimeStampClearedUtc" 
        //                            };
        //    IList<string> errors = QaTransactionDtoAssert.AreEqualWithErrorList(originalDto, copiedDto, excludeProps, false);
        //    return errors;
        //}

        //public static List<string> CompareQaTransactionDTOs(QaTransactionDTO originalDto, QaTransactionDTO copiedDto)
        //{
            
        //    var errors = new List<string>();

        //    if (originalDto.DealType !=  copiedDto.DealType) 
        //        errors.Add( "Values for property 'DealType' are not equal: " + originalDto.DealType + " != " + copiedDto.DealType );
            
        //    if (originalDto.BuySell != copiedDto.BuySell) 
        //        errors.Add("Values for property 'DealType' are not equal: " + originalDto.BuySell + " != " + copiedDto.BuySell);

        //    if (originalDto.BuySell != copiedDto.BuySell) 
        //        errors.Add("Values for property 'BuySell' are not equal: " + originalDto.BuySell + " != " + copiedDto.BuySell);
           
        //    if (originalDto.DeliveryType != copiedDto.DeliveryType) 
        //        errors.Add("Values for property 'DeliveryType' are not equal: " + originalDto.DeliveryType + " != " + copiedDto.DeliveryType);
        //    ////Portfolio
        //    if (originalDto.Portfolios.PortfolioName != copiedDto.Portfolios.PortfolioName) 
        //        errors.Add("Values for property 'PortfolioName' are not equal: " + originalDto.Portfolios.PortfolioName + " != " + copiedDto.Portfolios.PortfolioName);
           
        //    if (originalDto.Portfolios.PortfolioExternalId != copiedDto.Portfolios.PortfolioExternalId) 
        //        errors.Add("Values for property 'PortfolioExternalId' are not equal: " + originalDto.Portfolios.PortfolioExternalId + " != " + copiedDto.Portfolios.PortfolioExternalId);
            
        //    if (originalDto.Portfolios.CounterpartyPortfolioExternalId != copiedDto.Portfolios.CounterpartyPortfolioExternalId) 
        //        errors.Add("Values for property 'CounterpartyPortfolioExternalId' are not equal: " + originalDto.Portfolios.CounterpartyPortfolioExternalId + " != " + copiedDto.Portfolios.CounterpartyPortfolioExternalId);
            
        //    if (originalDto.Portfolios.CounterpartyPortfolioName != copiedDto.Portfolios.CounterpartyPortfolioName) 
        //        errors.Add("Values for property 'CounterpartyPortfolioName'  are not equal: " + originalDto.Portfolios.CounterpartyPortfolioName + " != " + copiedDto.Portfolios.CounterpartyPortfolioName);
            
        //    if (originalDto.ContractModelType != copiedDto.ContractModelType) 
        //        errors.Add("Values for property 'ContractModelType' are not equal: " + originalDto.ContractModelType + " != " + copiedDto.ContractModelType);
        //    //Console.WriteLine(originalDto.TransactionId);
        //    //Console.WriteLine(originalDto.DealType);
        //    //Console.WriteLine(originalDto.Portfolios.PortfolioName);

        //    ////InstrumentData
        //    if (originalDto.InstrumentData.ExecutionVenue != copiedDto.InstrumentData.ExecutionVenue) 
        //        errors.Add("Values for property 'ExecutionVenue' are not equal: " + originalDto.InstrumentData.ExecutionVenue + " != " + copiedDto.InstrumentData.ExecutionVenue);
            
        //    if (originalDto.InstrumentData.InstrumentName != copiedDto.InstrumentData.InstrumentName) 
        //        errors.Add("Values for property 'InstrumentName' are not equal: " + originalDto.InstrumentData.InstrumentName + " != " + copiedDto.InstrumentData.InstrumentName);
            
        //    if (originalDto.InstrumentData.PriceBasis != copiedDto.InstrumentData.PriceBasis)
        //        errors.Add("Values for property 'PriceBasis'  are not equal: " + originalDto.InstrumentData.PriceBasis + " != " + copiedDto.InstrumentData.PriceBasis);
            
        //    if (originalDto.InstrumentData.PriceBasisToArea != copiedDto.InstrumentData.PriceBasisToArea)
        //        errors.Add("Values for property 'PriceBasisToArea'  are not equal: " + originalDto.InstrumentData.PriceBasisToArea + " != " + copiedDto.InstrumentData.PriceBasisToArea);
            
        //    if (originalDto.InstrumentData.PriceType != copiedDto.InstrumentData.PriceType)
        //        errors.Add("Values for property 'PriceType'  are not equal: " + originalDto.InstrumentData.PriceType + " != " + copiedDto.InstrumentData.PriceType);
            
        //    if (originalDto.InstrumentData.FromDate != copiedDto.InstrumentData.FromDate)
        //        errors.Add("Values for property 'FromDate' are not equal: " + originalDto.InstrumentData.FromDate + " != " + copiedDto.InstrumentData.FromDate);
            
        //    if (originalDto.InstrumentData.ToDate != copiedDto.InstrumentData.ToDate)
        //        errors.Add("Values for property 'ToDate' are not equal: " + originalDto.InstrumentData.ToDate + " != " + copiedDto.InstrumentData.ToDate);
            
        //    if (originalDto.InstrumentData.LoadProfile != copiedDto.InstrumentData.LoadProfile)
        //        errors.Add("Values for property 'LoadProfile' are not equal: " + originalDto.InstrumentData.LoadProfile + " != " + copiedDto.InstrumentData.LoadProfile);
            
        //    if (originalDto.InstrumentData.TimeZone != copiedDto.InstrumentData.TimeZone)
        //        errors.Add("Values for property 'TimeZone' are not equal: " + originalDto.InstrumentData.TimeZone + " != " + copiedDto.InstrumentData.TimeZone);
            
        //    if (originalDto.InstrumentData.PutCall != copiedDto.InstrumentData.PutCall)
        //        errors.Add("Values for property 'PutCall' are not equal: " + originalDto.InstrumentData.PutCall + " != " + copiedDto.InstrumentData.PutCall);
            
        //    if (originalDto.InstrumentData.BalanceArea != copiedDto.InstrumentData.BalanceArea)
        //        errors.Add("Values for property 'BalanceArea' are not equal: " + originalDto.InstrumentData.BalanceArea + " != " + copiedDto.InstrumentData.BalanceArea);
            
        //    if (originalDto.InstrumentData.Strike != copiedDto.InstrumentData.Strike)
        //        errors.Add("Values for property 'Strike' are not equal: " + originalDto.InstrumentData.Strike + " != " + copiedDto.InstrumentData.Strike);
            
        //    if (originalDto.InstrumentData.ExpiryTime != copiedDto.InstrumentData.ExpiryTime) 
        //        errors.Add("Values for property 'ExpiryTime' are not equal: " + originalDto.InstrumentData.ExpiryTime + " != " + copiedDto.InstrumentData.ExpiryTime);
            
        //    if (originalDto.InstrumentData.ExpiryDate != copiedDto.InstrumentData.ExpiryDate)
        //        errors.Add("Values for property 'ExpiryDate' are not equal: " + originalDto.InstrumentData.ExpiryDate + " != " + copiedDto.InstrumentData.ExpiryDate);
            
        //    if (originalDto.InstrumentData.IndexFormula != copiedDto.InstrumentData.IndexFormula)
        //        errors.Add("Values for property 'IndexFormula' are not equal: " + originalDto.InstrumentData.IndexFormula + " != " + copiedDto.InstrumentData.IndexFormula);
            
        //    if (originalDto.InstrumentData.BaseIsoCurrency != copiedDto.InstrumentData.BaseIsoCurrency)
        //        errors.Add("Values for property 'BaseIsoCurrency' are not equal: " + originalDto.InstrumentData.BaseIsoCurrency + " != " + copiedDto.InstrumentData.BaseIsoCurrency);
            
        //    if (originalDto.InstrumentData.CrossIsoCurrency != copiedDto.InstrumentData.CrossIsoCurrency)
        //        errors.Add("Values for property 'CrossIsoCurrency' are not equal: " + originalDto.InstrumentData.CrossIsoCurrency + " != " + copiedDto.InstrumentData.CrossIsoCurrency);
            
        //    if (originalDto.InstrumentData.TransferDate != copiedDto.InstrumentData.TransferDate)
        //        errors.Add("Values for property 'TransferDate' are not equal: " + originalDto.InstrumentData.TransferDate + " != " + copiedDto.InstrumentData.TransferDate);
            
        //    if (originalDto.InstrumentData.CertificateType != copiedDto.InstrumentData.CertificateType)
        //        errors.Add("Values for property 'CertificateType' are not equal: " + originalDto.InstrumentData.CertificateType + " != " + copiedDto.InstrumentData.CertificateType);
            
        //    if (originalDto.InstrumentData.ProductionFacility != copiedDto.InstrumentData.ProductionFacility)
        //        errors.Add("Values for property 'ProductionFacility' are not equal: " + originalDto.InstrumentData.ProductionFacility + " != " + copiedDto.InstrumentData.ProductionFacility);
            
        //    if (originalDto.InstrumentData.EnvironmentLabel != copiedDto.InstrumentData.EnvironmentLabel)
        //        errors.Add("Values for property 'EnvironmentLabel' are not equal: " + originalDto.InstrumentData.EnvironmentLabel + " != " + copiedDto.InstrumentData.EnvironmentLabel);
            
        //    if (originalDto.InstrumentData.FromCountry != copiedDto.InstrumentData.FromCountry)
        //        errors.Add("Values for property 'FromCountry' are not equal: " + originalDto.InstrumentData.FromCountry + " != " + copiedDto.InstrumentData.FromCountry);
            
        //    if (originalDto.InstrumentData.ToCountry != copiedDto.InstrumentData.ToCountry)
        //        errors.Add("Values for property 'ToCountry' are not equal: " + originalDto.InstrumentData.ToCountry + " != " + copiedDto.InstrumentData.ToCountry);
            
        //    if (originalDto.InstrumentData.SamplingPeriod != copiedDto.InstrumentData.SamplingPeriod)
        //        errors.Add("Values for property 'SamplingPeriod' are not equal: " + originalDto.InstrumentData.SamplingPeriod + " != " + copiedDto.InstrumentData.SamplingPeriod);
            
        //    if (originalDto.InstrumentData.Interconnector != copiedDto.InstrumentData.Interconnector)
        //        errors.Add("Values for property 'Interconnector' are not equal: " + originalDto.InstrumentData.Interconnector + " != " + copiedDto.InstrumentData.Interconnector);
            
        //    if (originalDto.InstrumentData.ExpiryOffset != copiedDto.InstrumentData.ExpiryOffset)
        //        errors.Add("Values for property 'ExpiryOffset' are not equal: " + originalDto.InstrumentData.ExpiryOffset + " != " + copiedDto.InstrumentData.ExpiryOffset);
            
        //    if (originalDto.InstrumentData.Priority != copiedDto.InstrumentData.Priority)
        //        errors.Add("Values for property 'Priority' are not equal: " + originalDto.InstrumentData.Priority + " != " + copiedDto.InstrumentData.Priority);
            
        //    if (originalDto.InstrumentData.MinVol != copiedDto.InstrumentData.MinVol)
        //        errors.Add("Values for property 'MinVol' are not equal:" + originalDto.InstrumentData.MinVol + " != " + copiedDto.InstrumentData.MinVol);
            
        //    if (originalDto.InstrumentData.MaxVol != copiedDto.InstrumentData.MaxVol)
        //        errors.Add("Values for property 'MaxVol' are not equal: " + originalDto.InstrumentData.MaxVol + " != " + copiedDto.InstrumentData.MaxVol);
            
        //    if (originalDto.InstrumentData.CapacityId != copiedDto.InstrumentData.CapacityId)
        //        errors.Add("Values for property 'CapacityId' are not equal: " + originalDto.InstrumentData.CapacityId + " != " + copiedDto.InstrumentData.CapacityId);

        //    ////SettlementData
        //    if (originalDto.SettlementData.Quantity != copiedDto.SettlementData.Quantity)
        //        errors.Add("Values for property 'Quantity' are not equal:" + originalDto.SettlementData.Quantity + " != " + copiedDto.SettlementData.Quantity);
            
        //    if (originalDto.SettlementData.QuantityUnit != copiedDto.SettlementData.QuantityUnit)
        //        errors.Add("Values for property 'QuantityUnit are not equal: " + originalDto.SettlementData.QuantityUnit + " != " + copiedDto.SettlementData.QuantityUnit);
            
        //    if (originalDto.SettlementData.Price != copiedDto.SettlementData.Price)
        //        errors.Add("Values for property 'Price' are not equal: " + originalDto.SettlementData.Price + " != " + copiedDto.SettlementData.Price);
            
        //    if (originalDto.SettlementData.PriceIsoCurrency != copiedDto.SettlementData.PriceIsoCurrency)
        //        errors.Add("Values for property 'PriceIsoCurrency' are not equal: " + originalDto.SettlementData.PriceIsoCurrency + " != " + copiedDto.SettlementData.PriceIsoCurrency);
            
        //    if (originalDto.SettlementData.PriceVolumeUnit != copiedDto.SettlementData.PriceVolumeUnit)
        //        errors.Add("Values for property 'PriceVolumeUnit' are not equal: " + originalDto.SettlementData.PriceVolumeUnit + " != " + copiedDto.SettlementData.PriceVolumeUnit);
            
        //    if (originalDto.SettlementData.CurrencySource != copiedDto.SettlementData.CurrencySource)
        //        errors.Add("Values for property 'CurrencySource' are not equal: " + originalDto.SettlementData.CurrencySource + " != " + copiedDto.SettlementData.CurrencySource);
            
        //    if (originalDto.SettlementData.MarketPriceMultiplicator != copiedDto.SettlementData.MarketPriceMultiplicator)
        //        errors.Add("Values for property 'MarketPriceMultiplicator' are not equal: " + originalDto.SettlementData.MarketPriceMultiplicator + " != " + copiedDto.SettlementData.MarketPriceMultiplicator);
            
        //    if (originalDto.SettlementData.MasterAgreement != copiedDto.SettlementData.MasterAgreement)
        //        errors.Add("Values for property 'MasterAgreement' are not equal: " + originalDto.SettlementData.MasterAgreement + " != " + copiedDto.SettlementData.MasterAgreement);
            
        //    if (originalDto.SettlementData.SettlementRule != copiedDto.SettlementData.SettlementRule)
        //        errors.Add("Values for property 'SettlementRule' are not equal: " + originalDto.SettlementData.SettlementRule + " != " + copiedDto.SettlementData.SettlementRule);
            
        //    if (originalDto.SettlementData.Resolution != copiedDto.SettlementData.Resolution)
        //        errors.Add("Values for property 'Resolution' are not equal: " + originalDto.SettlementData.Resolution + " != " + copiedDto.SettlementData.Resolution);
            
        //    if (originalDto.SettlementData.NominationsHourly != copiedDto.SettlementData.NominationsHourly)
        //        errors.Add("Values for property 'NominationsHourly' are not equal: " + originalDto.SettlementData.NominationsHourly + " != " + copiedDto.SettlementData.NominationsHourly);

        //    if (originalDto.SettlementData.TimeSeriesSet != null)
        //    {
        //        for (int i = 0; i < originalDto.SettlementData.TimeSeriesSet.TimeSeries.Count(); i++)
        //        {
        //            if (originalDto.SettlementData.TimeSeriesSet.TimeSeries[i].Resolution != 
        //                copiedDto.SettlementData.TimeSeriesSet.TimeSeries[i].Resolution)
        //                errors.Add("Values for property 'TimeSeriesSet.TimeSeries.Resolution' are not equal at index = " + i + ": "+
        //                    originalDto.SettlementData.TimeSeriesSet.TimeSeries[i].Resolution + " != " + 
        //                    copiedDto.SettlementData.TimeSeriesSet.TimeSeries[i].Resolution);

        //            if (originalDto.SettlementData.TimeSeriesSet.TimeSeries[i].TimeSeriesTypeName != 
        //                copiedDto.SettlementData.TimeSeriesSet.TimeSeries[i].TimeSeriesTypeName)
        //                errors.Add("Values for property 'TimeSeriesSet.TimeSeries.TimeSeriesTypeName' are not equal at index = " + i + ": " + 
        //                    originalDto.SettlementData.TimeSeriesSet.TimeSeries[i].TimeSeriesTypeName + " != "+ 
        //                    copiedDto.SettlementData.TimeSeriesSet.TimeSeries[i].TimeSeriesTypeName);

        //            if (originalDto.SettlementData.TimeSeriesSet.TimeSeries[i].TimezoneName !=
        //                copiedDto.SettlementData.TimeSeriesSet.TimeSeries[i].TimezoneName)
        //                errors.Add( "Values for property 'TimeSeriesSet.TimeSeries.TimezoneName' are not equal at index = " + i + ": " +
        //                    originalDto.SettlementData.TimeSeriesSet.TimeSeries[i].TimezoneName + " != " + 
        //                    copiedDto.SettlementData.TimeSeriesSet.TimeSeries[i].TimezoneName);

        //            QaLookUp.TimeSeriesValue[] expectedTSV = originalDto.SettlementData.TimeSeriesSet.TimeSeries[i].TimeSeriesValues;
        //            QaLookUp.TimeSeriesValue[] actualTSV = copiedDto.SettlementData.TimeSeriesSet.TimeSeries[i].TimeSeriesValues;

        //            for (int j = 0; j < expectedTSV.Length; j++)
        //            {
        //                if (expectedTSV[j].FromDateTime != actualTSV[j].FromDateTime)
        //                    errors.Add("Values for property 'FromDateTime' for '" + originalDto.SettlementData.TimeSeriesSet.TimeSeries[i].TimeSeriesTypeName + "' are not equal at index = " + j + ": "+
        //                      expectedTSV[j].FromDateTime + " != "+ actualTSV[j].FromDateTime);
                        
        //                if (expectedTSV[j].ToDateTime != actualTSV[j].ToDateTime)
        //                    errors.Add("Values for property 'ToDateTime' for '" + originalDto.SettlementData.TimeSeriesSet.TimeSeries[i].TimeSeriesTypeName + "' are not equal at index = " + j + ": " +
        //                        expectedTSV[j].ToDateTime + " != "+ actualTSV[j].ToDateTime);
                        
        //                if (!Equals(expectedTSV[j].FeeValue, actualTSV[j].FeeValue))
        //                    errors.Add("Values for property 'FeeValue' for '" + originalDto.SettlementData.TimeSeriesSet.TimeSeries[i].TimeSeriesTypeName + "' are not equal at index = " + j + ": " +
        //                        expectedTSV[j].FeeValue +" != " +  actualTSV[j].FeeValue);
        //            }
        //        }
        //    }
        //    // add PriceVolumeTimeSeries
        //    if (originalDto.SettlementData.PriceVolumeTimeSeriesDetails != null)
        //    {
        //        if (originalDto.SettlementData.PriceVolumeTimeSeriesDetails.Count() != copiedDto.SettlementData.PriceVolumeTimeSeriesDetails.Count())
        //            errors.Add("Values for property 'PriceVolumeTimeSeriesDetails.Count()' are not equal:");
        //        for (int i = 0; i < originalDto.SettlementData.PriceVolumeTimeSeriesDetails.Count(); i++)
        //        {
        //            if (originalDto.SettlementData.PriceVolumeTimeSeriesDetails[i].FromDateTime != copiedDto.SettlementData.PriceVolumeTimeSeriesDetails[i].FromDateTime)
        //                errors.Add("'PriceVolumeTimeSeriesDetails[" + i + "].FromDate' properties are not equal:" +
        //                    originalDto.SettlementData.PriceVolumeTimeSeriesDetails[i].FromDateTime + " != " + copiedDto.SettlementData.PriceVolumeTimeSeriesDetails[i].FromDateTime);
        //            if (originalDto.SettlementData.PriceVolumeTimeSeriesDetails[i].ToDateTime != copiedDto.SettlementData.PriceVolumeTimeSeriesDetails[i].ToDateTime)
        //                errors.Add("'PriceVolumeTimeSeriesDetails[" + i + "].ToDate' properties are not equal:" +
        //                    originalDto.SettlementData.PriceVolumeTimeSeriesDetails[i].ToDateTime+ " != " + copiedDto.SettlementData.PriceVolumeTimeSeriesDetails[i].ToDateTime);
        //            if (Math.Abs(originalDto.SettlementData.PriceVolumeTimeSeriesDetails[i].Price - copiedDto.SettlementData.PriceVolumeTimeSeriesDetails[i].Price) > TOLERANCE)
        //                errors.Add("'PriceVolumeTimeSeriesDetails[" + i + "].Price' properties are not equal:" +
        //                  originalDto.SettlementData.PriceVolumeTimeSeriesDetails[i].Price + " != " + copiedDto.SettlementData.PriceVolumeTimeSeriesDetails[i].Price);
        //            if (Math.Abs(originalDto.SettlementData.PriceVolumeTimeSeriesDetails[i].Volume - copiedDto.SettlementData.PriceVolumeTimeSeriesDetails[i].Volume) > TOLERANCE)
        //                errors.Add("'PriceVolumeTimeSeriesDetails[" + i + "].Volume' properties are not equal:" +
        //                    originalDto.SettlementData.PriceVolumeTimeSeriesDetails[i].Volume + " != " + copiedDto.SettlementData.PriceVolumeTimeSeriesDetails[i].Volume);
        //        }
        //    }

        //    ////DealDetails
        //    if (originalDto.DealDetails.Trader != copiedDto.DealDetails.Trader)
        //        errors.Add("Values for property 'Trader' are not equal: " + originalDto.DealDetails.Trader + " != " + copiedDto.DealDetails.Trader);
           
        //    if (originalDto.DealDetails.Status != copiedDto.DealDetails.Status)
        //        errors.Add("Values for property 'Status' are not equal: " + originalDto.DealDetails.Status+ " != " + copiedDto.DealDetails.Status);
           
        //    if (originalDto.DealDetails.TradeDateTimeUtc != copiedDto.DealDetails.TradeDateTimeUtc)
        //        errors.Add("Values for property 'TradeDateTimeUtc' are not equal: " + originalDto.DealDetails.TradeDateTimeUtc + " != " + copiedDto.DealDetails.TradeDateTimeUtc);
        //    ////FeesData
        //    if (originalDto.FeesData.Broker != copiedDto.FeesData.Broker)
        //        errors.Add("Values for property 'Broker' are not equal: " + originalDto.FeesData.Broker + " != " + copiedDto.FeesData.Broker);
           
        //    if (originalDto.FeesData.ClearingType != copiedDto.FeesData.ClearingType)
        //        errors.Add("Values for property 'ClearingType' are not equal: " + originalDto.FeesData.ClearingType + " != " + copiedDto.FeesData.ClearingType);

        //    if ((originalDto.FeesData.Fees != null) && (copiedDto.FeesData.Fees == null)) errors.Add("Values for property 'Fees' are not equal: " + originalDto.FeesData.Fees + " != null");
        //    if ((originalDto.FeesData.Fees == null) && (copiedDto.FeesData.Fees != null)) errors.Add("Values for property 'Fees' are not equal: null != " + copiedDto.FeesData.Fees);

        //    if ((originalDto.FeesData.Fees != null) && (copiedDto.FeesData.Fees != null))
        //    {
        //        if (originalDto.FeesData.Fees.Count() != copiedDto.FeesData.Fees.Count())
        //            errors.Add("Values for property 'Fees.Count' are not equal: " + originalDto.FeesData.Fees.Count() +
        //                       " != " + copiedDto.FeesData.Fees.Count());
        //        else
        //        {
        //            List<Fee> fees = originalDto.FeesData.Fees.ToList();
        //            IList<Fee> copiedfees = copiedDto.FeesData.Fees.ToList();

        //            List<Fee> FeesSortedList = fees.OrderBy(o => o.ValueType).ToList();
        //            List<Fee> CopiedFeesSortedList = copiedfees.OrderBy(o => o.ValueType).ToList();
        //            originalDto.FeesData.Fees = FeesSortedList.ToArray();
        //            copiedDto.FeesData.Fees = CopiedFeesSortedList.ToArray();

        //            for (int i = 0; i < originalDto.FeesData.Fees.Count(); i++)
        //            {
        //                if (originalDto.FeesData.Fees[i].ValueType != copiedDto.FeesData.Fees[i].ValueType)
        //                    errors.Add("'Fees." + originalDto.FeesData.Fees[i].ValueType + ".ValueType' properties at index= [" + i + "] are not equal: " +
        //                        originalDto.FeesData.Fees[i].ValueType + " != " + copiedDto.FeesData.Fees[i].ValueType);

        //                if (Math.Abs(originalDto.FeesData.Fees[i].FeeValue - copiedDto.FeesData.Fees[i].FeeValue) > TOLERANCE)
        //                    errors.Add("'Fees." + originalDto.FeesData.Fees[i].ValueType + ".FeeValue' properties at index= [" + i + "] are not equal: " +
        //                        originalDto.FeesData.Fees[i].FeeValue + " != " + copiedDto.FeesData.Fees[i].FeeValue);

        //                if (originalDto.FeesData.Fees[i].FeeUnit != copiedDto.FeesData.Fees[i].FeeUnit)
        //                    errors.Add("'Fees." + originalDto.FeesData.Fees[i].ValueType + ".FeeUnit' properties at index= [" + i + "] are not equal: " +
        //                        originalDto.FeesData.Fees[i].FeeUnit + " != " + copiedDto.FeesData.Fees[i].FeeUnit);
        //            }

        //           // IList<Fee> fees = originalDto.FeesData.Fees.ToList();
        //           // IList<Fee> copiedfees = copiedDto.FeesData.Fees.ToList();

        //           // List<Fee> FeesSortedList = fees.OrderBy(o => o.ValueType).ToList();
        //           // List<Fee> CopiedFeesSortedList = copiedfees.OrderBy(o => o.ValueType).ToList();
        //           //// FeesSortedList.SequenceEqual(CopiedFeesSortedList);

        //           // // origTransaction.FeesData.Fees = FeesSortedList.ToArray();
        //           // foreach (var fee in FeesSortedList)
        //           // {
        //           //     fee.ValueType = CopiedFeesSortedList.
        //           // }

        //        }
        //    }

        //    //ReferenceData
        //    if (originalDto.ReferenceData.ExternalId != copiedDto.ReferenceData.ExternalId)
        //        errors.Add("Values for property 'ExternalId' are not equal:");
        //    if (originalDto.ReferenceData.ExternalSource != copiedDto.ReferenceData.ExternalSource)
        //        errors.Add("Values for property 'ExternalSource' are not equal:");
        //    if (originalDto.ReferenceData.TicketNumber != copiedDto.ReferenceData.TicketNumber)
        //        errors.Add("Values for property 'TicketNumber' are not equal:");

        //    if ((originalDto.ReferenceData.DealGroups != null) && (copiedDto.ReferenceData.DealGroups == null)) errors.Add("Values for property 'Fees' are not equal: " + originalDto.ReferenceData.DealGroups + " != null");
        //    if ((originalDto.ReferenceData.DealGroups == null) && (copiedDto.ReferenceData.DealGroups != null)) errors.Add("Values for property 'Fees' are not equal: null != " + copiedDto.ReferenceData.DealGroups);
            
        //    if ((originalDto.ReferenceData.DealGroups != null) && (copiedDto.ReferenceData.DealGroups != null))
        //    {
        //        if (originalDto.ReferenceData.DealGroups.Count() != copiedDto.ReferenceData.DealGroups.Count())
        //            errors.Add("Values for property 'DealGroups' are not equal:");

        //        for (int i = 0; i < originalDto.ReferenceData.DealGroups.Count(); i++)
        //        {
        //            if (originalDto.ReferenceData.DealGroups[i].Name != copiedDto.ReferenceData.DealGroups[i].Name)
        //                errors.Add("'DealGroups." + originalDto.ReferenceData.DealGroups[i].Name + ".Name' properties at index= [" + i + "] are not equal: " +
        //                 originalDto.ReferenceData.DealGroups[i].Name + " != " + copiedDto.ReferenceData.DealGroups[i].Name);
                    
        //            if (originalDto.ReferenceData.DealGroups[i].FeeValue != copiedDto.ReferenceData.DealGroups[i].FeeValue)
        //                errors.Add("'DealGroups." + originalDto.ReferenceData.DealGroups[i].Name + ".FeeValue' properties at index= [" + i + "] are not equal: " +
        //                    originalDto.ReferenceData.DealGroups[i].FeeValue +" != " + copiedDto.ReferenceData.DealGroups[i].FeeValue);
        //        }
        //    }
        //    else 
        //        if (originalDto.ReferenceData.DealGroups != copiedDto.ReferenceData.DealGroups)
        //            errors.Add("Values for property 'DealGroups' are not equal: " + originalDto.ReferenceData.DealGroups +" != "+ copiedDto.ReferenceData.DealGroups);

        //    if (originalDto.ReferenceData.Comment != copiedDto.ReferenceData.Comment)
        //        errors.Add("Values for property 'Comment' are not equal: " + originalDto.ReferenceData.Comment + " != " + copiedDto.ReferenceData.Comment);
            
        //    if (originalDto.ReferenceData.QuotaRegion != copiedDto.ReferenceData.QuotaRegion)
        //        errors.Add("Values for property 'QuotaRegion' are not equal: " + originalDto.ReferenceData.QuotaRegion+ " != " + copiedDto.ReferenceData.QuotaRegion);
            
        //    if (Math.Abs(originalDto.ReferenceData.RiskValue - copiedDto.ReferenceData.RiskValue) > TOLERANCE)
        //        errors.Add("Values for property 'RiskValue' are not equal: " + originalDto.ReferenceData.RiskValue + " != " + copiedDto.ReferenceData.RiskValue);
            
        //    if (originalDto.ReferenceData.Originator != copiedDto.ReferenceData.Originator)
        //        errors.Add("Values for property 'Originator' are not equal: " + originalDto.ReferenceData.Originator + " != " + copiedDto.ReferenceData.Originator);
            
        //    if (originalDto.ReferenceData.Deliveries != copiedDto.ReferenceData.Deliveries)
        //        errors.Add("Values for property 'Deliveries' are not equal: " + originalDto.ReferenceData.Deliveries+ " != "+copiedDto.ReferenceData.Deliveries);
            
        //    if (originalDto.ReferenceData.DeclareId != copiedDto.ReferenceData.DeclareId)
        //        errors.Add("Values for property 'DeclareId' are not equal: " + 
        //            originalDto.ReferenceData.DeclareId + " != " + copiedDto.ReferenceData.DeclareId);
            
        //    if (originalDto.ReferenceData.ContractSplitId != copiedDto.ReferenceData.ContractSplitId)
        //        errors.Add("Values for property 'ContractSplitId' are not equal: " +
        //            originalDto.ReferenceData.ContractSplitId+ " != " + copiedDto.ReferenceData.ContractSplitId);

        //    if (originalDto.ReferenceData.DistributionParentTransactionId != copiedDto.ReferenceData.DistributionParentTransactionId)
        //        errors.Add("Values for property 'DistributionParentTransactionId' are not equal: " +
        //            originalDto.ReferenceData.DistributionParentTransactionId + " != " + copiedDto.ReferenceData.DistributionParentTransactionId);
            
        //    if (originalDto.ReferenceData.DistributedQuantity != copiedDto.ReferenceData.DistributedQuantity)
        //        errors.Add("Values for property 'DistributedQuantity' are not equal: " +
        //            originalDto.ReferenceData.DistributedQuantity+ " != " + copiedDto.ReferenceData.DistributedQuantity);
            
        //    if (Math.Abs(originalDto.ReferenceData.OriginalQuantity - copiedDto.ReferenceData.OriginalQuantity) > TOLERANCE)
        //       errors.Add("Values for property 'OriginalQuantity' are not equal: " +
        //           originalDto.ReferenceData.OriginalQuantity + " != "+ copiedDto.ReferenceData.OriginalQuantity);
            
        //    if (originalDto.ReferenceData.CascadingOriginIds != copiedDto.ReferenceData.CascadingOriginIds)
        //        errors.Add("Values for property 'CascadingOriginIds' are not equal: " +
        //            originalDto.ReferenceData.CascadingOriginIds +" != " + copiedDto.ReferenceData.CascadingOriginIds);

        //    //TransactionWorkFlowDetails
        //    if (originalDto.TransactionWorkFlowDetails.Authorised != copiedDto.TransactionWorkFlowDetails.Authorised)
        //        errors.Add("Values for property 'Authorised' are not equal: " +
        //            originalDto.TransactionWorkFlowDetails.Authorised + " != " + copiedDto.TransactionWorkFlowDetails.Authorised);
            
        //    if (originalDto.TransactionWorkFlowDetails.Audited != copiedDto.TransactionWorkFlowDetails.Audited)
        //        errors.Add("Values for property 'Audited' are not equal: " +
        //             originalDto.TransactionWorkFlowDetails.Audited + " != " + copiedDto.TransactionWorkFlowDetails.Audited);
            
        //    if (originalDto.TransactionWorkFlowDetails.Paid != copiedDto.TransactionWorkFlowDetails.Paid)
        //        errors.Add("Values for property 'Paid' are not equal: " +
        //            originalDto.TransactionWorkFlowDetails.Paid + " != " + copiedDto.TransactionWorkFlowDetails.Paid);
            
        //    if (originalDto.TransactionWorkFlowDetails.ConfirmedByBroker != copiedDto.TransactionWorkFlowDetails.ConfirmedByBroker)
        //        errors.Add("Values for property 'ConfirmedByBroker' are not equal: " +
        //            originalDto.TransactionWorkFlowDetails.ConfirmedByBroker + " != " + copiedDto.TransactionWorkFlowDetails.ConfirmedByBroker);
            
        //    if (originalDto.TransactionWorkFlowDetails.ConfirmedByCounterparty != copiedDto.TransactionWorkFlowDetails.ConfirmedByCounterparty)
        //        errors.Add("Values for property 'ConfirmedByCounterparty' are not equal: " +
        //            originalDto.TransactionWorkFlowDetails.ConfirmedByCounterparty + " != " + copiedDto.TransactionWorkFlowDetails.ConfirmedByCounterparty);

        //   // Assert.That(copiedDto.TransactionWorkFlowDetails.TimeStampClearedUtc, Is.EqualTo(originalDto.TransactionWorkFlowDetails.TimeStampClearedUtc).Within(1).Seconds, "Values for property 'TimeStampClearedUTC' are not equal:");
        //   // Assert.That(copiedDto.TransactionWorkFlowDetails.TimeStampConfirmationBrokerUtc, Is.EqualTo(originalDto.TransactionWorkFlowDetails.TimeStampConfirmationBrokerUtc).Within(1).Seconds, "Values for property 'TimeStampConfirmationBrokerUTC' are not equal: ");
        //   // Assert.That(copiedDto.TransactionWorkFlowDetails.TimeStampConfirmationCounterPartyUtc, Is.EqualTo(originalDto.TransactionWorkFlowDetails.TimeStampConfirmationCounterPartyUtc).Within(1).Seconds, "Values for property 'TimeStampConfirmationCounterPartyUTC' are not equal:");

        //    //if (originalDto.TransactionWorkFlowDetails.TimeStampCounterpartyAuthorised, copiedDto.TransactionWorkFlowDetails.TimeStampCounterpartyAuthorised, "Values for property 'TimeStampCounterpartyAuthorised' are not equal:");
        //    if ((originalDto.PropertyGroups != null) && (copiedDto.PropertyGroups == null)) errors.Add("Values for property 'Fees' are not equal: " + originalDto.PropertyGroups + " != null");
        //    if ((originalDto.PropertyGroups == null) && (copiedDto.PropertyGroups != null)) errors.Add("Values for property 'Fees' are not equal: null != " + copiedDto.PropertyGroups);
            
        //    if ((originalDto.PropertyGroups != null)&& (copiedDto.PropertyGroups != null))
        //    {
        //        if (originalDto.PropertyGroups.Length == copiedDto.PropertyGroups.Length)
        //        {
        //            for (int i = 0; i < originalDto.PropertyGroups.Length; i++)
        //            {
        //                if (originalDto.PropertyGroups[i].Name != copiedDto.PropertyGroups[i].Name)
        //                    errors.Add("PropertyGroup Name was not equal: " + originalDto.PropertyGroups[i].Name + " != " + copiedDto.PropertyGroups[i].Name);

        //                if (originalDto.PropertyGroups[i].Properties.Length != copiedDto.PropertyGroups[i].Properties.Length)
        //                    errors.Add("Different number of Properites in : " + originalDto.PropertyGroups[i].Name +
        //                " (Check subproperties: EMIR status, DealCompression, UTI or others)");

        //                if (originalDto.PropertyGroups[i].Properties != null)
        //                {
        //                    for (int j = 0; j < originalDto.PropertyGroups[i].Properties.Length; j++)
        //                    {
        //                        if (originalDto.PropertyGroups[i].Properties[j].Name != copiedDto.PropertyGroups[i].Properties[j].Name)
        //                        errors.Add("Property Name is not equal in PropertyGroup" + originalDto.PropertyGroups[i].Name);
        //                        if (originalDto.PropertyGroups[i].Properties[j].FeeValue != copiedDto.PropertyGroups[i].Properties[j].FeeValue)
        //                        errors.Add("Property FeeValue is not equal in PropertyGroup" + originalDto.PropertyGroups[i].Name);
        //                        if (originalDto.PropertyGroups[i].Properties[j].ValueType != copiedDto.PropertyGroups[i].Properties[j].ValueType)
        //                        errors.Add("Property ValueType is not equal in PropertyGroup" + originalDto.PropertyGroups[i].Name);
        //                    }
        //                }
        //            }
        //        }
        //        else
        //            errors.Add("Values for property 'PropertyGroups.Length' are not equal: " +
        //                       originalDto.PropertyGroups.Length + " != " + copiedDto.PropertyGroups.Length);

        //    }

        //    //print out alll errors    
        //    //foreach (var item in errors)
        //    //{
        //    //    Console.WriteLine(item);
        //    //}
        //    return errors;
        //}

        
    }
}
