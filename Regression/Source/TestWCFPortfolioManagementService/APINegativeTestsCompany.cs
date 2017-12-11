using System;
using System.ServiceModel;
using ElvizTestUtils;
using ElvizTestUtils.PortfolioManagementServiceReference;
using NUnit.Framework;

namespace TestWCFPortfolioManagementService
{
    class APINegativeTestsCompany
    {
        private string StatusActive = "Active";
        private string StatusDeleted = "Deleted";

    [Test]
    public void TestCompanyCreateWrongExternalID()
        {
            IPortfolioManagementService service = WCFClientUtil.GetPortfolioManagementServiceProxy();
            // create company 
            CompanyDto myCompany = new CompanyDto
            {
                ExternalId = String.Empty,
                Name = "Test Company A",
                OrgNo = "OrgNo",
                Status = StatusActive,
                Roles = new[] {CompanyRole.Customer, CompanyRole.Broker, CompanyRole.Exchange}
            };
            try
            {
                service.CreateCompanies((new[] { myCompany}));
            }
            catch (Exception ex)
            {
                Assert.IsTrue(ex.Message.Contains("Failed to create companies. ExternalId can't be empty"));
            }
            myCompany.ExternalId = "Test Company A";
            try
            {
                service.CreateCompanies((new[] { myCompany }));
            }
            catch (Exception ex)
            {
                Assert.IsTrue(ex.Message.Contains("ExternalId is not unique"));
                return;
            }
            Assert.Fail("Expected to fail");
        }

    [Test]
     public void TestCompanyCreateWithWrongData()
     {
        IPortfolioManagementService service = WCFClientUtil.GetPortfolioManagementServiceProxy();
            // create company 
            CompanyDto myCompany = new CompanyDto
            {
                ExternalId = Guid.NewGuid().ToString(),
                Name = "TestCompany",
                OrgNo = "OrgNo",
                Status = StatusActive
            };
            //no roles
            try
            {
                service.CreateCompanies((new[] { myCompany}));
            }
            catch (Exception ex)
            {
                Assert.IsTrue(ex.Message.Contains("Company must have at least one role!"));
            }
            myCompany.Roles = new[] {CompanyRole.Customer, CompanyRole.Broker, CompanyRole.Exchange};
            myCompany.Name = null;
            // name = null
            try
            {
                service.CreateCompanies((new[] { myCompany }));
            }
            catch (Exception ex)
            {
                Assert.IsTrue(ex.Message.Contains("Name can't be empty"));
            }
            //OrgNo = String.Empty
            myCompany.Name = "TestCompany";
            myCompany.OrgNo = String.Empty;
            try
            {
                service.CreateCompanies((new[] { myCompany }));
            }
            catch (Exception ex)
            {
               Assert.IsTrue(ex.Message.Contains("OrgNo can't be empty. It accepts null or meaningful value."));
            }
            //Status = null
            myCompany.OrgNo = "String.Empty";
            myCompany.Status = null;
            try
            {
                service.CreateCompanies((new[] { myCompany }));
            }
            catch (Exception ex)
            {
                Assert.IsTrue(ex.Message.Contains("Company may have the following statuses only: Active,Deleted"));
            }
            //wrong status
            myCompany.Status = "Created";
            try
            {
                service.CreateCompanies((new[] { myCompany }));
            }
            catch (Exception ex)
            {
                Assert.IsTrue(ex.Message.Contains("Company may have the following statuses only: Active,Deleted"));
                return;
            }
            Assert.Fail("Expected to fail");
        }
    [Test]
    public void TestCompanyUpdateWrongValues()
    {
        IPortfolioManagementService service = WCFClientUtil.GetPortfolioManagementServiceProxy();
        //create good portfolio
        string myExternalId = Guid.NewGuid().ToString();
       CompanyDto myCompany = new CompanyDto
        {
            ExternalId = myExternalId,
            Name = "Company",
            OrgNo = "OrgNo",
            Roles = new[] {CompanyRole.Customer, CompanyRole.Broker, CompanyRole.Exchange},
            Status = StatusActive
        };
        //creating first
        try
        {
            service.CreateCompanies(new[] { myCompany });
        }
        catch (Exception)
        {
             Assert.Pass("Expected to pass");
        }
       
        //try to update ExternalId
        myCompany.ExternalId = "not possible to update";
        try
        {
            service.UpdateCompanies(new[] { myCompany });
        }
        catch (Exception ex)
        {
            Assert.IsTrue(ex.Message.Contains("Failed to update companies. Company with ExternalId 'not possible to update' doesn't exist."));
        }
        myCompany.ExternalId = myExternalId;
        //update to the status = null
        myCompany.Status = null;
        try
        {
            service.UpdateCompanies(new[] { myCompany });
        }
        catch (Exception ex)
        {
            Assert.IsTrue(ex.Message.Contains("Company may have the following statuses only: Active,Deleted"));
        }
        //update to the wrong status
        myCompany.Status = "Created";
        try
        {
            service.UpdateCompanies(new[] { myCompany });
        }
        catch (Exception ex)
        {
            Assert.IsTrue(ex.Message.Contains("Company may have the following statuses only: Active,Deleted"));
        }
        //company name=null
        myCompany.Status = StatusActive;
        myCompany.Name = null;
        try
        {
            service.UpdateCompanies(new[] { myCompany });
        }
        catch (Exception ex)
        {
            Assert.IsTrue(ex.Message.Contains("Failed to update companies. Name can't be empty"));
        }  
        myCompany.Name = "CompanyUpdated";
        myCompany.OrgNo = String.Empty;
        try
        {
            service.UpdateCompanies(new[] { myCompany });
        }
        catch (Exception ex)
        {
            Assert.IsTrue(ex.Message.Contains("OrgNo can't be empty. It accepts null or meaningful value."));
        } 
        
        // ////correct update, set Status = deleted
        myCompany.OrgNo = "OrgNo";
        myCompany.Status = StatusDeleted;
        try
        {
            service.UpdateCompanies(new[] { myCompany });
        }
        catch (Exception ex)
        {
            Assert.IsTrue(ex.Message.Contains("Update portfolio failed."));
        }
    }

