using ElvizTestUtils.PortfolioManagementServiceReference;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace TestWCFPortfolioManagementService
{
    public class TestReportingDbCompanies
    {
        [Test]
        public void RdCreateAndFindCompanies()
        {
            string rdMotherExtId = "Mother_RDB_" + Guid.NewGuid();
            string rdChildExtId = "_Child_" + rdMotherExtId;
            MotherAndChildCompanies expectedCompanies = CompanyPortfolioUtil.GetTestCompanyDtos(rdMotherExtId, rdChildExtId);

            //breakpoint here
            CompanyPortfolioUtil.CreateCompany(expectedCompanies.MotherCompany);
            CompanyPortfolioUtil.CreateCompany(expectedCompanies.ChildCompany);

            Console.WriteLine(DateTime.Now);

            CompanyDto createdMotherCompanyDto = ElvizTestUtils.QaLookUp.QAReportingDBCompanyDTOLookUp.GetCompanyDtoFromReportingDb(rdMotherExtId);
            CompanyDto createdChildCompanyDto = ElvizTestUtils.QaLookUp.QAReportingDBCompanyDTOLookUp.GetCompanyDtoFromReportingDb(rdChildExtId);

            CompanyPortfolioUtil.AssertRdCompanyDTO(expectedCompanies.MotherCompany, createdMotherCompanyDto);
            CompanyPortfolioUtil.AssertRdCompanyDTO(expectedCompanies.ChildCompany, createdChildCompanyDto);

        }

        [Test]
        public void RdUpdateCompanies()
        {
            string rdMotherExtId = "Mother_RDB_" + Guid.NewGuid();
            string rdChildExtId = "_Child_" + rdMotherExtId;
            MotherAndChildCompanies expectedCompanies = CompanyPortfolioUtil.GetTestCompanyDtos(rdMotherExtId, rdChildExtId);

            CompanyPortfolioUtil.CreateCompany(expectedCompanies.MotherCompany);
            CompanyPortfolioUtil.CreateCompany(expectedCompanies.ChildCompany);

            expectedCompanies.MotherCompany.Name = "Updated Name";
            expectedCompanies.ChildCompany.ParentExternalId = null;

            IList<CompanyRole> roles = expectedCompanies.MotherCompany.Roles.ToList();
            roles.Add(CompanyRole.BalanceResponsibleParty);
            expectedCompanies.MotherCompany.Roles = roles.ToArray();

            CompanyPortfolioUtil.UpdateCompanies(new[] { expectedCompanies.MotherCompany, expectedCompanies.ChildCompany });

            CompanyDto createdMotherCompanyDto = ElvizTestUtils.QaLookUp.QAReportingDBCompanyDTOLookUp.GetCompanyDtoFromReportingDb(rdMotherExtId);
            CompanyDto createdChildCompanyDto = ElvizTestUtils.QaLookUp.QAReportingDBCompanyDTOLookUp.GetCompanyDtoFromReportingDb(rdChildExtId);

            CompanyPortfolioUtil.AssertRdCompanyDTO(expectedCompanies.MotherCompany, createdMotherCompanyDto);
            CompanyPortfolioUtil.AssertRdCompanyDTO(expectedCompanies.ChildCompany, createdChildCompanyDto);
        }

        [Test]
        public void RdDeleteCompanies()
        {
            string rdMotherExtId = "Deleted_Mother_RDB_" + Guid.NewGuid();
            string rdChildExtId = "_Deleted_Child_" + rdMotherExtId;
            MotherAndChildCompanies expectedCompanies = CompanyPortfolioUtil.GetTestCompanyDtos(rdMotherExtId, rdChildExtId);

            CompanyPortfolioUtil.CreateCompany(expectedCompanies.MotherCompany);
            CompanyPortfolioUtil.CreateCompany(expectedCompanies.ChildCompany);

            expectedCompanies.MotherCompany.Status = "Deleted";
            expectedCompanies.ChildCompany.Status = "Deleted";

            Thread.Sleep(5000);

            CompanyPortfolioUtil.UpdateCompanies(new[] { expectedCompanies.MotherCompany, expectedCompanies.ChildCompany });

            CompanyDto createdMotherCompanyDto = ElvizTestUtils.QaLookUp.QAReportingDBCompanyDTOLookUp.GetCompanyDtoFromReportingDb(rdMotherExtId);
            CompanyDto createdChildCompanyDto = ElvizTestUtils.QaLookUp.QAReportingDBCompanyDTOLookUp.GetCompanyDtoFromReportingDb(rdChildExtId);

            CompanyPortfolioUtil.AssertRdCompanyDTO(expectedCompanies.MotherCompany, createdMotherCompanyDto);
            CompanyPortfolioUtil.AssertRdCompanyDTO(expectedCompanies.ChildCompany, createdChildCompanyDto);
        }
    }
}
