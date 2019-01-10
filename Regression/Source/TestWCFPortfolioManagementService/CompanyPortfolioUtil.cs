using System;
using System.Collections.Generic;
using System.IO;
using ElvizTestUtils;
using ElvizTestUtils.PortfolioManagementServiceReference;
using Microsoft.SqlServer.Server;
using NUnit.Framework;
using TestCompany;


namespace TestWCFPortfolioManagementService
{
    public class MotherAndChildCompanies
    {
        public CompanyDto MotherCompany;
        public CompanyDto ChildCompany;
    }

    public class CompanyAndPortfolios
    {
        public MotherAndChildCompanies MotherAndChildCompanies;
        public PortfolioDto MotherPortfolio;
        public PortfolioDto ChildPortfolio;
    }

    public class CompanyPortfolioUtil
    {
        static readonly string test1Filepath = Path.Combine(TestContext.CurrentContext.TestDirectory, "TestFiles\\test1.xml");
        public static MotherAndChildCompanies GetTestCompanyDtos(string motherExternalId,string childExternalId)
        {
            TestClass test = TestXmlTool.Deserialize<TestClass>(test1Filepath);

            int count = test.Addresses.Count;
            Property[] companyAddresses = new Property[count];
            for (int i = 0; i < count; i++)
            {
                companyAddresses[i] = new Property();
                {
                    companyAddresses[i].Name = test.Addresses[i].name;
                    companyAddresses[i].Value = test.Addresses[i].value;
                    companyAddresses[i].ValueType = test.Addresses[i].valueType;
                }
            }

            //Phones
            int numbers = test.Phones.Count;
            Property[] companyPhones = new Property[numbers]; //[propertyLenght];
            for (int i = 0; i < numbers; i++)
            {
                companyPhones[i] = new Property();
                {
                    companyPhones[i].Name = test.Phones[i].name;
                    companyPhones[i].Value = test.Phones[i].value;
                    companyPhones[i].ValueType = test.Phones[i].valueType;
                }
            }

            //Roles
            List<CompanyRole> companyRoles = new List<CompanyRole>();
            foreach (string role in test.CompanyRoles)
            {
                companyRoles.Add((CompanyRole)Enum.Parse(typeof(CompanyRole), role));
            }

            CompanyRole[] myRoles = companyRoles.ToArray();
            CompanyRole[] myChildRoles = new[] { CompanyRole.Customer };

            CompanyDto motherCompany = new CompanyDto
            {
                Addresses = companyAddresses,
                Phones = companyPhones,
                CreditRiskProperties = test.CreditRiskProperties, // credit risk properties
                CountryIsoCode = test.CountryIsoCode,
                ExternalId = motherExternalId,
                Name = test.Name,
                ParentExternalId = null,
                OrgNo = test.OrgNo,
                Status = "Active",
                ShortName = test.ShortName,
                Roles = myRoles
            };

            CreditRiskPropertiesDto creditRiskForChild = new CreditRiskPropertiesDto();
            creditRiskForChild.CreditLimit = 0.01020304050607080;
            creditRiskForChild.CreditLimitIsoCurrency = "NOK";
            creditRiskForChild.CreditRating = "CCC";
            creditRiskForChild.Netting = true;

            //create a child company
            CompanyDto childCompany = new CompanyDto
            {
                Addresses = companyAddresses,
                Phones = companyPhones,
                CreditRiskProperties = creditRiskForChild,
                CountryIsoCode = test.CountryIsoCode,
                ExternalId = childExternalId,
                Name = test.Name,
                ParentExternalId = motherExternalId,
                OrgNo = test.OrgNo,
                Status = "Active",
                ShortName = test.ShortName,
                Roles = myChildRoles
            };

            MotherAndChildCompanies companies = new MotherAndChildCompanies
            {
                MotherCompany = motherCompany,
                ChildCompany = childCompany
            };

            return companies;
        }