    [Test]
    public void TestCompanyWrongParentExternalID()
    {
        IPortfolioManagementService service = WCFClientUtil.GetPortfolioManagementServiceProxy();
        //CREATE 
        string myExternalId = Guid.NewGuid().ToString();
        //ParentExternalId = string.Empty
        CompanyDto myCompany = new CompanyDto
        {
            ExternalId = myExternalId,
            Name = "Company",
            OrgNo = "OrgNo",
            Roles = new[] { CompanyRole.Customer, CompanyRole.Broker, CompanyRole.Exchange },
            Status = StatusActive,
            ParentExternalId = string.Empty
        };
        try
        {
            service.CreateCompanies((new[] { myCompany }));
        }
        catch (Exception ex)
        {
            Assert.IsTrue(ex.Message.Contains("ParentExternalId can't be empty. It accepts null or meaningful value"));
        }
        //wrong ParentExternalId = company ExternalId
        myCompany.ParentExternalId = myExternalId;
        try
        {
            service.CreateCompanies((new[] { myCompany }));
        }
        catch (Exception ex)
        {
            Assert.IsTrue(ex.Message.Contains("Failed to create companies. Company with ExternalId"));
        }
        //successful company creating
        myCompany.ParentExternalId = null;
        try
        {
            service.CreateCompanies((new[] { myCompany }));
        }
        catch (Exception ex)
        {
            Assert.IsTrue(ex.Message.Contains("Create company failed."));
        }
        //UPDATE
        // //ParentExternalId = string.Empty;
        myCompany.ParentExternalId = string.Empty;
        try
        {
            service.UpdateCompanies(new[] { myCompany });
        }
        catch (Exception ex)
        {
            Assert.IsTrue(ex.Message.Contains("ParentExternalId can't be empty. It accepts null or meaningful value."));
        }
        //wrong ParentExternalId
        myCompany.ParentExternalId = Guid.NewGuid().ToString();
        try
        {
            service.UpdateCompanies(new[] { myCompany });
        }
        catch (Exception ex)
        {
            Assert.IsTrue(ex.Message.Contains("Failed to update companies"));
        }
        ////successful updating, set Status = deleted
        myCompany.ParentExternalId = null;
        myCompany.Status = StatusDeleted;
        try
        {
            service.UpdateCompanies(new[] { myCompany });
        }
        catch (Exception ex)
        {
            Assert.IsTrue(ex.Message.Contains("Update company failed."));
        }
    }

