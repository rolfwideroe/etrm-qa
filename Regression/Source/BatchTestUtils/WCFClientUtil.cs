using System;
using System.ServiceModel;
using ElvizTestUtils.CurrencyAccountsServiceReference;
using ElvizTestUtils.CurveServiceReference;
using ElvizTestUtils.DealServiceReference;
using ElvizTestUtils.HistoricMarketDataServiceReference;
using ElvizTestUtils.InternalJobService;
using ElvizTestUtils.InternalReportingServiceReference;
using ElvizTestUtils.LookUpServiceReference;
using ElvizTestUtils.PortfolioManagementServiceReference;
using ElvizTestUtils.QAAppserverServiceReference;
using ElvizTestUtils.VolatilityServiceReference;

namespace ElvizTestUtils
{
    public class WCFClientUtil
    {
        public static BasicHttpBinding GetDefaultBasicHttpBinding()
        {
            BasicHttpBinding basicHttpBinding = new BasicHttpBinding();
            basicHttpBinding.CloseTimeout = new TimeSpan(0, 0, 20, 0);
            basicHttpBinding.OpenTimeout = new TimeSpan(0, 0, 20, 0);
            basicHttpBinding.ReceiveTimeout = new TimeSpan(0, 0, 20, 0);
            basicHttpBinding.SendTimeout = new TimeSpan(0, 0, 20, 0);
            basicHttpBinding.MaxBufferSize = 2147483647;
            basicHttpBinding.MaxBufferPoolSize = 2147483647;
            basicHttpBinding.MaxReceivedMessageSize = 2147483647;
            basicHttpBinding.ReaderQuotas.MaxDepth = 2147483647;
            basicHttpBinding.ReaderQuotas.MaxStringContentLength = 2147483647;
            basicHttpBinding.ReaderQuotas.MaxArrayLength = 2147483647;
            basicHttpBinding.ReaderQuotas.MaxBytesPerRead = 2147483647;

            return basicHttpBinding;
        }

        public static EndpointAddress GetDefaultEndPointAdresss(string url)
        {
            string appServerName = ElvizInstallationUtility.GetAppServerName();
            return new EndpointAddress(string.Format(url, appServerName));
        }

        public static EndpointAddress GetDefaultEndPointAdresss(string url, string appServerName)
        {            
            return new EndpointAddress(string.Format(url, appServerName));
        }
        private const string LookUpServiceUrl = "http://{0}:8009/LookUpService";
        private const string CurveServiceUrl = "http://{0}:8009/CurveService";
        private const string DealServiceUrl = "http://{0}:8009/DealService";
        private const string HistoricMarketDataServiceUrl = "http://{0}:8009/HistoricMarketDataService";
        private const string PortfolioManagementServiceUrl = "http://{0}:8009/PortfolioManagementService";
        private const string InternalReportingServiceUrl = "http://{0}:8009/InternalReportingServiceAPI";
        private const string VolatilityServiceUrl = "http://{0}:8009/VolatilityService";
        private const string CurrencyAccountServiceUrl = "http://{0}:8009/CurrencyAccountService";
        private const string QaAppServerServiceUrl = "http://{0}:8009/QaTestService";
        private const string JobServiceUrl = "http://{0}:8009/JobService";

        public static LookupServiceClient GetLookUpServiceServiceProxy()
        {
            return new LookupServiceClient(GetDefaultBasicHttpBinding(), GetDefaultEndPointAdresss(LookUpServiceUrl));
        }

        public static LookupServiceClient GetLookUpServiceServiceProxy(string appServerName)
        {
            return new LookupServiceClient(GetDefaultBasicHttpBinding(), new EndpointAddress(string.Format(LookUpServiceUrl, appServerName)));
        }

        public static CurveServiceClient GetCurveServiceServiceProxy()
        {
            return new CurveServiceClient(GetDefaultBasicHttpBinding(), GetDefaultEndPointAdresss(CurveServiceUrl));
        }

        public static DealServiceClient GetDealServiceServiceProxy()
        {
            return new DealServiceClient(GetDefaultBasicHttpBinding(), GetDefaultEndPointAdresss(DealServiceUrl));
        }

        public static HistoricMarketDataServiceClient GetHistoricMarketDateServiceProxy()
        {
            return new HistoricMarketDataServiceClient(GetDefaultBasicHttpBinding(), GetDefaultEndPointAdresss(HistoricMarketDataServiceUrl));
        }

        public static PortfolioManagementServiceClient GetPortfolioManagementServiceProxy()
        {
            return new PortfolioManagementServiceClient(GetDefaultBasicHttpBinding(), GetDefaultEndPointAdresss(PortfolioManagementServiceUrl));
        }

        public static InternalReportingApiServiceClient GetInternalReportingServiceProxy()
        {
            return new InternalReportingApiServiceClient(GetDefaultBasicHttpBinding(), GetDefaultEndPointAdresss(InternalReportingServiceUrl));
        }

        public static VolatilityServiceClient GetVolatilityServiceProxy()
        {
            return new VolatilityServiceClient(GetDefaultBasicHttpBinding(), GetDefaultEndPointAdresss(VolatilityServiceUrl));
        }

        public static CurrencyAccountServiceClient GetCurrencyAccountServiceProxy()
        {
            return new CurrencyAccountServiceClient(GetDefaultBasicHttpBinding(), GetDefaultEndPointAdresss(CurrencyAccountServiceUrl));
        }

        public static QaAppServerServiceClient GetQaAppServerServiceClient()
        {
            return new QaAppServerServiceClient(GetDefaultBasicHttpBinding(), GetDefaultEndPointAdresss(QaAppServerServiceUrl));
        }

        public static JobServiceClient GetJobServiceClient()
        {
            return new JobServiceClient(GetDefaultBasicHttpBinding(), GetDefaultEndPointAdresss(JobServiceUrl));
        }

        public static JobServiceClient GetJobServiceClient(string appServerName)
        {
            return new JobServiceClient(GetDefaultBasicHttpBinding(), new EndpointAddress(string.Format(JobServiceUrl, appServerName)));
        }
    }
}