        public static CompanyAndPortfolios CreateCompanyAndPortfolios(string motherCompanyExtId,string childCompanyExtId, string motherPortfolioExtId, string childPortfolioExtId)
        {
            MotherAndChildCompanies companies = GetTestCompanyDtos(motherCompanyExtId, childCompanyExtId);

            CreateCompany(companies.MotherCompany);
            CreateCompany(companies.ChildCompany);

            CompanyAndPortfolios portfolios =new CompanyAndPortfolios();

            portfolios.MotherPortfolio=new PortfolioDto() {CompanyExternalId = motherCompanyExtId,  ExternalId = motherPortfolioExtId,Name = "MotherPortfolio", ShortName = "ShortName api test" , Status = "Active"};
            portfolios.ChildPortfolio=new PortfolioDto() {CompanyExternalId = motherCompanyExtId, ParentPortfolioExternalId = motherPortfolioExtId, ExternalId = childPortfolioExtId, Name = "ChildPortfolio",Status = "Active"};

            CreatePortfolios(new []{portfolios.MotherPortfolio,portfolios.ChildPortfolio});

            return portfolios;
        }

        public static void AssertRdCompanyDTO(CompanyDto expectedCompanyDto, CompanyDto actualCompanyDto)
        {
            // CompanyDto companyE = ElvizTestUtils.QaLookUp.QAReportingDBCompanyDTOLookUp.GetCompanyDtoFromReportingDb("Test Company E");

            Assert.AreEqual(expectedCompanyDto.ExternalId, actualCompanyDto.ExternalId);
            Assert.AreEqual(expectedCompanyDto.Name, actualCompanyDto.Name);
            Assert.AreEqual(expectedCompanyDto.Status, actualCompanyDto.Status);
         
            Assert.AreEqual(expectedCompanyDto.CountryIsoCode, actualCompanyDto.CountryIsoCode);
            Assert.AreEqual(expectedCompanyDto.ParentExternalId, actualCompanyDto.ParentExternalId);
            Assert.AreEqual(expectedCompanyDto.CountryIsoCode, actualCompanyDto.CountryIsoCode);
 
            Assert.AreEqual(expectedCompanyDto.Status, actualCompanyDto.Status);
            Assert.AreEqual(expectedCompanyDto.CountryIsoCode, actualCompanyDto.CountryIsoCode);

            CompanyRole[] expectedRoles = expectedCompanyDto.Roles;
            CompanyRole[] actualRoles = actualCompanyDto.Roles;

            if (expectedRoles == null && actualRoles != null) Assert.Fail("Expected Roles is null but Actual Roles are not");
            if (expectedRoles != null && actualRoles == null) Assert.Fail("Expected Roles is not null but Actual Roles are null");


            if (expectedRoles != null && actualRoles != null)
            {
                for (int i = 0; i < expectedRoles.Length; i++)
                {
                    Assert.AreEqual(expectedRoles[i], actualRoles[i]);
                }
            }

            Property[] expectedAdresses = expectedCompanyDto.Addresses;
            Property[] actualAdresses = actualCompanyDto.Addresses;

            if (expectedAdresses == null && actualAdresses != null) Assert.Fail("Expected Adresses is null but Actual Adresses are not");
            if (expectedAdresses != null && actualRoles == null) Assert.Fail("Expected Adresses is not null but Actual Adresses are null");


            if (expectedAdresses != null && actualAdresses != null)
            {
                for (int i = 0; i < expectedAdresses.Length; i++)
                {
                    Assert.AreEqual(expectedAdresses[i].Name, expectedAdresses[i].Name);
                    Assert.AreEqual(expectedAdresses[i].Value, expectedAdresses[i].Value);
                }
            }

            Property[] expectedPhones = expectedCompanyDto.Phones;
            Property[] actualPhones = actualCompanyDto.Phones;

            if (expectedPhones == null && actualPhones != null) Assert.Fail("Expected Phones is null but Actual Phones are not");
            if (expectedPhones != null && actualPhones == null) Assert.Fail("Expected Phones is not null but Actual Phones are null");


            if (expectedPhones != null && actualPhones != null)
            {
                for (int i = 0; i < actualPhones.Length; i++)
                {
                    Assert.AreEqual(expectedPhones[i].Name, actualPhones[i].Name);
                    Assert.AreEqual(expectedPhones[i].Value, actualPhones[i].Value);
                }
            }

            if (expectedCompanyDto.CreditRiskProperties != null && actualCompanyDto.CreditRiskProperties == null )
                Assert.Fail("Expected CreditRiskProperties are not null but Actual CreditRiskProperties are null ");
            if (expectedCompanyDto.CreditRiskProperties == null && actualCompanyDto.CreditRiskProperties != null)
                Assert.Fail("Expected CreditRiskProperties are null but Actual CreditRiskProperties are not");

            Assert.AreEqual(expectedCompanyDto.CreditRiskProperties.CreditLimit, actualCompanyDto.CreditRiskProperties.CreditLimit, "CreditRiskProperties.CreditLimit has wrong value: ");
            Assert.AreEqual(expectedCompanyDto.CreditRiskProperties.CreditLimitIsoCurrency, actualCompanyDto.CreditRiskProperties.CreditLimitIsoCurrency, "CreditRiskProperties.CreditLimitIsoCurrency has wrong value: ");
            Assert.AreEqual(expectedCompanyDto.CreditRiskProperties.CreditRating, actualCompanyDto.CreditRiskProperties.CreditRating, "CreditRiskProperties.CreditRating has wrong value: ");
            Assert.AreEqual(expectedCompanyDto.CreditRiskProperties.Netting, actualCompanyDto.CreditRiskProperties.Netting, "CreditRiskProperties.Netting has wrong value: ");
            //   //delete test companis
            //   companyParent.Status = StatusDeleted;
            //   companyDaughter.Status = StatusDeleted;
            //   service.UpdateCompanies(new[] { companyParent, companyDaughter });
            //   CompanyDto[] foundCompaniesUpd =
            //       service.FindCompanies(new [] { companyParent.ExternalId, companyDaughter.ExternalId });
            //   Assert.AreEqual(foundCompaniesUpd[0].Status, StatusDeleted);
            //   Assert.AreEqual(foundCompaniesUpd[1].Status, StatusDeleted);
        }