    [Test]
    public void TestCompanyWrongCountryIsoCode()
        {
            IPortfolioManagementService service = WCFClientUtil.GetPortfolioManagementServiceProxy();
            string myExternalId = Guid.NewGuid().ToString();
            string CountryIsoCode = "NO";
            CompanyDto myCompanyDto = new CompanyDto
            {
                ExternalId = myExternalId,
                Name = "Name",
                ParentExternalId = null,
                CountryIsoCode = string.Empty, // string.Empty is not allowed
                OrgNo = "OrgNo",
                Status = StatusActive,
                Roles = new[] { CompanyRole.Customer, CompanyRole.Broker, CompanyRole.Exchange },
            };

            try
            {
               service.CreateCompanies(new[] { myCompanyDto });
            }
            catch (Exception ex)
            {
                Assert.IsTrue(ex.Message.Contains("CountryISOCode can't be empty. It accepts null or meaningful value"));
            }
            //passing, null is acceptable
            myCompanyDto.CountryIsoCode = null;
            try
            {
                service.CreateCompanies(new[] { myCompanyDto });
            }
            catch (Exception)
            {
                Assert.Fail("Expect to pass.");
            }
            //passing
            myCompanyDto.CountryIsoCode = CountryIsoCode;
            try
            {
                service.UpdateCompanies(new[] { myCompanyDto });
            }
            catch (Exception)
            {
                Assert.Fail("Expect to pass.");
            }

            CompanyDto[] companies =
                service.FindCompanies(new[] { myExternalId });
            Assert.AreEqual(myCompanyDto.ExternalId, companies[0].ExternalId);
            Assert.AreEqual(myCompanyDto.Name, companies[0].Name);
            Assert.AreEqual(CountryIsoCode, companies[0].CountryIsoCode);

            //Delete created company
            myCompanyDto.Status = StatusDeleted;
            service.UpdateCompanies(new[] { myCompanyDto });
            CompanyDto[] foundCompanyDel = service.FindCompanies(new [] { myExternalId });
            Assert.AreEqual(myCompanyDto.Status, foundCompanyDel[0].Status);
        }

     [Test] 
     public void TestCompanyWrongAddressesAndPhones()
        {
            IPortfolioManagementService service = WCFClientUtil.GetPortfolioManagementServiceProxy();
            
            string myExternalId = Guid.NewGuid().ToString();
         
            CompanyDto myCompany = new CompanyDto
            {
                ExternalId = myExternalId,
                ParentExternalId = null,
                CountryIsoCode = "DK",
                Name = "APINegativeTest",
                OrgNo = "new org NO",
                Status = StatusActive,
                Roles = new[] {CompanyRole.Customer, CompanyRole.Broker}
            };
            //Addresses
            Property[] companyAddresses = new Property[1];
            companyAddresses[0] = new Property
            {
                Name = "NonExisting property",
                Value = "test.Addresses[i].value",
                ValueType = "System.String"
            };

            myCompany.Addresses = companyAddresses;
                // tring to create and update company with nonexisted property name
                try
                {
                    service.CreateCompanies(new[] { myCompany });
                }
                catch (Exception ex)
                {
                    Assert.IsTrue(ex.Message.Contains("Property NonExisting property is not recognized"));
                }
                try
                {
                    service.UpdateCompanies(new[] { myCompany });
                }
                catch (Exception ex)
                {
                    Assert.IsTrue(ex.Message.Contains("Property NonExisting property is not recognized"));
                }
            //set correct value for Address
            myCompany.Addresses = null;
            //Phones
            Property[] companyPhones = new Property[1];//[propertyLenght];
            companyPhones[0] = new Property();
            {
                companyPhones[0].Name = "Mobile";
                companyPhones[0].Value = "65856126555";
                companyPhones[0].ValueType = "Elviz.API.CompanyDto"; //incorect
            }

            myCompany.Phones = companyPhones;
                try
                {
                    service.CreateCompanies(new[] { myCompany });
                }
                catch (Exception ex)
                {
                    Assert.IsTrue(ex.Message.Contains("Property type [Elviz.API.CompanyDto] provided for Mobile. Only string properties are allowed"));
                }
         companyPhones[0].ValueType = "System.String"; //correct
         companyPhones[0].Name = "MyMobile"; //incorrect
         myCompany.Phones = companyPhones;
         try
         {
             service.CreateCompanies(new[] { myCompany });
         }
         catch (Exception ex)
         {
             Assert.IsTrue(ex.Message.Contains("Property MyMobile is not recognized"));
         }
         
        }



