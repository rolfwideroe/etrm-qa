using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ElvizTestUtils.AssertTools;
using NUnit.Framework;

namespace ElvizTestUtils.QaLookUp
{
    public class QaTransactionDtoAssert
    {
   

        public static void AreEqual(QaTransactionDTO expected,QaTransactionDTO actual,string[] properties,bool include)
        {
            AssertErrorList(AreEqualWithErrorList(expected,actual,properties,include));
        }

        public static IList<string> AreEqualWithErrorList(QaTransactionDTO expected, QaTransactionDTO actual, string[] properties, bool include)
        {
            List<string> errorList=new List<string>();
            
            string[] exclude = new[] { "SettlementData", "InstrumentData", "Portfolios", "DealDetails", "FeesData", "ReferenceData", "TransactionWorkFlowDetails", "PropertyGroups" };

            PropertyInfo[] propertyInfos = typeof(QaTransactionDTO).GetProperties();

            PropertyInfo[] propertyInfosWithExclude = GetProperties(propertyInfos, exclude);

            errorList.AddRange(AssertValuesWithErrorList(expected, actual, properties, include, propertyInfosWithExclude, ""));

            errorList.AddRange(AreEqualWithErrorList(expected.SettlementData, actual.SettlementData, properties, include));

            errorList.AddRange(AreEqualWithErrorList(expected.InstrumentData, actual.InstrumentData, properties, include));

            errorList.AddRange(AreEqualWithErrorList(expected.Portfolios, actual.Portfolios, properties, include));

            errorList.AddRange(AreEqualWithErrorList(expected.DealDetails, actual.DealDetails, properties, include));

            errorList.AddRange(AreEqualWithErrorList(expected.FeesData, actual.FeesData, properties, include));

            errorList.AddRange(AreEqualWithErrorList(expected.ReferenceData, actual.ReferenceData, properties, include));

            errorList.AddRange(AreEqualWithErrorList(expected.TransactionWorkFlowDetails, actual.TransactionWorkFlowDetails, properties, include));

            errorList.AddRange(AreEqualWithErrorList(expected.PropertyGroups, actual.PropertyGroups, properties, include));

            return errorList;
        }

        

        private static PropertyInfo[] GetProperties(PropertyInfo[] propertyInfos,string[] excludeNames)
        {
            IList<string> exclude = excludeNames.ToList();

            IList<PropertyInfo> t = propertyInfos;

            IList<PropertyInfo> excludeList = t.ToList().Where(x => exclude.Contains(x.Name)).ToList();

            return t.Except(excludeList).ToArray();
        }

        public static void AreEqual(FeesData expected, FeesData actual, string[] properties,
            bool include)
        {
            AssertErrorList(AreEqualWithErrorList(expected, actual, properties, include));
        }

