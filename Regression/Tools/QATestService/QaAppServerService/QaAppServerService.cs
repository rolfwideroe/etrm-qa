using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.ServiceProcess;
using System.Configuration;
using System.Configuration.Install;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using Microsoft.Win32;

namespace QATestService
{

    public partial class QaAppServerService : IQaAppServerService
    {
        const string HLM_Root = @"HKEY_LOCAL_MACHINE\";

        const string AppServerRegistryElvizKey =
            @"SOFTWARE\Wow6432Node\VIZ Risk Management Services\Elviz\Contract Manager\";

        const string AppServerRegistryElvizInstallInfo =
            @"SOFTWARE\Wow6432Node\VIZ Risk Management Services\Elviz\InstallInfo\";

        public enum ElvizStringRegKeys
        {
            ElvizUserName,
            ElvizPassword,
            VizECM,
            VizPrices,
            Vizsystem,
            VizDatawarehouse,
            ReportingDatabase,
            MembershipServiceURL
        };

        public enum ElvizDWordRegKeys
        {
            UseMembershipAuthentication,
            UseIntegratedSecurity
        };

        public enum ElvizDWordRegValues { On = 1, Off = 0 };
  

        public KeyValuePair<string, ElvizDWordRegValues> SetSettingsDWord(ElvizDWordRegKeys keyName, ElvizDWordRegValues keyValue)
       {

           KeyValuePair<string, ElvizDWordRegValues> settings;

           string keyNameString = keyName.ToString();

            string sqlserver = GetSqlServerName();
           string AppServerRegistryKeyNameToSet = HLM_Root + AppServerRegistryElvizKey + sqlserver;

           try
           {
               Registry.SetValue(AppServerRegistryKeyNameToSet, keyNameString, keyValue, RegistryValueKind.DWord);
               string value = GetRegistryValue(AppServerRegistryElvizKey + sqlserver, keyNameString);
               ElvizDWordRegValues res = (ElvizDWordRegValues) Enum.Parse(typeof(ElvizDWordRegValues), value);

               settings = new KeyValuePair<string, ElvizDWordRegValues>(keyNameString, res);

            }
           catch (Exception ex)
           {
               settings = new KeyValuePair<string, ElvizDWordRegValues>(ex.Message, ElvizDWordRegValues.Off);
               return settings;
           }

            return settings;

       }

