using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.ServiceModel;
using System.Threading;
using System.Xml.Serialization;
using ElvizTestUtils;
using ElvizTestUtils.DealServiceReference;
using ElvizTestUtils.PortfolioManagementServiceReference;
using NUnit.Framework;
using TestCompany;

using ElvizProperty = ElvizTestUtils.PortfolioManagementServiceReference.Property;
using CreditRiskPropertiesDto = ElvizTestUtils.PortfolioManagementServiceReference.CreditRiskPropertiesDto;

namespace TestWCFPortfolioManagementService
{
    [TestFixture]

        public class TestCompanyAPI
        {

        string motherExternalId;
        string childExternalId;
        private CompanyDto expectedChildCompany;
        private CompanyDto expectedMotherCompany;

        [TestFixtureSetUp]
        public void SetUp()
        {
            //Generate Company ExternalID
            motherExternalId = Guid.NewGuid().ToString();
            childExternalId = "_child_" + motherExternalId;
            //CompanyDto expectedChildCompany = new CompanyDto();
            string filepath = Path.Combine(Directory.GetCurrentDirectory(), "TestFiles\\" + testFilepath);
            Console.WriteLine(motherExternalId);

            MotherAndChildCompanies companies= CompanyPortfolioUtil.GetTestCompanyDtos(motherExternalId,childExternalId);
            
            this.expectedMotherCompany = companies.MotherCompany;
            this.expectedChildCompany = companies.ChildCompany;
            //Addresses
        }

         private string StatusActive = "Active";
         private string StatusDeleted = "Deleted";
         private string testFilepath = "test1.xml";

        [Test]
        public void CreateAndFindCompaniesDtos()
        {
           ////create expectedMotherCompany
            CompanyPortfolioUtil.CreateCompany(expectedMotherCompany);
            CompanyPortfolioUtil.CreateCompany(expectedChildCompany);
            IPortfolioManagementService service = WCFClientUtil.GetPortfolioManagementServiceProxy();
            CompanyDto[] companies =
                service.FindCompanies(new[] { motherExternalId, childExternalId });

            CompanyDto returnedMotherCompany = companies.SingleOrDefault(x => x.ExternalId == this.expectedMotherCompany.ExternalId);
            CompanyDto returnedChildCompany = companies.SingleOrDefault(x => x.ExternalId == this.expectedChildCompany.ExternalId);

            CompanyPortfolioUtil.AssertCompanyDTO(expectedMotherCompany, returnedMotherCompany);
            CompanyPortfolioUtil.AssertCompanyDTO(expectedChildCompany, returnedChildCompany);

        }

       [Test]
       public void FindCompaniesDtos()    
       {
           IPortfolioManagementService service = WCFClientUtil.GetPortfolioManagementServiceProxy();
           //try to get existed and not existed companies
           TestClass test = TestXmlTool.Deserialize<TestClass>("TestFiles\\"+testFilepath);

           CompanyDto[] notExistCompanies =
                service.FindCompanies(new[] { test.CompanyForUpdate, "nExistExternalId" });

           Assert.AreEqual(1, notExistCompanies.Length);
           Assert.AreEqual(test.CompanyForUpdate, notExistCompanies[0].ExternalId);
           Assert.AreEqual(0, service.FindCompanies(new[] { "nExistExternalId", Guid.NewGuid().ToString() }).Length);
       }

