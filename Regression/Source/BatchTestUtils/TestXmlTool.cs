using System;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace ElvizTestUtils
{
    public class TestXmlTool
    {
        const string XmlDateFormat= "yyyy-MM-dd";
        const string XmlDateTimeFormat = "yyyy-MM-ddTHH:mm:ss";
       


        public static string ConvertToBatchXmlDateTimeString(DateTime? dateTime)
        {
            if (dateTime == null) return "NULL";

            DateTime d = (DateTime) dateTime;
  
                return d.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture); 
        }

        public static string ConvertToXmlDateTimeString(DateTime? dateTime)
        {
            if (dateTime == null) throw new ArgumentException("DateTime cannot be null");

            DateTime d = (DateTime)dateTime;
            return d.ToString("yyyy-MM-ddTHH:mm:ss");
        }

        public static string ConvertToXmlDateString(DateTime? dateTime)
        {
            if (dateTime == null) throw new ArgumentException("DateTime cannot be null");

            DateTime d = (DateTime)dateTime;
            return d.ToString(XmlDateFormat);
        }

        public static DateTime ConvertXmlDate(string dateString)
        {
            DateTime d;

            DateTime.TryParseExact(dateString, XmlDateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out d);

            return d;
        }

        public static T Deserialize<T>(string testFile, string testSubFolder)
        {
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), testSubFolder, testFile);

            return Deserialize<T>(filePath);
        }

        public static void SerializeToXml<T>(T entity, string filePath)
        {
            FileInfo info=new FileInfo(filePath);

            if (info.Exists && info.IsReadOnly) throw new ArgumentException("file is read only");

            if (File.Exists(filePath))
                File.Delete(filePath);

            FileStream fileStream = File.Open(
                filePath,
                FileMode.OpenOrCreate,
                FileAccess.Write,
                FileShare.ReadWrite);

         //   xmlSerializer.Serialize(fileStream, entity);

            using (StreamWriter writer = new StreamWriter(fileStream))
            {
                 writer.Write(Serialize(entity));

                writer.Flush();
                writer.Close();
                writer.Dispose();
            }

           

            fileStream.Close();
            fileStream.Dispose();
        }

        public static T Deserialize<T>(string filePath)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));



            FileStream readFileStream = File.Open(
                filePath,
                FileMode.Open,
                FileAccess.Read,
                FileShare.Read);

            T returnClass = (T)xmlSerializer.Deserialize(readFileStream);

            readFileStream.Close();
            return returnClass;

        }
        public static T DeserializeFromZip<T>(string filePath)
        {

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
            T returnClass = default(T);

            using (ZipArchive archive = ZipFile.OpenRead(filePath))
            {
                //update in case of few files for one archive
                foreach (ZipArchiveEntry entry in archive.Entries)
                {
                    {
                        Stream entryStream = entry.Open();
                        returnClass = (T)xmlSerializer.Deserialize(entryStream);
                        entryStream.Close();
                    }
                }
            }
            GC.Collect();

            return returnClass;

        }

        public static string Serialize<T>(T entity)
        {
            XmlSerializer x = new XmlSerializer(entity.GetType());

            XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
            namespaces.Add("","");
           

            XmlWriterSettings settings = new XmlWriterSettings
            {
                Encoding = new UnicodeEncoding(false, false),
                Indent = true,
                OmitXmlDeclaration = true,
                NamespaceHandling = new NamespaceHandling()
            };

            StringWriter textWriter = new StringWriter();

            XmlWriter xmlWriter = XmlWriter.Create(textWriter, settings);

            x.Serialize(xmlWriter,entity,namespaces);

            return textWriter.ToString();
        }
    }
}
