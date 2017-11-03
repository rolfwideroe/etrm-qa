namespace DealEntryXml.Model
{
    public class Transaction
    {
        public Transaction(){}

        public Transaction(string commodity, string instrumentType, GeneralData generalData, Portfolios portfolios, InstrumentData instrumentData, SettlementData settlementData, Fees fees, DealDetails dealDetails, ReferenceData referenceData)
        {
            Commodity = commodity;
            InstrumentType = instrumentType;
            GeneralData = generalData;
            Portfolios = portfolios;
            InstrumentData = instrumentData;
            SettlementData = settlementData;
            Fees = fees;
            DealDetails = dealDetails;
            ReferenceData = referenceData;
        }

        public string Commodity { get; set; }

        public string InstrumentType { get; set; }

        public GeneralData GeneralData { get; set; }

        public Portfolios Portfolios { get; set; }

        public InstrumentData InstrumentData { get; set; }

        public SettlementData SettlementData { get; set; }

        public Fees Fees { get; set; }

        public DealDetails DealDetails { get; set; }

        public ReferenceData ReferenceData { get; set; }


    }
}