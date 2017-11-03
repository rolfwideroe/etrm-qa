using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestCompleteRequestToActiveMQ
{
    class ElvizQAUtil
    {
        public static string ReturnElvizServerName()
        {
            string servername = ElvizTestUtils.ElvizInstallationUtility.GetAppServerName();

            return servername;
        }
    }
}
