using System;
using System.IO;

namespace TestSchedulingManager
{
    public class CsvFileComparer
    {
        public static string CompareFiles(string outputFileName, string expectedOutputFileName)
        {
            
            try
            {
               using (StreamReader f1 = new StreamReader(outputFileName))
                using (StreamReader f2 = new StreamReader(expectedOutputFileName))
                {

                    int lineNumber = 0;
                    string errors =  null;
                    while (!f1.EndOfStream) 
                    {
                        if (f2.EndOfStream)
                        {
                            errors += ("Differing number of lines - f2 has less.");
                           break;
                        }

                        lineNumber++;
                        var line1 = f1.ReadLine();
                        var line2 = f2.ReadLine();

                        if (line1 != line2) 
                        {
                            errors+= (string.Format("Line {0} differs. File 1: {1}, File 2: {2} \n", lineNumber, line1, line2));
                        }
                    }

                    if (!f2.EndOfStream) {
                        errors += ("Differing number of lines - f1 has less.");
                    }
                    return errors;
                }
                
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
                throw new Exception(ex.Message);
            }
        }
    }
}
