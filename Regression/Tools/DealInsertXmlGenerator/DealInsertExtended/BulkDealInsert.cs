using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DealInsertExtended
{
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    public partial class BulkDealInsert
    {

        private DealInsert[] dealInsertField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("DealInsert")]
        public DealInsert[] DealInsert
        {
            get
            {
                return this.dealInsertField;
            }
            set
            {
                this.dealInsertField = value;
            }
        }
    }
}
