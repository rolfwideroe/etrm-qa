using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using ElvizTestUtils.QaLookUp;

namespace TestWCFLookupService.TestCases
{
    public class TransactionByUTITestCase
    {
           public string InsertXml { get; set; }
           public QaTransactionDTO[] ExpectedResult { get; set; }
     }

    public class ExpectedQaTransactionDTO
    {

    }
}
