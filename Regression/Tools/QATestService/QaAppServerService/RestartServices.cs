using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace QATestService
{
    public partial class QaAppServerService : IQaAppServerService
    {

        public void RestartElvizServices()
        {
            StopElvizServices();
            Thread.Sleep(5000);
            StartElvizServices();
        }
        public void StopElvizServices()
        {
            string[] servicesStop =
            {
                "Brady.ETRM.File.Watching", "Brady.ETRM.WCF.Publishing","Brady.ETRM.Message.Queue.Listener",
                "Brady.ETRM", "Brady.ETRM.Priceboard"
            };

            string mashinename = Environment.MachineName;
            foreach (string serviceName in servicesStop)
            {
                ServiceController service = new ServiceController(serviceName, mashinename);
                if (service.Status == ServiceControllerStatus.Running)
                {
                    if (serviceName == "Brady.ETRM.Message.Queue.Listener")
                    {
                        try
                        {
                            Process[] proc = Process.GetProcessesByName("Viz.Integration.Core.MessageQueueListener.exe");
                            proc[0].Kill();
                            //process should be killed twice, it recovers after first attempt 
                            Thread.Sleep(15000);
                            if (service.Status == ServiceControllerStatus.Running)
                            {
                                Process[] procRecovered = Process.GetProcessesByName("Viz.Integration.Core.MessageQueueListener.exe");
                                procRecovered[0].Kill();
                            }

                            break;
                        }

                        catch (Exception)
                        {
                            // ignored
                        }
                    }

                    service.Stop();
                    service.WaitForStatus(ServiceControllerStatus.Stopped);

                    if (serviceName == "Brady.ETRM" || serviceName == "Brady.ETRM.Priceboard")
                        Thread.Sleep(30000);
                }
                if(service.Status!=ServiceControllerStatus.Stopped) throw new ArgumentException("The service: "+serviceName+" was not stopped");
                
            }
        }

        public void StartElvizServices()
        {
         
            string[] servicesStart =
            {
                  "Brady.ETRM.Priceboard", "Brady.ETRM", "Brady.ETRM.WCF.Publishing", 
                "Brady.ETRM.File.Watching", "Brady.ETRM.Message.Queue.Listener"
                
            };
            string mashinename = Environment.MachineName;

            foreach (string serviceName in servicesStart)
            {
                ServiceController service = new ServiceController(serviceName, mashinename);

                if (service.Status == ServiceControllerStatus.Stopped)
                {
                    service.Start();
                    service.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(30));
                }
                if (service.Status != ServiceControllerStatus.Running) throw new ArgumentException("The service: " + serviceName + " is not running");
            }
        }

    
    }
}
