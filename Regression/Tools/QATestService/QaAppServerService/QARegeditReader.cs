using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace QATestService
{

    public class QARegeditReader : ServiceBase
    {
        public ServiceHost serviceHost = null;

        public QARegeditReader()
        {
            // Name the Windows Service
            ServiceName = "QATestService";
        }

        public static void Main()
            {
                ServiceBase.Run(new QARegeditReader());
            }

            // Start the Windows service.
            protected override void OnStart(string[] args)
            {
                string server = Environment.MachineName;
                string URL = "http://" + server + ":8009/QATestService";
                Uri baseAddress = new Uri(URL); //("http://localhost:8009/QATestService");
               // serviceHost = new ServiceHost(typeof(QaAppServerService));
                serviceHost = new ServiceHost(typeof(QaAppServerService),baseAddress);
                serviceHost.AddServiceEndpoint(typeof(IQaAppServerService), new BasicHttpBinding(), baseAddress);
                ServiceMetadataBehavior serviceMetadataBehavior = new ServiceMetadataBehavior
                {
                    HttpGetEnabled = true,
                    MetadataExporter = {PolicyVersion = PolicyVersion.Policy15}
                };

                serviceHost.Description.Behaviors.Add(serviceMetadataBehavior);

                ServiceDebugBehavior debug = serviceHost.Description.Behaviors.Find<ServiceDebugBehavior>();
                // if not found - add behavior with setting turned on 
                if (debug == null)
                {
                    serviceHost.Description.Behaviors.Add(
                         new ServiceDebugBehavior() { IncludeExceptionDetailInFaults = true });
                }
                else
                {
                    // make sure setting is turned ON
                    if (!debug.IncludeExceptionDetailInFaults)
                    {
                        debug.IncludeExceptionDetailInFaults = true;
                    }
                }

              serviceHost.AddServiceEndpoint(ServiceMetadataBehavior.MexContractName, MetadataExchangeBindings.CreateMexHttpBinding(), "mex");
              // Open the ServiceHostBase to create listeners and start listening for messages.
              serviceHost.Open();
            
           }

            // Stop the Windows service.
            protected override void OnStop()
            {
                if (serviceHost != null)
                {
                    serviceHost.Close();
                    serviceHost = null;
                }
            }

        }
    
}