       [Test]
       public void DeleteCompaniesDtos()
       {
            IPortfolioManagementService service = WCFClientUtil.GetPortfolioManagementServiceProxy();
            TestClass test = TestXmlTool.Deserialize<TestClass>("TestFiles\\"+ testFilepath);

            string parentExternalID = Guid.NewGuid().ToString(); 
           ////CreateCompany(...)
            CompanyDto companyParent = new CompanyDto
                        {
                            CountryIsoCode = test.CountryIsoCode,
                            CreditRiskProperties = test.CreditRiskProperties,
                            ExternalId = parentExternalID,
                            Name = test.Name,
                            ParentExternalId = null,
                            OrgNo = test.OrgNo,
                            Status = StatusActive,
                            ShortName = "TestShortNameCompany",
                            Roles =  new[] {CompanyRole.Customer, CompanyRole.Broker}

                        };

            CompanyDto companyDaughter = new CompanyDto
                        {
                            CountryIsoCode = test.CountryIsoCode,
                            ExternalId = Guid.NewGuid().ToString(),
                            Name = test.ChildCompanyName,
                            ParentExternalId = parentExternalID,
                            OrgNo = test.ChildCompanyOrgNo,
                            Status = test.Status,
                            ShortName = "TestShortNameChild",
                            Roles = new[] {CompanyRole.Customer, CompanyRole.Broker}
                        };

           try
           {
                service.CreateCompanies(new[] {companyParent, companyDaughter});
           }
           catch (Exception)
           {
               
               throw;
           }

           CompanyDto[] companies = service.FindCompanies(new [] { companyParent.ExternalId, companyDaughter.ExternalId });
           Assert.AreEqual(2, companies.Length);
           CompanyDto parentCompany = companies[0];
           CompanyDto dautherCompany = companies[1];

           //Delete
           parentCompany.Status = StatusDeleted;
           dautherCompany.Status = StatusDeleted;
           service.UpdateCompanies(new[] { parentCompany, dautherCompany });

           //ask Deleted expectedMotherCompany
           CompanyDto[] foundCompaniesDel = service.FindCompanies(new [] { companyParent.ExternalId, companyDaughter.ExternalId });

           CompanyDto deletedParentCompany = foundCompaniesDel[0];
           CompanyDto deletedDautherCompany = foundCompaniesDel[1];

           Assert.AreEqual(parentCompany.Status, deletedParentCompany.Status);
           Assert.AreEqual(dautherCompany.Status, deletedDautherCompany.Status);

            //Restore childcompany from Deleted to Active, parents expectedMotherCompany is deleted
            deletedDautherCompany.Status = StatusActive;
            deletedDautherCompany.OrgNo = "From Deleted To Active";
            service.UpdateCompanies(new[] {deletedDautherCompany});

            CompanyDto[] foundCompanyAct = service.FindCompanies(new [] {deletedDautherCompany.ExternalId});
            Assert.AreEqual(deletedDautherCompany.Status, foundCompanyAct[0].Status);
           //Delete child expectedMotherCompany
            foundCompanyAct[0].Status = StatusDeleted;
            service.UpdateCompanies(new[] { foundCompanyAct[0] });
            CompanyDto[] foundCompanyDel = service.FindCompanies(new [] { foundCompanyAct[0].ExternalId });
            Assert.AreEqual(foundCompanyAct[0].Status, foundCompanyDel[0].Status);
       }

