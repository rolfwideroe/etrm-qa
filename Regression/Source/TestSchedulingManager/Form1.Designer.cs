using System.IO;

namespace TestSchedulingManager
{
    partial class TestSchedulingManager
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.lblPublicationName = new System.Windows.Forms.Label();
            this.lblExpectedOutputFile = new System.Windows.Forms.Label();
            this.lblCompare = new System.Windows.Forms.Label();
            this.lblSchedulingManagerPath = new System.Windows.Forms.Label();
            this.tbxSchedulingManagerPath = new System.Windows.Forms.TextBox();
            this.tbxPublicationName = new System.Windows.Forms.TextBox();
            this.tbxExpectedOutputFile = new System.Windows.Forms.TextBox();
            this.tbxCompare = new System.Windows.Forms.TextBox();
            this.btnPublishAndCompare = new System.Windows.Forms.Button();
            this.dlgSchedulingManagerPathDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.dlgOpenPublication = new System.Windows.Forms.OpenFileDialog();
            this.fwOutputFileWatcher = new System.IO.FileSystemWatcher();
            this.dlgExpectedOutputFile = new System.Windows.Forms.OpenFileDialog();
            ((System.ComponentModel.ISupportInitialize)(this.fwOutputFileWatcher)).BeginInit();
            this.SuspendLayout();
            // 
            // lblPublicationName
            // 
            this.lblPublicationName.AutoSize = true;
            this.lblPublicationName.Location = new System.Drawing.Point(34, 85);
            this.lblPublicationName.Name = "lblPublicationName";
            this.lblPublicationName.Size = new System.Drawing.Size(59, 13);
            this.lblPublicationName.TabIndex = 1;
            this.lblPublicationName.Text = "Publication";
            // 
            // lblExpectedOutputFile
            // 
            this.lblExpectedOutputFile.AutoSize = true;
            this.lblExpectedOutputFile.Location = new System.Drawing.Point(37, 201);
            this.lblExpectedOutputFile.Name = "lblExpectedOutputFile";
            this.lblExpectedOutputFile.Size = new System.Drawing.Size(85, 13);
            this.lblExpectedOutputFile.TabIndex = 3;
            this.lblExpectedOutputFile.Text = "Expected Result";
            // 
            // lblCompare
            // 
            this.lblCompare.AutoSize = true;
            this.lblCompare.Location = new System.Drawing.Point(37, 237);
            this.lblCompare.Name = "lblCompare";
            this.lblCompare.Size = new System.Drawing.Size(153, 13);
            this.lblCompare.TabIndex = 4;
            this.lblCompare.Text = "Compare Output and Expected";
            // 
            // lblSchedulingManagerPath
            // 
            this.lblSchedulingManagerPath.AutoSize = true;
            this.lblSchedulingManagerPath.Location = new System.Drawing.Point(34, 36);
            this.lblSchedulingManagerPath.Name = "lblSchedulingManagerPath";
            this.lblSchedulingManagerPath.Size = new System.Drawing.Size(130, 13);
            this.lblSchedulingManagerPath.TabIndex = 0;
            this.lblSchedulingManagerPath.Text = "Scheduling Manager Path";
            // 
            // tbxSchedulingManagerPath
            // 
            this.tbxSchedulingManagerPath.Location = new System.Drawing.Point(298, 29);
            this.tbxSchedulingManagerPath.Name = "tbxSchedulingManagerPath";
            this.tbxSchedulingManagerPath.Size = new System.Drawing.Size(276, 20);
            this.tbxSchedulingManagerPath.TabIndex = 5;
            this.tbxSchedulingManagerPath.Text = "C:\\Development\\Elviz\\Development\\Integration\\Viz.Integration.Core.SchedulingModul" +
    "e";
            this.tbxSchedulingManagerPath.Click += new System.EventHandler(this.TbxSchedulingManagerPathClick);
            // 
            // tbxPublicationName
            // 
            this.tbxPublicationName.Location = new System.Drawing.Point(298, 85);
            this.tbxPublicationName.Name = "tbxPublicationName";
            this.tbxPublicationName.Size = new System.Drawing.Size(276, 20);
            this.tbxPublicationName.TabIndex = 6;
            this.tbxPublicationName.Text = "New";
            this.tbxPublicationName.Click += new System.EventHandler(this.TbxPublicationNameClick);
            // 
            // tbxExpectedOutputFile
            // 
            this.tbxExpectedOutputFile.Location = new System.Drawing.Point(298, 194);
            this.tbxExpectedOutputFile.Name = "tbxExpectedOutputFile";
            this.tbxExpectedOutputFile.Size = new System.Drawing.Size(276, 20);
            this.tbxExpectedOutputFile.TabIndex = 8;
            this.tbxExpectedOutputFile.Text = "C:\\Development\\Elviz\\Development\\Integration\\DealImport\\2012-11-15_New-Expected.x" +
    "ml";
            this.tbxExpectedOutputFile.Click += new System.EventHandler(this.TbxExpectedOutputFileClicked);
            // 
            // tbxCompare
            // 
            this.tbxCompare.Location = new System.Drawing.Point(37, 265);
            this.tbxCompare.Multiline = true;
            this.tbxCompare.Name = "tbxCompare";
            this.tbxCompare.Size = new System.Drawing.Size(534, 59);
            this.tbxCompare.TabIndex = 9;
            // 
            // btnPublishAndCompare
            // 
            this.btnPublishAndCompare.Location = new System.Drawing.Point(37, 160);
            this.btnPublishAndCompare.Name = "btnPublishAndCompare";
            this.btnPublishAndCompare.Size = new System.Drawing.Size(124, 23);
            this.btnPublishAndCompare.TabIndex = 10;
            this.btnPublishAndCompare.Text = "Publish and Compare";
            this.btnPublishAndCompare.UseVisualStyleBackColor = true;
            this.btnPublishAndCompare.Click += new System.EventHandler(this.BtnPublishAndCompareClick);
            // 
            // dlgOpenPublication
            // 
            this.dlgOpenPublication.FileName = "publicationName";
            // 
            // fwOutputFileWatcher
            // 
            this.fwOutputFileWatcher.EnableRaisingEvents = true;
            this.fwOutputFileWatcher.SynchronizingObject = this;
            this.fwOutputFileWatcher.Changed += new System.IO.FileSystemEventHandler(this.FileSystemWatcher1Changed);
            this.fwOutputFileWatcher.NotifyFilter = NotifyFilters.LastWrite;
            this.fwOutputFileWatcher.IncludeSubdirectories = false;
            // 
            // dlgExpectedOutputFile
            // 
            this.dlgExpectedOutputFile.FileName = "expectedOutputFile";
            // 
            // TestSchedulingManager
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(586, 355);
            this.Controls.Add(this.btnPublishAndCompare);
            this.Controls.Add(this.tbxCompare);
            this.Controls.Add(this.tbxExpectedOutputFile);
            this.Controls.Add(this.tbxPublicationName);
            this.Controls.Add(this.tbxSchedulingManagerPath);
            this.Controls.Add(this.lblCompare);
            this.Controls.Add(this.lblExpectedOutputFile);
            this.Controls.Add(this.lblPublicationName);
            this.Controls.Add(this.lblSchedulingManagerPath);
            this.Name = "TestSchedulingManager";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.fwOutputFileWatcher)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblPublicationName;
        private System.Windows.Forms.Label lblExpectedOutputFile;
        private System.Windows.Forms.Label lblCompare;
        private System.Windows.Forms.Label lblSchedulingManagerPath;
        private System.Windows.Forms.TextBox tbxSchedulingManagerPath;
        private System.Windows.Forms.TextBox tbxPublicationName;
        private System.Windows.Forms.TextBox tbxExpectedOutputFile;
        private System.Windows.Forms.TextBox tbxCompare;
        private System.Windows.Forms.Button btnPublishAndCompare;
        private System.Windows.Forms.FolderBrowserDialog dlgSchedulingManagerPathDialog;
        private System.Windows.Forms.OpenFileDialog dlgOpenPublication;
        private System.IO.FileSystemWatcher fwOutputFileWatcher;
        private System.Windows.Forms.OpenFileDialog dlgExpectedOutputFile;

    }
}

