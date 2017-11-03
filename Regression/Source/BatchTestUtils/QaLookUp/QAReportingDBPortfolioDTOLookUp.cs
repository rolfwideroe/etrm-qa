using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ElvizTestUtils.DatabaseTools;
using ElvizTestUtils.PortfolioManagementServiceReference;
using NUnit.Framework;

namespace ElvizTestUtils.QaLookUp
{
    [TestFixture]
    public class QAReportingDBPortfolioDTOLookUp
    {
        private static string GetParentPortfolioExternalIdByParentId(int parentId)
            {
                // int parentId =54;
                string sqlCommand = string.Format(@"select * from PortfolioExports exId
                                        where exId.PortfolioId = {0}", parentId);

                DataTable result = QaDao.DataTableFromSql("ReportingDatabase", sqlCommand);
                if (result.Rows.Count != 1) throw new Exception("Query returned null or more than one company.");

                DataRow row = result.Rows[0];

                string parentPortfolioExternalId = DataRowTool.GetColumnStringValue(row, "PortfolioExternalId");

                if ((parentPortfolioExternalId == null) || (parentPortfolioExternalId == String.Empty))
                    throw new Exception("Parent portfolio external id = NULL.");

                //Console.WriteLine(parentCompanyExternalId);
                return parentPortfolioExternalId;

            }

            public static PortfolioDto GetPortfolioDtoFromReportingDb(string externalID)
            {
                //int transactionID = 265;
                string sqlCommand = string.Format(@"select * from PortfolioExports where PortfolioExternalId = '{0}'", externalID);

                DataTable result = QaDao.DataTableFromSql("ReportingDatabase", sqlCommand);
                if (result.Rows.Count == 0) throw new Exception("Query did not return any portfolio.");
                if (result.Rows.Count > 1) throw new Exception("Query returned more than one portfolio.");

                DataRow row = result.Rows[0];
                
                PortfolioDto portfolioDto = new PortfolioDto();
                portfolioDto.ExternalId = DataRowTool.GetColumnStringValue(row, "PortfolioExternalId");
                portfolioDto.Name = DataRowTool.GetColumnStringValue(row, "PortfolioName");
                //portfolioDto. = DataRowTool.GetColumnStringValue(row, "PortfolioName");
                int? portfolioCompanyId = DataRowTool.GetColumnIntValue(row, "PortfolioCompanyExportId");
                portfolioDto.CompanyExternalId =
                    QAReportingDBCompanyDTOLookUp.GetParentCompanyExternalIdByParentId((int)portfolioCompanyId);

                int? portfolioParentId = DataRowTool.GetColumnIntValue(row, "PortfolioParentExportId");
                if (portfolioParentId != null) portfolioDto.ParentPortfolioExternalId = GetParentPortfolioExternalIdByParentId((int)portfolioParentId);

                portfolioDto.Status = DataRowTool.GetColumnStringValue(row, "PortfolioStatus");
               
                return portfolioDto;
            }

        }
    }
