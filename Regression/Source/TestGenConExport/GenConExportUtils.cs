using ElvizTestUtils;
using Microsoft.XmlDiffPatch;
using System;
using System.Diagnostics;
using System.IO;
using System.Xml;

namespace TestGenConExport
{
    class GenConExportUtils
    {
        public static bool ExportGenCon(string exportPath, string filterName)
        {
            try
            {
                string installationName = ElvizInstallationUtility.GetEtrmDbName("VizECM");

                //update ExportParameters.xml with filter details
                string exportParameterPath = Path.Combine(Directory.GetCurrentDirectory(), "TestConfig\\",
                    "TestExportParameters.xml");
                File.SetAttributes(exportParameterPath, FileAttributes.Normal);
                string dbUserName = string.Empty;
                string dbUserPassword = String.Empty;
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(exportParameterPath);
                XmlNodeList nodeList = xmlDoc.SelectNodes("ExportParameters/ExportParameter");
                foreach (XmlNode nodeItem in nodeList)
                {
                    switch (nodeItem.Attributes["Name"].Value)
                    {
                        case "exportFilterName":
                            nodeItem.Attributes["Value"].Value = filterName;
                            break;
                        case "exportFile":
                            nodeItem.Attributes["Value"].Value = exportPath;
                            break;
                        case "dbUserName":
                            dbUserName = nodeItem.Attributes["Value"].Value;
                            break;
                        case "dbUserPassword":
                            dbUserPassword = nodeItem.Attributes["Value"].Value;
                            break;
                    }
                }
                xmlDoc.Save(exportParameterPath);

                string args =
                    string.Format(
                        "/c:\"{0}\" /e:gencon /d:\"{1}\" /p:\"{3}\" /u:\"{2}\"",
                        exportParameterPath, installationName, dbUserName, dbUserPassword);

                ProcessStartInfo startInfo =
                    new ProcessStartInfo(
                        Path.Combine(@"c:\BradyETRM(Client)\Integration",
                            "Viz.Integration.Core.ElvizEntityExport.exe"), args)
                    { UseShellExecute = false };

                Process p = Process.Start(startInfo);
                p.WaitForExit(GlobalConstTestSettings.MAX_BATCH_WAIT_TIME);

                if (p.HasExited == false)
                {
                    //Process is still running.
                    //Test to see if the process is hung up.
                    if (p.Responding)
                    {
                        //Process was responding; close the main window.
                        p.CloseMainWindow();
                    }
                    else
                    {
                        //Process was not responding; force the process to close.
                        p.Kill();
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }

        }

        static public bool XMLDiffCompare(string path, string expectedfile, string actualfile, string Id)
        {
            try
            {
                string file1 = expectedfile;
                string file2 = actualfile;
                string diffile = path + @"diff-" + Path.GetFileNameWithoutExtension(Id) + ".xml";
                string htmlfile = path + @"errors-" + Path.GetFileNameWithoutExtension(Id) + ".html";

                System.Xml.XmlWriter writer = System.Xml.XmlWriter.Create(diffile);

                XmlDiff diff =
                    new XmlDiff(XmlDiffOptions.IgnoreChildOrder | XmlDiffOptions.IgnoreComments |
                                XmlDiffOptions.IgnoreWhitespace);

                bool isEqual = false;
                isEqual = diff.Compare(file1, file2, false, writer);
                writer.Close();

                if (isEqual)
                {
                    File.Delete(diffile);
                    File.Delete(htmlfile);
                    return isEqual;

                }
                else
                {
                    XmlDiffView diffView = new XmlDiffView();

                    //Load the original file again and the diff file.
                    XmlTextReader original = new XmlTextReader(file1);
                    XmlTextReader diffGram = new XmlTextReader(diffile);
                    diffView.Load(original, diffGram);

                    //Wrap the HTML file with necessary html and 
                    //body tags and prepare it before passing it to 
                    //the GetHtml method.

                    StreamWriter sw1 = new StreamWriter(htmlfile);
                    //Wrapping
                    sw1.Write("<html><body><table>");
                    sw1.Write("<tr><td><b>Legend:</b> <font style='background-color: yellow'" +
                              " color='black'>added</font>&nbsp;&nbsp;<font style='background-color: red'" +
                              "color='black'>removed</font>&nbsp;&nbsp;<font style='background-color: " +
                              "lightgreen' color='black'>changed</font>&nbsp;&nbsp;" +
                              "<font style='background-color: red' color='blue'>moved from</font>" +
                              "&nbsp;&nbsp;<font style='background-color: yellow' color='blue'>moved to" +
                              "</font>&nbsp;&nbsp;<font style='background-color: white' color='#AAAAAA'>" +
                              "ignored</font></td></tr>");
                    sw1.Write("<tr><td><b>");
                    sw1.Write(file1);
                    sw1.Write("</b></td><td><b>");
                    sw1.Write(file2);
                    sw1.Write("</b></td></tr>");

                    //This gets the differences but just has the 
                    //rows and columns of an HTML table
                    diffView.GetHtml(sw1);

                    sw1.Write("</table></body></html>");

                    //HouseKeeping...close everything we dont want to lock.
                    sw1.Close();
                    diffView = null;
                    original.Close();
                    diffGram.Close();

                    return isEqual;
                }
            }
            catch
            {
                return false;
            }
        }
    }
}