        public static void CreateCompany(CompanyDto myCompanyDto)
        {
            IPortfolioManagementService service = WCFClientUtil.GetPortfolioManagementServiceProxy();

            try
            {
                service.CreateCompanies(new[] { myCompanyDto });
                Console.WriteLine("Company created successfully.");
            }
            catch (Exception)
            {
                Console.WriteLine("Company was not created.");
                throw;
            }
        }

        public static void UpdateCompanies(CompanyDto[] companies)
        {
            IPortfolioManagementService service = WCFClientUtil.GetPortfolioManagementServiceProxy();

            service.UpdateCompanies(companies);
        }

        public static void CreatePortfolios(PortfolioDto[] portfolios)
        {
            IPortfolioManagementService service = WCFClientUtil.GetPortfolioManagementServiceProxy();

            service.CreatePortfolios(portfolios);

        }

        public static PortfolioDto FindPortfolio(string extId)
        {
            IPortfolioManagementService service = WCFClientUtil.GetPortfolioManagementServiceProxy();

            PortfolioDto[] dtos = service.FindPortfolios(new[] {extId});

            if (dtos.Length == 1)
                return dtos[0];
            throw new ArgumentException("Did not return 1 dto for extid:"+extId);
        }

        public static void UpdatePortfolios(PortfolioDto[] portfolios)
        {
            IPortfolioManagementService service = WCFClientUtil.GetPortfolioManagementServiceProxy();

            service.UpdatePortfolios(portfolios);

        }



        public static void AssertCompanyDTO(CompanyDto expectedCompanyDto, CompanyDto actualCompanyDto)
        {
            AssertRdCompanyDTO(expectedCompanyDto,actualCompanyDto);
            Assert.AreEqual(expectedCompanyDto.OrgNo, actualCompanyDto.OrgNo);
        }

        public static void AssertPortfolioDto(PortfolioDto expected, PortfolioDto actual)
        {
            Assert.AreEqual(expected.Name, actual.Name);
            Assert.AreEqual(expected.ExternalId, actual.ExternalId);
            Assert.AreEqual(expected.CompanyExternalId, actual.CompanyExternalId);
            Assert.AreEqual(expected.Status, actual.Status);
            Assert.AreEqual(expected.ParentPortfolioExternalId, actual.ParentPortfolioExternalId);
        }
    }
}
