using System;
using System.ServiceModel;
using System.Threading;
using ElvizTestUtils;
using ElvizTestUtils.PortfolioManagementServiceReference;
using NUnit.Framework;

namespace TestWCFPortfolioManagementService
{
    class APINegativeTestsPortfolio
    {
        private string StatusActive = "Active";
        private string StatusDeleted = "Deleted";
        // Values for this to variables should be get from Datebase
        private string PortfolioForTest_ExternalID = "Test Company B";
        private string CompanyForTest_ExternalId = "Test Company B";

        [Test]
        public void ExistenceChecking()
        {
            IPortfolioManagementService service = WCFClientUtil.GetPortfolioManagementServiceProxy();

            CompanyDto[] ExistCompanies = service.FindCompanies(new string[] { CompanyForTest_ExternalId });
            Assert.AreEqual(1, ExistCompanies.Length);
        }

        [Test]
        public void TestPortfolioCreateWrongExternalID()
        {
            IPortfolioManagementService service = WCFClientUtil.GetPortfolioManagementServiceProxy();
            // create portfolio with wrong ExternalId
            PortfolioDto myPortfolio = new PortfolioDto
            {
                ExternalId = string.Empty,
                Name = "Test Company A",
                ParentPortfolioExternalId = "parentPortfolio",
                CompanyExternalId = CompanyForTest_ExternalId, // "Id Roles",
                Status = StatusActive
            };
            try
            {
                service.CreatePortfolios(new[] { myPortfolio });
            }
            catch (Exception ex)
            {
                Assert.IsTrue(ex.Message.Contains("Failed to create portfolios. Portfolio's ExternalId can't be empty"));
            }

            //ExternalId is not unique
            myPortfolio.ParentPortfolioExternalId = null;
            myPortfolio.ExternalId = "Test Company A";
            try
            {
                service.CreatePortfolios(new[] { myPortfolio });
            }
            catch (Exception ex)
            {
                Assert.IsTrue(ex.Message.Contains("ExternalId is not unique"));
                return;
            }
            Assert.Fail("Expected to fail");
        }

        [Test]
        public void TestPortfolioCreateWrongParentExternalID()
        {
            IPortfolioManagementService service = WCFClientUtil.GetPortfolioManagementServiceProxy();
            // create portfolio with ParentExternalId = string.Empty
            PortfolioDto portfolioChild = new PortfolioDto
            {
                ExternalId = Guid.NewGuid().ToString(),
                ParentPortfolioExternalId = string.Empty,
                CompanyExternalId = "Id Roles",
                Name = "ChildPortfolio",
                Status = StatusActive,
            };
            try
            {
                service.CreatePortfolios(new[] { portfolioChild });
            }
            catch (Exception ex)
            {
                Assert.IsTrue(ex.Message.Contains("ParentPortfolioExternalId can't be empty"));
            }
            //wrong ParentExternalId
            portfolioChild.ParentPortfolioExternalId = Guid.NewGuid().ToString();
            try
            {
                service.CreatePortfolios(new[] { portfolioChild });
            }
            catch (Exception ex)
            {
                Assert.IsTrue(ex.Message.Contains("Failed to create portfolios"));
                return;
            }

            Assert.Fail("Expected to fail");

        }
        [Test]
        public void TestPortfolioCreateWithWrongCompany()
        {
            IPortfolioManagementService service = WCFClientUtil.GetPortfolioManagementServiceProxy();
            // create portfolio with wrong CompanyExternalId
            string myExternalID = Guid.NewGuid().ToString();
            PortfolioDto myPortfolio = new PortfolioDto
            {
                ExternalId = myExternalID,
                CompanyExternalId = "CompanyExternalId",
                Name = "ChildPortfolio",
                Status = StatusActive,
            };
            try
            {
                service.CreatePortfolios(new[] { myPortfolio });
            }
            catch (Exception ex)
            {
                Assert.IsTrue(ex.Message.Contains("Company with ExternalId 'CompanyExternalId' doesn't exist."));
            }
            // CompanyExternalId = String.Empty
            myPortfolio.CompanyExternalId = String.Empty;
            try
            {
                service.CreatePortfolios(new[] { myPortfolio });
            }
            catch (Exception ex)
            {
                Assert.IsTrue(ex.Message.Contains("CompanyExternalId can't be empty"));

            }
            //ParentExternalID does not belong to the current company
            myPortfolio.ExternalId = Guid.NewGuid().ToString();
            myPortfolio.CompanyExternalId = CompanyForTest_ExternalId;
            myPortfolio.ParentPortfolioExternalId = "Test Company A"; // + prefix; 
            //service.CreatePortfolios(new[] { myPortfolio });
            try
            {
                service.CreatePortfolios(new[] { myPortfolio });
            }
            catch (Exception ex)
            {
                Assert.IsTrue(ex.Message.Contains("does not belong to the company"));
                return;
            }
            Assert.Fail("Expected to fail");
        }

