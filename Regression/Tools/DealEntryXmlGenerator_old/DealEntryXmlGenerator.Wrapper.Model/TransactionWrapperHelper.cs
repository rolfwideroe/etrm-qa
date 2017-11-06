using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace DealEntryXmlGenerator.Wrapper.Model
{
    public class TransactionWrapperHelper
    {
        public static void SetProp(DealEntryWrapper wrapper, string property, object value)
        {


            

            FieldInfo info = wrapper.GetType().GetField(property);

            //Type fieldtype = info.FieldType.UnderlyingSystemType;

            //if(info.FieldType.UnderlyingSystemType==typeof(DateTime)&&(string) value=="")
            //    return;

            try
            {
                info.SetValue(wrapper, System.Convert.ChangeType(value, info.FieldType));
            }
            catch (Exception)
            {
                throw new ArgumentException("The Property: " +property + " is invalid");
            }


        }
    }
}
