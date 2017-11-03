using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using ElvizTestUtils.DatabaseTools;

namespace ElvizTestUtils
{
    public class ElvizConfigurationTool:IDisposable
    {
        private readonly QaDao qaDao;
        private bool disposed;
        public ElvizConfigurationTool()
        {
            this.qaDao=new QaDao();
        }

        public void RevertConfigurationToDefault(ElvizConfiguration[] elvizConfiguration)
        {
            bool configurationIsUpdated = false;

            foreach (ElvizConfiguration configuration in elvizConfiguration)
            {
                if (string.IsNullOrEmpty(configuration.Name)) throw new ArgumentException("Elviz Configuration is missing name");
    
                this.qaDao.RevertConfigurationToDefault(configuration.Name);
                configurationIsUpdated = true;
                Console.WriteLine(configuration.Name + " reverted to default value:" + configuration.DefaultValue);
            }

            ConfigurationIsUpdated(configurationIsUpdated);
        }

        public void RevertAllConfigurationsToDefault()
        {
            ElvizConfiguration[] allElvizConfigurations = GetAllElvizConfigurations();

            IList<ElvizConfiguration> configurationsToRevertToDefault = new List<ElvizConfiguration>();

            foreach (ElvizConfiguration configuration in allElvizConfigurations)
            {
                if (configuration.Value != configuration.DefaultValue)
                    configurationsToRevertToDefault.Add(configuration);
            }

            RevertConfigurationToDefault(configurationsToRevertToDefault.ToArray());
        }

        public ElvizConfiguration[] GetAllElvizConfigurations()
        {
            return this.qaDao.GetAllElvizConfigurations();
        }

        public ElvizConfiguration[] GetAllNonDefulatElvizConfigurations()
        {
            return this.qaDao.GetAllNonDefulatElvizConfigurations();
        }

        public void UpdateConfiguration(ElvizConfiguration[] elvizConfiguration)
        {
            bool configurationIsUpdated = false;

            ElvizConfiguration[] allConfigurations = GetAllElvizConfigurations();

            foreach (ElvizConfiguration customConfiguration in elvizConfiguration)
            {
                if (allConfigurations.FirstOrDefault(x => x.Name == customConfiguration.Name) == null) throw new ArgumentException("Test Custom Configuration : " + customConfiguration.Name + " Does not exits in Elviz");
            }

            foreach (ElvizConfiguration currentConfiguration in allConfigurations)
            {
                ElvizConfiguration customConfiguration =
                    elvizConfiguration.FirstOrDefault(x => x.Name == currentConfiguration.Name);

                if (customConfiguration == null)
                {
                    if (currentConfiguration.Value != currentConfiguration.DefaultValue)
                    {
                        this.qaDao.RevertConfigurationToDefault(currentConfiguration.Name);
                        Console.WriteLine("Updated Current Cunfiguration: " + currentConfiguration.Name + " To : " + currentConfiguration.DefaultValue);
                        configurationIsUpdated = true;
                    }
                }
                else
                {
                    if (customConfiguration.Value != currentConfiguration.Value)
                    {
                        if (string.IsNullOrEmpty(customConfiguration.Name))
                            throw new ArgumentException("Elviz Configuration is missing name");

                        if (string.IsNullOrEmpty(customConfiguration.Value))
                            throw new ArgumentException("Elviz Configuration : " + customConfiguration.Name + " is missing Value");

                        this.qaDao.UpdateConfiguration(customConfiguration.Name,customConfiguration.Value);

                        Console.WriteLine("Updated Custom Configuration: " + customConfiguration.Name + " To : " + customConfiguration.Value);
                        configurationIsUpdated = true;
                    }
                }

            }

            ConfigurationIsUpdated(configurationIsUpdated);

        }

        private static void ConfigurationIsUpdated(bool configurationIsUpdated)
        {
            if (configurationIsUpdated)
            {
                Thread.Sleep(2500);
            }
        }




        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // Protected implementation of Dispose pattern. 
        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                this.qaDao.Dispose();
                // Free any other managed objects here. 
                //
            }

            // Free any unmanaged objects here. 
            //
            disposed = true;
        }
    }


}
