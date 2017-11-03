using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using ElvizTestUtils.LookUpServiceReference;

namespace ElvizTestUtils.QaLookUp
{

    public class QaLookUpClient
    {
        private readonly ILookupService lookupService;

        public QaLookUpClient()
        {
            this.lookupService = WCFClientUtil.GetLookUpServiceServiceProxy();
        }

        public static string GetEtrmVersion(string appServerName)
        {

            ILookupService lookUpServiceClient = WCFClientUtil.GetLookUpServiceServiceProxy(appServerName);
            string elvizVersion = null;
            string simplequery = @"<LookupMessage><MetaData><Action>UserLookup</Action><MessageId>1234</MessageId></MetaData><UserLookup></UserLookup></LookupMessage>";
            string resultXML = lookUpServiceClient.LookUp(simplequery);
            if (resultXML != null)
            {
                XmlDocument resultXml = new XmlDocument();
                resultXml.LoadXml(resultXML);

                XmlNodeList versiontNode = resultXml.GetElementsByTagName("ElvizVersion");
                elvizVersion = versiontNode[0].InnerText;
            }
            return elvizVersion;
        }

        //public string[] GetExternalIdsByTransactionIds(int[] transactionIds)
        //{
        //    IList<string> externalIds = new List<string>();


        //    TransactionDTO[] dtos= this.lookupService.GetTransactionsByIds(transactionIds);

        //    foreach (TransactionDTO transactionDTO in dtos)
        //    {
        //        string extId = transactionDTO.ExternalId;

        //        if(string.IsNullOrEmpty(extId)) throw new ArgumentException("No ExternalId for Transaction id : "+transactionDTO.TransactionId);
        //         externalIds.Add(extId);   
        //    }



        //    return externalIds.OrderBy(x => x).ToArray();
        //}


//        public int[] GetTransactionIdByExternalId(string[] externalId)
//        {
//            IList<int> transIds = new List<int>();



        

//            foreach (string extId in externalId)
//            {
//                string xmlLookUp = string.Format(@"<LookupMessage>
//														<MetaData>
//															<Action>TransactionLookup</Action>
//															<MessageId>1234</MessageId>
//														</MetaData>
//														<TransactionLookup>
//															<Filter>
//																<ExternalId>{0}</ExternalId>
//																<IsOriginator>1</IsOriginator>
//															</Filter>
//															</TransactionLookup>
//													</LookupMessage>", extId);

//                string resultXmlAsString = this.lookupService.LookUp(xmlLookUp);

//                XmlDocument resultXml = new XmlDocument();
//                resultXml.LoadXml(resultXmlAsString);

//                XmlNode xmlNode = resultXml.SelectSingleNode("/LookupResult/Transactions[1]//ReferenceData/ElvizId");

//                if (xmlNode != null)
//                {
//                    int transId;
//                    if (int.TryParse(xmlNode.InnerText, out transId))
//                    {
//                        transIds.Add(transId);
//                    }
//                }
//            }
//            return transIds.ToArray();
//        }

        public QaTransactionDTO GetQaTransactionDTO(int id)
        {
            TransactionDTO[] dtos = lookupService.GetTransactionsByIds(new[] { id });

            return ToQaTransactionDto(dtos[0]);
        }

        public List<QaTransactionDTO> GetQaTransactionDtos(string[] externalIds)
        {
            TransactionDTO[] transactions = lookupService.GetOriginalTransactionsByExternalIds(externalIds);

            return transactions.Select(ToQaTransactionDto).ToList();
        }

        public List<QaTransactionDTO> GetQaTransactionDtosByUti(string[] utiIds)
        {
            TransactionDTO[] transactions = lookupService.GetOriginalTransactionsByUTI(utiIds);

            return transactions.Select(ToQaTransactionDto).ToList();
        }

