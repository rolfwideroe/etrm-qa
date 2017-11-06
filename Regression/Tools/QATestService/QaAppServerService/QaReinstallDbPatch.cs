using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.ComponentModel;
using System.ServiceProcess;
using System.Configuration;
using System.Configuration.Install;
using Microsoft.Win32;

namespace QATestService
{
    //[TestFixture]
    public partial class QaAppServerService : IQaAppServerService
    {
       
    // [Test]
        public void ReinstallDbPatch()
        {
            UninstallDbPatch();
            Thread.Sleep(15000);
            InstallDBPatch();
        }

        private void InstallDBPatch()
        {
            string Arguments;
            string DbPatch_Path = @"C:\Elviz\Support\BradyDbPatch.msi";
            string USERNAME = "";
            string PASSWORD = "";
            string DBSYS = "";
            string DBECM = "";
            string DBPRC = "";
            string DBCRV = "";
            string DBDWH = "";
            string DBRDB = "";
            string BCR = "";
            string INTSECURITY = "";

            //Getting information from Regedit
            string SERVER = GetSqlServerName();

            IDictionary<string, string> DBsettings = new Dictionary<string, string>();
            string[] settingNames = { "DefaultUser", "DefaultPW", "VizSystem", "VizECM", "VizPrices", "VizDatawarehouse", "UseIntegratedSecurity" ,
                                      "ElvizVersion", "ReportingDatabase", "CrvDatabaseName" };

            DBsettings = GetSettings(settingNames);

            if (DBsettings != null)
            {
                if (!(DBsettings.TryGetValue("DefaultUser", out USERNAME)))
                {
                    throw new ApplicationException(
                        "Error when reading 'DefaultUser' value from regedit: " + AppServerRegistryElvizKey + "\\" +
                        SERVER);
                }

                if (!(DBsettings.TryGetValue("DefaultPw", out PASSWORD)))
                {
                    throw new ApplicationException(
                        "Error when reading 'DefaultPW' value from regedit: " + AppServerRegistryElvizKey + "\\" +
                        SERVER);
                }

                if (!(DBsettings.TryGetValue("VizSystem", out DBSYS)))
                {
                    throw new ApplicationException(
                        "Error when reading 'VizSystem' value from regedit: " + AppServerRegistryElvizKey + "\\" +
                        SERVER);
                }

                if (!(DBsettings.TryGetValue("VizECM", out DBECM)))
                {
                    throw new ApplicationException(
                        "Error when reading 'VizECM' value from regedit: " + AppServerRegistryElvizKey + "\\" + SERVER);
                }

                if (!(DBsettings.TryGetValue("VizPrices", out DBPRC)))
                {
                    throw new ApplicationException(
                        "Error when reading 'VizPrices' value from regedit: " + AppServerRegistryElvizKey + "\\" +
                        SERVER);
                }

                if (!(DBsettings.TryGetValue("VizDatawarehouse", out DBDWH)))
                {
                    throw new ApplicationException(
                        "Error when reading 'VizDatawarehouse' value from regedit: " + AppServerRegistryElvizKey + "\\" +
                        SERVER);
                }

                if (!(DBsettings.TryGetValue("ReportingDatabase", out DBRDB)))
                {
                    throw new ApplicationException(
                        "Error when reading 'ReportingDatabase' value from regedit: " + AppServerRegistryElvizKey + "\\" +
                        SERVER);
                }

                if (!(DBsettings.TryGetValue("CrvDatabaseName", out DBCRV)))
                {
                    //throw new ApplicationException(
                    //    "Error when reading 'CrvDatabaseName' value from regedit: " + AppServerRegistryElvizKey + "\\" +
                    //    SERVER);
                }

                //if (!(DBsettings.TryGetValue("BcrContractDatabase", out BCR)))
                //{
                //    throw new ApplicationException(
                //        "Error when reading 'BcrContractDatabase' value from regedit: " + AppServerRegistryElvizKey + "\\" +
                //        SERVER);
                //}

                //if (!(DBsettings.TryGetValue("BcrContractDatabaseServer", out BCR)))
                //{
                //    throw new ApplicationException(
                //        "Error when reading 'BcrContractDatabaseServer' value from regedit: " + AppServerRegistryElvizKey + "\\" +
                //        SERVER);
                //}

                //if (!(DBsettings.TryGetValue("BcrUrl", out BCR)))
                //{
                //    throw new ApplicationException(
                //        "Error when reading 'BcrUrl' value from regedit: " + AppServerRegistryElvizKey + "\\" +
                //        SERVER);
                //}


                if (!(DBsettings.TryGetValue("UseIntegratedSecurity", out INTSECURITY)))
                {
                    throw new ApplicationException(
                        "Error when reading 'UseIntegratedSecurity' value from regedit: " + AppServerRegistryElvizKey + "\\" +
                        SERVER);
                }

                foreach (KeyValuePair<string, string> keyValuePair in DBsettings)
                {
                    //Console.WriteLine(keyValuePair.Key + " = " + keyValuePair.Value);
                }
            }
            string exitcode= "null" ;
       
            if (INTSECURITY == "1")
            {
                Arguments = string.Format(
                   @"/c {0} /l* C:\Elviz\Integration\QATestService\Log\Install.log /qn ALLUSERS=1 IS_SQLSERVER_SERVER={1} IS_SQLSERVER_AUTHENTICATION=0 IS_SQLSERVER_DBSYS={2} IS_SQLSERVER_DBECM={3} IS_SQLSERVER_DBPRC={4} IS_SQLSERVER_DBDWH={5} IS_SQLSERVER_DBRD={6}",
                   DbPatch_Path, SERVER, DBSYS, DBECM, DBPRC, DBDWH, DBRDB);
            }
            else
            {
                Arguments = string.Format(
                    @"/c {0} /l* C:\Elviz\Integration\QATestService\Log\Install.log /qn ALLUSERS=1 IS_SQLSERVER_SERVER={1} IS_SQLSERVER_USERNAME={2} IS_SQLSERVER_PASSWORD={3} IS_SQLSERVER_DBSYS={4} IS_SQLSERVER_DBECM={5} IS_SQLSERVER_DBPRC={6} IS_SQLSERVER_DBDWH={7} IS_SQLSERVER_DBRD={8}",
                    DbPatch_Path, SERVER, USERNAME, PASSWORD, DBSYS, DBECM, DBPRC, DBDWH, DBRDB);

            }
            if (!string.IsNullOrEmpty(DBCRV)) Arguments = Arguments + " IS_SQLSERVER_DBCRV=" + DBCRV;


            //Create installation process
            try
            {
                string prog = "cmd";

                // string Arguments = @"/c msiexec /i C:\Elviz\Support\BradyDbPatch.msi /l* C:\Elviz\Integration\QATestService\Log\Install.log /qn ALLUSERS=1 IS_SQLSERVER_SERVER=netsv-dbs12reg IS_SQLSERVER_AUTHENTICATION=0 IS_SQLSERVER_USERNAME=EcmDbUser IS_SQLSERVER_PASSWORD=EcmDbQaReg IS_SQLSERVER_DBSYS=VizSystem_152 IS_SQLSERVER_DBECM=VizECM_152 IS_SQLSERVER_DBPRC=VizPrices_152 IS_SQLSERVER_DBDWH=VizDatawarehouse_152 IS_SQLSERVER_DBRD=VizReporting_161";
               
                ProcessStartInfo startInfo = new ProcessStartInfo(prog, Arguments);

                //Run
                Process p = Process.Start(startInfo);
                p.WaitForExit(100000); //timeOut
                exitcode = p.ExitCode.ToString();
                if (p.ExitCode != 0) 
                    throw new ApplicationException(
                        "Brady ETRM Database Patch wasn't install. Please check log for details.");
            }
            catch (Exception)
            {
                throw new ApplicationException(
                        "Brady ETRM Database Patch wasn't install. Please check log for details. Exit code = " + exitcode);
            }
        }

        //uninstalling Brady DBPatch
        private void UninstallDbPatch()
        {
            const string command = @"/c msiexec /X{E957CFF0-DB0B-43B9-ACF3-BF11EAF2EB59} /l* C:\Elviz\Integration\QATestService\Log\Uninstall.log /qn";
            try
            {
                ProcessStartInfo startInfo = new ProcessStartInfo("cmd", command);
                
                Process p = Process.Start(startInfo);
                p.WaitForExit(60000); //timeOut

                if (p.ExitCode != 0)
                {
                    if (p.ExitCode != 1605)
                        throw new ApplicationException(
                            "Brady ETRM Database Patch deinstallation failed. Please check log for details.");
                }
                if (p.HasExited == false)
                    if (p.Responding)
                        p.CloseMainWindow();
                    else
                    {
                        p.Kill();
                        throw new ApplicationException(
                        "Unstallation process of Brady ETRM Database Patch failed: not responding. Please check log for details.");
                    }
            }
            catch (Exception e)
            {
                throw new ApplicationException(e.Message);
            }
        }
    }
}

   
