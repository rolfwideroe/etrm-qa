using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Xml.Serialization;
using ElvizTestUtils;
using ElvizTestUtils.PortfolioManagementServiceReference;
using NUnit.Framework;
using TestCompany;

namespace TestWCFPortfolioManagementService
{

    [TestFixture]

    public class TestPortfolioAPI
    {
        [OneTimeSetUp]
        public void RunBeforeAnyTests()
        {
            Environment.CurrentDirectory = TestContext.CurrentContext.TestDirectory;
        }

        [Test]
        public void CreateAndFindPortfolios()
        {
            string motherCompanyExtid = "MotherCompany_" + Guid.NewGuid();
            string chilldCompanyExtid = "ChildCompany_" + motherCompanyExtid;
            string motherPortfolioExtid = "MotherPortfolio_" + Guid.NewGuid();
            string childPortFolioExtid = "ChildPortfolio_" + motherPortfolioExtid;

           CompanyAndPortfolios    expectedCompanyAndPortfolios = CompanyPortfolioUtil.CreateCompanyAndPortfolios(motherCompanyExtid,chilldCompanyExtid ,motherPortfolioExtid, childPortFolioExtid);

            PortfolioDto actualMotherPortfolio = CompanyPortfolioUtil.FindPortfolio(expectedCompanyAndPortfolios.MotherPortfolio.ExternalId);
            PortfolioDto actualChildPortfolio = CompanyPortfolioUtil.FindPortfolio(expectedCompanyAndPortfolios.ChildPortfolio.ExternalId);

            CompanyPortfolioUtil.AssertPortfolioDto(expectedCompanyAndPortfolios.MotherPortfolio,actualMotherPortfolio);
            CompanyPortfolioUtil.AssertPortfolioDto(expectedCompanyAndPortfolios.ChildPortfolio,actualChildPortfolio);
        }


        [Test]
        [Retry(3)]
        public void UpdateOwnerCompanyPortfolioDtos()
        {
            string motherCompanyExtid = "Upd_Owner_MotherCompany_" + Guid.NewGuid();
            string chilldCompanyExtid = "Upd_Owner_ChildCompany_" + motherCompanyExtid;
            string motherPortfolioExtid = "Upd_Owner_MotherPortfolio_" + Guid.NewGuid();
            string childPortFolioExtid = "Upd_Owner_ChildPortfolio_" + motherPortfolioExtid;

            CompanyAndPortfolios expectedCompanyAndPortfolios =
                CompanyPortfolioUtil.CreateCompanyAndPortfolios(motherCompanyExtid, chilldCompanyExtid,
                    motherPortfolioExtid, childPortFolioExtid);

            PortfolioDto motherPortfolio = expectedCompanyAndPortfolios.MotherPortfolio;
            PortfolioDto childPortfolio = expectedCompanyAndPortfolios.ChildPortfolio;

            motherPortfolio.CompanyExternalId = chilldCompanyExtid;
            childPortfolio.CompanyExternalId = chilldCompanyExtid;

            CompanyPortfolioUtil.UpdatePortfolios(new[] {motherPortfolio,childPortfolio});

            PortfolioDto actualMotherPortfolio = CompanyPortfolioUtil.FindPortfolio(expectedCompanyAndPortfolios.MotherPortfolio.ExternalId);
            PortfolioDto actualChildPortfolio = CompanyPortfolioUtil.FindPortfolio(expectedCompanyAndPortfolios.ChildPortfolio.ExternalId);

            CompanyPortfolioUtil.AssertPortfolioDto(motherPortfolio, actualMotherPortfolio);
            CompanyPortfolioUtil.AssertPortfolioDto(childPortfolio, actualChildPortfolio);
        }

        [Test]
        public void UpdatePortfolioDtos()
        {
            string motherCompanyExtid = "Upd_MotherCompany_" + Guid.NewGuid();
            string chilldCompanyExtid = "Upd_ChildCompany_" + motherCompanyExtid;
            string motherPortfolioExtid = "Upd_MotherPortfolio_" + Guid.NewGuid();
            string childPortFolioExtid = "Upd_ChildPortfolio_" + motherPortfolioExtid;

            CompanyAndPortfolios expectedCompanyAndPortfolios =
                CompanyPortfolioUtil.CreateCompanyAndPortfolios(motherCompanyExtid, chilldCompanyExtid,
                    motherPortfolioExtid, childPortFolioExtid);


            PortfolioDto motherPortfolio = expectedCompanyAndPortfolios.MotherPortfolio;
            PortfolioDto childPortfolio = expectedCompanyAndPortfolios.ChildPortfolio;

            motherPortfolio.Name = "updatedNameMother";
            childPortfolio.Name = "updatedNameChild";

            CompanyPortfolioUtil.UpdatePortfolios(new[] { motherPortfolio, childPortfolio });

            PortfolioDto actualMotherPortfolio = CompanyPortfolioUtil.FindPortfolio(expectedCompanyAndPortfolios.MotherPortfolio.ExternalId);
            PortfolioDto actualChildPortfolio = CompanyPortfolioUtil.FindPortfolio(expectedCompanyAndPortfolios.ChildPortfolio.ExternalId);

            CompanyPortfolioUtil.AssertPortfolioDto(expectedCompanyAndPortfolios.MotherPortfolio, actualMotherPortfolio);
            CompanyPortfolioUtil.AssertPortfolioDto(expectedCompanyAndPortfolios.ChildPortfolio, actualChildPortfolio);
        }

        [Test]
        [Retry(3)]
        public void DeletedPortfolioDtos()
        {
            string motherCompanyExtid = "Del_MotherCompany_" + Guid.NewGuid();
            string chilldCompanyExtid = "Del_ChildCompany_" + motherCompanyExtid;
            string motherPortfolioExtid = "Del_MotherPortfolio_" + Guid.NewGuid();
            string childPortFolioExtid = "Del_ChildPortfolio_" + motherPortfolioExtid;

            CompanyAndPortfolios expectedCompanyAndPortfolios =
                CompanyPortfolioUtil.CreateCompanyAndPortfolios(motherCompanyExtid, chilldCompanyExtid,
                    motherPortfolioExtid, childPortFolioExtid);

            PortfolioDto motherPortfolio = expectedCompanyAndPortfolios.MotherPortfolio;
            PortfolioDto childPortfolio = expectedCompanyAndPortfolios.ChildPortfolio;

            motherPortfolio.Status = "Deleted";
            childPortfolio.Status = "Deleted";
            childPortfolio.ParentPortfolioExternalId = null;

            CompanyPortfolioUtil.UpdatePortfolios(new[] { childPortfolio });
            Thread.Sleep(1000);
            CompanyPortfolioUtil.UpdatePortfolios(new[] { motherPortfolio });


            PortfolioDto actualMotherPortfolio = CompanyPortfolioUtil.FindPortfolio(expectedCompanyAndPortfolios.MotherPortfolio.ExternalId);
            PortfolioDto actualChildPortfolio = CompanyPortfolioUtil.FindPortfolio(expectedCompanyAndPortfolios.ChildPortfolio.ExternalId);

            CompanyPortfolioUtil.AssertPortfolioDto(expectedCompanyAndPortfolios.MotherPortfolio, actualMotherPortfolio);
            CompanyPortfolioUtil.AssertPortfolioDto(expectedCompanyAndPortfolios.ChildPortfolio, actualChildPortfolio);
        }

    }
}