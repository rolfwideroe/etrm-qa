using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using ElvizTestUtils.DatabaseTools;
using ElvizTestUtils.QaLookUp;
using NUnit.Framework;

namespace ElvizTestUtils.QaLookUp
{
    public class QAReportingDBLookUp
    {
        private const string DatabaseUsername = "qainstall";
        private const string DatabasePassword = "qainstall";

   //    [Test]
        public static QaTransactionDTO GetTransactionDTOFromReportingDB(int transactionID)
        {
          //  int transactionID = 2055;

            string sqlCommand = string.Format(@"select *                                         
                                        from ContractExportsView c
                                        left join ContractExportsCustomFields f 
                                        on f.ContractExportId=c.ContractExportId 
                                        where ContractId = {0}
                                        and IsOriginator = 1
                                        ", transactionID);

            DataTable result = QaDao.DataTableFromSql("ReportingDatabase", sqlCommand);

            if (result.Rows.Count == 0) throw new Exception("Query did not return any transaction.");
            if (result.Rows.Count > 1) throw new Exception("Query returned more than one transaction.");

            DataRow row = result.Rows[0];

            QaTransactionDTO dto = new QaTransactionDTO();

            dto.TransactionId = DataRowTool.GetColumnIntValue(row, "contractId");
            string commodity = DataRowTool.GetColumnStringValue(row, "Commodity");
            string instrumentType = DataRowTool.GetColumnStringValue(row, "InstrumentType");
            instrumentType = instrumentType.Replace(" ", "");
            dto.DealType = commodity + "-" + instrumentType;

            string paidRec = DataRowTool.GetColumnStringValue(row, "CashFlowPaidReceived");
            if (!string.IsNullOrEmpty(paidRec))
            {
                if (paidRec == "Paid") dto.PaidReceived = QaPaidReceived.Paid;
                else if (paidRec == "Received") dto.PaidReceived = QaPaidReceived.Received;
                else throw new ArgumentException("Paid/Received has incorrect value");
            }
            else
            {


                string buySell = DataRowTool.GetColumnStringValue(row, "BuySell");
                if (!string.IsNullOrEmpty(buySell))
                {
                    if (buySell == "Buy") dto.BuySell = QaBuySell.Buy;
                    else if (buySell == "Sell") dto.BuySell = QaBuySell.Sell;
                    else throw new ArgumentException("Buy/Sell has incorrect value");
                }
            }
            string settlementtype = DataRowTool.GetColumnStringValue(row, "SettlementType");
            if (!string.IsNullOrEmpty(settlementtype))
            {
                if (settlementtype == "Financial") dto.DeliveryType = QaDeliveryType.Financial;
                else if (settlementtype == "Physical") dto.DeliveryType = QaDeliveryType.Physical;
                else throw new ArgumentException("DeliveryType (SettlementType) has incorrect value");
            }

            dto.ContractModelType = DataRowTool.GetColumnStringValue(row, "ContractModelType");
            dto.DeliveryLocation = DataRowTool.GetColumnStringValue(row, "DeliveryLocation");

            dto.Portfolios = new Portfolios();
            dto.Portfolios.PortfolioName = DataRowTool.GetColumnStringValue(row, "PortfolioName");
            dto.Portfolios.PortfolioExternalId = DataRowTool.GetColumnStringValue(row, "PortfolioExternalId");
            dto.Portfolios.CounterpartyPortfolioName = DataRowTool.GetColumnStringValue(row, "CounterpartyPortfolioName");
            dto.Portfolios.CounterpartyPortfolioExternalId = DataRowTool.GetColumnStringValue(row, "CounterpartyPortfolioExternalId");

            dto.InstrumentData = new InstrumentData();
            string executionVenue = DataRowTool.GetColumnStringValue(row, "ExecutionVenue");
            //if (executionVenue == null) executionVenue = "None";
            dto.InstrumentData.ExecutionVenue = executionVenue;
            dto.InstrumentData.InstrumentName = DataRowTool.GetColumnStringValue(row, "InstrumentName");
            dto.InstrumentData.PriceBasis = DataRowTool.GetColumnStringValue(row, "PriceBasis");
            dto.InstrumentData.PriceBasisToArea = DataRowTool.GetColumnStringValue(row, "ToPriceBasis");
            dto.InstrumentData.FromDate = DataRowTool.GetColumnDateTimeValue(row, "FromDate");
            dto.InstrumentData.ToDate = DataRowTool.GetColumnDateTimeValue(row, "ToDate");

            QaPutCall putCall;

            if (QaPutCall.TryParse(DataRowTool.GetColumnStringValue(row, "PutCall"), false, out putCall))
                dto.InstrumentData.PutCall = putCall;

            dto.InstrumentData.Strike = DataRowTool.GetColumnDoubleValue(row, "Strike");
            dto.InstrumentData.ExpiryDate = DataRowTool.GetColumnDateTimeValue(row, "ExpiryDate");

            dto.InstrumentData.LoadProfile = DataRowTool.GetColumnStringValue(row, "LoadProfile");
            dto.InstrumentData.TimeZone = DataRowTool.GetColumnStringValue(row, "ContractTimeZone");
            dto.InstrumentData.IndexFormula = DataRowTool.GetColumnStringValue(row, "IndexName");
            dto.InstrumentData.BaseIsoCurrency = DataRowTool.GetColumnStringValue(row, "BaseCurrency");
            dto.InstrumentData.CrossIsoCurrency = DataRowTool.GetColumnStringValue(row, "CrossCurrency");
            dto.InstrumentData.TransferDate = DataRowTool.GetColumnDateTimeValue(row, "TransferDate");
            dto.InstrumentData.CertificateType = DataRowTool.GetColumnStringValue(row, "CertificateType");
            dto.InstrumentData.ProductionFacility = DataRowTool.GetColumnStringValue(row, "ProductionFacility");
            dto.InstrumentData.EnvironmentLabel = DataRowTool.GetColumnStringValue(row, "EnvironmentLabel");
            dto.InstrumentData.FromCountry = DataRowTool.GetColumnStringValue(row, "FromCountry");
            dto.InstrumentData.ToCountry = DataRowTool.GetColumnStringValue(row, "ToCountry");
            dto.InstrumentData.SamplingPeriod = DataRowTool.GetColumnStringValue(row, "SamplingPeriod");
            dto.InstrumentData.Interconnector = DataRowTool.GetColumnStringValue(row, "Interconnector");
            dto.InstrumentData.LossFactor = DataRowTool.GetColumnDoubleValue(row, "LossFactor");

            TimeSpan? span = DataRowTool.GetColumnTimeSpanValue(row, "StripExpiryTimeOfDay");
            if (span != null) dto.InstrumentData.ExpiryTime = span.ToString();
            dto.InstrumentData.ExpiryOffset = DataRowTool.GetColumnIntValue(row, "StripExpiryOffset");
            dto.InstrumentData.Priority = DataRowTool.GetColumnStringValue(row, "Priority");
            dto.InstrumentData.MinVol = DataRowTool.GetColumnDoubleValue(row, "MinVolume");
            dto.InstrumentData.MaxVol = DataRowTool.GetColumnDoubleValue(row, "MaxVolume");
            dto.InstrumentData.CapacityId = DataRowTool.GetColumnStringValue(row, "CapacityId");
            dto.InstrumentData.UnderlyingExecutionVenue = DataRowTool.GetColumnStringValue(row, "UnderlyingVenue");
            dto.InstrumentData.UnderlyingDeliveryType = DataRowTool.GetColumnStringValue(row, "UnderlyingSettlementType");
            dto.InstrumentData.UnderlyingInstrumentType = DataRowTool.GetColumnStringValue(row, "UnderlyingInstrumentType");
            dto.InstrumentData.SamplingFrom = DataRowTool.GetColumnDateTimeValue(row, "SamplingFromDate");
            dto.InstrumentData.SamplingTo = DataRowTool.GetColumnDateTimeValue(row, "SamplingToDate");

            int volumeReferenceId = Convert.ToInt32(DataRowTool.GetColumnIntValue(row, "QuantityDependencyReferenceContractId"));
            if (volumeReferenceId > 0)
                dto.InstrumentData.VolumeReferenceExternalId = GetExternalIdByContractId(volumeReferenceId);

            dto.InstrumentData.Interconnector = DataRowTool.GetColumnStringValue(row, "Interconnector");
            string priceType = DataRowTool.GetColumnStringValue(row, "ContractPriceType");
            if (priceType != null)
            {
                switch (priceType)
                {
                    case "Spot w/margin":
                        dto.InstrumentData.PriceType = "Spot";
                        break;
                    case "Average traded close w/margin":
                        dto.InstrumentData.PriceType = "AverageTradedCloseWithMargin";
                        break;
                    case "Fixed":
                        dto.InstrumentData.PriceType = "Fixed";
                        break;
                    case "Indexed w/margin":
                        dto.InstrumentData.PriceType = "Indexed";
                        break;
                    case "End User El-Certificate":
                        dto.InstrumentData.PriceType = "EndUserElCertificate";
                        break;
                    case "Cap/Floor":
                        dto.InstrumentData.PriceType = "CapFloor";
                        break;
                    default:
                        dto.InstrumentData.PriceType = priceType;
                        break;

                }
            }

            dto.InstrumentData.HistoricContractPrices = DataRowTool.GetColumnStringValue(row, "HistoricContractPriceSeries");
            dto.InstrumentData.ReferencePriceSeries = DataRowTool.GetColumnStringValue(row, "ReferencePriceSeries");
            dto.InstrumentData.DestinationReferencePriceSeries = DataRowTool.GetColumnStringValue(row, "DestinationReferencePriceSeries");
            dto.InstrumentData.CapFloorPricingResolution = DataRowTool.GetColumnStringValue(row, "CapFloorPricingResolution");
            dto.InstrumentData.AuctionType = DataRowTool.GetColumnStringValue(row, "AuctionType");
            dto.InstrumentData.BalanceArea = DataRowTool.GetColumnStringValue(row, "BalanceArea");

            dto.SettlementData = new SettlementData();
            dto.SettlementData.CapacityBidQuantity = DataRowTool.GetColumnDoubleValue(row, "CapacityBidVolume");
            dto.SettlementData.CapacityTradedQuantity = DataRowTool.GetColumnDoubleValue(row, "CapacityTradedVolume");
            dto.SettlementData.CapacityPrice = DataRowTool.GetColumnDoubleValue(row, "CapacityBidPrice");
            dto.SettlementData.Price = DataRowTool.GetColumnDoubleValue(row, "Price");
            dto.SettlementData.InitialPrice = DataRowTool.GetColumnDoubleValue(row, "InitialPrice");
            dto.SettlementData.Quantity = DataRowTool.GetColumnDoubleValue(row, "Quantity");
            dto.SettlementData.QuantityUnit = DataRowTool.GetColumnStringValue(row, "QuantityUnit");
            dto.SettlementData.PriceVolumeUnit = DataRowTool.GetColumnStringValue(row, "PriceVolumeUnit");
            dto.SettlementData.PriceIsoCurrency = DataRowTool.GetColumnStringValue(row, "Currency");
            dto.SettlementData.CurrencySource = DataRowTool.GetColumnStringValue(row, "CurrencySource");
            dto.SettlementData.MarketPriceMultiplicator = DataRowTool.GetColumnDoubleValue(row, "MarketPriceMultiplier");
            dto.SettlementData.MasterAgreement = DataRowTool.GetColumnStringValue(row, "MasterAgreement");
            dto.SettlementData.SettlementRule = DataRowTool.GetColumnStringValue(row, "SettlementRule");
            dto.SettlementData.Resolution = DataRowTool.GetColumnStringValue(row, "Resolution");

            dto.DealDetails = new DealDetails();
            dto.DealDetails.Trader = DataRowTool.GetColumnStringValue(row, "Trader");
            dto.DealDetails.CounterpartyTrader = DataRowTool.GetColumnStringValue(row, "CounterpartyTrader");
            dto.DealDetails.Status = DataRowTool.GetColumnStringValue(row, "Status").Replace(" ", "");
            dto.DealDetails.TradeDateTimeUtc = DataRowTool.GetColumnDateTimeValue(row, "TradeDateTimeUtc");
            dto.DealDetails.Trader = DataRowTool.GetColumnStringValue(row, "Trader");
            dto.FeesData = new FeesData();
            dto.FeesData.Broker = DataRowTool.GetColumnStringValue(row, "BrokerCompanyName");
            dto.FeesData.ClearingType = DataRowTool.GetColumnStringValue(row, "ClearingType");

            IList<string> feeTypes = new string[] {"Broker", "Clearing", "Commission", "Trading", "Entry", "Exit"};
            IList<Fee> fees = new List<Fee>();
            foreach (string feeType in feeTypes)
            {
                string feeTypeColumnName = feeType + "FeeFixed";
                double? fixedFeeValue = DataRowTool.GetColumnDoubleValue(row, feeTypeColumnName);
                if (fixedFeeValue != null && fixedFeeValue != 0)
                {
                    Fee fixedFee = new Fee();
                    fixedFee.FeeType = feeType;
                    fixedFee.FeeValueType = "Fixed";
                    fixedFee.FeeValue = (double) fixedFeeValue;
                    fixedFee.FeeUnit = DataRowTool.GetColumnStringValue(row, feeType + "FeeFixedUnit"); //replace to Fixed
                    fees.Add(fixedFee);
                }
            }

            foreach (string feeType in feeTypes)
            {
                string feeTypeColumnName = feeType + "FeeVariable";
                double? fixedFeeValue = DataRowTool.GetColumnDoubleValue(row, feeTypeColumnName);
                if (fixedFeeValue != null && fixedFeeValue != 0)
                {
                    Fee fixedFee = new Fee();
                    fixedFee.FeeType = feeType;
                    fixedFee.FeeValueType = "Variable";
                    fixedFee.FeeValue = (double) fixedFeeValue;
                    fixedFee.FeeUnit = DataRowTool.GetColumnStringValue(row, feeType + "FeeVariableUnit");
                    fees.Add(fixedFee);
                }
            }

       //     fees.OrderBy(fees.)
            if (fees.Count > 0) dto.FeesData.Fees = fees.ToArray();

            //Group names hardcoded, Reporting DB structure does not keep information about group names

            IList<PropertyGroup> PropertyGroups = new List<PropertyGroup>();
            PropertyGroup compliancePropertyGroup = new PropertyGroup {Name = "Compliance"};

            List<Property> complianceProperties = new List<Property>();

            bool? compresionProp = DataRowTool.GetColumnBoolValue(row, "DealCompression");
            if (compresionProp != null)
                complianceProperties.Add(new Property("DealCompression", compresionProp.ToString(), "System.Boolean"));

            DateTime? termDate = DataRowTool.GetColumnDateTimeValue(row, "TerminationDate");
            if (termDate != null)
            {
                string termDateProp = Convert.ToDateTime(termDate).ToString("yyyy-MM-ddTHH:mm:ss");
                complianceProperties.Add(new Property("TerminationDate", termDateProp, "System.DateTime"));
            }

            string emirStatusProp = DataRowTool.GetColumnStringValue(row, "EMIRStatus");
            if (emirStatusProp != null)
                complianceProperties.Add(new Property("EMIRStatus", emirStatusProp, "System.String"));

            bool? remitReportingProp = DataRowTool.GetColumnBoolValue(row, "RemitReporting");
            if (remitReportingProp != null && remitReportingProp != false)
                complianceProperties.Add(new Property("RemitReporting", remitReportingProp.ToString(), "System.Boolean"));

            string buyerProp = DataRowTool.GetColumnStringValue(row, "BuyerInitiatorAggressor");
            if (buyerProp != null)
                complianceProperties.Add(new Property("BuyerInitiatorAggressor", buyerProp, "System.String"));

            string sellerProp = DataRowTool.GetColumnStringValue(row, "SellerInitiatorAggressor");
            if (sellerProp != null)
                complianceProperties.Add(new Property("SellerInitiatorAggressor", sellerProp, "System.String"));

            string utiProp = DataRowTool.GetColumnStringValue(row, "UTI");
            if (utiProp != null)
                complianceProperties.Add(new Property("UTI", utiProp, "System.String"));

            //add Compliance property group
            if (complianceProperties.Count > 0)
            {
                compliancePropertyGroup.Properties = complianceProperties.ToArray();
                PropertyGroups.Add(compliancePropertyGroup);
            }



            //QA property group, contains 3 properties

            PropertyGroup QaPropertyGroup = new PropertyGroup { Name = "QA" };

            List<Property> QaPropertyList = new List<Property>();

            string PropertyCustom1 = DataRowTool.GetColumnStringValue(row, "PropertyCustom1");
            if (PropertyCustom1 != null)
                QaPropertyList.Add(new Property("PropertyCustom1", PropertyCustom1, "System.String"));

            string PropertyCustom2 = DataRowTool.GetColumnStringValue(row, "PropertyCustom2");
            if (PropertyCustom2 != null)
                QaPropertyList.Add(new Property("PropertyCustom2", PropertyCustom2, "System.String"));

            string PropertyCustom3 = DataRowTool.GetColumnStringValue(row, "PropertyCustom3");
            if (PropertyCustom3 != null)
                QaPropertyList.Add(new Property("PropertyCustom3", PropertyCustom3, "System.String"));
            
            //add TestCustomer property group
            if (QaPropertyList.Count > 0)
            {
                QaPropertyGroup.Properties = QaPropertyList.ToArray();//add to QA group
                PropertyGroups.Add(QaPropertyGroup);//add to common PropertyGroups
            }

            //QA property group, contains 3 properties

            PropertyGroup tCPropertyGroup = new PropertyGroup { Name = "TestCustomer" };

            List<Property> tCPropertyList = new List<Property>();

            string PropertyCustom4 = DataRowTool.GetColumnStringValue(row, "PropertyCustom4");
            if (PropertyCustom4 != null)
                tCPropertyList.Add(new Property("PropertyCustom4", PropertyCustom4, "System.String"));

            string PropertyCustom5 = DataRowTool.GetColumnStringValue(row, "PropertyCustom5");
            if (PropertyCustom5 != null)
                tCPropertyList.Add(new Property("PropertyCustom5", PropertyCustom5, "System.String"));

            string PropertyCustom6 = DataRowTool.GetColumnStringValue(row, "PropertyCustom6");
            if (PropertyCustom6 != null)
                tCPropertyList.Add(new Property("PropertyCustom6", PropertyCustom6, "System.String"));

            string PropertyCustom7 = DataRowTool.GetColumnStringValue(row, "PropertyCustom7");
            if (PropertyCustom7 != null)
                tCPropertyList.Add(new Property("PropertyCustom7", PropertyCustom7, "System.String"));

            string PropertyCustom8 = DataRowTool.GetColumnStringValue(row, "PropertyCustom8");
            if (PropertyCustom8 != null)
                tCPropertyList.Add(new Property("PropertyCustom8", PropertyCustom8, "System.String"));

            string PropertyCustom9 = DataRowTool.GetColumnStringValue(row, "PropertyCustom9");
            if (PropertyCustom9 != null)
                tCPropertyList.Add(new Property("PropertyCustom9", PropertyCustom9, "System.String"));

            string PropertyCustom10 = DataRowTool.GetColumnStringValue(row, "PropertyCustom10");
            if (PropertyCustom10 != null)
                tCPropertyList.Add(new Property("PropertyCustom10", PropertyCustom10, "System.String"));


            //add TestCustomer property group
            if (tCPropertyList.Count > 0)
            {
                tCPropertyGroup.Properties = tCPropertyList.ToArray();
                PropertyGroups.Add(tCPropertyGroup);

            }

            //add PropertyGroups
            dto.PropertyGroups = PropertyGroups.ToArray();


            dto.ReferenceData = new ReferenceData();
          
            IDictionary<string, string> dealGroups = new Dictionary<string, string>();

            dealGroups.Add("GroupField1", "DealGroup1");
            dealGroups.Add("GroupField2", "DealGroup2");
            dealGroups.Add("GroupField3", "DealGroup3");

            //for property group
            List<Property> qapProperties = new List<Property>();

            //deal group - old
            IList<DealGroup> dealGroupList = new List<DealGroup>();
            foreach (KeyValuePair<string, string> group in dealGroups)
            {
                string groupValue = DataRowTool.GetColumnStringValue(row, group.Key );
                if (groupValue != null && groupValue != string.Empty)
                {
                    DealGroup dealGroup = new DealGroup();
                    dealGroup.Name = group.Value;
                    dealGroup.Value = groupValue;
                    dealGroupList.Add(dealGroup);//old deal group
                    qapProperties.Add(new Property(group.Key, groupValue, "System.String")); //del group in properties
                }
            }

            if (dealGroupList.Count > 0 && qapProperties.Count >0)
            {
                dto.ReferenceData.DealGroups = dealGroupList.ToArray();

                PropertyGroup[] elvizPropertyGroups = dto.PropertyGroups;
                if (elvizPropertyGroups != null)
                {
                    IList<PropertyGroup> propertiesGroups = elvizPropertyGroups.ToList();
                    PropertyGroup dealgroup = propertiesGroups.FirstOrDefault(x => x.Name == "Deal Group");
                    if (dealgroup != null)
                    {
                        dealgroup.Properties = qapProperties.ToArray();
                        propertiesGroups.Add(dealgroup);
                    }
                    else
                    {
                        PropertyGroup prop = new PropertyGroup {Name = "Deal Group"};
                        prop.Properties = qapProperties.ToArray();
                        propertiesGroups.Add(prop);
                    }
                   // propertiesGroups.Add(dealgroup);
                    elvizPropertyGroups = propertiesGroups.ToArray();
                }

                else
                {
                    PropertyGroup prop = new PropertyGroup {Name = "Deal Group"};
                    prop.Properties = qapProperties.ToArray();
                    elvizPropertyGroups = new PropertyGroup[] {prop};
                }
                
                //convert all properties to DTO Property Group, order by group name
                dto.PropertyGroups = elvizPropertyGroups.OrderBy(x => x.Name).ToArray();
             }

            dto.ReferenceData.ExternalId = DataRowTool.GetColumnStringValue(row, "ExternalId");
            dto.ReferenceData.ExternalSource= DataRowTool.GetColumnStringValue(row, "ExternalSource");
            dto.ReferenceData.TicketNumber = DataRowTool.GetColumnStringValue(row, "TicketNumber");

            
            dto.ReferenceData.Comment = DataRowTool.GetColumnStringValue(row, "Comment");
            dto.ReferenceData.ModificationDateTimeUtc = (DateTime)row["LastUpdatedTimeUTC"];
            dto.ReferenceData.QuotaRegion = DataRowTool.GetColumnStringValue(row, "QuotaRegion");

            dto.ReferenceData.Originator = (bool)row["IsOriginator"];
            dto.ReferenceData.OriginalQuantity = (double)row ["OriginalQuantity"] ;
            dto.ReferenceData.DistributedQuantity = DataRowTool.GetColumnDoubleValue(row, "DistributedQuantity");
            //dto.ReferenceData = DataRowTool.GetColumnStringValue(row, "");

            dto.TransactionWorkFlowDetails = new TransactionWorkFlowDetails();
            dto.TransactionWorkFlowDetails.Audited = (bool)row["Audited"];
            dto.TransactionWorkFlowDetails.Authorised = (bool)row["Authorized"];
            dto.TransactionWorkFlowDetails.ConfirmedByBroker = (bool)row["ConfirmedByBroker"];
            dto.TransactionWorkFlowDetails.ConfirmedByCounterparty = (bool)row["ConfirmedByCounterparty"];
            dto.TransactionWorkFlowDetails.Paid = (bool)row["Paid"];

            //dto.TransactionWorkFlowDetails.TimeStampAuthorised = DataRowTool.GetColumnDateTimeValue(row, "");
            //dto.TransactionWorkFlowDetails.TimeStampCounterpartyAuthorised = DataRowTool.GetColumnDateTimeValue(row, "");

            dto.TransactionWorkFlowDetails.TimeStampClearedUtc = DataRowTool.GetColumnDateTimeValue(row, "TimeStampClearedUtc");
            dto.TransactionWorkFlowDetails.TimeStampConfirmationBrokerUtc = DataRowTool.GetColumnDateTimeValue(row, "TimeStampConfirmationBrokerUtc");
            dto.TransactionWorkFlowDetails.TimeStampConfirmationCounterPartyUtc = DataRowTool.GetColumnDateTimeValue(row, "TimeStampConfirmationCounterPartyUtc");

          //  Console.WriteLine(TestXmlTool.Serialize(dto));
            return dto;

        }

    
        public static string GetExternalIdByContractId(int Id)
        {
           //  int parentId =54;
            string sqlCommand = string.Format(@"select * from ContractExportsView exId
                                        where exId.ContractId = {0}", Id);

            DataTable result = QaDao.DataTableFromSql("ReportingDatabase", sqlCommand);

            if (result.Rows.Count > 1) throw new Exception("Query returned more than one transaction.");
            if (result.Rows.Count == 0) throw new Exception("Query did not return any transaction.");

            DataRow row = result.Rows[0];

            string externalId = DataRowTool.GetColumnStringValue(row, "ExternalId");

            //problem when uderlying deal is Gencon deal, than transaction id is wrong, it is id of ordinary transaction, not gencon
            if (Id ==3) externalId = "PassElectricityGencon";

           return externalId;

        }

    }
}
