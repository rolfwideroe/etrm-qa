using System;
using NUnit.Framework;

namespace ElvizTestUtils.QaLookUp
{
    public class CustomAssert
{ 
  
        public static string AssertValue(Type type,object expected, object actual, string fullName)
        {
           
            
            if (expected == null&actual==null) return string.Empty;

            Assert.AreEqual(true, true);

            if ( type== typeof (double?))
            {
                double? expectedDoubleNullable = expected as double?;
                double? actualDoubleNullable = actual as double?;

                if (expectedDoubleNullable == actualDoubleNullable) return string.Empty;

                if (fullName == "SettlementData.Price" || fullName == "SettlementData.Quantity")
                {
                    if (expectedDoubleNullable == null && actualDoubleNullable != null) return string.Empty;
                }

                if (expected == null & actual != null)
                    return fullName + " did not match. Expected : NULL, but was : " + actual;

                if ((expectedDoubleNullable != null) == (actualDoubleNullable != null))
                {
                    double expectedDouble = (double) expectedDoubleNullable;
                    double actualDouble = (double) actualDoubleNullable;

                    if (Math.Abs(expectedDouble/actualDouble - 1) > GlobalConstTestSettings.DEFAULT_FLOATING_POINT_TOLLERANCE)
                        return (fullName  + " did not match. Expected : " +
                                expectedDouble + ", but was : " + actualDouble + ", Relative error of : " +
                                (expectedDouble/actualDouble - 1));
                }
                else
                {
                    return (fullName + " did not match. Expected : " + NullableObjectToString(expectedDoubleNullable) + ", but was : " + NullableObjectToString(actualDoubleNullable));
                }
            }
            else
            {

                if (expected == null & actual != null)
                    return fullName + " did not match. Expected : NULL, but was : " + actual;

                if (!expected.Equals(actual))
                {
                    return (fullName + " did not match. Expected : " + NullableObjectToString(expected) +
                            ", but was : " + NullableObjectToString(actual));
                }
          
                    
            }
           
            return string.Empty;
        }

        public static string NullableObjectToString(object nullableObject)
        {
            if (nullableObject == null)
                return "NULL";
            return nullableObject.ToString();
        }
    }
}