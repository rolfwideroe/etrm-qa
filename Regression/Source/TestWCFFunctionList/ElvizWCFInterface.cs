using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Description;
using ElvizTestUtils;
using NUnit.Framework;

namespace TestWCFFunctionList
{
    public class ElvizWCFInterface
    {
        [Test]
        public void CheckLookupService()
        {
                const string DealServiceUrl = "http://{0}:8009/LookupService";
                string appServerName = ElvizInstallationUtility.GetAppServerName();
                EndpointAddress address = WCFClientUtil.GetDefaultEndPointAdresss(DealServiceUrl, appServerName);

                List<string> actualFunctionList = new List<string>();
                List<string> expectedFunctionList = new List<string>(new String[]
                {
                    "LookUp",
                    "GetAllPortfolios",
                    "GetAllCompanies",
                    "GetTransactionsByFilterAndTradeDate",
                    "GetTransactionsInFilter",
                    "DoesFilterExist",
                    "GetTransactionsInFilterForUser",
                    "DoesFilterExistForUser",
                    "GetExecutionVenueHolidays",
                    "GetAuthorisationsForAllPortfolios",
                    "GetTransactionsByIds",
                    "GetAuditTrailEntries",
                    "GetTransactionsInFilterByIds",
                    "GetTransactionsByFilterAndReportDate",
                    "GetTransactionsByIdsAndReportDate",
                    "GetInstrumentCodesByTransactionIds",
                    "GetInstrumentCodesByTransactionExternalIds",
                    "GetProductNames",
                    "GetOriginalTransactionsByExternalIds",
                    "GetOriginalTransactionsByUTI",
                    "GetProductionFacilities"
                });
                expectedFunctionList.Sort();

                try
                {
                    // Define target endpoint
                    // Setup the mexClient and importer to get the metadata
                    HttpBindingBase binding = WCFClientUtil.GetDefaultBasicHttpBinding(); // setting binding same as for server

                    MetadataExchangeClient mexClient = new MetadataExchangeClient(binding)
                    {
                        ResolveMetadataReferences = true
                    };

                    mexClient.MaximumResolvedReferences = 100; 
                    MetadataSet metadataSet = mexClient.GetMetadata(new Uri(address.Uri + "/mex"), MetadataExchangeClientMode.HttpGet);

                    WsdlImporter importer = new WsdlImporter(metadataSet);
                    // Now get all the metadata from the importer
                    ServiceEndpointCollection endpointCollection = importer.ImportAllEndpoints();
                    foreach (ServiceEndpoint endpoint in endpointCollection)
                    {
                       // Console.WriteLine(endpoint.Address);
                    }
                
                    foreach (ServiceEndpoint endpoint in endpointCollection)
                    {
                        foreach (OperationDescription operation in endpoint.Contract.Operations)
                        {
                            actualFunctionList.Add(operation.Name);
                           // Console.WriteLine(operation.Name);
                        }
                    }
                    actualFunctionList.Sort();
                }
                catch (Exception ex)
                {
                    Assert.Fail("Retrieving operations failed for " + address + ". " + ex.Message +". Inner = "+ ex.InnerException);
                }

                Assert.AreEqual(expectedFunctionList.Count, actualFunctionList.Count,
                    "Function list for Lookup Service is not equal to expected one: ");
                for (int i = 0; i < expectedFunctionList.Count; i++)
                {
                    Assert.AreEqual(expectedFunctionList[i], actualFunctionList[i], "Names of function are not equal: ");
                }
        }

        [Test]
        public void CheckDealService()
        {
            const string ServiceUrl = "http://{0}:8009/DealService";
            string appServerName = ElvizInstallationUtility.GetAppServerName();
            EndpointAddress address = WCFClientUtil.GetDefaultEndPointAdresss(ServiceUrl, appServerName);
            
            List<string> actualFunctionList = new List<string>();
            List<string> expectedFunctionList = new List<string>(new String[]
                {
                    "ImportDeal",
                    "UpdateStatus",
                    "SetPropertyValues",
                    "SetGroupField",
                    "PartialUpdateImportDeal"
                });
            expectedFunctionList.Sort();

            try
            {
                // Define target endpoint
                // Setup the mexClient and importer to get the metadata
                HttpBindingBase binding = WCFClientUtil.GetDefaultBasicHttpBinding(); // setting binding same as for server
                MetadataExchangeClient mexClient = new MetadataExchangeClient(binding)
                {
                    ResolveMetadataReferences = true
                };
                MetadataSet metadataSet = mexClient.GetMetadata(new Uri(address.Uri + "/mex"), MetadataExchangeClientMode.HttpGet);

                WsdlImporter importer = new WsdlImporter(metadataSet);
                // Now get all the metadata from the importer
                ServiceEndpointCollection endpointCollection = importer.ImportAllEndpoints();
                foreach (ServiceEndpoint endpoint in endpointCollection)
                {
                    foreach (OperationDescription operation in endpoint.Contract.Operations)
                    {
                        //Console.WriteLine(operation.Name);
                        actualFunctionList.Add(operation.Name);
                    }
                }
                actualFunctionList.Sort();
            }
            catch (Exception)
            {
                Assert.Fail("Retrieving operations failed for " + address);
            }
            
            Assert.AreEqual(expectedFunctionList.Count, actualFunctionList.Count,
                   "Function list for Lookup Service is not equal to expected one: ");
            for (int i = 0; i < expectedFunctionList.Count; i++)
            {
                Assert.AreEqual(expectedFunctionList[i], actualFunctionList[i], "Names of function are not equal: ");
            }
        }

