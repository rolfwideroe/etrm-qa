using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace TestSchedulingManager
{
    public class XMLFilesComparer
    {

        public static string CompareFiles(string outputFileName, string expectedOutputFileName)
        {

            try
            {
                XDocument expected = XDocument.Load(expectedOutputFileName);
                XDocument actual = XDocument.Load(outputFileName);
                XElement expectedRoot = expected.Root;
                XElement actualRoot = actual.Root;

                string value = Compare(null, expectedRoot, actualRoot);
                return value;
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
                throw new Exception(ex.Message);
            }
        }

        private static string Compare(string errors, XElement expectedRoot, XElement actualRoot )
        {
            if (XNode.DeepEquals(expectedRoot, actualRoot))
            {
                return errors;
            }
            else
            {
                IEnumerable<XElement> expectedElements = expectedRoot.Elements();
                IEnumerable<XElement> actualElements = actualRoot.Elements();

                int index = -1;
                string errorsInThisNode = null;
                foreach (XElement actualElement in actualElements)
                {
                    index++;
                    string errorsFromElement= null;
                    errorsFromElement = Compare(errors, expectedElements.ElementAt(index), actualElement);
                   
                    if (!String.IsNullOrEmpty(errorsFromElement))
                    errorsInThisNode += errorsFromElement;
                }
                
                if(!actualRoot.HasElements && String.IsNullOrEmpty(errorsInThisNode))
                {
                   return errors + actualRoot + "should be " + expectedRoot + " \n";
                }
                return errors+ errorsInThisNode;
            }
        }

    }
}