        private QaTransactionDTO ToQaTransactionDto(TransactionDTO dto)
        {
            QaTransactionDTO qaDto = new QaTransactionDTO();

            if (dto.DeliveryType.ToString().ToUpper() == "FINANCIAL")
            {
                qaDto.DeliveryType = QaDeliveryType.Financial;
            }
            else
            {
                qaDto.DeliveryType = QaDeliveryType.Physical;
            }

            qaDto.DeliveryLocation = dto.DeliveryLocation;
            qaDto.TransactionId = dto.TransactionId;

            if (dto.InstrumentType.Name.ToUpper() == "CASH FLOW")
            {
                if (dto.Type.ToString().ToUpper() == "BUY")
                {
                    qaDto.PaidReceived = QaPaidReceived.Received;
                }
                else
                {
                    qaDto.PaidReceived = QaPaidReceived.Paid;
                }
            }
            else
            {
                if (dto.Type.ToString().ToUpper() == "BUY")
                {
                    qaDto.BuySell = QaBuySell.Buy;
                }
                else
                {
                    qaDto.BuySell = QaBuySell.Sell;
                }
            }

            qaDto.ContractModelType = dto.ContractModelType;
            Portfolios portfolios = new Portfolios
            {
                PortfolioName = dto.Portfolio.PortfolioName,
                PortfolioExternalId = dto.Portfolio.ExternalId,
                CounterpartyPortfolioName = dto.CounterPartyPortfolio.PortfolioName,
                CounterpartyPortfolioExternalId = dto.CounterPartyPortfolio.ExternalId
            };

            qaDto.Portfolios = portfolios;

            qaDto.DealType = dto.Commodity.Name + "-" + dto.InstrumentType.Name.Replace(" ", string.Empty);

            InstrumentData instrumentData = GetInstrumentData(dto);

            qaDto.InstrumentData = instrumentData;

            qaDto.SettlementData = GetSettlementData(dto);

            qaDto.DealDetails = GetDealDetails(dto);

            qaDto.FeesData = GetFeeData(dto);

            qaDto.ReferenceData = GetReferenceData(dto);

            qaDto.PropertyGroups = GetPropertyGroups(dto);

            qaDto.TransactionWorkFlowDetails = GetTransactionWorkFlowDetails(dto);
            return qaDto;
        }

        private TransactionWorkFlowDetails GetTransactionWorkFlowDetails(TransactionDTO dto)
        {
            TransactionWorkFlowDetails details = new TransactionWorkFlowDetails();

            details.Audited = dto.Audited;
            details.Authorised = dto.Authorised;
            details.ConfirmedByBroker = dto.ConfirmedByBroker;
            details.TimeStampCounterpartyAuthorised = dto.CounterpartyTransactionAuthorised;
            details.Paid = dto.Paid;
            details.TimeStampClearedUtc = dto.TimeStampClearedUTC;
            details.TimeStampConfirmationBrokerUtc = dto.TimeStampConfirmationBrokerUTC;
            details.TimeStampConfirmationCounterPartyUtc = dto.TimeStampConfirmationCounterPartyUTC;
            details.TimeStampCounterpartyAuthorised = dto.TransactionAuthorised;
            details.TimeStampAuthorised = dto.TransactionAuthorised;


            return details;

        }

        private PropertyGroup[] GetPropertyGroups(TransactionDTO dto)
        {
            if (dto.PropertyGroups == null)
                return null;

            IList<PropertyGroup> qaGroups = new List<PropertyGroup>();

            LookUpServiceReference.PropertyGroup[] elvizPropertyGroups = dto.PropertyGroups;

            foreach (LookUpServiceReference.PropertyGroup elvizPropertyGroup in elvizPropertyGroups)
            {
                PropertyGroup qaGroup = new PropertyGroup { Name = elvizPropertyGroup.Name };
                IList<Property> qapProperties = new List<Property>();

                foreach (LookUpServiceReference.Property elvizProperty in elvizPropertyGroup.Properties)
                {
                    qapProperties.Add(new Property(elvizProperty.Name, elvizProperty.Value, elvizProperty.ValueType));
                }

                qaGroup.Properties = qapProperties.ToArray();

                qaGroups.Add(qaGroup);
            }

            return qaGroups.ToArray();
        }

        private void VerifyOldFeeEqualNewFee(double oldFee, LookUpServiceReference.Fee[] newFees,string newFeeType)
        {
            if(Math.Abs(oldFee) > GlobalConstTestSettings.DEFAULT_FLOATING_POINT_TOLLERANCE)
            {
                LookUpServiceReference.Fee elvizNewFee =
                    newFees.Where(x => x.FeeType == newFeeType).FirstOrDefault(x => x.FeeValueType == "Variable");

                if (elvizNewFee == null)
                    throw new ArgumentException(string.Format("DTO has old style {0} fee but not new type", newFeeType));

                if (Math.Abs(elvizNewFee.FeeValue - oldFee) > GlobalConstTestSettings.DEFAULT_FLOATING_POINT_TOLLERANCE)
                    throw new ArgumentException(
                        string.Format(
                            "Old Style {0} fee does not equal new style brokerfee: " + oldFee + " vs " +
                            elvizNewFee.FeeValue, newFeeType));
            }
            else
            {
                LookUpServiceReference.Fee elvizNewFee =newFees.Where(x => x.FeeType == newFeeType).FirstOrDefault(x => x.FeeValueType == "Variable");

                if (elvizNewFee!=null)
                    throw new ArgumentException(string.Format("DTO has new style {0} fee but not old type", newFeeType));
            }
        }

