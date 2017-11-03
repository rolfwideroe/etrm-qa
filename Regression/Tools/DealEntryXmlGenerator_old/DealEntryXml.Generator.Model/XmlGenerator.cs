using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using DealEntryXml.Model;
using DealEntryXmlGenerator.Wrapper.Model;

namespace DealEntryXml.Generator.Model
{
    public class XmlGenerator
    {
        private string dropFolder;

        public XmlGenerator(string dropFolder)
        {
            this.dropFolder = dropFolder;
        }


        public void GenerateDealEntryXmlFile(DealEntryWrapper wrapper)
        {
            DealEntry dealEntry = DealEntryFactory.CreateDealEntryXml(wrapper);

            string filepath = Path.Combine(this.dropFolder, wrapper.FileName);




            if (File.Exists(filepath))
                File.Delete(filepath);




            XmlSerializer xmlSerializer = new XmlSerializer(typeof(DealEntry));
            FileStream fileStream = File.Open(
                filepath,
                FileMode.OpenOrCreate,
                FileAccess.Write,
                FileShare.ReadWrite);

            xmlSerializer.Serialize(fileStream, dealEntry);

            fileStream.Close();
            fileStream.Dispose();

        }

    }
}
