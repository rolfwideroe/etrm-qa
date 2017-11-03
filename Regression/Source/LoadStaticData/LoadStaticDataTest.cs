using NUnit.Framework;

namespace LoadStaticData
{
    [TestFixture]
    public class LoadStaticDataTest
    {
        ElvizTestUtils.LoadStaticData.LoadStaticData loadStaticData=new ElvizTestUtils.LoadStaticData.LoadStaticData();

        [Test]
        public void TestRunEcmDbScripts()
        {
            loadStaticData.RunEcmDbScripts();   
        }

        [Test]
        public void TestInsertPortfolios()  
        {
            loadStaticData.InsertPortfolios();
        }
    }
    }

