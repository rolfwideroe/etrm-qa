using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ElvizTestUtils;
using ElvizTestUtils.QAAppserverServiceReference;

namespace QAAppServerServiceClientConsole
{
    class Program
    {
        static int Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine("Please enter on correct argument: ElvizVersion, PatchDB");
                return 1;
            }

            string arg = args[0];

            switch (arg.ToUpper())
            {
                case "ELVIZVERSION":
                {
                    const string setting = "ElvizVersion";
                    const string error = "Could not find a Elviz Version";

                    try
                    {
                        GetSetting(setting, error); 
                    }
                    catch (Exception ex)
                    {
                        
                        Console.WriteLine(ex.Message);
                        return 1;
                    }
                
                }
                break;
                case "PATCHDB":
                {
                    QaAppServerServiceClient client = WCFClientUtil.GetQaAppServerServiceClient();
                    try
                    {
                        client.ReinstallDbPatch();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        return 1;
                    }
                }
                break;
                case "STARTELVIZSERVICES":
                {
                    QaAppServerServiceClient client = WCFClientUtil.GetQaAppServerServiceClient();
                    try
                    {
                        client.StartElvizServices();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        return 1;
                    }
                }
                break;
                case "STOPELVIZSERVICES":
                {
                    QaAppServerServiceClient client = WCFClientUtil.GetQaAppServerServiceClient();
                    try
                    {
                        client.StopElvizServices();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        return 1;
                    }
                }
                break;
                default:
                {
                    Console.WriteLine("The argument: "+arg+" is not valid. Valid arguments are: ElvizVersion,PatchDB");
                    return 1;
                }
            }

         

            return 0;
        }

        private static void GetSetting(string setting, string errorMessage)
        {
            QaAppServerServiceClient client = WCFClientUtil.GetQaAppServerServiceClient();
            IDictionary<string, string> settings = client.GetSettings(new[] { setting });
            if (settings == null) throw new ArgumentException(ElvizInstallationUtility.GetAppServerName() + " - " + errorMessage);
            if (!settings.ContainsKey(setting)) throw new ArgumentException(ElvizInstallationUtility.GetAppServerName()+" - "+errorMessage);
            Console.WriteLine(settings[setting]);
        }
    }
}
