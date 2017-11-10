using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ElvizTestUtils;
using ElvizTestUtils.DatabaseTools;
using ElvizTestUtils.QaLookUp;
using Microsoft.Office.Interop.Excel;
using NUnit.Framework;

namespace TestCompleteRequestToActiveMQ
{
    public class InstrumentPricesTest
    {
        //\\BERSV-FS01\Felles\QA\Regression_EUT\GreenCertificate
       // "GreenCertificateInstrumentPriceRegression", "Historic Data Update Job"

        [Test]
        public static void PriceImport()
        {
            //running job for download prices from local folder
            int jobId = JobAPI.GetJobsIdByDescription("GreenCertificateInstrumentPriceRegression", "Historic Data Update Job");
            JobAPI.ExecuteAndAssertJob(jobId, 120);
        }

        [Test]
        public static void DeleteInstrumentPriceRecordUsingParameters()
        {
            QaDao.DeleteInstrumentPriceRecordUsingParameters("GoO Y Italy2016", "GoO", "2017-03-30");
        }

        //idea here was to update xls file with new price value for report date
        [Test]
        public static void UpdatePriceDateInXLSFile()
        {
           //IWorksheets vizPrices = 

        }
    //    [Test]
    //    public void ProcessWorkbook()
    //    {
    //        string file = @"C:\Users\Chris\Desktop\TestSheet.xls";
    //        Console.WriteLine(file);

    //        Excel.Application excel = null;
    //        Excel.Workbook wkb = null;

    //        try
    //        {
    //            excel = new Excel.Application();

    //            wkb = ExcelTools.OfficeUtil.OpenBook(excel, file);

    //            Excel.Worksheet sheet = wkb.Sheets["Data"] as Excel.Worksheet;

    //            Excel.Range range = null;

    //            if (sheet != null)
    //                range = sheet.get_Range("A1", Missing.Value);

    //            string A1 = String.Empty;

    //            if (range != null)
    //                A1 = range.Text.ToString();

    //            Console.WriteLine("A1 value: {0}", A1);

    //        }
    //        catch (Exception ex)
    //        {
    //            //if you need to handle stuff
    //            Console.WriteLine(ex.Message);
    //        }
    //        finally
    //        {
    //            if (wkb != null)
    //                ExcelTools.OfficeUtil.ReleaseRCM(wkb);

    //            if (excel != null)
    //                ExcelTools.OfficeUtil.ReleaseRCM(excel);
    //        }
    //    }
    //}


   

    }
}
