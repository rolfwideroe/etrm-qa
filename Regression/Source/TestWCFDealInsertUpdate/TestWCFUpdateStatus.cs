using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using ElvizTestUtils;
using ElvizTestUtils.DatabaseTools;
using ElvizTestUtils.DealServiceReference;
using ElvizTestUtils.LookUpServiceReference;
using ElvizTestUtils.QaLookUp;
using NUnit.Framework;
using TransactionStatus = ElvizTestUtils.DealServiceReference.TransactionStatus;

namespace TestWCFDealInsertUpdate
{
    [TestFixture]
    class TestWCFUpdateStatus
    {
        private Dictionary<string, int> testCases;
            
        [SetUp]
        public void SetUp()
        {
            testCases=new Dictionary<string, int>();

            AddTestCase( CreateActiveElForward );
            AddTestCase( CreateActiveElReserveCapacity);
        }

        private void AddTestCase(Func<KeyValuePair<string,int>> getPair )
        {
            KeyValuePair<string, int> elForwardKeyValuePair = getPair();
            testCases.Add(elForwardKeyValuePair.Key, elForwardKeyValuePair.Value);
        }

        [Test]
        public void UpdateElectricityForward()
        {
            int elvizId = testCases["Electricity-Forward"];
            TestStatus(elvizId, TransactionStatus.Draft);
            TestStatus(elvizId, TransactionStatus.Onhold);
            TestStatus(elvizId, TransactionStatus.Cleared);
            TestStatus(elvizId, TransactionStatus.Cancelled);
            TestStatus(elvizId, TransactionStatus.Active);
            TestStatus(elvizId, TransactionStatus.Deleted);
        }


        [Test]
        public void Electricity_Reserve_Capacity_Can_Be_Rejected()
        {
            int elvizId = testCases["Electricity-ReserveCapacity"];
            TestStatus(elvizId, TransactionStatus.BidRejected);
        }

        [Test]
        public void Electricity_Forward_Cannot_Be_Rejected()
        {
            int elvizId = testCases["Electricity-Forward"];
            Assert.Catch(
                ()=> { TestStatus(elvizId, TransactionStatus.BidRejected);  },
                   "Not a valid transaction status for TransactionId="+elvizId) ;
        }

        private void TestStatus(int elvizId,TransactionStatus status)
        {
            IDealService dealServiceClient = WCFClientUtil.GetDealServiceServiceProxy();
            ILookupService lookupServiceClient = WCFClientUtil.GetLookUpServiceServiceProxy();


            Dictionary<int, TransactionStatus> dictionary = new Dictionary<int, TransactionStatus> {{elvizId, status}};

            bool updateresult = dealServiceClient.UpdateStatus(dictionary);

            if (!updateresult) Assert.Fail("Update status failed");

            TransactionDTO[] dtos = lookupServiceClient.GetTransactionsByIds(new[] { elvizId });

            if (dtos.Count() != 1) Assert.Fail("Did not return 1 dto as expected");

            TransactionDTO dto = dtos[0];

            Assert.AreEqual(status.ToString(), dto.Status.ToString());
        }



        private KeyValuePair<string, int> CreateActiveElForward()
        {
            string insertdeal = @"<DealInsert>
	                            <MetaData>
		                            <MessageId>MessageId</MessageId>
		                            <Timezone>CET</Timezone>
	                            </MetaData>
	                            <Transaction>
		                            <TransactionType>Automatic</TransactionType>
		                            <DealType>Electricity-Forward</DealType>
		                            <Portfolios>
			                            <Portfolio>API Test Customer</Portfolio>
			                            <CounterpartyPortfolio>API Test Counterparty</CounterpartyPortfolio>
			                            <BuySell>Buy</BuySell>
		                            </Portfolios>
		                            <ElectricityForward>
			                            <InstrumentData>
				                            <ExecutionVenue>None</ExecutionVenue>
				                            <PriceBasis>SYOSL</PriceBasis>
				                            <LoadProfile>Base Week</LoadProfile>
				                            <FromDate>2008-01-01</FromDate>
				                            <ToDate>2008-12-31</ToDate>
				                            <Financial/>
			                            </InstrumentData>
			                            <SettlementData>
				                            <Quantity>40</Quantity>
				                            <QuantityUnit>MW</QuantityUnit>
				                            <Price>-28</Price>
				                            <Currency>EUR</Currency>
				                            <PriceVolumeUnit>PerMWh</PriceVolumeUnit>
				                            <CurrencySource>Viz</CurrencySource>
				                            <Clearing>Electronic</Clearing>
				                            <ClearingCurrency>EUR</ClearingCurrency>
				                            <ClearingFee>0</ClearingFee>
				                            <BrokerTradingFee>0</BrokerTradingFee>
				                            <BrokerCurrency>EUR</BrokerCurrency>
			                            </SettlementData>
		                            </ElectricityForward>
		                            <DealDetails>
			                            <Status>Active</Status>
			                            <Trader>DealImport</Trader>
			                            <TradeDate>2007-12-24T00:00:00</TradeDate>
			                            <ConfirmedByBroker>false</ConfirmedByBroker>
			                            <ConfirmedByCounterparty>false</ConfirmedByCounterparty>
			                            <Authorised>false</Authorised>
			                            <Paid>false</Paid>
		                            </DealDetails>
		                            <ReferenceData>			
			                            <Comment>ElectricityForward; should pass</Comment>
			                            <DealGroup>API Test</DealGroup>
			                            <ExternalId>PassElecForwardUpdateStatus</ExternalId>
			                            <ExternalSource>XMLFileImport</ExternalSource>
			                            <TicketNumber>TicketNumber</TicketNumber>
		                            </ReferenceData>
	                            </Transaction>
                            </DealInsert> ";
    
            return CreateDeal(insertdeal, "PassElecForwardUpdateStatus", "Electricity-Forward");
        }

