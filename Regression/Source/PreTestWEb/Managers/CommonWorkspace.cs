namespace PreTestWeb.Managers
{
    public class CommonWorkspace
    {
        public  EcmErm EcmErm { get; set; }
        public string WorkspaceName { get; set; }
        public string DisplayName { get; set; }

    }

    public enum EcmErm
    {
        ECM,
        ERM
    }
}