       public KeyValuePair<string, string> SetSettingsString(ElvizStringRegKeys keyName, string keyValue)
       {

           KeyValuePair<string, string> settings;// = new KeyValuePair<string, string>();

           string sqlserver = GetSqlServerName();
           string keyNameString = keyName.ToString();
           string AppServerRegistryKeyNameToSet = HLM_Root + AppServerRegistryElvizKey + sqlserver;

           try
           {
               {
                   Registry.SetValue(AppServerRegistryKeyNameToSet, keyNameString, keyValue, RegistryValueKind.String);
                   string value = GetRegistryValue(AppServerRegistryElvizKey + sqlserver, keyNameString);
                   settings = new KeyValuePair<string, string>(keyNameString, value); 
               }
           }
           catch (Exception ex)
           {
               
               settings = new KeyValuePair<string, string>("Error", ex.Message);

                return settings;
           }

           return settings;

       }
        public IDictionary<string, string> GetSettings(string[] settingNames)
        {
            string sqlserver = GetSqlServerName();
            string AppServerRegistryEcmDbName = AppServerRegistryElvizKey + sqlserver;

            foreach (var param in settingNames)
            {
               if (param == null || param == string.Empty)
                    throw new ApplicationException(string.Format(@"SettingName can not be empty"));
            }

            IDictionary<string,string> settings=new Dictionary<string, string>();

            if (settingNames.Any(x => x.ToUpper() == "SQLSERVERNAME"))
            {
               settings.Add("SQLServerName", sqlserver);
            }

            if (settingNames.Any(x => x.ToUpper() == "VIZECM"))
            {
                string value = GetRegistryValue(AppServerRegistryEcmDbName,"VizECM");
                settings.Add("VizECM", value);
            }

            if (settingNames.Any(x => x.ToUpper() == "VIZSYSTEM"))
            {
                string value = GetRegistryValue(AppServerRegistryEcmDbName, "VizSystem");
                settings.Add("VizSystem", value);
            }

            if (settingNames.Any(x => x.ToUpper() == "VIZPRICES"))
            {
                string value = GetRegistryValue(AppServerRegistryEcmDbName, "VizPrices");
                settings.Add("VizPrices", value);
            }

            if (settingNames.Any(x => x.ToUpper() == "VIZFORWARDCURVE"))
            {
                string value = GetRegistryValue(AppServerRegistryEcmDbName, "VizForwardCurve");
                settings.Add("VizForwardCurve", value);
            }

            if (settingNames.Any(x => x.ToUpper() == "VIZDATAWAREHOUSE"))
            {
                string value = GetRegistryValue(AppServerRegistryEcmDbName, "VizDatawarehouse");
                settings.Add("VizDatawarehouse", value);
            }

            if (settingNames.Any(x => x.ToUpper() == "REPORTINGDATABASE"))
            {
                string value = GetRegistryValue(AppServerRegistryEcmDbName, "ReportingDatabase");
                settings.Add("ReportingDatabase", value);
            }

            if (settingNames.Any(x => x.ToUpper() == "CRVDATABASENAME"))
            {
                string value = GetRegistryValueIfExist(AppServerRegistryEcmDbName, "CrvDatabaseName");
                if (!string.IsNullOrEmpty(value)) settings.Add("CrvDatabaseName", value);
            }

            if (settingNames.Any(x => x.ToUpper() == "BCRCONTRACTDATABASE"))
            {
                string value = GetRegistryValue(AppServerRegistryEcmDbName, "BcrContractDatabase");
                settings.Add("BcrContractDatabase", value);
            }

            if (settingNames.Any(x => x.ToUpper() == "BCRCONTRACTDATABASESERVER"))
            {
                string value = GetRegistryValue(AppServerRegistryEcmDbName, "BcrContractDatabaseServer");
                settings.Add("BcrContractDatabaseServer", value);
            }

            if (settingNames.Any(x => x.ToUpper() == "BCRURL"))
            {
                string value = GetRegistryValue(AppServerRegistryEcmDbName, "BcrUrl");
                settings.Add("BcrUrl", value);
            }
            if (settingNames.Any(x => x.ToUpper() == "DEFAULTUSER"))
            {
                string value = GetRegistryValueIfExist(AppServerRegistryEcmDbName, "DefaultUser");
                settings.Add("DefaultUser", value);
            }

            if (settingNames.Any(x => x.ToUpper() == "DEFAULTPW"))
            {
              
                string value = GetRegistryValueIfExist(AppServerRegistryEcmDbName, "DefaultPw");
                settings.Add("DefaultPw", value);
            }
            
            if (settingNames.Any(x => x.ToUpper() == "USEINTEGRATEDSECURITY"))
            {
                string value = GetRegistryValue(AppServerRegistryEcmDbName, "UseIntegratedSecurity");
                settings.Add("UseIntegratedSecurity", value);
            }
           
            if (settingNames.Any(x => x.ToUpper() == "USEMEMBERSHIPAUTHENTICATION"))
            {
                string value = GetRegistryValue(AppServerRegistryEcmDbName, "UseMembershipAuthentication");
                settings.Add(" UseMembershipAuthentication", value);
            }

             if (settingNames.Any(x => x.ToUpper() == "MEMBERSHIPSERVICEURL"))
            {
                string value = GetRegistryValue(AppServerRegistryEcmDbName, "MembershipServiceURL");
                settings.Add("MembershipServiceURL", value);
            }


            if (settingNames.Any(x => x.ToUpper() == "ELVIZUSERNAME"))
            {
                string value = GetRegistryValue(AppServerRegistryEcmDbName, "ElvizUserName");
                settings.Add(" ElvizUserName", value);
            }

            if (settingNames.Any(x => x.ToUpper() == "ELVIZPASSWORD"))
            {
                string value = GetRegistryValue(AppServerRegistryEcmDbName, "ElvizPassword");
                settings.Add(" ElvizPassword", value);
            }

            if (settingNames.Any(x => x.ToUpper() == "ELVIZVERSION"))
            {
                string value = GetRegistryValue(AppServerRegistryElvizInstallInfo, "ELVIZVERSION");
                settings.Add("ElvizVersion", value);
            }
            
            if (settings.Count > 0) return settings;

                return null;
        }

        private static string GetRegistryValue(string path, string valueName)
        {
        	string res = string.Empty;
            RegistryKey regKey = Registry.LocalMachine;
            regKey= regKey.OpenSubKey(path);
            if (regKey != null)
            {
                res = (string)regKey.GetValue(valueName).ToString();
               // res = Registry.GetValue(path, valueName, "").ToString();
            }
            if (string.IsNullOrEmpty(res))
                throw new ApplicationException(string.Format(@"Cannot read key value from {0}->{1}", path, valueName));

            return res;
        }

        private static string GetRegistryValueIfExist(string path, string valueName)
        {
            string res = string.Empty;
            RegistryKey regKey = Registry.LocalMachine;
            regKey = regKey.OpenSubKey(path);
            if (regKey != null)
            {
                res = (string) regKey.GetValue(valueName);
                // res = Registry.GetValue(path, valueName, "").ToString();
            }
            return res;
        }



        private static string GetSqlServerName()
      {
            string res = string.Empty;
            RegistryKey regKey = Registry.LocalMachine;
            regKey = regKey.OpenSubKey(@"Software\Wow6432Node\VIZ Risk Management Services\Elviz\Contract Manager\Servers");

            if (regKey != null)
            {
               res = (string) regKey.GetValue("Default");
            }

            if (string.IsNullOrEmpty(res))
                throw new ApplicationException(string.Format(@"Cannot read sql server name from {0}Server\Default ", AppServerRegistryElvizKey));
            
          return res;
     }
         
   }
}

   