        [Test] //, Timeout(1000 * 1000), TestCaseSource(typeof(TestCasesFileEnumerator), "TestCaseFiles")]
        public void UpdateCompanyProperties()//string testFilepath)      //  {TestClass test)
        {
            IPortfolioManagementService service = WCFClientUtil.GetPortfolioManagementServiceProxy();
            TestClass test = TestXmlTool.Deserialize<TestClass>("TestFiles\\"+testFilepath);
            CompanyDto[] createdCompanies = service.FindCompanies(new string[] { test.CompanyForUpdate });

            Assert.AreEqual(1, createdCompanies.Length);

            CompanyDto myCompany = createdCompanies[0];
            myCompany.CountryIsoCode = "DK";
            myCompany.CreditRiskProperties = new CreditRiskPropertiesDto();
            myCompany.CreditRiskProperties.CreditRating = "BBB-";
            myCompany.CreditRiskProperties.CreditLimit = 12;
            myCompany.CreditRiskProperties.CreditLimitIsoCurrency = "USD";
            myCompany.CreditRiskProperties.Netting = true;

            myCompany.Name = test.CompanyForUpdate + "_updated";
            myCompany.OrgNo = "new org NO";
            myCompany.Roles = new [] {  CompanyRole.Customer, CompanyRole.Broker};
            //Addresses
            int count = test.Addresses.Count;
            ElvizProperty[] companyAddresses = new ElvizProperty[count];
            for (int i = 0; i < count; i++)
            {
                companyAddresses[i] = new ElvizProperty();
                {
                    companyAddresses[i].Name = test.Addresses[i].name;
                    companyAddresses[i].Value = test.Addresses[i].value;
                    companyAddresses[i].ValueType = test.Addresses[i].valueType;
                }
            }
            //Phones
            int numbers = test.Phones.Count;
            ElvizProperty[] companyPhones = new ElvizProperty[numbers];//[propertyLenght];
            for (int i = 0; i < numbers; i++)
            {
                companyPhones[i] = new ElvizProperty();
                {
                    companyPhones[i].Name = test.Phones[i].name;
                    companyPhones[i].Value = test.Phones[i].value;
                    companyPhones[i].ValueType = test.Phones[i].valueType;
                }
            }

            myCompany.Addresses = companyAddresses;
            myCompany.Phones = companyPhones;
            myCompany.ShortName = "short name viz";
            
            service.UpdateCompanies(new[] { myCompany });
            CompanyDto[] foundCompaniesUpd =
                service.FindCompanies(new[] { myCompany.ExternalId });

            CompanyDto updatedCompany = foundCompaniesUpd[0];

            Assert.AreEqual(myCompany.CountryIsoCode, updatedCompany.CountryIsoCode);
            Assert.AreEqual(myCompany.ShortName, updatedCompany.ShortName);

            Assert.AreEqual(myCompany.Addresses.Count(), updatedCompany.Addresses.Count());
            Assert.AreEqual(myCompany.Phones.Count(), updatedCompany.Phones.Count());

           for (int i = 0; i < myCompany.Phones.Count(); i++)
            {
                {
                    Assert.AreEqual(myCompany.Phones[i].Name, updatedCompany.Phones[i].Name);
                    Assert.AreEqual(myCompany.Phones[i].Value, updatedCompany.Phones[i].Value);
                }
            }

           CompanyRole[] companyRoles = myCompany.Roles;
           for (int i = 0; i < companyRoles.Length; i++)
           {
               Assert.AreEqual(myCompany.Roles, updatedCompany.Roles);
           }
        }

        [Test]
        public void TryCompanyProperties()
        {
            IPortfolioManagementService service = WCFClientUtil.GetPortfolioManagementServiceProxy();
          
            ElvizProperty[] test = new ElvizProperty[1];//[propertyLenght];
            test[0] = new ElvizProperty();
                {
                    test[0].Name = "Test";
                    test[0].Value = "value";
                    test[0].ValueType = "System.String";
                }
            
            CompanyDto myCompany = new CompanyDto

            {
                CountryIsoCode = "UK",
                ExternalId = Guid.NewGuid().ToString(),
                Properties = test,
                Name = "TestProperties123",
                OrgNo = "OrgNo",
                Status = "Active",
                ShortName = "1234567890123456789012345",
                Roles = new[] {CompanyRole.Customer, CompanyRole.Broker, CompanyRole.Exchange}
                
            };

            try
            {
                service.CreateCompanies((new[] {myCompany}));
            }

            catch (FaultException<ExceptionDetail> ex)
            {
                Assert.Fail("Creating expectedMotherCompany failed: " + ex.Message);
            }
        }
    }

    //public class MyClass
    //{
    //    public MyClass(int age)
    //    {
    //        Age = age;
    //    }

    //    public int Age;

    //    public override bool Equals(object obj)
    //    {
    //        MyClass c = (MyClass)obj;
    //        if (this.Age == c.Age) return true;
    //        return false;
    //    }
    //}

    //[TestFixture]
    //public class Ts
    //{
    //    [Test]
    //    public void Test()
    //    {
    //        MyClass a = new MyClass(1);

    //        MyClass b = new MyClass(1);

    //        Assert.AreEqual(a, b);
    //    }
    //}
}