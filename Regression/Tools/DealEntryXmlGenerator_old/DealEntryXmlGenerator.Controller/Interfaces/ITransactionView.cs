using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DealEntryXmlGenerator.Wrapper.Model;
namespace DealEntryXmlGenerator.Controller.Interfaces
{
 
        public interface ITransactionView : IMessageView
        {
            IList<DealEntryWrapper> Wrappers { get; }
            string XmlFilePath { get; }
        }
    
}
