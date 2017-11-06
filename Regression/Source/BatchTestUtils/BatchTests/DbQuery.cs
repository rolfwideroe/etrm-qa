using ElvizTestUtils.AssertTools;

namespace ElvizTestUtils.BatchTests
{
    public class DbQuery
    {
        public Column[] Columns { get; set; }

        public string SqlQuery { get; set; }

        public string PreparedSqlQuery { get; set; }
        
    }
}