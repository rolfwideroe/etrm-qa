using System.Collections.Generic;

namespace Test.VizIntegration.Core.Regression
{
      
    /// <summary>
    /// Configuration reader
    /// </summary>
    public static class TestCaseConfiguration
    {
        private static Dictionary<string, FileProcessor> _watchers = null;
        private static object _lock = new object();
        public static Dictionary<string, FileProcessor> FileProcessors
        {
            get
            {
                lock (_lock)
                {
                    if (_watchers != null) return _watchers;

                    var configuration = (FileProcessorConfigurationSection)System.Configuration.ConfigurationManager.GetSection("fileWatcherSettings");

                    _watchers = new Dictionary<string, FileProcessor>();

                    foreach (FileProcessor processor in configuration.FileProcessors)
                    {                        
                        _watchers.Add(processor.ProcessorName, processor);
                    }

                    return _watchers;
                }
            }
            
        }
    }
}
