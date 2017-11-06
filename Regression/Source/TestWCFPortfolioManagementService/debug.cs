using System;
using System.Collections.Generic;
using System.IO;
using System.ServiceModel;
using System.Xml.Serialization;
using NUnit.Framework;
using TestCompany;
using TestWCFPortfolioManagementService.ServiceReferenceCompanyAPI;
using Property = TestWCFPortfolioManagementService.ServiceReferenceCompanyAPI.Property;

namespace TestWCFPortfolioManagementService
{

    [TestFixture]

    public class Debug
    {

        private int prefix;
        private string StatusActive = "Active";
        private string StatusDeleted = "Deleted";

        private TestClass DeserializeXml(string testFilepath)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof (TestClass));

            string filepath = Path.Combine(Directory.GetCurrentDirectory(), "TestFiles\\" + testFilepath);

            FileStream readFileStream = File.Open(
                filepath,
                FileMode.Open,
                FileAccess.Read,
                FileShare.Read);

            return (TestClass) xmlSerializer.Deserialize(readFileStream);
        }
         
        [Test, Timeout(1000*1000), TestCaseSource(typeof (TestCasesFileEnumerator), "TestCaseFiles")]
        public void FindCompaniesDtosAndCheck(string testFilepath)
        {
            TestClass test = this.DeserializeXml(testFilepath);

            this.CreateCompaniesDtos(test);

            IPortfolioManagementService service = ServiceProxyUtil.GetPortfolioManagmentServiceServiceProxy();
            //try to get existed and not existed companies
            CompanyDto[] ExistedCompanies =
                service.FindCompanies(new string[] { test.ExternalId + prefix});

            Assert.AreEqual(1, ExistedCompanies.Length);
            Assert.AreEqual(test.ExternalId + prefix, ExistedCompanies[0].ExternalId);
            //Assert.AreEqual(test.CountryIsoCode, ExistedCompanies[0].CountryIsoCode);
            Assert.AreEqual(test.Name, ExistedCompanies[0].Name);

        }


        private void CreateCompaniesDtos(TestClass mytest)
        {
            IPortfolioManagementService service = ServiceProxyUtil.GetPortfolioManagmentServiceServiceProxy();

            TestClass test = mytest;
            Console.WriteLine(mytest.CountryIsoCode);

            
            //Roles
            List<CompanyRole> companyRoles = new List<CompanyRole>();
            foreach (string role in test.CompanyRoles)
            {
                companyRoles.Add((CompanyRole) Enum.Parse(typeof (CompanyRole), role));
            }

            CompanyRole[] myRoles = companyRoles.ToArray();
            CompanyRole[] myChildRoles = new[] {CompanyRole.Customer};
          
            Property[] CompanyAddress = new Property[1];
            CompanyAddress[0] = new Property();
            CompanyAddress[0].Name = "ElvizProperty Line 1";
            CompanyAddress[0].Value = "Street1";
            CompanyAddress[0].ValueType = "System.String";

            Random rnd = new Random();
            prefix = rnd.Next(50000);
            ////CreateCompany(...)
                CompanyDto companyParent = new CompanyDto
                                                        {
                                                            Addresses = CompanyAddress,
                                                            CountryIsoCode = test.CountryIsoCode,
                                                            ExternalId = test.ExternalId + prefix,
                                                            Name = test.Name,
                                                            ParentExternalId = null,
                                                            OrgNo = test.OrgNo,
                                                            Status = test.Status,
                                                            Roles = myRoles
                                                        };

           
            try
            {
                service.CreateCompanies(new[] {companyParent});
                Console.WriteLine("Company created successfully.");
               
            }
            catch (Exception)
            {
                Console.WriteLine("Company was not created.");
                throw;
            }

            CompanyDto[] companies =
                service.FindCompanies(new string[] {test.ExternalId + prefix});
            Assert.AreEqual(companyParent.Name, companies[0].Name);
            Assert.AreEqual(companyParent.OrgNo, companies[0].OrgNo);
            Assert.AreEqual(companyParent.Status, companies[0].Status);
            Console.WriteLine(companies[0].CountryIsoCode);
           // Assert.AreEqual(companyParent.CountryIsoCode, companies[0].CountryIsoCode);


            CompanyRole[] getRoles = companies[0].Roles;
            for (int i = 0; i < getRoles.Length; i++)
            {
                Assert.AreEqual(companyParent.Roles[i], getRoles[i]);
            }

           
        }

        private CompanyDto[] FindCompaniesDtos(TestClass test)
        {
            IPortfolioManagementService service = ServiceProxyUtil.GetPortfolioManagmentServiceServiceProxy();
            //try to get existed and not existed companies
            CompanyDto[] notExistCompanies =
                service.FindCompanies(new string[] {test.ExternalId + prefix, "nExistExternalId"});

            Assert.AreEqual(1, notExistCompanies.Length);
            Assert.AreEqual(test.ExternalId + prefix, notExistCompanies[0].ExternalId);
            Assert.AreEqual(0, service.FindCompanies(new[] {"nExistExternalId", Guid.NewGuid().ToString()}).Length);
            //try to get existed companies
            return service.FindCompanies(new string[] {test.ExternalId + prefix});

        }

       
        private void DeleteCompaniesDtos(CompanyDto[] companies)
        {

            IPortfolioManagementService service = ServiceProxyUtil.GetPortfolioManagmentServiceServiceProxy();

            //CompanyDto[] createdCompanies = this. FindAndCreateCompanyDtos(test);
            Assert.AreEqual(2, companies.Length);
            CompanyDto parentCompany = companies[0];
            CompanyDto dautherCompany = companies[1];

            //Delete
            parentCompany.Status = StatusDeleted;
            dautherCompany.Status = StatusDeleted;
            service.UpdateCompanies(new[] {parentCompany, dautherCompany});

            //ask Deleted company
            CompanyDto[] foundCompaniesDel =
                service.FindCompanies(new string[] {parentCompany.ExternalId, dautherCompany.ExternalId});
            CompanyDto deletedParentCompany = foundCompaniesDel[0];
            CompanyDto deletedDautherCompany = foundCompaniesDel[1];

            Assert.AreEqual(parentCompany.Status, deletedParentCompany.Status);
            Assert.AreEqual(dautherCompany.Status, deletedDautherCompany.Status);

            //Restore childcompany from Deleted to Active, parents company is deleted
            deletedDautherCompany.Status = StatusActive;
            deletedDautherCompany.OrgNo = "From Deleted To Active";
            service.UpdateCompanies(new[] {deletedDautherCompany});

            CompanyDto[] foundCompanyAct = service.FindCompanies(new string[] {deletedDautherCompany.ExternalId});
            Assert.AreEqual(deletedDautherCompany.Status, foundCompanyAct[0].Status);

            //Delete child company
            foundCompanyAct[0].Status = StatusDeleted;
            service.UpdateCompanies(new[] {foundCompanyAct[0]});
            CompanyDto[] foundCompanyDel = service.FindCompanies(new string[] {foundCompanyAct[0].ExternalId});
            Assert.AreEqual(foundCompanyAct[0].Status, foundCompanyDel[0].Status);
        }

        private CompanyDto[] UpdateCompaniesDtos(CompanyDto[] companies)
        {
            IPortfolioManagementService service = ServiceProxyUtil.GetPortfolioManagmentServiceServiceProxy();

//            Assert.AreEqual(2, companies.Length);

            CompanyDto parentCompany = companies[0];
           // CompanyDto dautherCompany = companies[1];

            parentCompany.Name = "QA Test Name ";
            parentCompany.OrgNo = null;
            parentCompany.CountryIsoCode = "UA";
            //parentCompany.ExternalId = "try to set new";
            //parentCompany.ExternalId = String.Empty;
            CompanyRole[] threeRoles = new[] {CompanyRole.Customer, CompanyRole.Broker, CompanyRole.Exchange};
            parentCompany.Roles = threeRoles;

           service.UpdateCompanies(new[] {parentCompany});
            CompanyDto[] foundCompaniesUpd =
                service.FindCompanies(new string[] {parentCompany.ExternalId});

            CompanyDto updatedParentCompany = foundCompaniesUpd[0];

            Assert.AreEqual(parentCompany.ExternalId, updatedParentCompany.ExternalId);
            Assert.AreEqual(parentCompany.Name, updatedParentCompany.Name);
            Assert.AreEqual(parentCompany.OrgNo, updatedParentCompany.OrgNo);
            Assert.AreEqual(parentCompany.Status, updatedParentCompany.Status);
//            Assert.AreEqual(parentCompany.CountryIsoCode, updatedParentCompany.CountryIsoCode);
            CompanyRole[] parentCompanyRoles = updatedParentCompany.Roles;
            for (int i = 0; i < parentCompanyRoles.Length; i++)
            {
                Assert.AreEqual(threeRoles[i], parentCompanyRoles[i]);
            }

           
            return foundCompaniesUpd;
        }


        [Test, Timeout(1000*1000), TestCaseSource(typeof (TestCasesFileEnumerator), "TestCaseFiles")]
        public void CompaniesApi(string testFilepath)
        {

            TestClass test = this.DeserializeXml(testFilepath);

            this.CreateCompaniesDtos(test);

            CompanyDto[] createdCompanies = this.FindCompaniesDtos(test);
            CompanyDto[] updatedCompanies = this.UpdateCompaniesDtos(createdCompanies);

            // Delete companies
            //  this.DeleteCompaniesDtos(updatedCompanies);
            Console.WriteLine("Test passed.");
        }
    }
}