        private FeesData GetFeeData(TransactionDTO dto)
        {
            FeesData feesData = new FeesData();



            feesData.ClearingType = dto.ClearingType.ToString();

            IList<Fee> fees = new List<Fee>();

            if (dto.Broker != null)
            {
                feesData.Broker = dto.Broker.CompanyName;
            }

            ElvizTestUtils.LookUpServiceReference.Fee[] elivzFees = dto.Fees;

            if (elivzFees == null) return feesData;

            foreach (LookUpServiceReference.Fee elivzFee in elivzFees)
            {
                fees.Add(new Fee(elivzFee.FeeType, elivzFee.FeeValue, elivzFee.FeeUnit, elivzFee.FeeValueType));
            }

           
                VerifyOldFeeEqualNewFee(dto.BrokerFee, elivzFees, "Broker");              

                VerifyOldFeeEqualNewFee(dto.ClearingFee, elivzFees, "Clearing");
                    
                VerifyOldFeeEqualNewFee(dto.CommissionFee, elivzFees, "Commission");
                       
                VerifyOldFeeEqualNewFee(dto.TradingFee, elivzFees, "Trading");
                      
                VerifyOldFeeEqualNewFee(dto.EntryFee, elivzFees, "Entry");
                       
                VerifyOldFeeEqualNewFee(dto.ExitFee, elivzFees, "Exit");
                     
                VerifyOldFeeEqualNewFee(dto.NominationFee, elivzFees, "Nomination");
            

        


            if (fees.Count > 0)
                feesData.Fees = fees.ToArray();

            return feesData;
        }

        private ReferenceData GetReferenceData(TransactionDTO dto)
        {
            ReferenceData referenceData = new ReferenceData();
            referenceData.CascadingOriginIds = dto.CascadingOriginIds;
            if (dto.ContractSplitId != 0) referenceData.ContractSplitId = dto.ContractSplitId;
            referenceData.Comment = dto.Comment;

            if (!string.IsNullOrEmpty(dto.GroupField1) | !string.IsNullOrEmpty(dto.GroupField2) | !string.IsNullOrEmpty(dto.GroupField3))
            {
                IList<DealGroup> dealGroups = new List<DealGroup>();

                if (!string.IsNullOrEmpty(dto.GroupField1)) dealGroups.Add(new DealGroup("DealGroup1", dto.GroupField1));
                if (!string.IsNullOrEmpty(dto.GroupField2)) dealGroups.Add(new DealGroup("DealGroup2", dto.GroupField2));
                if (!string.IsNullOrEmpty(dto.GroupField3)) dealGroups.Add(new DealGroup("DealGroup3", dto.GroupField3));

                if (dealGroups.Count > 0) referenceData.DealGroups = dealGroups.ToArray();
            }

            if (referenceData.DeclareId > 0) referenceData.DeclareId = dto.DeclareId;

            referenceData.Deliveries = dto.Deliveries;

            if (Math.Abs(dto.DistributedQuantity) > GlobalConstTestSettings.DEFAULT_FLOATING_POINT_TOLLERANCE) referenceData.DistributedQuantity = dto.DistributedQuantity;

            if (dto.DistributionParentTransactionId != 0) referenceData.DistributionParentTransactionId = dto.DistributionParentTransactionId;

            referenceData.ExternalId = dto.ExternalId;

            referenceData.ExternalSource = dto.ExternalSource;

            referenceData.ModificationDateTimeUtc = dto.ModificationDate.ToUniversalTime();

            if (dto.PriceVolumeTimeSeriesDetails == null && dto.FlexibleTimeSeries == false)
                referenceData.OriginalQuantity = dto.OriginalQuantity;

            referenceData.Originator = dto.Originator;

            referenceData.QuotaRegion = dto.QuotaRegion;

            referenceData.ReferringId = dto.ReferringId;

            if (!string.IsNullOrEmpty(dto.TicketNumber)) referenceData.TicketNumber = dto.TicketNumber;

            return referenceData;
        }

