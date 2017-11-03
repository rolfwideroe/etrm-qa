using System;
using System.Reflection;

namespace DealInsertXmlGenerator.Model
{
    public class TransactionWrapperHelper
    {
        public static void SetProp(TransactionWrapper wrapper, string property, object value)
        {
            
            FieldInfo info = wrapper.GetType().GetField(property);

            try
            {
                    info.SetValue(wrapper, System.Convert.ChangeType(value, info.FieldType));
            }
            catch (Exception)
            {
                 throw new ArgumentException(property + " is invalid");
            }
               
           
        }


        public static TransactionWrapperType GetTransactionType(string transactionType)
        {
            TransactionWrapperType myType;
            if (!Enum.TryParse(transactionType, true, out myType))
                throw new ArgumentException(transactionType + " : No such type Exists");
            return myType;
        }
    }


}
