using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ElvizTestUtils.DatabaseTools;
using ElvizTestUtils.PortfolioManagementServiceReference;
using LoadStaticData;
using NUnit.Framework;

namespace ElvizTestUtils.LoadStaticData
{
    public class LoadStaticData
    {
        private static readonly IEnumerable<string> ScriptFiles = TestCasesFileEnumeratorByFolder.TestCaseFiles(EcmScriptFolder);
        private const string EcmScriptFolder = "..\\StaticData\\VizECMDbScripts\\";

        public void RunEcmDbScripts()
        {
            string ecmDbName = ElvizInstallationUtility.GetEtrmDbName("VizECM");
            string sqlServerName = ElvizInstallationUtility.GetSqlServerName();
            SqlConnection connection = new SqlConnection(
                $"Data Source={sqlServerName};Initial Catalog={ecmDbName};Trusted_Connection=True;");

            foreach (string scriptFile in ScriptFiles)
            {
                string scriptPath = Path.Combine(EcmScriptFolder, scriptFile);

                string script = File.ReadAllText(scriptPath);

                QaDao.NonQuery(connection, script);
            }

        
        }

        public void InsertPortfolios()
        {
            QaDao qaDao = new QaDao();



            string portfolioTestFile = Path.Combine(Directory.GetCurrentDirectory() + "..\\..\\StaticData\\Portfolios\\PortfolioInsert.xml");

            PortfolioInsert portfolioPortfolioInsert = TestXmlTool.Deserialize<PortfolioInsert>(portfolioTestFile);
            foreach (Portfolio portfolioItem in portfolioPortfolioInsert.Portfolios)
            {
                IPortfolioManagementService service = WCFClientUtil.GetPortfolioManagementServiceProxy();

                PortfolioDto[] dtos = service.FindPortfolios(new[] { portfolioItem.PortfolioNameAndExternalId });

                if (dtos.Length == 0)
                {
                    PortfolioDto myPortfolio = new PortfolioDto
                    {
                        ExternalId = portfolioItem.PortfolioNameAndExternalId,
                        Name = portfolioItem.PortfolioNameAndExternalId,
                        ParentPortfolioExternalId = null,//portfolioItem.ParentPortfolioExternalId,
                        CompanyExternalId = portfolioItem.CompanyExternalId,
                        Status = "Active"
                    };

                    try
                    {
                        service.CreatePortfolios(new[] { myPortfolio });
                        qaDao.CreateFilterForPortfolioAndSetOptionAllowed(portfolioItem.PortfolioNameAndExternalId);
                    }



                    catch (Exception ex)
                    {
                        Assert.AreEqual("Failed to create portfolio or Filter", ex.Message);
                    }

                }
                else Console.WriteLine("Portfolio with ExternalId = " + portfolioItem.PortfolioNameAndExternalId + " already exist.");
            }
        }
    }
}
