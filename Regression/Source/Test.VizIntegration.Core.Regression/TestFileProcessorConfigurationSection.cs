using System;
using System.Configuration;
using System.IO;
using System.Reflection;
using ElvizTestUtils;

namespace Test.VizIntegration.Core.Regression
{
    /// <summary>
    /// The file watcher supports multiple implementations of file processors which can be loaded dynamically.
    /// File processors are specified using a custom configuration section in the application settings
    /// </summary>
    public sealed class FileProcessorConfigurationSection : ConfigurationSection
    {
        [ConfigurationProperty("fileProcessors", IsDefaultCollection = true)]
        public FileProcessorCollection FileProcessors
        {
            get { return (FileProcessorCollection)base["fileProcessors"]; }
        }
    }

    /// <summary>
    /// Represents a file processor configuration entry
    /// </summary>
    public sealed class FileProcessor : ConfigurationElement
    {

        /// <summary>
        /// A unique identification of the file processor
        /// </summary>
        [ConfigurationProperty("name", IsKey = true, IsRequired = true)]
        public string ProcessorName
        {

            get { return (string)this["name"]; }

            set { this["name"] = value; }
        }

        /// <summary>
        /// The directory to write logs / results to
        /// </summary>
        [ConfigurationProperty("logDirectory", IsRequired = true)]
        public string LogDirectory
        {

            get { return string.Format((string) this["logDirectory"],ElvizInstallationUtility.GetAppServerName()); }

            set { this["logDirectory"] = value; }
        }

        /// <summary>
        /// The directory to write logs / results to
        /// </summary>
        [ConfigurationProperty("testFilesFolder", IsRequired = true)]
        public string TestFilesFolder
        {

            get { return string.Format((string)this["testFilesFolder"],ElvizInstallationUtility.GetAppServerName()); }

            set { this["testFilesFolder"] = value; }
        }

        /// <summary>
        /// Optional directory to place processed files in to avoid cluttering the watch path - this will remove them from the watch directory
        /// </summary>
        [ConfigurationProperty("processedDirectory", IsRequired = false)]
        public string ProcessedDirectory
        {

            get { return string.Format((string)this["processedDirectory"],ElvizInstallationUtility.GetAppServerName()); }

            set { this["processedDirectory"] = value; }
        }

        /// <summary>
        /// Optional directory to place failed files in for easy reprocessing and to avoid cluttering of the watch path - this will remove them from the watch directory
        /// </summary>
        [ConfigurationProperty("quarantineDirectory", IsRequired = false)]
        public string QuarantineDirectory
        {

            get { return string.Format((string)this["quarantineDirectory"],ElvizInstallationUtility.GetAppServerName()); }

            set { this["quarantineDirectory"] = value; }
        }

        /// <summary>
        /// The directory to watch
        /// </summary>
        [ConfigurationProperty("watchPath", IsRequired = true)]
        public string WatchPath
        {
            get { return string.Format((string)this["watchPath"],ElvizInstallationUtility.GetAppServerName()); }

            set { this["watchPath"] = value; }
        }

    }

    /// <summary>
    /// Represents the collection of file processor configuration entries
    /// </summary>
    public sealed class FileProcessorCollection : ConfigurationElementCollection
    {

        protected override ConfigurationElement CreateNewElement()
        {

            return new FileProcessor();

        }

        protected override object GetElementKey(ConfigurationElement element)
        {

            return ((FileProcessor)element).ProcessorName;

        }

        public override ConfigurationElementCollectionType CollectionType
        {

            get { return ConfigurationElementCollectionType.BasicMap; }

        }

        protected override string ElementName
        {

            get { return "fileProcessor"; }

        }

        public FileProcessor this[int index]
        {

            get { return (FileProcessor)BaseGet(index); }

            set
            {

                if (BaseGet(index) != null)
                {

                    BaseRemoveAt(index);

                }

                BaseAdd(index, value);

            }

        }

        new public FileProcessor this[string processorName]
        {

            get { return (FileProcessor)BaseGet(processorName); }

        }

        public bool ContainsKey(string key)
        {

            bool result = false;
            object[] keys = BaseGetAllKeys();
            foreach (object obj in keys)
            {
                if ((string)obj == key)
                {
                    result = true;
                    break;
                }
            }
            return result;

        }

    }
}