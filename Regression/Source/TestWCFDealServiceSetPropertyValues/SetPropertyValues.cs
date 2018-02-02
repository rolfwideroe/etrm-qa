using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Xml;
using System.Xml.Serialization;
using ElvizTestUtils;
using ElvizTestUtils.DealServiceReference;
using ElvizTestUtils.LookUpServiceReference;
using ElvizTestUtils.QaLookUp;
using NUnit.Framework;
using Property = ElvizTestUtils.LookUpServiceReference.Property;
using PropertyGroup = ElvizTestUtils.LookUpServiceReference.PropertyGroup;

namespace TestWCFDealServiceSetPropertyValues
{

    [TestFixture]
    public class TestSetPropertyValues
    {
        [OneTimeSetUp]
        public void RunBeforeAnyTests()
        {
            Environment.CurrentDirectory = TestContext.CurrentContext.TestDirectory;
        }


        [Test]
        public void SetUTI()
        {
            IDealService service = WCFClientUtil.GetDealServiceServiceProxy();
            
            int id = 107;
            Dictionary<int, PropertyValuePair[]> gDictionary = new Dictionary<int, PropertyValuePair[]>();
           
            PropertyValuePair[] pair = new PropertyValuePair[1];
            pair[0] = new PropertyValuePair();

            pair[0].PropertyGroup = "Compliance";
            pair[0].PropertyName = "UTI";
            pair[0].Value = "ComplianceUTI";
            pair[0].Value = "ComplianceUTI. Transaction id= " + id;

            gDictionary.Add(id, pair);

            bool result = service.SetPropertyValues(gDictionary);
            Assert.AreEqual(true, result);
           
           //file with expected values
            string filename = Directory.GetCurrentDirectory() + "\\TestFiles\\UTI\\Transaction-" +id + ".xml";

            XmlSerializer serializer = new XmlSerializer(typeof(QaTransactionDTO));
            XmlTextReader reader = new XmlTextReader(filename);
            reader.ReadToDescendant("QaTransactionDTO");
            QaTransactionDTO originalDto = (QaTransactionDTO)serializer.Deserialize(reader.ReadSubtree());
            reader.Close();

            QaLookUpClient c = new QaLookUpClient();
            QaTransactionDTO updatedDto = c.GetQaTransactionDTO(id);

            string[] excludeProps = {   "ReferenceData.ModificationDateTimeUtc", 
                                        "ReferenceData.ReferringId", 
                                        "TransactionWorkFlowDetails.TimeStampAuthorised", 
                                        "TransactionWorkFlowDetails.TimeStampCounterpartyAuthorised",
                                         "TransactionWorkFlowDetails.TimeStampClearedUtc" 
                                    };
           QaTransactionDtoAssert.AreEqual(originalDto, updatedDto, excludeProps, false);

           }
        
        [Test]
        public void SetUTIFewTransactions()
        {
            bool result = false;

            IDealService service = WCFClientUtil.GetDealServiceServiceProxy();
            //ILookupService serviceLookup = this.GetLookUpServiceServiceProxy();
            Dictionary<int, PropertyValuePair[]> gDictionary = new Dictionary<int, PropertyValuePair[]>();

            PropertyValuePair[] pair_1 = new PropertyValuePair[1];

            pair_1[0] = new PropertyValuePair();
            pair_1[0].PropertyGroup = "Compliance";
            pair_1[0].PropertyName = "UTI";
            pair_1[0].Value = "ComplianceUTI for 101-102";

            PropertyValuePair[] pair_2 = new PropertyValuePair[1];

            pair_2[0] = new PropertyValuePair();
            pair_2[0].PropertyGroup = "Compliance";
            pair_2[0].PropertyName = "UTI";
            pair_2[0].Value = "ComplianceUTI for 103-104";

            PropertyValuePair[] pair_3 = new PropertyValuePair[1];

            pair_3[0] = new PropertyValuePair();
            pair_3[0].PropertyGroup = "Compliance";
            pair_3[0].PropertyName = "UTI";
            pair_3[0].Value = "ComplianceUTI for 105-106";

            gDictionary.Add(101, pair_1);
            gDictionary.Add(103, pair_2);
            gDictionary.Add(105, pair_3);

            try
            {
                result = service.SetPropertyValues(gDictionary);
            }
            catch (Exception)
            {

            }

            Assert.AreEqual(true, result, "Wrong SetPropertyValues (few transactions) result: ");
        }
       