        public static IList<string> AreEqualWithErrorList(FeesData expected, FeesData actual, string[] properties, bool include)
        {
           

           List<string> errors = new List<string>();
            //return errors;
            if (expected == null && actual == null) return errors;

            if(expected==null)
            {
                errors.Add("FeeData are not equal: Expected is NULL, but actual is NOT NULL");
                return errors;
            }

            if (actual == null)
            {
                errors.Add("FeeData are not equal: Expected is NOT NULL, but actual is NULL");
                return errors;
            }
            
            ////FeesData
            //if (expected.Broker != actual.Broker)
              //  errors.Add("Values for property 'Broker' are not equal: " + expected.Broker + " != " + actual.Broker);
            
            if(expected.Broker!=actual.Broker) errors.Add("Broker did match, Expected is: "+expected.Broker+" but actual was: "+actual.Broker);

            if (expected.ClearingType != actual.ClearingType)
                errors.Add("Values for property 'ClearingType' are not equal: " + expected.ClearingType + " != " + actual.ClearingType);

            if ((expected.Fees != null) && (actual.Fees == null)) errors.Add("Values for property 'Fees' are not equal: " + expected.Fees + " != null");
            if ((expected.Fees == null) && (actual.Fees != null)) errors.Add("Values for property 'Fees' are not equal: null != " + actual.Fees);

            if ((expected.Fees != null) && (actual.Fees != null))
            {
                if (expected.Fees.Count() != actual.Fees.Count())
                    errors.Add("Values for property 'Fees.Count' are not equal: " + expected.Fees.Count() +
                               " != " + actual.Fees.Count());
                else
                {
                    List<Fee> fees = expected.Fees.ToList();
                    IList<Fee> copiedfees = actual.Fees.ToList();

                    List<Fee> FeesSortedList = fees.OrderBy(o => o.FeeType).ThenBy(o=>o.FeeValueType).ToList();

                    List<Fee> CopiedFeesSortedList = copiedfees.OrderBy(o => o.FeeType).ThenBy(o => o.FeeValueType).ToList();
              
                    expected.Fees = FeesSortedList.ToArray();
                    actual.Fees = CopiedFeesSortedList.ToArray();

                    for (int i = 0; i < expected.Fees.Count(); i++)
                    {
                        if (expected.Fees[i].FeeType != actual.Fees[i].FeeType)
                            errors.Add("'Fees." + expected.Fees[i].FeeType + ".ValueType' properties at index= [" + i + "] are not equal: " +
                                expected.Fees[i].FeeType + " != " + actual.Fees[i].FeeType);

                        if (Math.Abs(expected.Fees[i].FeeValue - actual.Fees[i].FeeValue) > GlobalConstTestSettings.DEFAULT_FLOATING_POINT_TOLLERANCE)
                            errors.Add("'Fees." + expected.Fees[i].FeeType + ".FeeValue' properties at index= [" + i + "] are not equal: " +
                                expected.Fees[i].FeeValue + " != " + actual.Fees[i].FeeValue);

                        if (expected.Fees[i].FeeUnit != actual.Fees[i].FeeUnit)
                            errors.Add("'Fees." + expected.Fees[i].FeeType + ".FeeUnit' properties at index= [" + i + "] are not equal: " +
                                expected.Fees[i].FeeUnit + " != " + actual.Fees[i].FeeUnit);

                        if (expected.Fees[i].FeeValueType != actual.Fees[i].FeeValueType)
                            errors.Add("'Fees." + expected.Fees[i].FeeType + ".FeeValueType' properties at index= [" + i + "] are not equal: " +
                                expected.Fees[i].FeeValueType + " != " + actual.Fees[i].FeeValueType);
                    }

                    

                }
            }

           

            return errors;

        }

        public static IList<string> AreEqualWithErrorList(TransactionWorkFlowDetails expected, TransactionWorkFlowDetails actual, string[] properties, bool include)
        {
            PropertyInfo[] instrumentData = typeof(TransactionWorkFlowDetails).GetProperties();

            return AssertValuesWithErrorList(expected, actual, properties, include, instrumentData, "TransactionWorkFlowDetails.");
        }

        public static void AreEqual(TransactionWorkFlowDetails expected, TransactionWorkFlowDetails actual, string[] properties, bool include)
        {
            AssertErrorList(AreEqualWithErrorList(expected,actual,properties,include));
        }

        public static IList<string> AreEqualWithErrorList(DealDetails expected, DealDetails actual, string[] properties, bool include)
        {
            PropertyInfo[] instrumentData = typeof(DealDetails).GetProperties();

            return AssertValuesWithErrorList(expected, actual, properties, include, instrumentData, "DealDetails.");
        }

        public static void AreEqual(DealDetails expected, DealDetails actual, string[] properties, bool include)
        {
            AssertErrorList(AreEqualWithErrorList(expected,actual,properties,include));
        }

