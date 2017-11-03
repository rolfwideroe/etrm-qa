using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ElvizTestUtils.InternalJobService;
using ElvizTestUtils.QaLookUp;
using ElvizTestUtils.QAAppserverServiceReference;
using NUnit.Framework;

namespace ElvizTestUtils
{
    public class ElvizRegistyConfigurationTool
    {

        //[Test]
        public static void RestartElvizServices ()
        {
          
            IQaAppServerService service = WCFClientUtil.GetQaAppServerServiceClient();
            try
            {
                string appServerName = ElvizInstallationUtility.GetAppServerName();
                Console.WriteLine("Restarting Elviz services for: " + appServerName);
                service.RestartElvizServices();
            }
            catch (Exception ex)
            {
                throw new ArgumentException("Restarting services failed: " + ex.Message);
            }
            
        }

        public static IDictionary<string, string> GetElvizStringValuesFromRegistry(string[] settingNames)
        {
           IDictionary<string, string> elvizSettings =new Dictionary<string, string>();

            IQaAppServerService service = WCFClientUtil.GetQaAppServerServiceClient();
            try
            {
                elvizSettings = service.GetSettings(settingNames);
            }
            catch (Exception ex)
            {
                throw new ArgumentException("Could not update setting: " + ex.Message);
            }

            return elvizSettings;
        }
        //for one values
        public static IDictionary<string, string> GetElvizStringValuesFromRegistry(string settingName)
        {
            IDictionary<string, string> elvizSettings = new Dictionary<string, string>();

            string[] settingNames = new[] {settingName};

            IQaAppServerService service = WCFClientUtil.GetQaAppServerServiceClient();
            try
            {
                elvizSettings = service.GetSettings(settingNames);
            }
            catch (Exception ex)
            {
                throw new ArgumentException("Could not update setting: " + ex.Message);
            }

            return elvizSettings;
        }

        public static KeyValuePair<string, string> SetElvizStringValueInRegistry(QaAppServerServiceElvizStringRegKeys key, string value)
        {
            KeyValuePair< string, string> newSettings = new KeyValuePair<string, string>();

            //string elvizUserName = "Vizard";
            //string elvizPassword = "elviz";
            IQaAppServerService service = WCFClientUtil.GetQaAppServerServiceClient();
            try
            {
                newSettings = service.SetSettingsString(key, value);
            }
            catch (Exception ex)
            {
                throw new ArgumentException("Could not update setting: " + ex.Message);
            }

            return newSettings;
    
        }

        public static KeyValuePair<string, QaAppServerServiceElvizDWordRegValues> SetElvizDWordValueInRegistry(QaAppServerServiceElvizDWordRegKeys key, QaAppServerServiceElvizDWordRegValues value)
        {
            KeyValuePair<string, QaAppServerServiceElvizDWordRegValues> newSettings = new KeyValuePair<string, QaAppServerServiceElvizDWordRegValues>();

            IQaAppServerService service = WCFClientUtil.GetQaAppServerServiceClient();
            try
            {
                newSettings = service.SetSettingsDWord(key, value);
            }
            catch (Exception ex)
            {
                throw new ArgumentException("Could not update setting: " + ex.Message);
            }

            return newSettings;

        }
    }
}
