namespace BatchTestCompareExcelAddIn
{
    partial class BatchTestCompareRibbon : Microsoft.Office.Tools.Ribbon.RibbonBase
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        public BatchTestCompareRibbon()
            : base(Globals.Factory.GetRibbonFactory())
        {
            InitializeComponent();
        }

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tab1 = this.Factory.CreateRibbonTab();
            this.group1 = this.Factory.CreateRibbonGroup();
            this.compareResults = this.Factory.CreateRibbonButton();
            this.updateBatchTestButton = this.Factory.CreateRibbonButton();
            this.separator1 = this.Factory.CreateRibbonSeparator();
            this.reportEngineCompareButton = this.Factory.CreateRibbonButton();
            this.updateReportEngineTestButton = this.Factory.CreateRibbonButton();
            this.updateReportEngineTestIncludingColumnsButton = this.Factory.CreateRibbonButton();
            this.separator2 = this.Factory.CreateRibbonSeparator();
            this.compareCurveTestButton = this.Factory.CreateRibbonButton();
            this.compareExchangeRateButton = this.Factory.CreateRibbonButton();
            this.separator3 = this.Factory.CreateRibbonSeparator();
            this.compareReportDbTimeSeriesEcnButton = this.Factory.CreateRibbonButton();
            this.compareRdTimeseriesButton = this.Factory.CreateRibbonButton();
            this.updateRdTimeSeriesButton = this.Factory.CreateRibbonButton();
            this.separator4 = this.Factory.CreateRibbonSeparator();
            this.createBatchTestFiles = this.Factory.CreateRibbonButton();
            this.compareCustomDwhButton = this.Factory.CreateRibbonButton();
            this.separator5 = this.Factory.CreateRibbonSeparator();
            this.tab1.SuspendLayout();
            this.group1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tab1
            // 
            this.tab1.ControlId.ControlIdType = Microsoft.Office.Tools.Ribbon.RibbonControlIdType.Office;
            this.tab1.Groups.Add(this.group1);
            this.tab1.Label = "TabAddIns";
            this.tab1.Name = "tab1";
            // 
            // group1
            // 
            this.group1.Items.Add(this.compareResults);
            this.group1.Items.Add(this.updateBatchTestButton);
            this.group1.Items.Add(this.separator1);
            this.group1.Items.Add(this.reportEngineCompareButton);
            this.group1.Items.Add(this.updateReportEngineTestButton);
            this.group1.Items.Add(this.updateReportEngineTestIncludingColumnsButton);
            this.group1.Items.Add(this.separator2);
            this.group1.Items.Add(this.compareCurveTestButton);
            this.group1.Items.Add(this.compareExchangeRateButton);
            this.group1.Items.Add(this.separator3);
            this.group1.Items.Add(this.compareReportDbTimeSeriesEcnButton);
            this.group1.Items.Add(this.compareRdTimeseriesButton);
            this.group1.Items.Add(this.updateRdTimeSeriesButton);
            this.group1.Items.Add(this.separator4);
            this.group1.Items.Add(this.createBatchTestFiles);
            this.group1.Items.Add(this.separator5);
            this.group1.Items.Add(this.compareCustomDwhButton);
            this.group1.Label = "QA Compare Tool";
            this.group1.Name = "group1";
            // 
            // compareResults
            // 
            this.compareResults.Label = "Compare Batch Test";
            this.compareResults.Name = "compareResults";
            this.compareResults.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.compareResults_Click);
            // 
            // updateBatchTestButton
            // 
            this.updateBatchTestButton.Label = "Update Batch Test";
            this.updateBatchTestButton.Name = "updateBatchTestButton";
            this.updateBatchTestButton.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.updateBatchTestButton_Click);
            // 
            // separator1
            // 
            this.separator1.Name = "separator1";
            // 
            // reportEngineCompareButton
            // 
            this.reportEngineCompareButton.Label = "Compare Report Engine Test";
            this.reportEngineCompareButton.Name = "reportEngineCompareButton";
            this.reportEngineCompareButton.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.reportEngineCompareButton_Click);
            // 
            // updateReportEngineTestButton
            // 
            this.updateReportEngineTestButton.Label = "Update Report Engine Artifact";
            this.updateReportEngineTestButton.Name = "updateReportEngineTestButton";
            this.updateReportEngineTestButton.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.updateReportEngineTestButton_Click);
			// 
			// updateReportEngineTestIncludingColumnsButton
			// 
			this.updateReportEngineTestIncludingColumnsButton.Label = "Update Report Engine Artifact including columns";
			this.updateReportEngineTestIncludingColumnsButton.Name = "updateReportEngineTestIncludingColumnsButton";
			this.updateReportEngineTestIncludingColumnsButton.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.updateReportEngineTestIncludingColumnsButton_Click);
			// 
			// separator2
			// 
			this.separator2.Name = "separator2";
            // 
            // compareCurveTestButton
            // 
            this.compareCurveTestButton.Label = "Compare Curve Test";
            this.compareCurveTestButton.Name = "compareCurveTestButton";
            this.compareCurveTestButton.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.compareCurveTestButton_Click);
            // 
            // compareExchangeRateButton
            // 
            this.compareExchangeRateButton.Label = "Compare Exchange Rate Test";
            this.compareExchangeRateButton.Name = "compareExchangeRateButton";
            this.compareExchangeRateButton.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.compareExchangeRateButton_Click);
            // 
            // separator3
            // 
            this.separator3.Name = "separator3";
            // 
            // compareReportDbTimeSeriesEcnButton
            // 
            this.compareReportDbTimeSeriesEcnButton.Label = "Compare RD Timeseries ECM";
            this.compareReportDbTimeSeriesEcnButton.Name = "compareReportDbTimeSeriesEcnButton";
            this.compareReportDbTimeSeriesEcnButton.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.compareReportDbTimeSeriesButton_Click);
            // 
            // compareRdTimeseriesButton
            // 
            this.compareRdTimeseriesButton.Label = "Compare Reporting DB Tests";
            this.compareRdTimeseriesButton.Name = "compareRdTimeseriesButton";
            this.compareRdTimeseriesButton.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.compareRdTimeseriesButton_Click);
            // 
            // updateRdTimeSeriesButton
            // 
            this.updateRdTimeSeriesButton.Label = "Update Reporting DB Tests";
            this.updateRdTimeSeriesButton.Name = "updateRdTimeSeriesButton";
            this.updateRdTimeSeriesButton.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.updateRdTimeSeriesButton_Click);
            // 
            // separator4
            // 
            this.separator4.Name = "separator4";
            // 
            // createBatchTestFiles
            // 
            this.createBatchTestFiles.Label = "Create Batch Files";
            this.createBatchTestFiles.Name = "createBatchTestFiles";
            this.createBatchTestFiles.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.createBatchFilesButton_Click);
            // 
            // compareCustomDwhButton
            // 
            this.compareCustomDwhButton.Label = "Compare Custom DWH";
            this.compareCustomDwhButton.Name = "compareCustomDwhButton";
            this.compareCustomDwhButton.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.compareCustomDwhButton_Click);
            // 
            // separator5
            // 
            this.separator5.Name = "separator5";
            // 
            // BatchTestCompareRibbon
            // 
            this.Name = "BatchTestCompareRibbon";
            this.RibbonType = "Microsoft.Excel.Workbook";
            this.Tabs.Add(this.tab1);
            this.Load += new Microsoft.Office.Tools.Ribbon.RibbonUIEventHandler(this.Ribbon1_Load);
            this.tab1.ResumeLayout(false);
            this.tab1.PerformLayout();
            this.group1.ResumeLayout(false);
            this.group1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        internal Microsoft.Office.Tools.Ribbon.RibbonTab tab1;
        internal Microsoft.Office.Tools.Ribbon.RibbonGroup group1;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton compareResults;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton reportEngineCompareButton;
        internal Microsoft.Office.Tools.Ribbon.RibbonSeparator separator1;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton updateBatchTestButton;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton updateReportEngineTestButton;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton updateReportEngineTestIncludingColumnsButton;
        internal Microsoft.Office.Tools.Ribbon.RibbonSeparator separator2;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton compareCurveTestButton;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton compareExchangeRateButton;
        internal Microsoft.Office.Tools.Ribbon.RibbonSeparator separator3;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton createBatchTestFiles;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton compareReportDbTimeSeriesEcnButton;
        internal Microsoft.Office.Tools.Ribbon.RibbonSeparator separator4;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton compareRdTimeseriesButton;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton updateRdTimeSeriesButton;
        internal Microsoft.Office.Tools.Ribbon.RibbonSeparator separator5;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton compareCustomDwhButton;
    }

    partial class ThisRibbonCollection
    {
        internal BatchTestCompareRibbon Ribbon1
        {
            get { return this.GetRibbon<BatchTestCompareRibbon>(); }
        }
    }
}
