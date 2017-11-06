using System;
using System.Text;

namespace EcmBatchTests.Utils
{
    public static class ArrayExtentions
    {
        public static string ToLogString(this object obj)
        {
            var stringBuilder = new StringBuilder();
            if (obj as object[] != null)
            {                
                foreach (var o in ((object[]) obj))
                {
                    stringBuilder.Append(o.ToLogString());
                    stringBuilder.Append(',');
                }
            }
            else
            {
                if (obj == DBNull.Value)
                {
                    stringBuilder.Append("null");
                }
                else
                {
                    if (obj.GetType() == typeof (DateTime))
                    {
                        var dt = (DateTime) obj;
                        stringBuilder.Append(dt.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                        /*
                        if (dt.Second == dt.Minute && dt.Minute == dt.Hour && dt.Hour == 0)
                            stringBuilder.Append(dt.ToString("yyyy-MM-dd"));
                        else
                        {
                            stringBuilder.Append(dt.ToString("yyyy-MM-dd HH:mm"));
                        }
                         * */
                    }
                    else if (obj.GetType() == typeof (bool))
                    {
                        stringBuilder.Append(((bool) obj) ? 1 : 0);
                    }
                    else stringBuilder.Append(obj.ToString());
                }
            }
            

            return stringBuilder.ToString().TrimEnd(',');
        }
    }
}