        private KeyValuePair<string, int> CreateActiveElReserveCapacity()
        {
            string insertdeal = @"<DealInsert>
	                                    <MetaData>
				                            <Timezone>CET</Timezone>
	                                    </MetaData>
	                                    <Transaction>
		                                    <TransactionType>Automatic</TransactionType>
				                            <DealType>Electricity-ReserveCapacity</DealType>
				                            <Portfolios>
					                            <Portfolio>API Test Customer</Portfolio>
					                            <CounterpartyPortfolio>API Test Counterparty</CounterpartyPortfolio>
					                            <BuySell>Sell</BuySell>
				                            </Portfolios>
				                            <ElectricityReserveCapacity>
					                            <InstrumentData>
						                            <AuctionType>Tertiary Market Day Ahead</AuctionType>
						                            <PriceBasis>EEX</PriceBasis>
						                            <LoadProfile>Iberia Peak D</LoadProfile>
						                            <FromDate>2016-03-01</FromDate>
						                            <ToDate>2016-03-01</ToDate>
						                            <Physical>
							                            <DeliveryArea AreaName=""ENBW"" />
                                                    </Physical>
                                                    <TransactionName>El-Reserve-Capacity-EEX-001</TransactionName>
                                                </InstrumentData>
                                                <SettlementData>
                                                    <CapacityBidQuantity>2</CapacityBidQuantity>
                                                    <CapacityTradeQuantity>2</CapacityTradeQuantity>
                                                    <CapacityPrice>11.2</CapacityPrice>
                                                    <Price>50.77</Price>
                                                    <ReserveCapacityTimeSeries>
                                                      <TimeSeries>
                                                        <TimeSeriesType>RealizedVolume</TimeSeriesType>
                                                        <Resolution>15Minute</Resolution>   
                                                           <TimeSeriesDetail FromDateTime = ""2016-03-01T08:00:00"" ToDateTime =""2016-03-01T08:15:00"" Value = ""2"" />
                                                      </TimeSeries>
                                           </ReserveCapacityTimeSeries>
                                           <QuantityUnit>MW</QuantityUnit>
                                           <Currency>EUR</Currency>
                                           <CurrencySource>Viz</CurrencySource>
                                       </SettlementData>
                                       </ElectricityReserveCapacity>
                                       <DealDetails>
                                           <Status>Active</Status>
                                           <Trader>DealImport</Trader>
                                           <TradeDate>2016-01-01T00:00:00</TradeDate>
                                       </DealDetails>
                                       <ReferenceData>
                                            <Comment>El-Reserve-Capacity; Should Pass</Comment>
                                            <ExternalId>El-Reserve-Capacity-EEX-101</ExternalId>
                                            <ExternalSource>API Test</ExternalSource>
                                            <TicketNumber>TicketNumber</TicketNumber>
                                       </ReferenceData>
                                    </Transaction>
                                 </DealInsert> ";

            return CreateDeal(insertdeal, "El-Reserve-Capacity-EEX-101", "Electricity-ReserveCapacity");
        }


        private KeyValuePair<string,int> CreateDeal(string insertDeal, string externalId, string instrument)
        {
            IDealService dealServiceClient = WCFClientUtil.GetDealServiceServiceProxy();

            ILookupService lookupServiceClient = WCFClientUtil.GetLookUpServiceServiceProxy();
            try
            {
                 dealServiceClient.ImportDeal(insertDeal);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }

            QaDao utility=new QaDao();

            int elvizId = utility.GetOriginalTransactionIdsFromExternalIds(new[] { externalId })[0];

            KeyValuePair<string,int> resultKeyValuePair=new KeyValuePair<string, int>(instrument,elvizId);

            return resultKeyValuePair;

    
        }



    }
}
