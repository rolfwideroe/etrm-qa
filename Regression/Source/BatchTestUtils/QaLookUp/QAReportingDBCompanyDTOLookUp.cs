using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ElvizTestUtils.BatchTests;
using ElvizTestUtils.DatabaseTools;
using ElvizTestUtils.PortfolioManagementServiceReference;
using NUnit.Framework;

namespace ElvizTestUtils.QaLookUp
{
    [TestFixture]

    public class QAReportingDBCompanyDTOLookUp
    {
        public static string GetParentCompanyExternalIdByParentId(int parentId)
        {
           // int parentId =54;
            string sqlCommand = string.Format(@"select * from CompanyExports exId
                                        where exId.CompanyId = {0}", parentId);

            DataTable result = QaDao.DataTableFromSql("ReportingDatabase", sqlCommand);
            if (result.Rows.Count != 1) throw new Exception("Query returned null or more than one company.");

            DataRow row = result.Rows[0];

            string parentCompanyExternalId = DataRowTool.GetColumnStringValue(row, "CompanyExternalId");

            if ((parentCompanyExternalId == null) || (parentCompanyExternalId == String.Empty))
                throw new Exception("Parent company external id = NULL.");

            //Console.WriteLine(parentCompanyExternalId);
            return parentCompanyExternalId;

        }

        public static CompanyDto GetCompanyDtoFromReportingDb(string externalID)
        {
            //int transactionID = 265;
            string sqlCommand = string.Format(@"select * from CompanyExports where CompanyExternalID = '{0}'", externalID);

            DataTable result = QaDao.DataTableFromSql("ReportingDatabase", sqlCommand);
            if (result.Rows.Count == 0) throw new Exception("Query did not return any company.");
            if (result.Rows.Count > 1) throw new Exception("Query returned more than one company.");

            DataRow row = result.Rows[0];

            CompanyDto companyDto = new CompanyDto();
            companyDto.ExternalId = DataRowTool.GetColumnStringValue(row, "CompanyExternalId");
            companyDto.Name = DataRowTool.GetColumnStringValue(row, "CompanyName");
            companyDto.CountryIsoCode = GetCountryIsoCode(DataRowTool.GetColumnStringValue(row, "CompanyCountry"));

            int? parentcompnayid  = DataRowTool.GetColumnIntValue(row, "CompanyParentExportId");

            if (parentcompnayid !=null) companyDto.ParentExternalId = GetParentCompanyExternalIdByParentId((int)parentcompnayid);

            companyDto.Status = DataRowTool.GetColumnStringValue(row, "CompanyStatus");

            IList<string> addressList= new string[] {  "AddressLine1", "AddressLine2", "AddressLine3", "OfficeAddress", "Email1", "Email2", "PrivateAddress", "VisitAddress" };

            IList<PortfolioManagementServiceReference.Property> addressProperties = new List<PortfolioManagementServiceReference.Property>();
            foreach (string addressColumn in addressList)
            {
                string dbAddress = DataRowTool.GetColumnStringValue(row, addressColumn);
                if ((dbAddress != null) || (dbAddress == String.Empty))
                {
                    PortfolioManagementServiceReference.Property companyAddresses = new PortfolioManagementServiceReference.Property();
                    {
                        companyAddresses.Name = addressColumn;
                        companyAddresses.ValueType = "String";
                        companyAddresses.Value = dbAddress;
                        addressProperties.Add(companyAddresses);
                    }
                }
            }

            if (addressProperties.Count > 0) companyDto.Addresses = addressProperties.ToArray();

            IDictionary<string, string> phonesList = new Dictionary<string, string>();

            phonesList.Add("BusinessPhone", "Business");
            phonesList.Add("BusinessSwitchboardPhone", "Business Switchboard");
            phonesList.Add("Fax", "Fax");
            phonesList.Add("MobilePhone", "Mobile");
            phonesList.Add("PrivatePhone", "Private");

            IList<PortfolioManagementServiceReference.Property> phonesProperties = new List<PortfolioManagementServiceReference.Property>();
            foreach (KeyValuePair<string, string> phone in phonesList)
            {
                string dbPhones = DataRowTool.GetColumnStringValue(row, phone.Key);
                if ((dbPhones != null) || (dbPhones == String.Empty))
                {
                    PortfolioManagementServiceReference.Property phones = new PortfolioManagementServiceReference.Property();
                    phones.Name = phone.Value;
                    phones.ValueType = "String";
                    phones.Value = dbPhones;
                    phonesProperties.Add(phones);
                }
            }

            if (phonesList.Count > 0) companyDto.Phones = phonesProperties.ToArray();
            IList<string> rolesList = new string[] { "TradeCompany", "Customer", "Counterparty", "Broker", "Exchange", "BillingCompany",
                "PortfolioManager", "CurrencyCounterparty", "GeneralClearingMember", "BalanceResponsibleParty" };
            List<CompanyRole> companyRoles = new List<CompanyRole>();

            foreach (string roleitem in rolesList)
            {
                string roleDbName = roleitem + "Role";
                bool roleStatus = (bool)row[roleDbName];
                if (roleStatus)
                    companyRoles.Add((CompanyRole)Enum.Parse(typeof(CompanyRole), roleitem));
            }
           
            if (companyRoles.Count > 0) companyDto.Roles = companyRoles.ToArray();

            CreditRiskPropertiesDto creditRiskProperties = new CreditRiskPropertiesDto();
            creditRiskProperties.CreditLimit = DataRowTool.GetColumnDoubleValue(row, "Limit");
            creditRiskProperties.CreditLimitIsoCurrency = DataRowTool.GetColumnStringValue(row, "LimitCurrency");
            creditRiskProperties.CreditRating = DataRowTool.GetColumnStringValue(row, "CreditRating");
            creditRiskProperties.Netting = (bool)row["Netting"];

            if (creditRiskProperties != null)
            companyDto.CreditRiskProperties = creditRiskProperties;

            return companyDto;
        }

        private static string GetCountryIsoCode(string countryName)
        {
            switch (countryName)
            {
                case "Sweden":
                    return "SE";
                default: throw new ArgumentException("Countrycode does not exist for : "+countryName);

            }
        }
    }


}