        [Test]
        public void TestPortfolioCreateWrongProperties()
        {

            IPortfolioManagementService service = WCFClientUtil.GetPortfolioManagementServiceProxy();
            // create portfolio with Status= null
            PortfolioDto myPortfolio = new PortfolioDto
            {
                ExternalId = Guid.NewGuid().ToString(),
                Name = "Portfolio",
                CompanyExternalId = "Id Role",
                Status = null
            };
            try
            {
                service.CreatePortfolios(new[] { myPortfolio });
            }
            catch (Exception ex)
            {
                //Assert.AreEqual("Parameter 'ExternalId' can't be empty.", ex.Message);
                Assert.IsTrue(ex.Message.Contains("Portfolio may have the following statuses only: Active,Deleted"));
            }
            // create portfolio with wrong  Status
            myPortfolio.Status = "Created";
            try
            {
                service.CreatePortfolios(new[] { myPortfolio });
            }
            catch (Exception ex)
            {

                Assert.IsTrue(ex.Message.Contains("Portfolio may have the following statuses only: Active,Deleted"));
            }
            //empty Name
            myPortfolio.Status = StatusActive;
            myPortfolio.Name = null;
            try
            {
                service.CreatePortfolios(new[] { myPortfolio });
            }
            catch (Exception ex)
            {
                Assert.IsTrue(ex.Message.Contains("Name can't be empty"));
                return;
            }

            Assert.Fail("Expected to fail");
        }

        [Test]
        public void TestPortfolioUpdateWrongValues()
        {

            IPortfolioManagementService service = WCFClientUtil.GetPortfolioManagementServiceProxy();
            //create good portfolio
            string myExternalId = Guid.NewGuid().ToString();
            PortfolioDto myPortfolio = new PortfolioDto
            {
                ExternalId = myExternalId,
                Name = "Portfolio",
                CompanyExternalId = CompanyForTest_ExternalId,
                Status = StatusActive
            };
            try
            {
                service.CreatePortfolios(new[] { myPortfolio });
            }
            catch (Exception)
            {

                Assert.Pass("Expect to pass.");
            }


            //try to update ExternalId
            myPortfolio.ExternalId = "not possible to update";
            try
            {
                service.UpdatePortfolios(new[] { myPortfolio });
            }
            catch (Exception ex)
            {
                Assert.IsTrue(ex.Message.Contains("Failed to update portfolios"));
            }
            myPortfolio.ExternalId = myExternalId;
            //update to the status = null
            myPortfolio.Status = null;
            try
            {
                service.UpdatePortfolios(new[] { myPortfolio });
            }
            catch (Exception ex)
            {
                Assert.IsTrue(ex.Message.Contains("Failed to update portfolios"));
            }
            //update to the wrong status
            myPortfolio.Status = "Created";
            try
            {
                service.UpdatePortfolios(new[] { myPortfolio });
            }
            catch (Exception ex)
            {
                Assert.IsTrue(ex.Message.Contains("Failed to update portfolios"));
            }
            myPortfolio.Status = StatusActive;
            myPortfolio.Name = null;
            try
            {
                service.UpdatePortfolios(new[] { myPortfolio });
            }
            catch (Exception ex)
            {
                Assert.IsTrue(ex.Message.Contains("Name can't be empty"));
            }
        }

        [Test]
        public void TestPortfolioUpdateWrongParentExternalID()
        {
            IPortfolioManagementService service = WCFClientUtil.GetPortfolioManagementServiceProxy();
            //create good portfolio
            string myExternalId = Guid.NewGuid().ToString();
            PortfolioDto myPortfolio = new PortfolioDto
            {
                ExternalId = myExternalId,
                Name = "Portfolio",
                CompanyExternalId = CompanyForTest_ExternalId,
                Status = StatusActive
            };

            try
            {
                service.CreatePortfolios(new[] { myPortfolio });
            }
            catch (Exception ex)
            {
                Assert.Fail("Create portfolio. Expect to pass: " + ex.Message);
            }

            Thread.Sleep(5000);

            //wrong ParentPortfolioExternalId
            myPortfolio.Name = "PortfolioUpdated";
            myPortfolio.ParentPortfolioExternalId = string.Empty;
            try
            {
                service.UpdatePortfolios(new[] { myPortfolio });
            }
            catch (Exception ex)
            {
                Assert.IsTrue(ex.Message.Contains("It accepts null or meaningful value."));
            }
            //correct update
            myPortfolio.ParentPortfolioExternalId = PortfolioForTest_ExternalID;
            try
            {
                service.UpdatePortfolios(new[] { myPortfolio });
            }
            catch (Exception ex)
            {
                Assert.Fail("Expect to pass after correct update: " + ex.Message);
            }

            myPortfolio.ParentPortfolioExternalId = "null";
            myPortfolio.Status = StatusDeleted;
            try
            {
                service.UpdatePortfolios(new[] { myPortfolio });
            }
            catch (Exception ex)
            {
                Assert.IsTrue(ex.Message.Contains("Portfolio with ExternalId 'null' doesnt exist."));
            }

        }

    }
}