        public static IList<string> AreEqualWithErrorList(PropertyGroup[] expected, PropertyGroup[] actual, string[] properties, bool include)
        {
            IList<string> errorList=new List<string>();

            if (ShouldAssertProperty("PropertyGroups", properties, include))
            {
                if (expected == null && actual == null) return errorList;

                if (expected != null)
                {
                    if (actual == null)
                    {
                        errorList.Add("PropertyGroups Length did not match. Expected : " + expected.Length + ", but was null. ");
                        return errorList;
                    }
                    

                    if (expected.Length != actual.Length)
                    {
                        errorList.Add("PropertyGroups Length did not match. Expected : "+expected.Length+", but was : "+actual.Length);
                        return errorList;
                    }

                    for (int i = 0; i < expected.Length; i++)
                    {

                        if (expected[i].Name != actual[i].Name)
                        {
                            errorList.Add("Name for PropertyGroup did not match. Expected : " + expected[i].Name +
                                          " , but was : " + actual[i].Name);
                            return errorList;
                        }

                        if(expected[i].Properties.Length!=actual[i].Properties.Length)
                        {
                            errorList.Add("Number of Properties for PropertyGroup : "+expected[i].Name+" did not match. Expected : " + expected[i].Properties.Length +
                                          " , but was : " + actual[i].Properties.Length);
                            return errorList;
                        }

                        if (expected[i].Properties != null)
                        {
                            Property[] expectedProperties = expected[i].Properties.OrderBy(x => x.Name).ToArray();
                            Property[] actualProperties = actual[i].Properties.OrderBy(x => x.Name).ToArray();

                            for (int j = 0; j < expectedProperties.Length; j++)
                            {
                                bool hasError = expectedProperties[j].Name != actualProperties[j].Name || 
                                    expectedProperties[j].Value != actualProperties[j].Value || 
                                    expectedProperties[j].ValueType != actualProperties[j].ValueType;

                                if (hasError)
                                {
                                    string errorString = "Properties in PropertyGroup" + expected[i].Name + " did not match\n"
                                                         + "\t Expected : Name = " + expectedProperties[j].Name +
                                                         " Value = " + expectedProperties[j].Value + " ValueType = " +
                                                         expectedProperties[j].ValueType + "\n"
                                                         + "\t Actual   : Name = " + actualProperties[j].Name +
                                                         " Value = " + actualProperties[j].Value + " ValueType = " +
                                                         actualProperties[j].ValueType;

                                    errorList.Add(errorString);
                                }
                            }
                        }
                    }
                }
            }

            return errorList;
        }

        public static IList<string> AreEqualWithErrorList(ReferenceData expected, ReferenceData actual, string[] properties, bool include)
        {
            PropertyInfo[] refData = typeof(ReferenceData).GetProperties();

            string[] exclude = new[] { "DealGroups" };

            PropertyInfo[] propertyInfosWithExclude = GetProperties(refData, exclude);

            return AssertValuesWithErrorList(expected, actual, properties, include, propertyInfosWithExclude, "ReferenceData.");
        }

        public static void AreEqual(ReferenceData expected, ReferenceData actual, string[] properties, bool include)
        {
            AssertErrorList(AreEqualWithErrorList(expected,actual,properties,include));
        }

        public static IList<string> AreEqualWithErrorList(Portfolios expected, Portfolios actual, string[] properties, bool include)
        {
            PropertyInfo[] portfolioData = typeof(Portfolios).GetProperties();


            return AssertValuesWithErrorList(expected, actual, properties, include, portfolioData, "Portfolios.");
        }

        public static void AreEqual(Portfolios expected, Portfolios actual, string[] properties, bool include)
        {
            AssertErrorList(AreEqualWithErrorList(expected, actual, properties, include));
        }

        public static void AreEqual(InstrumentData expected, InstrumentData actual, string[] properties, bool include)
        {
            AssertErrorList(AreEqualWithErrorList(expected,actual,properties,include));
        }

        public static IList<string> AreEqualWithErrorList(InstrumentData expected, InstrumentData actual, string[] properties, bool include)
        {
            PropertyInfo[] instrumentData = typeof(InstrumentData).GetProperties();

            return AssertValuesWithErrorList(expected, actual, properties, include, instrumentData, "InstrumentData.");
        }


        public static void AreEqual(SettlementData expected, SettlementData actual, string[] properties,
            bool include)
        {
            AssertErrorList(AreEqualWithErrorList(expected,actual,properties,include));
        }

