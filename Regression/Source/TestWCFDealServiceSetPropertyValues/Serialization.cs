using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace TestWCFDealServiceSetPropertyValues
{

    namespace TestWCFDealServiceSimpleSerialization
    {

        [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
        [Serializable()]
        [System.Diagnostics.DebuggerStepThroughAttribute()]
        [System.ComponentModel.DesignerCategoryAttribute("code")]
        [XmlType(AnonymousType = true)]
        [XmlRoot(Namespace = "", IsNullable = false)]
        public class TestCase
        {
            private string groupField1 { get; set; }
            private string groupField2 { get; set; }
            private string groupField3 { get; set; }

            [XmlElement("TransactionID", typeof (int))]
            public int transactionID { get; set; }

            [XmlElement("GroupField1", typeof (string))]
            public string GroupField1
            {
                get { return this.groupField1; }
                set { this.groupField1 = value; }
            }

            [XmlElement("GroupField2", typeof (string))]
            public string GroupField2
            {
                get { return this.groupField2; }
                set { this.groupField2 = value; }
            }

            [XmlElement("GroupField3", typeof (string))]
            public string GroupField3
            {
                get { return this.groupField3; }
                set { this.groupField3 = value; }
            }

        }
    }
}
