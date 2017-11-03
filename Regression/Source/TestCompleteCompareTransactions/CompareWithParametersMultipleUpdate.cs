using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using ElvizTestUtils.QaLookUp;
using NUnit.Framework;

namespace TestCompleteCompareTransactions
{
    class CompareWithParametersMultipleUpdate
    {
        private const string relativePath = "\\DealManager\\DealManager\\TestFiles\\TestInfo\\MultipleUpdate\\";

        [Test]
        public static string GetTransactionsBeforeUpdates(int transID, double clearingFee, string clearingFeeCurrency, string ticketN, string dealgroupfirst,
            string qaproperty, string customproperty)
        {
            //int transID = 61; double clearingFee = 5; string ticketN = ""; string dealgroupfirst = ""; string qaproperty=""; string customproperty ="";

            string baseDirectory =
                Directory.GetParent(Directory.GetParent(Directory.GetCurrentDirectory()).ToString()).ToString();
            //var errors = new List<string>();
            string result = "";
            QaLookUpClient c = new QaLookUpClient();
            QaTransactionDTO origTransaction = c.GetQaTransactionDTO(transID);
            string feePriceUnit = clearingFeeCurrency + origTransaction.SettlementData.PriceVolumeUnit.Replace("Per", "/");
     
            if (origTransaction.TransactionId == transID) 
            {

                DateTime modificationDate = new DateTime();
                origTransaction.ReferenceData.ModificationDateTimeUtc = modificationDate;
                //origTransaction.InstrumentData.ExpiryDate = modificationDate;
              
                if (origTransaction.FeesData.Fees != null)
                {
                    IList<Fee> fees = origTransaction.FeesData.Fees.ToList();
                    Fee fee = fees.FirstOrDefault(x => x.FeeType == "Clearing");
                    if (fee != null)
                    {
                        if (fee.FeeValueType == "Variable")
                        {
                            fee.FeeUnit = feePriceUnit;
                            fee.FeeValue = clearingFee;
                        }
                    }
                    else
                    {
                        fees.Add(new Fee()
                        {
                            FeeType = "Clearing",
                            FeeValue = clearingFee,
                            FeeUnit = feePriceUnit,
                            FeeValueType = "Variable"
                        });
                        origTransaction.FeesData.Fees = fees.ToArray();
                    }
                }
                else
                {
                    origTransaction.FeesData.Fees = new Fee[] { new Fee("Clearing", clearingFee, feePriceUnit, "Variable"), };
                }

                origTransaction.ReferenceData.TicketNumber = ticketN;

                IList<PropertyGroup> propGroup = origTransaction.PropertyGroups.ToList();

                PropertyGroup propertyDealGroup = propGroup.FirstOrDefault(x => x.Name == "Deal Group");
                if (propertyDealGroup != null)
                {
                    List<Property> propList = propertyDealGroup.Properties.ToList();
                    Property item = propList.FirstOrDefault(x => x.Name == "GroupField1");
                    if (item != null) item.Value = dealgroupfirst;
                    else
                    {
                        Property newItem = new Property("GroupField1", dealgroupfirst, "System.String");
                        propList.Add(newItem);
                        propList = propList.OrderBy(x => x.Name).ToList();
                        propertyDealGroup.Properties = propList.ToArray();

                    }

                }
                else
                {
                    PropertyGroup newDealGroup = new PropertyGroup();
                    newDealGroup.Name = "Deal Group";
                    newDealGroup.Properties = new Property[1] { new Property("GroupField1", dealgroupfirst, "System.String") };
                    propGroup.Add(newDealGroup);
                    origTransaction.PropertyGroups = propGroup.ToArray();
                }

                //IList<PropertyGroup> QAPropGroupList = origTransaction.PropertyGroups.ToList();
                PropertyGroup qaPropertyGroup = propGroup.FirstOrDefault(x => x.Name == "QA");
                if (qaPropertyGroup != null)
                {
                    List<Property> propList = qaPropertyGroup.Properties.ToList();
                    Property item = propList.FirstOrDefault(x => x.Name == "PropertyCustom1");
                    if (item != null) item.Value = qaproperty;
                    else
                    {
                        Property newItem = new Property("PropertyCustom1", customproperty, "System.String");
                        propList.Add(newItem);
                        propList = propList.OrderBy(x => x.Name).ToList();
                        qaPropertyGroup.Properties = propList.ToArray();
                    }


                }
                else
                {
                    PropertyGroup newQaPropertyGroup = new PropertyGroup();
                    newQaPropertyGroup.Name = "QA";
                    newQaPropertyGroup.Properties = new Property[1] { new Property("PropertyCustom1", qaproperty, "System.String") };
                    propGroup.Add(newQaPropertyGroup);
                    origTransaction.PropertyGroups = propGroup.ToArray();
                }

                //IList<PropertyGroup> testCustomerGroupList = origTransaction.PropertyGroups.ToList();
                PropertyGroup testCustomerGroup = propGroup.FirstOrDefault(x => x.Name == "TestCustomer");
                if (testCustomerGroup != null)
                {
                    List<Property> propList = testCustomerGroup.Properties.ToList();
                    Property item = propList.FirstOrDefault(x => x.Name == "PropertyCustom4");
                    if (item != null) item.Value = customproperty;
                    else
                    {
                        Property newItem = new Property("PropertyCustom4", customproperty, "System.String");
                        propList.Add(newItem);
                        propList = propList.OrderBy(x => x.Name).ToList();
                        testCustomerGroup.Properties = propList.ToArray();
                    }


                }
                else
                {
                    PropertyGroup newPropGroup = new PropertyGroup();
                    newPropGroup.Name = "TestCustomer";
                    newPropGroup.Properties = new Property[1] { new Property("PropertyCustom4", customproperty, "System.String") };
                    propGroup.Add(newPropGroup);
                    origTransaction.PropertyGroups = propGroup.ToArray();
                }

                propGroup = propGroup.OrderBy(x => x.Name).ToList();
                origTransaction.PropertyGroups = propGroup.ToArray();

                string filename = Path.GetFullPath(baseDirectory + relativePath) + transID + ".xml";
               // string filename = @"E:\TFS\Development\QA\Regression\Source\TestComplete\DealManager\DealManager\TestFiles\TestInfo\MultipleUpdate\\" +transID + ".xml";
                XmlDocument xmlDocument = new XmlDocument();
                XmlSerializer serializer = new XmlSerializer(origTransaction.GetType());
                MemoryStream stream = new MemoryStream();
                serializer.Serialize(stream, origTransaction);
                stream.Position = 0;
                xmlDocument.Load(stream);
                xmlDocument.Save(filename);
            }

        else result = "Error retrieving transaction by ID";
                    return result;
        }