        public static IList<string> AreEqualWithErrorList(SettlementData expected, SettlementData actual, string[] properties,
            bool include)
        {
            List<string> errorList=new List<string>();

            if (expected == null && actual == null) return errorList;

            PropertyInfo[] settlementDataPropertyInfos = typeof(SettlementData).GetProperties();

            string[] exclude = new[] { "PriceVolumeTimeSeriesDetails", "TimeSeriesSet" };

            PropertyInfo[] propertyInfosWithExclude = GetProperties(settlementDataPropertyInfos, exclude);

            errorList.AddRange(AssertValuesWithErrorList(expected, actual, properties, include, propertyInfosWithExclude, "SettlementData."));

            errorList.AddRange(AreEqualWithErrorList(expected.PriceVolumeTimeSeriesDetails, actual.PriceVolumeTimeSeriesDetails, properties, include));

            errorList.AddRange(AreEqualWithErrorList(expected.TimeSeriesSet, actual.TimeSeriesSet, properties, include));

            return errorList;
        }

        public void AreEqual(TimeSeriesSet expected, TimeSeriesSet actual,
            string[] properties, bool include)
        {
            AssertErrorList(AreEqualWithErrorList(expected,actual,properties,include));
        }

        public static IList<string> AreEqualWithErrorList(TimeSeriesSet expected,TimeSeriesSet actual,string[] properties,bool include)
        {
            IList<string> errorList=new List<string>();

            if (!include)
            {
                return errorList;
            }

            if (actual == null && expected == null) return errorList;

            if (actual == null)
            {
                errorList.Add("Actual TimeSeriesSet is NULL");
                return errorList;
            }

            TimeSeries[] expectedSeries = expected.TimeSeries;
            TimeSeries[] actualSeries = actual.TimeSeries;

            if (expectedSeries == null && actualSeries == null) return errorList;

            if (expectedSeries == null)
            {
                errorList.Add("Actual TimeSeries is NULL");
                return errorList;
            }

            Assert.AreEqual(expectedSeries.Length,actualSeries.Length);

            for (int i = 0; i < expectedSeries.Count(); i++)
            {
                bool seriesHasErors = false;

                if (expectedSeries[i].Resolution != actualSeries[i].Resolution) seriesHasErors = true;
                if (expectedSeries[i].TimeSeriesTypeName != actualSeries[i].TimeSeriesTypeName) seriesHasErors = true;
                if (expectedSeries[i].TimezoneName != actualSeries[i].TimezoneName) seriesHasErors = true;


                if (seriesHasErors)
                {
                    string errorString = "TimeSeries did not match.\n"
                                         + "Expected : Resolution=" + expectedSeries[i].Resolution
                                         + " TimeSeriesTypeName=" + expectedSeries[i].TimeSeriesTypeName
                                         + " TimezoneName=" + expectedSeries[i].TimezoneName + "\n"
                                         + "Actual   : Resolution=" + actualSeries[i].Resolution
                                         + " TimeSeriesTypeName=" + actualSeries[i].TimeSeriesTypeName
                                         + " TimezoneName=" + actualSeries[i].TimezoneName;

                    errorList.Add(errorString);
                    continue;
                }

               

                TimeSeriesValue[] expectedTSV = expectedSeries[i].TimeSeriesValues;
                TimeSeriesValue[] actualTSV = actualSeries[i].TimeSeriesValues;

                for (int j = 0; j < expectedTSV.Length; j++)
                {
                    bool tsvHasErros = false;

                    if (expectedTSV[j].FromDateTime != actualTSV[j].FromDateTime) tsvHasErros = true;
                    if (expectedTSV[j].ToDateTime != actualTSV[j].ToDateTime) tsvHasErros = true;
                    if (expectedTSV[j].Value != actualTSV[j].Value) tsvHasErros = true;


                    if (tsvHasErros)
                    {
                        string errorString = "TimeSeriesValue did not match for TimeSeries : "+expectedSeries[i].TimeSeriesTypeName+". \n"
                                             + "Expected : FromDateTime=" + expectedTSV[j].FromDateTime
                                             + " ToDateTime=" + expectedTSV[j].ToDateTime
                                             + " Value=" + CustomAssert.NullableObjectToString(expectedTSV[j].Value) + " \n"
                                             + "Actual   : FromDateTime=" + actualTSV[j].FromDateTime
                                             + " ToDateTime=" + actualTSV[j].ToDateTime
                                             + " Value=" + CustomAssert.NullableObjectToString(actualTSV[j].Value);

                        errorList.Add(errorString);
                        continue;
                    }

                   
                }
            }
            return errorList;

        }

