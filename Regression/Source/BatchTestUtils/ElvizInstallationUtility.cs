using ElvizTestUtils.QAAppserverServiceReference;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;

namespace ElvizTestUtils
{
    public class ElvizInstallationUtility
    {

        const string ClientRegistryPath = "HKEY_LOCAL_MACHINE\\Software\\Wow6432Node\\Elviz ETRM\\InstallInfo";

        private static readonly string AppServerRegistryKeyDwhName =
            @"HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\VIZ Risk Management Services\Elviz\Contract Manager\" +
            GetSqlServerName();

        private static readonly string AppServerRegistryEcmDbName =
            @"HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\VIZ Risk Management Services\Elviz\Contract Manager\" +
            GetSqlServerName();



        public static string GetEtrmHome()
        {
            const string variableName = "Elviz_ETRM_HOME";

            string etrmHome = Environment.GetEnvironmentVariable(variableName, EnvironmentVariableTarget.Machine);

            if (etrmHome == null) throw new ArgumentException("No ETRM Home Environment Variable");

            return etrmHome;
        }


        public static string GetRegistryValue(string path, string valueName)
        {
            string res = string.Empty;

            res = Registry.GetValue(path, valueName, "").ToString();


            if (string.IsNullOrEmpty(res))
                throw new ApplicationException(string.Format(@"Cannot read sql server name from {0}->{1}", path,
                    valueName));

            return res;
        }

        public static string ElvizVersion
        {
            get
            {
                if (IsClientInstallation())
                {
                    return Registry.GetValue(
                        @"HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\Elviz ETRM\InstallInfo", "02_INSTALLDIR",
                        @"C:\ElvizClient").ToString();
                }

                return Registry.GetValue(
                    @"HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\Elviz ETRM\InstallInfo", "ELVIZVERSION", "2015.x.x.x")
                    .ToString();

            }
        }

        public static string AcerXmlFolder(string emirServerName)
        {
            // get
            {
                if (IsClientInstallation())
                {
                    // string server = GetAppServerName();
                    string resultClientFileDirectory = @"\\" + emirServerName + "\\AcerXml";
                    return resultClientFileDirectory;
                }

                string etrmBinFolder = ElvizInstallationUtility.GetEtrmHome();
                string etrmInstallFolder = Path.GetFullPath(Path.Combine(etrmBinFolder, @"..\"));
                string resultFileDirectory = etrmInstallFolder + @"\Emir\AcerXml";
                return resultFileDirectory;

            }
        }

        public static string RegisTRXmlFolder(string emirServerName)
        {
            // get
            {
                if (IsClientInstallation())
                {
                    // string server = GetAppServerName();
                    string resultClientFileDirectory = @"\\" + emirServerName + "\\RegisTRXml";
                    return resultClientFileDirectory;
                }

                string etrmBinFolder = ElvizInstallationUtility.GetEtrmHome();
                string etrmInstallFolder = Path.GetFullPath(Path.Combine(etrmBinFolder, @"..\"));
                string resultFileDirectory = etrmInstallFolder + @"\Emir\RegisTRXml";
                return resultFileDirectory;

            }
        }


        /// <summary>
        /// Checks whether test runs on client installation
        /// </summary>
        /// <returns>true - if runs on the client installation, false - if runs on server installation</returns>
        public static bool IsClientInstallation()
        {
            const string app_server = "01_APP_SERVER";

            string value = (string)Registry.GetValue(ClientRegistryPath, app_server, String.Empty);

            return !String.IsNullOrEmpty(value);
        }

        public static string GetAppServerName()
        {
            if (IsClientInstallation())
            {
                const string app_server_name = "03_APP_SERVER_NAME";

                return (string)Registry.GetValue(ClientRegistryPath, app_server_name, String.Empty);
            }
            return Environment.MachineName;
        }

        public static string GetElvizVersion()
        {
            if (IsClientInstallation())
            {
                const string elvizversion = "ELVIZVERSION";

                return (string)Registry.GetValue(ClientRegistryPath, elvizversion, String.Empty);
            }
            return "2015.x.x.x";
        }

        public static string GetEtrmDbName(string dbType)
        {
            //VizDatawarehouse
            //VizSystem
            //VizECM
            //VizPrices

            if (IsClientInstallation())
            {
                string datawareHouse = GetSettingFromAppServer(dbType);

                return datawareHouse;
            }

            return GetRegistryValue(
                AppServerRegistryKeyDwhName, dbType);
        }


        public static string GetSqlServerName()
        {
            if (IsClientInstallation())
            {
                const string sqlServerSetting = "SQLServerName";

                string sqlServerName = GetSettingFromAppServer(sqlServerSetting);

                return sqlServerName;
            }

            return GetRegistryValue(
                @"HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\VIZ Risk Management Services\Elviz\Contract Manager\Servers",
                "Default");

        }

        private static string GetSettingFromAppServer(string settingName)
        {
            IQaAppServerService client = WCFClientUtil.GetQaAppServerServiceClient();

            IDictionary<string, string> settings = client.GetSettings(new[] { settingName });

            if (settings == null)
                throw new ArgumentException("Could not find setting: " + settingName + " on Appserver: " +
                                            GetAppServerName());

            if (settings.ContainsKey(settingName))
            {
                return settings[settingName];
            }

            throw new ArgumentException("Could not find setting: " + settingName + " on Appserver: " +
                                        GetAppServerName());
        }
    }


}