        private DealDetails GetDealDetails(TransactionDTO dto)
        {
            TimeZoneInfo info = TimeZoneInfo.FindSystemTimeZoneById("Central Europe Standard Time");

            DateTime tradeDateTimeUtc = TimeZoneInfo.ConvertTimeToUtc(dto.TradeTime, info);

            DealDetails dealDetails = new DealDetails
            {
                Status = dto.Status.ToString(),
                TradeDateTimeUtc = tradeDateTimeUtc,
                Trader = dto.Trader,
                CounterpartyTrader = dto.CounterPartyTrader
            };

            return dealDetails;
        }

        private SettlementData GetSettlementData(TransactionDTO dto)
        {
            SettlementData settlementData = new SettlementData();

            settlementData.CurrencySource = dto.CurrencySource.Name;

            if (HasMarketPriceMulitplicator(dto))
                settlementData.MarketPriceMultiplicator = dto.MarketPriceMultiplicator;

            settlementData.MasterAgreement = dto.MasterAgreementName;


            settlementData.PriceIsoCurrency = dto.Currency.ISOCode;

            settlementData.PriceVolumeUnit = dto.PriceVolumeUnit.ToString();



            if (dto.Commodity.Name != "Currency")
                settlementData.QuantityUnit = dto.QuantityUnit.ToString();

            if (dto.PriceVolumeTimeSeriesDetails != null)
                settlementData.Resolution = dto.Resolution.ToString();

            if (dto.SettlementRule != null) settlementData.SettlementRule = dto.SettlementRule.Name;

            if (dto.PriceVolumeTimeSeriesDetails != null)
            {
                PriceVolumeTimeSeriesDetail[] qaDetails = new PriceVolumeTimeSeriesDetail[dto.PriceVolumeTimeSeriesDetails.Length];

                for (int i = 0; i < dto.PriceVolumeTimeSeriesDetails.Length; i++)
                {
                    LookUpServiceReference.PriceVolumeTimeSeriesDetail elvizDetail = dto.PriceVolumeTimeSeriesDetails[i];
                    PriceVolumeTimeSeriesDetail qadDetail = new PriceVolumeTimeSeriesDetail();

                    qadDetail.FromDateTime = elvizDetail.FromDateTime;
                    qadDetail.ToDateTime = elvizDetail.UntilDateTime;
                    qadDetail.Price = elvizDetail.Price;
                    qadDetail.Volume = elvizDetail.Volume;

                    qaDetails[i] = qadDetail;
                }

                if (dto.InstrumentType.Name.ToUpper() == "CAPACITY STRUCTURED DEAL" || dto.InstrumentType.Name.ToUpper() == "ASIAN")
                    settlementData.Price = dto.Price;


                settlementData.PriceVolumeTimeSeriesDetails = qaDetails;
                return settlementData;
            }

            if (dto.FlexibleTimeSeries)
            {
                TimeSeriesSet qaTimeSeriesSet = new TimeSeriesSet();

                int numberOfTimeSeries = dto.TimeSeriesSet.Count();

                TimeSeries[] qaTimeSeries = new TimeSeries[numberOfTimeSeries];

                for (int i = 0; i < numberOfTimeSeries; i++)
                {
                    TimeSeries qaSerie = new TimeSeries();
                    LookUpServiceReference.TimeSeries elvizSerie = dto.TimeSeriesSet[i];

                    qaSerie.Resolution = elvizSerie.Resolution.ToString();
                    qaSerie.TimeSeriesTypeName = elvizSerie.TimeSeriesType.Name;
                    qaSerie.TimezoneName = elvizSerie.TimezoneName;

                    LookUpServiceReference.TimeSeriesValue[] elvizValues = elvizSerie.TimeSeriesValues;

                    int numberOfValues = elvizValues.Count();

                    TimeSeriesValue[] qaValues = new TimeSeriesValue[numberOfValues];

                    for (int j = 0; j < numberOfValues; j++)
                    {
                        TimeSeriesValue qaValue = new TimeSeriesValue();
                        LookUpServiceReference.TimeSeriesValue elvizValue = elvizValues[j];

                        qaValue.FromDateTime = elvizValue.FromDateTime;
                        qaValue.ToDateTime = elvizValue.UntilDateTime;
                        //  qaValue.UtcFromDateTime = elvizValue.UtcFromDateTime;
                        //  qaValue.UtcToDateTime = elvizValue.UtcUntilDateTime;
                        qaValue.Value = elvizValue.Value;
                        //.HasValue ? elvizValue.FeeValue.FeeValue : 0.0;

                        qaValues[j] = qaValue;
                    }

                    qaSerie.TimeSeriesValues = qaValues;

                    qaTimeSeries[i] = qaSerie;

                }

                qaTimeSeriesSet.TimeSeries = qaTimeSeries;

                settlementData.TimeSeriesSet = qaTimeSeriesSet;

                //return settlementData;

            }
            settlementData.Quantity = dto.Quantity;
            settlementData.Price = dto.Price;
            settlementData.InitialPrice = dto.InitialPrice;

            if(dto.CapacityBidQuantity!=null) settlementData.CapacityBidQuantity = dto.CapacityBidQuantity;
            if(dto.CapacityTradeQuantity!=null) settlementData.CapacityTradedQuantity = dto.CapacityTradeQuantity;
            if(dto.CapacityPrice!=null) settlementData.CapacityPrice = dto.CapacityPrice;



            return settlementData;
        }