        [Test]
        public static IList<string> CompareAfterMultipleUpdate(int updId)
        {
           //int origId = 21;
           //int updId = 61 ;
           IList<string> results = new List<string>();
           string baseDirectory = Directory.GetParent(Directory.GetParent(Directory.GetCurrentDirectory()).ToString()).ToString();
           string filename = Path.GetFullPath(baseDirectory + relativePath) + updId + ".xml";
           // string filename = @"E:\TFS\Development\QA\Regression\Source\TestComplete\DealManager\DealManager\TestFiles\TestInfo\MultipleUpdate\\" + updId + ".xml";
            XmlSerializer mySerializer = new XmlSerializer(typeof (QaTransactionDTO)); 
            FileStream myFileStream = new FileStream(filename, FileMode.Open);
            QaTransactionDTO origTransactionDto = (QaTransactionDTO)
                mySerializer.Deserialize(myFileStream);

            QaLookUpClient c = new QaLookUpClient();
            QaTransactionDTO updatedDto = c.GetQaTransactionDTO(updId);
            Assert.AreEqual(updatedDto.TransactionId, updId);
            //change obviously different properties to the same
            DateTime modificationDate = new DateTime();
            updatedDto.ReferenceData.ModificationDateTimeUtc = modificationDate;
           // results = CompareUpdatedTransaction.CompareQaTransactionDTOs(origTransactionDto, updatedDto);
            string[] excludeProps = {   "TransactionId", 
                                        "ReferenceData.ModificationDateTimeUtc", 
                                        "ReferenceData.ReferringId", 
                                        "TransactionWorkFlowDetails.TimeStampAuthorised", 
                                        "TransactionWorkFlowDetails.TimeStampCounterpartyAuthorised",
                                         "TransactionWorkFlowDetails.TimeStampClearedUtc" 
                                    };
            results = QaTransactionDtoAssert.AreEqualWithErrorList(origTransactionDto, updatedDto, excludeProps, false);

            myFileStream.Close();
            foreach (string item in results) Console.WriteLine(item);
            return results;
        }


