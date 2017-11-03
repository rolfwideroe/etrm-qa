using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ElvizTestUtils;
using NUnit.Framework;

namespace TestCompleteCompareTransactions
{
    class PAMSettings
    {
        [Test]
        public static void PAMOn()
        {
            if (! ConfigurationTool.PamEnabled)
            {
                ConfigurationTool.PamEnabled = true;
            }
        }

         [Test]
        public static void PAMOff()
        {
            if (ConfigurationTool.PamEnabled)
            {
                ConfigurationTool.PamEnabled = false;
            }
        }
    }
}
