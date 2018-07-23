using System;

namespace TestElvizUpdateTool.Helpers
{
    public class ErrorRecorder
    {
        public string TestName { get; set; }
        public string Field { get; set; }
        public string Description { get; set; }
        public DateTime RecordedOn { get; set; }
    }
}
