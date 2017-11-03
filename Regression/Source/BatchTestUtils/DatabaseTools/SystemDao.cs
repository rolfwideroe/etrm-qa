using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElvizTestUtils.DatabaseTools
{
    public class SystemDao
    {

        public static string GetEtrmInstallation(SqlConnection systemConnection)
        {
            const string query = "SELECT [SysOwnerName] FROM SystemInfo";

            return QaDao.ReadScalar<string>(systemConnection, query);
        }

        //public static DateTime GetSystemDate(SqlConnection systemConnection)
        //{
        //    string sysQuery = @"SELECT [SysTradeDate] FROM [SystemInfo]";

        //    DateTime sysDate = QaDao.ReadScalar<DateTime>(systemConnection, sysQuery);

        //    return sysDate;
        //}

        public static void TurnOfAutomaticOpening(SqlConnection systemConnection)
        {
            string query = @"  UPDATE [SystemInfo] SET TradeDateAutoOpen=0";
            QaDao.NonQuery(systemConnection, query);
        }

        public static void UpdateSystemDateToToday(SqlConnection systemConnection)
        {
            string query = @"  UPDATE [SystemInfo] SET [SysTradeDate]=@today";

            DateTime today=DateTime.Today;

            IList<SqlParameter> parameters=new List<SqlParameter>();
            parameters.Add(new SqlParameter("@today",today));

            QaDao.NonQuery(systemConnection, query,parameters);
        }
    }
}
