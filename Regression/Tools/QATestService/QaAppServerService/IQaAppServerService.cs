using System.Collections;
using System.Collections.Generic;
using System.ServiceModel;

namespace QATestService
{
    [ServiceContract]
    public interface IQaAppServerService
    {
        [OperationContract]
        IDictionary<string, string> GetSettings(string[] settingNames);

        [OperationContract]
        KeyValuePair<string, QaAppServerService.ElvizDWordRegValues> SetSettingsDWord(QaAppServerService.ElvizDWordRegKeys keyName, QaAppServerService.ElvizDWordRegValues value);

        [OperationContract]
        KeyValuePair<string, string> SetSettingsString(QaAppServerService.ElvizStringRegKeys keyName, string keyValue);

        [OperationContractAttribute]
        void ReinstallDbPatch();

        [OperationContract]
        void StartElvizServices();

        [OperationContract]
        void StopElvizServices();
        
        [OperationContract]
        void RestartElvizServices();

    }
}