        [Test]
        public void DEBUG_CompareAfterMultipleUpdate()
        {
            int updId = 25 ;
            IList<string> results = new List<string>();
            string filename = @"E:\TFS\Development\QA\Regression\Source\TestComplete\DealManager\DealManager\TestFiles\TestInfo\MultipleUpdate\\" + updId + ".xml";
            XmlSerializer mySerializer = new XmlSerializer(typeof(QaTransactionDTO));
            FileStream myFileStream = new FileStream(filename, FileMode.Open);
            QaTransactionDTO origTransactionDto = (QaTransactionDTO)
                mySerializer.Deserialize(myFileStream);
            myFileStream.Close();
            QaLookUpClient c = new QaLookUpClient();
            QaTransactionDTO updatedDto = c.GetQaTransactionDTO(updId);
            Assert.AreEqual(updatedDto.TransactionId, updId);
            //change obviously different properties to the same
            DateTime modificationDate = new DateTime();
            updatedDto.ReferenceData.ModificationDateTimeUtc = modificationDate;
            //results = CompareUpdatedTransaction.CompareQaTransactionDTOs(origTransactionDto, updatedDto);
            string[] excludeProps = {   "TransactionId", 
                                        "ReferenceData.ModificationDateTimeUtc", 
                                        "ReferenceData.ReferringId", 
                                        "TransactionWorkFlowDetails.TimeStampAuthorised", 
                                        "TransactionWorkFlowDetails.TimeStampCounterpartyAuthorised",
                                         "TransactionWorkFlowDetails.TimeStampClearedUtc" 
                                    };
            results = QaTransactionDtoAssert.AreEqualWithErrorList(origTransactionDto, updatedDto, excludeProps, false);
           
            foreach (string item in results) Console.WriteLine(item);
        }

