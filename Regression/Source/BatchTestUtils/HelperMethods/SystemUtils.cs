using System;
using System.IO;
using System.ServiceProcess;

namespace ElvizTestUtils
{
    /// <summary>
    /// Helper for system functions such start a service or copy a file
    /// </summary>
    public class SystemUtils
    {
        /// <summary>
        /// Deletes file if it exists
        /// </summary>
        /// <param name="filePath">path to file</param>
        public static void DeleteFileIfExist(string filePath)
        {
            if (File.Exists(filePath))
            {
                File.SetAttributes(filePath, FileAttributes.Normal);
                File.Delete(filePath);
            }
        }
    }
}