        private bool HasMarketPriceMulitplicator(TransactionDTO dto)
        {
            if (dto.Commodity.Name == "Electricity" && dto.InstrumentType.Name == "Swap") return true;

            string bookPriceType = dto.BookPriceType.ToString();
            return bookPriceType.ToUpper().Contains("INDEXED") | bookPriceType.ToUpper().Contains("SPOT") | bookPriceType.ToUpper().Contains("AVERAGETRADEDCLOSEWITHMARGIN");
        }

        private InstrumentData GetInstrumentData(TransactionDTO dto)
        {

            InstrumentData instrumentData = new InstrumentData();
            if (dto.BalanceArea != null)
                instrumentData.BalanceArea = dto.BalanceArea.Name;

            if (dto.BaseCurrency != null)
                instrumentData.BaseIsoCurrency = dto.BaseCurrency.ISOCode;

            if (dto.PriceBasis != null)
                instrumentData.PriceBasis = dto.PriceBasis.Name;

            instrumentData.CertificateType = dto.CertificateType;

            if (dto.CrossCurrency != null)
                instrumentData.CrossIsoCurrency = dto.CrossCurrency.ISOCode;

            instrumentData.EnvironmentLabel = dto.EnvironmentLabel;

            if (dto.Venue != null)
            {
                instrumentData.ExecutionVenue = dto.Venue.ExecutionVenueName;

            }
            else
            {
                instrumentData.ExecutionVenue = "None";
            }
            


            instrumentData.FromCountry = dto.FromCountry;

            instrumentData.ToCountry = dto.ToCountry;

            if (dto.FromDate != DateTime.MinValue) instrumentData.FromDate = dto.FromDate;
            if (dto.ToDate != DateTime.MinValue) instrumentData.ToDate = dto.ToDate;

            if (dto.IndexFormula != null) instrumentData.IndexFormula = dto.IndexFormula.Name;

            instrumentData.InstrumentName = dto.InstrumentName;

            if (dto.Interconnector != null) instrumentData.Interconnector = dto.Interconnector.Name;
            if (dto.InstrumentType.Name.ToUpper().Contains("FTR")) instrumentData.LossFactor = dto.LossFactor;

            if (dto.LoadProfile != null)
            {
                if (dto.LoadProfile.HourSelectMode == LoadProfileHourSelectMode.SelectOffPeak &&
                    dto.LoadProfile.SystemProfile == SystemLoadType.SystemOffPeak)
                    instrumentData.LoadProfile = "Off peak";
                else if (dto.LoadProfile.HourSelectMode == LoadProfileHourSelectMode.SelectPeak &&
                        dto.LoadProfile.SystemProfile == SystemLoadType.SystemPeak)
                    instrumentData.LoadProfile = "Peak";
                else instrumentData.LoadProfile = dto.LoadProfile.Name;
            }

            if (dto.InstrumentType.Name.ToUpper().Contains("SWING")) instrumentData.MaxVol = dto.MaxVol;
            if (dto.InstrumentType.Name.ToUpper().Contains("SWING")) instrumentData.MinVol = dto.MinVol;

            if (dto.PriceBasisToArea != null) instrumentData.PriceBasisToArea = dto.PriceBasisToArea.Name;

            if (dto.BookPriceType != null) instrumentData.PriceType = dto.BookPriceType.ToString();

            if (dto.HistoricContractPrices != null) instrumentData.HistoricContractPrices = dto.HistoricContractPrices.ExternalId;
            if (dto.ReferencePriceSeries != null) instrumentData.ReferencePriceSeries = dto.ReferencePriceSeries;
            if (dto.DestinationReferencePriceSeries != null) instrumentData.DestinationReferencePriceSeries = dto.DestinationReferencePriceSeries;

            instrumentData.CapFloorPricingResolution = dto.CapFloorPricingResolution;

            instrumentData.ProductionFacility = dto.ProductionFacility;

            if (IsOption(dto))
            {
                if (dto.PutCall == PutCall.Call) instrumentData.PutCall = QaPutCall.Call;
                if (dto.PutCall == PutCall.Put) instrumentData.PutCall = QaPutCall.Put;

                if(dto.UnderlyingInstrumentType!=null)
                    instrumentData.UnderlyingInstrumentType = dto.UnderlyingInstrumentType.Name;

                string instrumentType = dto.InstrumentType.Name.ToUpper();

                if (instrumentType.Contains("ASIAN"))
                {
                    instrumentData.Strike = dto.Strike;
                    instrumentData.SamplingPeriod = dto.SamplingPeriod.ToString();
                    if (dto.ExpiryDate > DateTime.MinValue)
                        instrumentData.ExpiryDate = dto.ExpiryDate;
                }
                
                else if (instrumentType.Contains("CAPACITY"))
                {
                    instrumentData.ExpiryTime = dto.ExpiryDate.ToString("hh:mm:ss");
                    instrumentData.ExpiryOffset = dto.ExpiryOffset;
                    instrumentData.Priority = dto.Priority.ToString();
                }
                else if (instrumentType.Contains("FTR"))
                {
                    instrumentData.ExpiryTime = dto.ExpiryDate.ToString("hh:mm:ss");//default will be set to -1
                    instrumentData.ExpiryOffset = 1; //default is yesterday
                    instrumentData.Strike = null;
                }
                else if (instrumentType.Contains("INDEXED OPTION"))
                {
                    instrumentData.Strike = null;
                    //instrumentData.ExpiryTime = dto.ExpiryDate.ToString("hh:mm:ss");
                    //instrumentData.ExpiryOffset = dto.ExpiryOffset;
                    //instrumentData.Priority = dto.Priority.ToString();
                }
                else
                {
                    instrumentData.Strike = dto.Strike;
                    instrumentData.ExpiryDate = dto.ExpiryDate;
                }



                //if (!dto.InstrumentType.Name.ToUpper().Contains("CAPACITY"))
                //{
                //    instrumentData.Strike = dto.Strike;
                //    instrumentData.ExpiryDate = dto.ExpiryDate;
                //}
                //else
                //{
                //    instrumentData.ExpiryTime = dto.ExpiryDate.ToString("hh:mm:ss");
                //    instrumentData.ExpiryOffset = dto.ExpiryOffset;
                //    instrumentData.Priority = dto.Priority.ToString();
                //}
            }

            if (dto.BookPriceType.ToString() == "AverageTradedCloseWithMargin" || IsOption(dto))
            {
                if (dto.UnderlyingExecutionVenue != null)
                    instrumentData.UnderlyingExecutionVenue = dto.UnderlyingExecutionVenue.ExecutionVenueName;
                if (dto.UnderlyingDeliveryType != null )
                    instrumentData.UnderlyingDeliveryType = dto.UnderlyingDeliveryType.ToString();
                if (dto.UnderlyingInstrumentType != null)
                    instrumentData.UnderlyingInstrumentType = dto.UnderlyingInstrumentType.Name;

                instrumentData.SamplingFrom = dto.SamplingFrom;
                instrumentData.SamplingTo = dto.SamplingTo;
            }

            instrumentData.TimeZone = dto.TimezoneName;

            if (dto.TransferDate != DateTime.MinValue) instrumentData.TransferDate = dto.TransferDate;

            instrumentData.CapacityId = dto.CapacityId;

            instrumentData.VolumeReferenceExternalId = dto.VolumeReferenceExternalId;
            if (dto.AuctionType !=null) instrumentData.AuctionType = dto.AuctionType.Replace(" ","");

          

            return instrumentData;
        }

        private bool IsOption(TransactionDTO dto)
        {
            string instrumentType = dto.InstrumentType.Name.ToUpper();
            return (instrumentType.Contains("EUROPEAN") | instrumentType.Contains("ASIAN") |
                    (instrumentType.Contains("CAPACITY") && !instrumentType.Contains("RESERVE")) | instrumentType.Contains("AMERICAN") |
                    instrumentType.Contains("OPTION"));
        }

    }
}
