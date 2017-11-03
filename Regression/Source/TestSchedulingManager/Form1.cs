using System;
using System.IO;
using System.Windows.Forms;


namespace TestSchedulingManager
{
    public partial class TestSchedulingManager : Form
    {
        public TestSchedulingManager()
        {
            InitializeComponent();
        }

        private void TbxSchedulingManagerPathClick(object sender, EventArgs e)
        {
            DialogResult result = dlgSchedulingManagerPathDialog.ShowDialog();
            if (result == DialogResult.OK) // Test result.
            {
                tbxSchedulingManagerPath.Text = dlgSchedulingManagerPathDialog.SelectedPath;                
            }            
        }

        private void TbxPublicationNameClick(object sender, EventArgs e)
        {
            DialogResult result = dlgOpenPublication.ShowDialog();
            if (result == DialogResult.OK) // Test result.
            {
                tbxPublicationName.Text = dlgOpenPublication.FileName;
            }
        }


        private void BtnPublishAndCompareClick(object sender, EventArgs e)
        {
            fwOutputFileWatcher.EnableRaisingEvents = true;
            RunSchedulingManagerFrom();            
        }

        private void CompareTextFiles(string newFileName)
        {
            tbxCompare.Text = XMLFilesComparer.CompareFiles(newFileName, tbxExpectedOutputFile.Text);
        }

        private void RunSchedulingManagerFrom()
        {

            SetFileWatcherPath();
            try
            {
                string argumentsToSchedulingManager = "/pub:" + tbxPublicationName.Text + " /u:Vizard /p:draziv /d:QAECM131";
                SchedulingManagerExecutor.Run(tbxSchedulingManagerPath.Text, argumentsToSchedulingManager);
            }
            catch (Exception ex)
            {
                MessageBox.Show( ex.Message);
            }
        }

        private void SetFileWatcherPath()
        {
            try
            {
                fwOutputFileWatcher.Path = Path.Combine(tbxSchedulingManagerPath.Text, "..\\DealImport");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Deal import folder not found" + fwOutputFileWatcher.Path);
            }
        }

        private void FileSystemWatcher1Changed(object sender, FileSystemEventArgs e)
        {            
            MessageBox.Show(e.Name + " Published" );
            CompareTextFiles(e.FullPath);
            fwOutputFileWatcher.EnableRaisingEvents = false;
        }

        private void TbxExpectedOutputFileClicked(object sender, EventArgs e)                                                                                             
        {
            DialogResult result = dlgExpectedOutputFile.ShowDialog();
            if (result == DialogResult.OK) // Test result.
            {
                tbxExpectedOutputFile.Text = dlgExpectedOutputFile.FileName;
            }
        }
    }
}
