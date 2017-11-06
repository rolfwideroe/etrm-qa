using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using NUnit.Framework;

namespace TestCompleteRequestToActiveMQ
{
    class Analize_NUnitResults
    {
        [Test]
        public void FindXMLResults()
        {
            string filename = string.Empty;
            string baseDirectory =Directory.GetParent(Directory.GetCurrentDirectory()).ToString();

            string[] subdirectoryEntries = Directory.GetDirectories(baseDirectory);
            foreach (string subdirectory in subdirectoryEntries)
            {
                string[] filenames = Directory.GetFiles(subdirectory, "TestResult.xml");

                if (filenames.Count() == 1)
                {
                    filename = filenames[0];
                    Console.WriteLine(subdirectory);
                    ReadAssertsNumberFromXML(filename);
                }
            }
        }

        public void ReadAssertsNumberFromXML(string filename)
        {
            int count = 0;
            XDocument doc = XDocument.Load(filename);
            IEnumerable<XElement> listAssembly = doc.Root.Descendants("test-suite")
                                  .Where(x => (string) x.Attribute("type") == "Assembly");

            foreach (XElement el in listAssembly)
            {
                IEnumerable<XElement> listNamespace = el.Descendants("test-suite")
                                      .Where(x => (string)x.Attribute("type") == "Namespace");
                int totaltestnumber = 0;
                foreach (XElement elem in listNamespace)
                {
                   IEnumerable<XElement> listTestFixture = elem.Descendants("test-suite")
                                     .Where(x => (string)x.Attribute("type") == "TestFixture");
                    
                    foreach (XElement test in listTestFixture)
                    {
                        IEnumerable<XElement> listTestCase = test.Descendants("test-case");
                        totaltestnumber = totaltestnumber + listTestCase.Count();
                        foreach (XElement testcase in listTestCase)
                        {
                            string name = (string)testcase.Attribute("name");
                            string asserts = (string)testcase.Attribute("asserts");
                            //Console.WriteLine(name+  "= " + asserts);
                            count = count +  Convert.ToInt32(asserts);
                            //Console.WriteLine(asserts);
                        }
                    }
                }
                //Console.WriteLine("Total test number = " + totaltestnumber);
                Console.WriteLine("Total asserts number = " + count);
            }

        }

    }
}
