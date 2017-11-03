using ElvizTestUtils;
using ElvizTestUtils.LookUpServiceReference;
using NUnit.Framework;

namespace CompareTransactions
{
    class DoesFilterExist
    {
         [Test]
        public static bool CheckDoesFilterExist(string filtername)
         {
             //string filtername = "FilterForStatusTests";
             ILookupService serviceLookup = WCFClientUtil.GetLookUpServiceServiceProxy();
            
             bool filterexist = serviceLookup.DoesFilterExistForUser(filtername, "Vizard");
            
             return filterexist;
         }

    }

}
