using System;
using System.IO;
using System.ServiceModel;
using System.Xml;
using ElvizTestUtils;
using ElvizTestUtils.DealServiceReference;
using ElvizTestUtils.LookUpServiceReference;
using ElvizTestUtils.PortfolioManagementServiceReference;
using NUnit.Framework;

namespace TestWCFPortfolioManagementService
{
    internal class InsertDealAfterCreatingCompany
    {
        

        [Test]
        public void InsertDeal()
        {
            IPortfolioManagementService service = WCFClientUtil.GetPortfolioManagementServiceProxy();
            IDealService dealServiceClient = WCFClientUtil.GetDealServiceServiceProxy();

            string companyExternalID = Guid.NewGuid().ToString();
            // create company 
            CompanyDto myCompany = new CompanyDto
            {
                CountryIsoCode = "NO",
                ExternalId = companyExternalID,
                Name = "TestCompanyInsertDeal",
                OrgNo = "OrgNo",
                Status = "Active",
                Roles = new[] {CompanyRole.Customer, CompanyRole.Broker, CompanyRole.Exchange}
            };
            try
            {
                service.CreateCompanies((new[] {myCompany}));
            }
            catch (FaultException<ExceptionDetail> ex)
            {
                Assert.Fail("Creating company failed: " + ex.Message);
            }
            
            //create portfolio
            string portfolioExternalId = Guid.NewGuid().ToString();
            string PortfolioName = "Portfolio-" + portfolioExternalId;
            PortfolioDto portfolioChild = new PortfolioDto
            {
                ExternalId = portfolioExternalId,
                ParentPortfolioExternalId = null,
                CompanyExternalId = companyExternalID,
                Name = PortfolioName,
                Status = "Active",
            };
            try
            {
                service.CreatePortfolios(new[] { portfolioChild });
            }
            catch (FaultException<ExceptionDetail> ex)
            {
                Assert.Fail("Creating portfolio failed: " + ex.Message);
            }
            
            XmlDocument doc = new XmlDocument();
            string testFilePath = Path.Combine(Directory.GetCurrentDirectory(), "TestFiles\\DealInsert\\PassElectricityForward.xml");
            doc.Load(testFilePath); // input XML
            XmlNodeList portfolioNode = doc.GetElementsByTagName("Portfolio");
            portfolioNode[0].InnerText = portfolioExternalId;

            int insertedTransactionID = 0;
            //insert deal where portfolio = just created portfolio
            try
            {
               string result =  dealServiceClient.ImportDeal(doc.InnerXml);
                Console.WriteLine(result);
               XmlDocument resultXml = new XmlDocument();
               resultXml.LoadXml(result);
               XmlNodeList resNode = resultXml.GetElementsByTagName("Result");
                if (resNode.Item(0).InnerXml == "Success")
                {
                    XmlNodeList IdNode = resultXml.GetElementsByTagName("TransactionId");
                    insertedTransactionID = Convert.ToInt32(IdNode[0].InnerText);
                     //check portfolio value for created transaction
                    ILookupService serviceLookUp = WCFClientUtil.GetLookUpServiceServiceProxy();
                    int[] ids = new[] { insertedTransactionID };
                    TransactionDTO[] dtos = serviceLookUp.GetTransactionsByIds(ids);
                    TransactionDTO dto = dtos[0];
                    Console.WriteLine(portfolioExternalId + "=" + dto.Portfolio.ExternalId);
                    Assert.AreEqual(portfolioExternalId, dto.Portfolio.ExternalId, "Created transaction has wrong Portfolio ");
                }
                else
                {
                    XmlNode xmlNode = resultXml.GetElementsByTagName("Message").Item(0);
                    if (xmlNode != null)
                        Assert.Fail("Inserting deal failed: " + xmlNode.InnerXml);
                }
            }
            catch (Exception ex)
            {
                Assert.Fail("Inserting deal failed: " + ex.Message);
            }
           
        }
    }
}
