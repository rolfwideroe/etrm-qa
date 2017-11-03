using ElvizTestUtils;
using ElvizTestUtils.PortfolioManagementServiceReference;
using NUnit.Framework;

namespace TestWCFPortfolioManagementService
{
    [TestFixture]
    public class TestPamPortfolio
    {

        [TestFixtureSetUp]
        public void Setup()
        {
            if (!ConfigurationTool.PamEnabled)
            {
                ConfigurationTool.PamEnabled = true;
            }
        }

        [TestFixtureTearDown]
        public void Teardown()
        {
            ConfigurationTool.PamEnabled = false;
        }
        
        [Test]
        public void TestPamAllowedUserGroup()
        {
            IPortfolioManagementService proxy = WCFClientUtil.GetPortfolioManagementServiceProxy();

            PortfolioDto[] dtosAllowed = proxy.FindPortfolios(new string[] { "PAM Test Portfolio A" });

            Assert.AreEqual(1, dtosAllowed.Length);
        }
        
        [Test]
        public void TestPamAllowedUserNoGroup()
        {
            IPortfolioManagementService proxy = WCFClientUtil.GetPortfolioManagementServiceProxy();

            //User has access, group doesn't 
            PortfolioDto[] dtosAllowed = proxy.FindPortfolios(new string[] { "PAM Test Portfolio B" });

            Assert.AreEqual(1, dtosAllowed.Length);
        }
        
        [Test]
        public void TestPamAllowedGroupNoUser()
        {
            IPortfolioManagementService proxy = WCFClientUtil.GetPortfolioManagementServiceProxy();

            //user doesn't have permissions, but group has
            PortfolioDto[] dtosAllowed = proxy.FindPortfolios(new string[] { "PAM Test Portfolio C" });

            Assert.AreEqual(1, dtosAllowed.Length); //one result for both user permissions ON/Off

            //try to get access to sub portfolio, group has access to current subportfolio
            PortfolioDto[] dtosAllowedSubPortfolioDtos = proxy.FindPortfolios(new string[] { "PAM Test SubPortfolio A" });

            Assert.AreEqual(1, dtosAllowedSubPortfolioDtos.Length);
        }

        [Test]
        public void TestPamAllowedNoGroupNoUser()
        {
           // ElvizInstallationUtility.PamEnabled = true;
            IPortfolioManagementService proxy = WCFClientUtil.GetPortfolioManagementServiceProxy();

            //Neither user nor group have access
            PortfolioDto[] dtosNotAllowed = proxy.FindPortfolios(new string[] { "PAM Test Portfolio E" });

            Assert.AreEqual(0, dtosNotAllowed.Length);
        }

        [Test]
        public void TestPamAllowedNoGroupSubPortfolioUserFor()
        {
            IPortfolioManagementService proxy = WCFClientUtil.GetPortfolioManagementServiceProxy();

            //User has access to subportfolio
            PortfolioDto[] dtosAllowed = proxy.FindPortfolios(new string[] { "PAM Test SubPortfolio E" });

            Assert.AreEqual(1, dtosAllowed.Length);
            
            //but doesn't have accsess to parent portfolio
            PortfolioDto[] dtosNotAllowed = proxy.FindPortfolios(new string[] { "PAM Test Portfolio E" });

            Assert.AreEqual(0, dtosNotAllowed.Length);
        }
        
        [Test]
        public void TestPamAllowedSecondGroupNoUser()
        {
            IPortfolioManagementService proxy = WCFClientUtil.GetPortfolioManagementServiceProxy();

            //User belongs to second group, second group has access
            PortfolioDto[] dtosAllowed = proxy.FindPortfolios(new string[] { "PAM for second user group" });

            Assert.AreEqual(1, dtosAllowed.Length);
        }
    }
}
