using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DealInsertXmlGenerator.Controller.Interfaces
{
    public interface IMessageView
    {
        void ShowUserError(string errorString);
        void ShowApplicationError(string errorString);
        void ShowUnknownError(string errorString);
        void ShowMessage(string messageString);
        bool ShowOkCancel(string messageString, string headerText);
    }
}
