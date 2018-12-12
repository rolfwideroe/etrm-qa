using System.IO;

namespace ElvizTestUtils.HelperMethods
{
    public static class FileSystemManager
    {
        public static void CheckAndCreateDirectory(string path)
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
        }
    }
}
