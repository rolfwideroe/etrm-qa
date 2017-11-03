using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using NUnit.Framework;
using PreTestWeb.Controllers;
using PreTestWeb.Models;

namespace PreTestWeb
{
    [TestFixture]
    public class TestPreTestController
    {
        [Test]
        public void TestIndexView()
        {
            PreTestsController preTestsController=new PreTestsController();

            ViewResult view=preTestsController.Index() as ViewResult;

            Assert.IsNotNull(view);

            IEnumerable<PreTest> preTests = view.Model as IEnumerable<PreTest>;

            Assert.IsNotNull(preTests);

            Assert.Greater(preTests.Count(),0);
        }
    }
}