        [Test]
        public void SetEmptyUTIValue()
        {

            IDealService service = WCFClientUtil.GetDealServiceServiceProxy();
            ILookupService serviceLookup = WCFClientUtil.GetLookUpServiceServiceProxy();
            Dictionary<int, PropertyValuePair[]> gDictionary = new Dictionary<int, PropertyValuePair[]>();

            PropertyValuePair[] pair_1 = new PropertyValuePair[1];
            int transactionID = 21;
            string propertyGroupName = "Compliance";
            string propertyName = "UTI";
            string propertyValue = string.Empty;

            pair_1[0] = new PropertyValuePair();
            pair_1[0].PropertyGroup = propertyGroupName;
            pair_1[0].PropertyName = propertyName;
            pair_1[0].Value = propertyValue;
            gDictionary.Add(transactionID, pair_1);

            try
            {
                bool result = service.SetPropertyValues(gDictionary);
                Assert.AreEqual(true, result);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }


            int[] ids = { transactionID };
            TransactionDTO[] transaction = serviceLookup.GetTransactionsByIds(ids);

            IList<PropertyGroup> propGroup = transaction[0].PropertyGroups.ToList();

            PropertyGroup propertyGroup = propGroup.FirstOrDefault(x => x.Name == propertyGroupName);
            if (propertyGroup != null)
            {
                List<Property> propList = propertyGroup.Properties.ToList();
                Property propertyItem = propList.FirstOrDefault(x => x.Name == propertyName);
                Assert.AreEqual(null, propertyItem, "Property UTI should not exist because values is empty");
            }
            else Assert.Fail("Error receiving property group" + propertyGroupName);

        }

        [Test]
        public void SetUTIWrongPropertyGroup()
        {

            IDealService service = WCFClientUtil.GetDealServiceServiceProxy();
            Dictionary<int, PropertyValuePair[]> gDictionary = new Dictionary<int, PropertyValuePair[]>();

            PropertyValuePair[] pair_1 = new PropertyValuePair[1];

            pair_1[0] = new PropertyValuePair();
            pair_1[0].PropertyGroup = "ComplianceWrong";
            pair_1[0].PropertyName = "UTI";
            pair_1[0].Value = "ComplianceUTI for 1-2 wrong property group";
            gDictionary.Add(1, pair_1);
            
            try
            {
                bool result = service.SetPropertyValues(gDictionary);
                Assert.AreEqual(true, result);
            }
            catch (Exception ex)
            {
                Assert.IsTrue(ex.Message.Contains("Property Group ComplianceWrong does not exist"));
                //Console.WriteLine("Error: " + ex.Message);
            }
        }

        [Test]
        public void SetUTIEmptyPropertyName()
        {

            IDealService service = WCFClientUtil.GetDealServiceServiceProxy();
            Dictionary<int, PropertyValuePair[]> gDictionary = new Dictionary<int, PropertyValuePair[]>();

            PropertyValuePair[] pair_1 = new PropertyValuePair[1];

            pair_1[0] = new PropertyValuePair();
            pair_1[0].PropertyGroup = "Compliance";
            pair_1[0].PropertyName = String.Empty; ///"UTI";
            pair_1[0].Value = "ComplianceUTI for 1-2 wrong property group";
            gDictionary.Add(1, pair_1);
           
            try
            {
                bool result = service.SetPropertyValues(gDictionary);
                Assert.AreEqual(true, result);
            }
            catch (Exception ex)
            {
                Assert.IsTrue(ex.Message.Contains("Compliance property '' is not supported."));
            }
            
        }