        [Test]
        public static string DEBUG_GetTransactionsBeforeUpdates()
        {
            int transID = 207; double clearingFee = 5;
            string clearingFeeCurrency= "NOK";
            string ticketN = ""; string dealgroupfirst = "group A";
            string qaproperty = "qa prop1";
            string customproperty = "cust prop2";

            string baseDirectory =
                Directory.GetParent(Directory.GetParent(Directory.GetCurrentDirectory()).ToString()).ToString();
            //var errors = new List<string>();
            string result = "";
            QaLookUpClient c = new QaLookUpClient();
            QaTransactionDTO origTransaction = c.GetQaTransactionDTO(transID);
            string feePriceUnit = clearingFeeCurrency + origTransaction.SettlementData.PriceVolumeUnit.Replace("Per", "/");

            if (origTransaction.TransactionId == transID)
            {

                DateTime modificationDate = new DateTime();
                origTransaction.ReferenceData.ModificationDateTimeUtc = modificationDate;
                //origTransaction.InstrumentData.ExpiryDate = modificationDate;

                if (origTransaction.FeesData.Fees != null)
                {
                    IList<Fee> fees = origTransaction.FeesData.Fees.ToList();

                    Fee fee = fees.FirstOrDefault(x => x.FeeType == "Clearing");
                    if (fee != null)
                    {
                        if (fee.FeeValueType == "Variable")
                        {
                            fee.FeeUnit = feePriceUnit;
                            fee.FeeValue = clearingFee;
                        }
                    }
                    else
                        {
                            fees.Add( new Fee("Clearing", clearingFee, feePriceUnit, "Variable"));
                            origTransaction.FeesData.Fees = fees.ToArray();

                        }
                }
                else
                    {
                        origTransaction.FeesData.Fees = new Fee[] { new Fee("Clearing", clearingFee, feePriceUnit, "Variable"), };
                    }   

                origTransaction.ReferenceData.TicketNumber = ticketN;

                IList<PropertyGroup> propGroup = origTransaction.PropertyGroups.ToList();

                PropertyGroup propertyDealGroup = propGroup.FirstOrDefault(x => x.Name == "Deal Group");
                if (propertyDealGroup != null)
                {
                    List<Property> propList = propertyDealGroup.Properties.ToList();
                    Property item = propList.FirstOrDefault(x => x.Name == "GroupField1");
                    if (item != null) item.Value = dealgroupfirst;
                    else
                    {
                        Property newItem = new Property("GroupField1", dealgroupfirst, "System.String");
                        propList.Add(newItem);
                        propList = propList.OrderBy(x => x.Name).ToList();
                        propertyDealGroup.Properties = propList.ToArray();

                    }
                    
                }
                else
                {
                    PropertyGroup newDealGroup = new PropertyGroup();
                    newDealGroup.Name = "Deal Group";
                    newDealGroup.Properties = new Property[1] { new Property("GroupField1", dealgroupfirst, "System.String") };
                    propGroup.Add(newDealGroup);
                    origTransaction.PropertyGroups = propGroup.ToArray();                   
                }
               
                //IList<PropertyGroup> QAPropGroupList = origTransaction.PropertyGroups.ToList();
                PropertyGroup qaPropertyGroup = propGroup.FirstOrDefault(x => x.Name == "QA");
                if (qaPropertyGroup != null)
                {
                    List<Property> propList = qaPropertyGroup.Properties.ToList();
                    Property item = propList.FirstOrDefault(x => x.Name == "PropertyCustom1");
                    if (item != null) item.Value = qaproperty;
                    else
                    {
                        Property newItem = new Property ("PropertyCustom1", customproperty, "System.String");
                        propList.Add(newItem);
                        propList = propList.OrderBy(x => x.Name).ToList();
                        qaPropertyGroup.Properties = propList.ToArray();
                    }
                    
                
                }
                else
                {
                    PropertyGroup newQaPropertyGroup = new PropertyGroup();
                    newQaPropertyGroup.Name = "QA";
                    newQaPropertyGroup.Properties = new Property[1] { new Property("PropertyCustom1", qaproperty, "System.String") };
                    propGroup.Add(newQaPropertyGroup);
                    origTransaction.PropertyGroups = propGroup.ToArray();
                }

                //IList<PropertyGroup> testCustomerGroupList = origTransaction.PropertyGroups.ToList();
                PropertyGroup testCustomerGroup = propGroup.FirstOrDefault(x => x.Name == "TestCustomer");
                if (testCustomerGroup != null)
                {
                    List<Property> propList = testCustomerGroup.Properties.ToList();
                    Property item = propList.FirstOrDefault(x => x.Name == "PropertyCustom4");
                    if (item != null) item.Value = customproperty;
                    else
                    {
                        Property newItem = new Property("PropertyCustom4", customproperty, "System.String");
                        propList.Add(newItem);
                        propList = propList.OrderBy(x => x.Name).ToList();
                        testCustomerGroup.Properties = propList.ToArray();
                    }
                  
                 
                }
                else
                {
                    PropertyGroup newPropGroup = new PropertyGroup();
                    newPropGroup.Name = "TestCustomer";
                    newPropGroup.Properties = new Property[1] { new Property("PropertyCustom4", customproperty, "System.String") };
                    propGroup.Add(newPropGroup);
                    origTransaction.PropertyGroups = propGroup.ToArray();
                }
                origTransaction.PropertyGroups = propGroup.ToArray();

                // string filename = Path.GetFullPath(baseDirectory + relativePath) + transID + ".xml";
                string filename = @"C:\TFS\Development\QA\Regression\Source\TestComplete\DealManager\DealManager\TestFiles\TestInfo\MultipleUpdate\\" + transID + ".xml";
                XmlDocument xmlDocument = new XmlDocument();
                XmlSerializer serializer = new XmlSerializer(origTransaction.GetType());
                MemoryStream stream = new MemoryStream();
                serializer.Serialize(stream, origTransaction);
                stream.Position = 0;
                xmlDocument.Load(stream);
                xmlDocument.Save(filename);
            }

            else result = "Error retrieving transaction by ID";
            return result;
        }
    }
}