        public static IList<string> AreEqualWithErrorList(PriceVolumeTimeSeriesDetail[] expected,
            PriceVolumeTimeSeriesDetail[] actual,
            string[] properties,
            bool include)
        {


            IList<string> errorList = new List<string>();

            if (properties.Contains("SettlementData.PriceVolumeTimeSeriesDetails") && !include) return errorList;

            if (actual == null && expected == null) return errorList;

            if (actual == null & expected != null)
            {
                errorList.Add(("SettlementData.PriceVolumeTimeSeriesDetails did not match. Actual is NULL, but Expected is NOT NULL"));
                return errorList;
            }

            if (expected == null)
            {
                errorList.Add(("SettlementData.PriceVolumeTimeSeriesDetails did not match. Actual is NOT NULL, but Expected is NULL"));
                return errorList;
            }

            if (expected.Length != actual.Length)
            {
                errorList.Add("Numbers of PriceVolumeTimeSeriesDetails did not match. Expected :"+ expected.Length+", but was : "+actual.Length);
                return errorList;
            }
            
            for (int i = 0; i < expected.Length; i++)
            {
                bool hasErrors = expected[i].FromDateTime != actual[i].FromDateTime;

                if (expected[i].ToDateTime != actual[i].ToDateTime) hasErrors = true;
                if(Math.Abs(expected[i].Price / actual[i].Price-1) > GlobalConstTestSettings.DEFAULT_FLOATING_POINT_TOLLERANCE) hasErrors = true;
                if(Math.Abs(expected[i].Volume / actual[i].Volume-1) > GlobalConstTestSettings.DEFAULT_FLOATING_POINT_TOLLERANCE) hasErrors = true;

                if (hasErrors)
                {
                    string errorString="PriceVolumeTimeSeriesDatail did not match.\n"
                        +"Expected : FromDateTime="+expected[i].FromDateTime
                        +" ToDateTime="+expected[i].ToDateTime
                        +" Volume="+expected[i].Volume
                        +" Price="+expected[i].Price+"\n"
                        +"Actual  :  FromDateTime="+actual[i].FromDateTime
                        +" ToDateTime="+actual[i].ToDateTime
                        +" Volume="+actual[i].Volume
                        +" Price="+actual[i].Price;

                    errorList.Add(errorString);
                }

                
            }

            return errorList;
        }

        public static void AreEqual(PriceVolumeTimeSeriesDetail[] expected, PriceVolumeTimeSeriesDetail[] actual,
    string[] properties,
    bool include)
        {
            AssertErrorList(AreEqualWithErrorList(expected,actual,properties,include));
        }

        private static bool ShouldAssertProperty(string property,string[] properties, bool include)
        {
            return (properties.Contains(property) == include);

        }


        private static IList<string> AssertValuesWithErrorList(object expected, object actual, string[] properties, bool include, PropertyInfo[] propertyInfos, string preFix)
        {
            IList<string> errorList=new List<string>();

          

            foreach (PropertyInfo propertyInfo in propertyInfos)
            {
                if (ShouldAssertProperty(preFix + propertyInfo.Name, properties, include))
                {
                    Type type = propertyInfo.PropertyType;
                    object expectedObject = propertyInfo.GetValue(expected);
                    object actualObject = propertyInfo.GetValue(actual);

                    string errorString = CustomAssert.AssertValue(type, expectedObject, actualObject, preFix + propertyInfo.Name);

                    if(errorString!=string.Empty)
                        errorList.Add(errorString);               
                }
            }

            return errorList;
        }


        private static void AssertErrorList(IList<string> errorList )
        {
            if (errorList.Count > 0)
            {
                string errorString="";
                foreach (string errorMessage in errorList)
                {
                    errorString += errorMessage + "\n";
                }

                Assert.Fail(errorString);
            }
        }


    }
}