        [Test]
        public void SetUTIWrongPropertyName()
        {

            IDealService service = WCFClientUtil.GetDealServiceServiceProxy();
            ILookupService serviceLookup = WCFClientUtil.GetLookUpServiceServiceProxy();
            Dictionary<int, PropertyValuePair[]> gDictionary = new Dictionary<int, PropertyValuePair[]>();

            PropertyValuePair[] pair_1 = new PropertyValuePair[1];

            pair_1[0] = new PropertyValuePair();
            pair_1[0].PropertyGroup = "Compliance";
            pair_1[0].PropertyName = "UTIWrongName";
            pair_1[0].Value = "ComplianceUTI for 1-2 wrong property group";
            gDictionary.Add(1, pair_1);

            try
            {
                bool result = service.SetPropertyValues(gDictionary);
                Assert.AreEqual(true, result);
            }
            catch (Exception ex)
            {
                Assert.IsTrue(ex.Message.Contains("Compliance property 'UTIWrongName' is not supported."));
            }

        }

        [Test]
        public void SetPropertyIsNotFromGroup()
        {

            IDealService service = WCFClientUtil.GetDealServiceServiceProxy();
            Dictionary<int, PropertyValuePair[]> gDictionary = new Dictionary<int, PropertyValuePair[]>();

            PropertyValuePair[] pair_1 = new PropertyValuePair[1];

            pair_1[0] = new PropertyValuePair();
            pair_1[0].PropertyGroup = "QA";
            pair_1[0].PropertyName = "UTIWrongName";
            pair_1[0].Value = "wrong property group";
            gDictionary.Add(1, pair_1);

            try
            {
                bool result = service.SetPropertyValues(gDictionary);
                Assert.AreEqual(true, result);
            }
            catch (Exception ex)
            {
               Assert.IsTrue(ex.Message.Contains("Custom Property UTIWrongName is not configured or is not user defined so cannot be imported."));
            }

        }

        [Test]
        public void SetPropertyFromOtherGroup()
        {

            IDealService service = WCFClientUtil.GetDealServiceServiceProxy();
            Dictionary<int, PropertyValuePair[]> gDictionary = new Dictionary<int, PropertyValuePair[]>();

            PropertyValuePair[] pair_1 = new PropertyValuePair[1];

            pair_1[0] = new PropertyValuePair();
            pair_1[0].PropertyGroup = "QA";
            pair_1[0].PropertyName = "PropertyCustom5";
            pair_1[0].Value = "PropertyCustom5 is from other group";
            gDictionary.Add(1, pair_1);

            try
            {
                bool result = service.SetPropertyValues(gDictionary);
                Assert.AreEqual(true, result);
            }
            catch (Exception ex)
            {
                Assert.IsTrue(
                    ex.Message.Contains("Custom Property PropertyCustom5 is not a member of property group QA."));
            }
        }

        [Test] 
        public void SetEmptyValueForGroupProperty()
        {
            //first set some value 
            IDealService service = WCFClientUtil.GetDealServiceServiceProxy();
            ILookupService serviceLookup = WCFClientUtil.GetLookUpServiceServiceProxy();
            
            int transactionID = 1;
            string propertyGroupName = "QA";
            string propertyName = "PropertyCustom2";
            string propertySetValue = "api test SetValueForGroupProperty";

            //first set some value for getting property exist
            SetPropertyValue(transactionID, propertyGroupName, propertyName, propertySetValue);
           
            //now set empty porperty value for checking that property with empty values is not exisit in transaction dto
            SetPropertyValue(transactionID, propertyGroupName, propertyName, string.Empty);

            int[] ids = { transactionID };
            TransactionDTO[] transaction = serviceLookup.GetTransactionsByIds(ids);
            
            IList<PropertyGroup> propGroup = transaction[0].PropertyGroups.ToList();

            PropertyGroup propertyGroup = propGroup.FirstOrDefault(x => x.Name == propertyGroupName);
            if (propertyGroup != null)
            {
                List<Property> propList = propertyGroup.Properties.ToList();
                Property propertyItem = propList.FirstOrDefault(x => x.Name == propertyName);
                Assert.AreEqual(null, propertyItem, "Property "+propertyName+ " should not exist because values is empty");
            }
            //else Assert.Fail("Error receiving property group " + propertyGroupName );

        }

