namespace DealInsertXmlGenerator.ExcelAddIn
{
    partial class GenaratorRibbon : Microsoft.Office.Tools.Ribbon.RibbonBase
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        public GenaratorRibbon()
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
            this.DealInsertGeneratorGroup = this.Factory.CreateRibbonGroup();
            this.GenerateButton = this.Factory.CreateRibbonButton();
            this.tab1.SuspendLayout();
            this.DealInsertGeneratorGroup.SuspendLayout();
            // 
            // tab1
            // 
            this.tab1.ControlId.ControlIdType = Microsoft.Office.Tools.Ribbon.RibbonControlIdType.Office;
            this.tab1.Groups.Add(this.DealInsertGeneratorGroup);
            this.tab1.Label = "TabAddIns";
            this.tab1.Name = "tab1";
            // 
            // DealInsertGeneratorGroup
            // 
            this.DealInsertGeneratorGroup.Items.Add(this.GenerateButton);
            this.DealInsertGeneratorGroup.Label = "Deal Insert Generator";
            this.DealInsertGeneratorGroup.Name = "DealInsertGeneratorGroup";
            // 
            // GenerateButton
            // 
            this.GenerateButton.Label = "Generate Xml";
            this.GenerateButton.Name = "GenerateButton";
            this.GenerateButton.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.GenerateButton_Click);
            // 
            // GenaratorRibbon
            // 
            this.Name = "GenaratorRibbon";
            this.RibbonType = "Microsoft.Excel.Workbook";
            this.Tabs.Add(this.tab1);
            this.Load += new Microsoft.Office.Tools.Ribbon.RibbonUIEventHandler(this.Ribbon1_Load);
            this.tab1.ResumeLayout(false);
            this.tab1.PerformLayout();
            this.DealInsertGeneratorGroup.ResumeLayout(false);
            this.DealInsertGeneratorGroup.PerformLayout();

        }

        #endregion

        internal Microsoft.Office.Tools.Ribbon.RibbonTab tab1;
        internal Microsoft.Office.Tools.Ribbon.RibbonGroup DealInsertGeneratorGroup;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton GenerateButton;
    }

    partial class ThisRibbonCollection
    {
        internal GenaratorRibbon Ribbon1
        {
            get { return this.GetRibbon<GenaratorRibbon>(); }
        }
    }
}