        [Test]
        public void CheckPortfolioManagementService()
        {
            const string ServiceUrl = "http://{0}:8009/PortfolioManagementService";
            string appServerName = ElvizInstallationUtility.GetAppServerName();
            EndpointAddress address = WCFClientUtil.GetDefaultEndPointAdresss(ServiceUrl, appServerName);

            List<string> actualFunctionList = new List<string>();
            List<string> expectedFunctionList = new List<string>(new String[]
                {
                    "CreateCompanies",
                    "UpdateCompanies",
                    "FindCompanies",
                    "GetAllPortfolios",
                    "CreatePortfolios",
                    "UpdatePortfolios",
                    "FindPortfolios"
                });
            expectedFunctionList.Sort();

            try
            {
                // Define target endpoint
                // Setup the mexClient and importer to get the metadata
                HttpBindingBase binding = WCFClientUtil.GetDefaultBasicHttpBinding(); // setting binding same as for server
                MetadataExchangeClient mexClient = new MetadataExchangeClient(binding)
                {
                    ResolveMetadataReferences = true
                };
                MetadataSet metadataSet = mexClient.GetMetadata(new Uri(address.Uri + "/mex"), MetadataExchangeClientMode.HttpGet);
                WsdlImporter importer = new WsdlImporter(metadataSet);
                // Now get all the metadata from the importer
                ServiceEndpointCollection endpointCollection = importer.ImportAllEndpoints();
                foreach (ServiceEndpoint endpoint in endpointCollection)
                {
                    foreach (OperationDescription operation in endpoint.Contract.Operations)
                    {
                        //Console.WriteLine(operation.Name);
                        actualFunctionList.Add(operation.Name);
                    }
                }
                actualFunctionList.Sort();
            }
            catch (Exception)
            {
                Assert.Fail("Retrieving operations failed for " + address);
            }

            Assert.AreEqual(expectedFunctionList.Count, actualFunctionList.Count,
                   "Function list for Lookup Service is not equal to expected one: ");
            for (int i = 0; i < expectedFunctionList.Count; i++)
            {
                Assert.AreEqual(expectedFunctionList[i], actualFunctionList[i], "Names of function are not equal: ");
            }
        }

        [Test]
        public void CheckCurveService()
        {
            const string ServiceUrl = "http://{0}:8009/CurveService";
            string appServerName = ElvizInstallationUtility.GetAppServerName();
            EndpointAddress address = WCFClientUtil.GetDefaultEndPointAdresss(ServiceUrl, appServerName);

            List<string> actualFunctionList = new List<string>();
            List<string> expectedFunctionList = new List<string>(new String[]
                {
                    "GetPriceCurve",
                    "GetPriceCurveByCurrency",
                    "GetForwardInterestRates",
                    "GetForwardExchangeRates",
                    "GetPriceCurveByCriteria"
                });
            expectedFunctionList.Sort();

            try
            {
                // Define target endpoint
                // Setup the mexClient and importer to get the metadata
                HttpBindingBase binding = WCFClientUtil.GetDefaultBasicHttpBinding(); // setting binding same as for server
                MetadataExchangeClient mexClient = new MetadataExchangeClient(binding)
                {
                    ResolveMetadataReferences = true
                };
                MetadataSet metadataSet = mexClient.GetMetadata(new Uri(address.Uri + "/mex"), MetadataExchangeClientMode.HttpGet);
                WsdlImporter importer = new WsdlImporter(metadataSet);
                // Now get all the metadata from the importer
                ServiceEndpointCollection endpointCollection = importer.ImportAllEndpoints();
                foreach (ServiceEndpoint endpoint in endpointCollection)
                {
                    foreach (OperationDescription operation in endpoint.Contract.Operations)
                    {
                       // Console.WriteLine(operation.Name);
                        actualFunctionList.Add(operation.Name);
                    }
                }
                actualFunctionList.Sort();
            }
            catch (Exception)
            {
                Assert.Fail("Retrieving operations failed for " + address);
            }

            Assert.AreEqual(expectedFunctionList.Count, actualFunctionList.Count,
                   "Function list for Lookup Service is not equal to expected one: ");
            for (int i = 0; i < expectedFunctionList.Count; i++)
            {
                Assert.AreEqual(expectedFunctionList[i], actualFunctionList[i], "Names of function are not equal: ");
            }
        }

