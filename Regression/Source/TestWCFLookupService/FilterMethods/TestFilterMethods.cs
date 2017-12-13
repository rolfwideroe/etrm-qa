using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ElvizTestUtils;
using ElvizTestUtils.LookUpServiceReference;
using NUnit.Framework;

namespace TestWCFLookupService.FilterMethods
{
    class TestFilterMethods
    {
        [Test]
        public void TestGetTransactionIdsByFilter()
        {

            ILookupService serviceLookup = WCFClientUtil.GetLookUpServiceServiceProxy();
            Int32[] transactionsList = serviceLookup.GetTransactionIdsByFilter("Update");

            Assert.Greater(transactionsList.Length, 1);

            //foreach (int tr_id in transactionsList)
            //{
            //    Console.Write(tr_id);
            //}
        }
    }
}
