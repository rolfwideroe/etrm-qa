using System.Collections.Generic;
using System.Xml.Serialization;
using ElvizTestUtils.PortfolioManagementServiceReference;

namespace TestCompany
{
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    public class TestClass
    {
        private string countryIsoCode;
       // private string externalId;
        private string name;
        private string parentCompanyExternalId;
        private string orgNo;
        private string shortName;

        private string childCompanyExternalId;
        private string childCompanyName;
        private string childCompanyOrgNo;

        private string status;
        private List<string> companyRoles;

        private string portfolioExternalId;
        private string portfolioName;
        private string parentPortfolioExternalId;
        private string portfolioStatus;

        private string childPortfolioName;
        private string childPortfolioExternalId;
        private string companyForUpdate;

     
        public string CountryIsoCode
        {
            get { return this.countryIsoCode; }
            set { this.countryIsoCode = value; }
        }

        //[System.Xml.Serialization.XmlElementAttribute("ExternalId", typeof(string))]
        //public string ExternalId
        //{
        //    get { return this.externalId; }
        //    set { this.externalId = value; }
        //}

        [System.Xml.Serialization.XmlElementAttribute("CompanyName", typeof (string))]
        public string Name
        {
            get { return this.name; }
            set { this.name = value; }
        }
        
        [System.Xml.Serialization.XmlElementAttribute("ParentCompanyExternalId", typeof(string))]
        public string ParentCompanyExternalId
        {
            get { return this.parentCompanyExternalId; }
            set { this.parentCompanyExternalId = value; }
        }

        [System.Xml.Serialization.XmlElementAttribute("OrgNo", typeof(string))]
        public string OrgNo
        {
            get { return this.orgNo; }
            set { this.orgNo = value; }
        }

        [System.Xml.Serialization.XmlElementAttribute("CompanyShortName", typeof(string))]
        public string ShortName
        {
            get { return this.shortName; }
            set { this.shortName = value; }
        }
        [System.Xml.Serialization.XmlElementAttribute("ChildCompanyExternalId", typeof(string))]
        public string ChildCompanyExternalId
        {
            get { return this.childCompanyExternalId; }
            set { this.childCompanyExternalId = value; }
        }

        [System.Xml.Serialization.XmlElementAttribute("ChildCompanyName", typeof(string))]
        public string ChildCompanyName
        {
            get { return this.childCompanyName; }
            set { this.childCompanyName = value; }
        }

        [System.Xml.Serialization.XmlElementAttribute("ChildCompanyOrgNo", typeof(string))]
        public string ChildCompanyOrgNo
        {
            get { return this.childCompanyOrgNo; }
            set { this.childCompanyOrgNo = value; }
        }

        [System.Xml.Serialization.XmlElementAttribute("Status", typeof(string))]
        public string Status
        {
            get { return this.status; }
            set { this.status = value; }
        }


        [XmlArray(ElementName = "CompanyRoles")]
        [XmlArrayItem("Role")]
        public List<string> CompanyRoles
        {
            get { return this.companyRoles; }
            set { this.companyRoles = value; }
        }

        [System.Xml.Serialization.XmlElementAttribute("PortfolioExternalId", typeof(string))]
        public string PortfolioExternalId
        {
            get { return this.portfolioExternalId; }
            set { this.portfolioExternalId = value; }
        }

        [System.Xml.Serialization.XmlElementAttribute("PortfolioName", typeof(string))]
        public string PortfolioName
        {
            get { return this.portfolioName; }
            set { this.portfolioName = value; }
        }

        [System.Xml.Serialization.XmlElementAttribute("ParentPortfolioExternalId", typeof(string))]
        public string ParentPortfolioExternalId
        {
            get { return this.parentPortfolioExternalId; }
            set { this.parentPortfolioExternalId = value; }
        }

        [System.Xml.Serialization.XmlElementAttribute("PortfolioStatus", typeof(string))]
        public string PortfolioStatus
        {
            get { return this.portfolioStatus; }
            set { this.portfolioStatus = value; }
        }
       
        [System.Xml.Serialization.XmlElementAttribute("ChildPortfolioName", typeof(string))]
        public string ChildPortfolioName
        {
            get { return this.childPortfolioName; }
            set { this.childPortfolioName = value; }
        }

        [System.Xml.Serialization.XmlElementAttribute("ChildPortfolioExternalId", typeof(string))]
        public string ChildPortfolioExternalId
        {
            get { return this.childPortfolioExternalId; }
            set { this.childPortfolioExternalId = value; }
        }

        [System.Xml.Serialization.XmlElementAttribute("CompanyForUpdate", typeof(string))]
        public string CompanyForUpdate
        {
            get { return this.companyForUpdate; }
            set { this.companyForUpdate = value; }
        }

        [XmlArray("Addresses")]
        [XmlArrayItem(typeof(ElvizProperty))]
        public List<ElvizProperty> Addresses { get; set; }
        
        [XmlArray("Phones")]
        [XmlArrayItem(typeof(ElvizProperty))]
        public List<ElvizProperty> Phones { get; set; }

        public CreditRiskPropertiesDto CreditRiskProperties { get; set; }
    }


    [XmlRoot("ElvizProperty")]
    public class ElvizProperty
    {
        [XmlAttribute("name")]
        public string name { get; set; }
        
        [XmlAttribute("value")]
        public string value { get; set; }
        
        [XmlAttribute("valueType")]
        public string valueType { get; set; }
    }
    


    //public class CreditRiskProperty
    //{
    //   public decimal? CreditLimit { get; set; }

    //   public string CreditLimitIsoCurrency { get; set; }
    //   public string CreditRating{ get; set; }

    //   public bool Netting { get; set; }
    //    //some other elements go here.
    //}
 
}
