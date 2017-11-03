using System;

namespace DealEntryXml.Model
{
    public class Fees
    {
        



        public string Broker { get; set; }

        public double BrokerFee { get; set; }

           public bool BrokerFeeSpecified { get { return Math.Abs(BrokerFee - 0) > double.Epsilon; } }

        public string BrokerFeeCurrency { get; set; }

        public double ClearingFee { get; set; }

            public bool ClearingFeeSpecified { get { return Math.Abs(BrokerFee - 0) > double.Epsilon; } }

        public string ClearingFeeCurrency { get; set; }

        public double CommissionFee { get; set; }

             public bool CommissionFeeSpecified { get { return Math.Abs(BrokerFee - 0) > double.Epsilon; } }

        public string CommissionFeeCurrency { get; set; }

        public double TradingFee { get; set; }

             public bool TradingFeeSpecified { get { return Math.Abs(BrokerFee - 0) > double.Epsilon; } }

        public string TradingFeeCurrency { get; set; }

    }
}