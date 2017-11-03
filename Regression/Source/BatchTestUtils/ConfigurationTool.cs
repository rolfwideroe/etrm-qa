using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ElvizTestUtils.DatabaseTools;

namespace ElvizTestUtils
{
    public class ConfigurationTool
    {

        public static string MissingRealizedDataStrategy
        {
            get { return QaDao.MissingRealizedDataStrategy; }
            set
            {
       
                switch (value)
                {
                    case "ReplaceWith0":
                    case "ReplaceWithForecast":
                    case "ThrowException":
                        //ok valid 
                        break;

                    default:
                        //invalid
                        throw new Exception("Invalid MissingRealizedDataStrategy in file: " + value + "\n");

                }
                QaDao.MissingRealizedDataStrategy = value;
            }
        }

        public static bool PamEnabled
        {
            get { return QaDao.PamEnabled; }
            set { QaDao.PamEnabled = value; }
        }
        public static bool AutorizationEnabled
        {
            get { return QaDao.AutorizationEnabled; }
            set { QaDao.AutorizationEnabled = value; }
        }
    }
}
