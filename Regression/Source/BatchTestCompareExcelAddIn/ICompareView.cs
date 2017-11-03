using System;
using System.Collections.Generic;
using ElvizTestUtils.BatchTests;
using DataTable = System.Data.DataTable;


namespace BatchTestCompareExcelAddIn
{
	public interface ICompareView 
    {
        IList<string> XmlFilePaths { get; }
        void ShowCompare(DataTable testData,DataTable actualTable,DataTable expectedTable,DataTable newXmlValues,double tollerance);

       // void Compare(DataTable testData, DataTable actualTable, DataTable expectedTable,double tollerance);
        void ShowUserError(string errorString);
        void ShowApplicationError(string errorString);
        void ShowUnknownError(string errorString);
        void ShowMessage(string messageString);
        bool ShowOkCancel(string messageString, string headerText);
        string CurrentFileDialogDirectory { get; }
        string CurrentTestFilePath { get; }
        string CurrentArtifactId { get; }
        string CurrentQueryFile { get; }
	    IDictionary<string, Type> GetCurrentExpectedColumnNamesAndTypes();
		string[] CurrentExpectedRecords { get; }
        IList<BatchTestFile> BatchTestFilesStructure{ get; }
        string ExportBatchTestFilesPath { get; }
    }
}
