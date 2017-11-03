using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DealInsertXmlGenerator.Model;

namespace DealInsertXmlGenerator.Controller.Interfaces
{
    public interface ITransactionView:IMessageView
    {
        IList<TransactionWrapper> Wrappers { get; }
        string XmlFilePath { get; }
        TransactionWrapperType TransactionWrapperType { get; }
    }
}