        [Test]
        public void CheckHistoricMarketDataService()
        {
            const string ServiceUrl = "http://{0}:8009/HistoricMarketDataService";
            string appServerName = ElvizInstallationUtility.GetAppServerName();
            EndpointAddress address = WCFClientUtil.GetDefaultEndPointAdresss(ServiceUrl, appServerName);

            List<string> actualFunctionList = new List<string>();
            List<string> expectedFunctionList = new List<string>(new String[]
                {
                    "GetHistoricExchangeRates"
                });
            expectedFunctionList.Sort();

            try
            {
                // Define target endpoint
                // Setup the mexClient and importer to get the metadata
                HttpBindingBase binding = WCFClientUtil.GetDefaultBasicHttpBinding(); // setting binding same as for server
                MetadataExchangeClient mexClient = new MetadataExchangeClient(binding)
                {
                    ResolveMetadataReferences = true
                };
                MetadataSet metadataSet = mexClient.GetMetadata(new Uri(address.Uri + "/mex"), MetadataExchangeClientMode.HttpGet);
                WsdlImporter importer = new WsdlImporter(metadataSet);
                // Now get all the metadata from the importer
                ServiceEndpointCollection endpointCollection = importer.ImportAllEndpoints();
                foreach (ServiceEndpoint endpoint in endpointCollection)
                {
                    foreach (OperationDescription operation in endpoint.Contract.Operations)
                    {
                       // Console.WriteLine(operation.Name);
                        actualFunctionList.Add(operation.Name);
                    }
                }
                actualFunctionList.Sort();
            }
            catch (Exception)
            {
                Assert.Fail("Retrieving operations failed for " + address);
            }

            Assert.AreEqual(expectedFunctionList.Count, actualFunctionList.Count,
                   "Function list for Lookup Service is not equal to expected one: ");
            for (int i = 0; i < expectedFunctionList.Count; i++)
            {
                Assert.AreEqual(expectedFunctionList[i], actualFunctionList[i], "Names of function are not equal: ");
            }
        }

        [Test]
        public void CheckVolatilityService()
        {
            const string ServiceUrl = "http://{0}:8009/VolatilityService";
            string appServerName = ElvizInstallationUtility.GetAppServerName();
            EndpointAddress address = WCFClientUtil.GetDefaultEndPointAdresss(ServiceUrl, appServerName);

            List<string> actualFunctionList = new List<string>();
            List<string> expectedFunctionList = new List<string>(new String[]
                {
                    "GetImplicitVolatilitiesFromSurface", 
                    "GetImplicitVolatilitiesForStripFromSurface"
                });
            expectedFunctionList.Sort();

            try
            {
                // Define target endpoint
                // Setup the mexClient and importer to get the metadata
                HttpBindingBase binding = WCFClientUtil.GetDefaultBasicHttpBinding(); // setting binding same as for server
                MetadataExchangeClient mexClient = new MetadataExchangeClient(binding)
                {
                    ResolveMetadataReferences = true
                };
                MetadataSet metadataSet = mexClient.GetMetadata(new Uri(address.Uri + "/mex"), MetadataExchangeClientMode.HttpGet);
                WsdlImporter importer = new WsdlImporter(metadataSet);
                // Now get all the metadata from the importer
                ServiceEndpointCollection endpointCollection = importer.ImportAllEndpoints();
                foreach (ServiceEndpoint endpoint in endpointCollection)
                {
                    foreach (OperationDescription operation in endpoint.Contract.Operations)
                    {
                //        Console.WriteLine(operation.Name);
                        actualFunctionList.Add(operation.Name);
                    }
                }
                actualFunctionList.Sort();
            }
            catch (Exception)
            {
                Assert.Fail("Retrieving operations failed for " + address);
            }

            Assert.AreEqual(expectedFunctionList.Count, actualFunctionList.Count,
                   "Function list for Lookup Service is not equal to expected one: ");
            for (int i = 0; i < expectedFunctionList.Count; i++)
            {
                Assert.AreEqual(expectedFunctionList[i], actualFunctionList[i], "Names of function are not equal: ");
            }
        }
    }
}