     [Test]
     public void TestCompanyWrongShortNameValue()
     {
         IPortfolioManagementService service = WCFClientUtil.GetPortfolioManagementServiceProxy();
         string myExternalId = Guid.NewGuid().ToString();
         CompanyDto myCompanyDto = new CompanyDto
         {
             ExternalId = myExternalId,
             Name = "Name",
             ParentExternalId = null,
             CountryIsoCode = "NO", // string.Empty is not allowed
             OrgNo = "OrgNo",
             Status = StatusActive,
             ShortName = "123456789012345678901234567", //max 25 symbols, in ECM max = 15 when creating, but can be showen more than 15
             Roles = new[] { CompanyRole.Customer, CompanyRole.Broker, CompanyRole.Exchange },
         };

         try
         {
             service.CreateCompanies(new[] { myCompanyDto });
         }
         catch (Exception ex)
         {
               Assert.IsTrue(ex.Message.Contains("Value for ShortName exceeds maximum allowed length of 25"));
         }
     }


        [Test]
        public void TestCompanyCreateWithWrongCreditRisk()
        {
            IPortfolioManagementService service = WCFClientUtil.GetPortfolioManagementServiceProxy();
            
            //set credit risk properties
            CreditRiskPropertiesDto crRiskProp = new CreditRiskPropertiesDto();
            crRiskProp.Netting = true;
            crRiskProp.CreditLimit = 0.010203040506;
            crRiskProp.CreditRating = "A";
            crRiskProp.CreditLimitIsoCurrency = "RRR";
            
            //wrong CreditLimitIsoCurrency
            // create company 
            CompanyDto myCompany = new CompanyDto
            {
                ExternalId = Guid.NewGuid().ToString(),
                Name = "TestCompanyCreditRisk",
                CountryIsoCode = "NO",
                CreditRiskProperties = crRiskProp,
                OrgNo = "OrgNo",
                ShortName = "BCR",
                Roles = new[] {CompanyRole.Customer},
                Status = StatusActive
            };

            try
            {
                service.CreateCompanies((new[] { myCompany }));
            }
            catch (Exception ex)
            {
                Assert.IsTrue(ex.Message.Contains("CreditLimitIsoCurrency RRR doesn't exist."));
            }


            //too long CreditLimitIsoCurrency
            crRiskProp.CreditLimitIsoCurrency = "NNOK";
            try
            {
                service.CreateCompanies((new[] { myCompany }));
            }
            catch (Exception ex)
            {
                Assert.IsTrue(ex.Message.Contains("Value for CreditRatingIsoCurrency exceeds maximum allowed length of 3."));
            }
            
          
            // wrong credit rating
            crRiskProp.CreditLimitIsoCurrency = "NOK";
            crRiskProp.CreditRating = "wrong";
            try
            {
                service.CreateCompanies((new[] { myCompany }));
            }
            catch (Exception ex)
            {
                 Assert.IsTrue(ex.Message.Contains("CreditRating wrong doesn't exist."));
            }
         
        }
    }
}