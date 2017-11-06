using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ElvizTestUtils.PortfolioManagementServiceReference;
using NUnit.Framework;

namespace TestWCFPortfolioManagementService
{
    [TestFixture]
    public class TestReportingDbPortfolios
    {
        [Test]
        public void RDB_CreateAndFindPortfolios()
        {
            string motherCompanyExtid = "MotherCompany_" + Guid.NewGuid();
            string chilldCompanyExtid = "ChildCompany_" + motherCompanyExtid;
            string motherPortfolioExtid = "MotherPortfolio_" + Guid.NewGuid();
            string childPortFolioExtid = "ChildPortfolio_" + motherPortfolioExtid;

            CompanyAndPortfolios expectedCompanyAndPortfolios = CompanyPortfolioUtil.CreateCompanyAndPortfolios(motherCompanyExtid, chilldCompanyExtid, motherPortfolioExtid, childPortFolioExtid);

            Thread.Sleep(1000);
            PortfolioDto actualMotherPortfolio = ElvizTestUtils.QaLookUp.QAReportingDBPortfolioDTOLookUp.GetPortfolioDtoFromReportingDb(
                    expectedCompanyAndPortfolios.MotherPortfolio.ExternalId);
            
            PortfolioDto actualChildPortfolio = ElvizTestUtils.QaLookUp.QAReportingDBPortfolioDTOLookUp.GetPortfolioDtoFromReportingDb(
                    expectedCompanyAndPortfolios.ChildPortfolio.ExternalId);

            CompanyPortfolioUtil.AssertPortfolioDto(expectedCompanyAndPortfolios.MotherPortfolio, actualMotherPortfolio);
            CompanyPortfolioUtil.AssertPortfolioDto(expectedCompanyAndPortfolios.ChildPortfolio, actualChildPortfolio);
        }


        [Test]
        public void RDB_UpdateOwnerCompanyPortfolioDtos()
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

            CompanyPortfolioUtil.UpdatePortfolios(new[] { motherPortfolio, childPortfolio });

            Thread.Sleep(1000);

            PortfolioDto actualMotherPortfolio = ElvizTestUtils.QaLookUp.QAReportingDBPortfolioDTOLookUp.GetPortfolioDtoFromReportingDb(expectedCompanyAndPortfolios.MotherPortfolio.ExternalId);
            PortfolioDto actualChildPortfolio = ElvizTestUtils.QaLookUp.QAReportingDBPortfolioDTOLookUp.GetPortfolioDtoFromReportingDb(expectedCompanyAndPortfolios.ChildPortfolio.ExternalId);

            CompanyPortfolioUtil.AssertPortfolioDto(motherPortfolio, actualMotherPortfolio);
            CompanyPortfolioUtil.AssertPortfolioDto(childPortfolio, actualChildPortfolio);
        }

        [Test]
        public void RDB_UpdatePortfolioDtos()
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
            Thread.Sleep(1000);

            PortfolioDto actualMotherPortfolio = ElvizTestUtils.QaLookUp.QAReportingDBPortfolioDTOLookUp.GetPortfolioDtoFromReportingDb(expectedCompanyAndPortfolios.MotherPortfolio.ExternalId);
            PortfolioDto actualChildPortfolio = ElvizTestUtils.QaLookUp.QAReportingDBPortfolioDTOLookUp.GetPortfolioDtoFromReportingDb(expectedCompanyAndPortfolios.ChildPortfolio.ExternalId);

            CompanyPortfolioUtil.AssertPortfolioDto(expectedCompanyAndPortfolios.MotherPortfolio, actualMotherPortfolio);
            CompanyPortfolioUtil.AssertPortfolioDto(expectedCompanyAndPortfolios.ChildPortfolio, actualChildPortfolio);
        }

        [Test]
        public void RDB_DeletedPortfolioDtos()
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
            CompanyPortfolioUtil.UpdatePortfolios(new[] { motherPortfolio });
            Thread.Sleep(1000);

            PortfolioDto actualMotherPortfolio = ElvizTestUtils.QaLookUp.QAReportingDBPortfolioDTOLookUp.GetPortfolioDtoFromReportingDb(expectedCompanyAndPortfolios.MotherPortfolio.ExternalId);
            PortfolioDto actualChildPortfolio = ElvizTestUtils.QaLookUp.QAReportingDBPortfolioDTOLookUp.GetPortfolioDtoFromReportingDb(expectedCompanyAndPortfolios.ChildPortfolio.ExternalId);

            CompanyPortfolioUtil.AssertPortfolioDto(expectedCompanyAndPortfolios.MotherPortfolio, actualMotherPortfolio);
            CompanyPortfolioUtil.AssertPortfolioDto(expectedCompanyAndPortfolios.ChildPortfolio, actualChildPortfolio);
        }

    }
}