        [Test]
        public void SetEmptyValueForAllGroupProperties()
        {
            //first set some values 
            ILookupService serviceLookup = WCFClientUtil.GetLookUpServiceServiceProxy();

            int transactionID = 5;
            string propertyGroupName = "QA";
            string propertyName1 = "PropertyCustom1";
            string propertyName2 = "PropertyCustom2";
            string propertyName3 = "PropertyCustom3";
            string propertySetValue = "api test SetEmptyValueForAllGroupProperties";

            //first set some value for getting property exist
            SetPropertyValue(transactionID, propertyGroupName, propertyName1, propertySetValue);
            SetPropertyValue(transactionID, propertyGroupName, propertyName2, propertySetValue);
            SetPropertyValue(transactionID, propertyGroupName, propertyName3, propertySetValue);

            //now set empty porperties values for checking that property froup will not exisit in transaction dto
            SetPropertyValue(transactionID, propertyGroupName, propertyName1, string.Empty);
            SetPropertyValue(transactionID, propertyGroupName, propertyName2, string.Empty);
            SetPropertyValue(transactionID, propertyGroupName, propertyName3, string.Empty);

            int[] ids = { transactionID };
            TransactionDTO[] transaction = serviceLookup.GetTransactionsByIds(ids);

            IList<PropertyGroup> propGroup = transaction[0].PropertyGroups.ToList();

            PropertyGroup propertyGroup = propGroup.FirstOrDefault(x => x.Name == propertyGroupName);
            if (propertyGroup != null)
                Assert.Fail("Error receiving property group " + propertyGroupName );
        }

        [Test]
        public void SetValueForGroupProperty()
        {
            ILookupService serviceLookup = WCFClientUtil.GetLookUpServiceServiceProxy();

            int transactionID = 3;
            string propertyGroupName = "QA";
            string propertyName = "PropertyCustom3";
            string propertyValue = "api test SetValueForGroupProperty test";

            //setting value
            SetPropertyValue(transactionID, propertyGroupName, propertyName, propertyValue);

            int[] ids = { transactionID };
            TransactionDTO[] transaction = serviceLookup.GetTransactionsByIds(ids);

            IList<PropertyGroup> propGroup = transaction[0].PropertyGroups.ToList();

            PropertyGroup propertyGroup = propGroup.FirstOrDefault(x => x.Name == propertyGroupName);
            if (propertyGroup != null)
            {
                List<Property> propList = propertyGroup.Properties.ToList();
                Property propertyItem = propList.FirstOrDefault(x => x.Name == propertyName);
                if (propertyItem != null)
                    Assert.AreEqual(propertyValue, propertyItem.Value, "Actual property value does not match expected:");
                else
                    Assert.Fail("Error receiving property + " + propertyName + " from " + propertyGroupName +
                                " property group");
            }
            else Assert.Fail("Error receiving property group" + propertyGroupName);

        }

        public static void SetPropertyValue(int transactionID, string propertyGroupName, string propertyName, string propertyValue)
        {
            IDealService service = WCFClientUtil.GetDealServiceServiceProxy();
            Dictionary<int, PropertyValuePair[]> gDictionary = new Dictionary<int, PropertyValuePair[]>();

            PropertyValuePair[] pair_1 = new PropertyValuePair[1];
          
            pair_1[0] = new PropertyValuePair();
            pair_1[0].PropertyGroup = propertyGroupName;
            pair_1[0].PropertyName = propertyName;
            pair_1[0].Value = propertyValue;
            gDictionary.Add(transactionID, pair_1);

            try
            {
                bool result = service.SetPropertyValues(gDictionary);
                Assert.AreEqual(true, result);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }


        [Test]
        public void SetPropertyEmptyProperty()
        {

            IDealService service = WCFClientUtil.GetDealServiceServiceProxy();
            ILookupService serviceLookup = WCFClientUtil.GetLookUpServiceServiceProxy();
            Dictionary<int, PropertyValuePair[]> gDictionary = new Dictionary<int, PropertyValuePair[]>();

            PropertyValuePair[] pair_1 = new PropertyValuePair[1];

            pair_1[0] = new PropertyValuePair();
            pair_1[0].PropertyGroup = "QA";
            pair_1[0].PropertyName = string.Empty;
            pair_1[0].Value = "Value";
            gDictionary.Add(1, pair_1);

            try
            {
                bool result = service.SetPropertyValues(gDictionary);
                Assert.AreEqual(true, result);
            }
            catch (Exception ex)
            {
               // Console.WriteLine(ex.Message);
                Assert.IsTrue(ex.Message.Contains("Custom Property  is not configured or is not user defined so cannot be imported."));
            }
        }

    }
}
