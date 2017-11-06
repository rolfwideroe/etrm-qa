using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ExcelTransactionWrapper
{
    public class Helper
    {
        public static void SetProp(object o, string property, object v)
        {
            
            FieldInfo info = o.GetType().GetField(property);
            info.SetValue(o, System.Convert.ChangeType(v, info.FieldType));
        }
    }
}
