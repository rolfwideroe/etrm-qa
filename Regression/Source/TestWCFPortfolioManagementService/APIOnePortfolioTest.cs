using System;
using ElvizTestUtils;
using ElvizTestUtils.PortfolioManagementServiceReference;
using NUnit.Framework;

namespace TestWCFPortfolioManagementService
{
    class APIOnePortfolioTest
    {
        [Test]
        public void CreatePortfolioDtos()
        {
            IPortfolioManagementService service = WCFClientUtil.GetPortfolioManagementServiceProxy();
          
            PortfolioDto myPortfolio = new PortfolioDto
            {
                ExternalId = Guid.NewGuid().ToString(),
                Name = "PortfolioName" ,
                ParentPortfolioExternalId = "Test Company A",
                CompanyExternalId = "Test Company A",
                Status = "Active"
            };
            
            try
            {
                service.CreatePortfolios(new[] { myPortfolio });
            }
            catch (Exception)
            {

                throw;
            }
            PortfolioDto[] foundPortfolio = service.FindPortfolios(new string[] { myPortfolio.ExternalId });

           // Console.WriteLine(myPortfolio.ExternalId);
         
            Assert.AreEqual(myPortfolio.Name, foundPortfolio[0].Name);
            Assert.AreEqual(myPortfolio.ExternalId, foundPortfolio[0].ExternalId);
            Assert.AreEqual(myPortfolio.CompanyExternalId, foundPortfolio[0].CompanyExternalId);
            Assert.AreEqual(myPortfolio.Status, foundPortfolio[0].Status);
            Assert.AreEqual(myPortfolio.ParentPortfolioExternalId, foundPortfolio[0].ParentPortfolioExternalId);

           

        }
    }
}
