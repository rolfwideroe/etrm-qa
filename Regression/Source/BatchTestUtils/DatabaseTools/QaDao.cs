using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Linq;
using System.Runtime.CompilerServices;
using ElvizTestUtils.BatchTests;

namespace ElvizTestUtils.DatabaseTools
{
    public class QaDao
    {
        private const string DatabaseReaderUsername = "EcmDbReader";
        private const string DatabaseReaderPassword = "EcmDbReader";

        private readonly SqlConnection ecmDbConnection;
        private readonly SqlConnection systemDbConnection;
        private readonly SqlConnection reportingDbConnection;
        private readonly SqlConnection vizProcesDbConnection;

        private bool disposed;

        public static SqlConnection VizEcmDbConnection
        {
            get
            {
                    string ecmDbName = ElvizInstallationUtility.GetEtrmDbName("VizECM");
                    string sqlServerName = ElvizInstallationUtility.GetSqlServerName();
                    return new SqlConnection(
                        $"Data Source={sqlServerName};Initial Catalog={ecmDbName};Trusted_Connection=True;");
            }
        }
        public static SqlConnection VizPricesDbConnection
        {
            get
            {
                string ecmDbName = ElvizInstallationUtility.GetEtrmDbName("VizPrices");
                string sqlServerName = ElvizInstallationUtility.GetSqlServerName();
                return new SqlConnection(
                    $"Data Source={sqlServerName};Initial Catalog={ecmDbName};Trusted_Connection=True;");
            }
        }
        public static SqlConnection SystemDbConnection
        {
            get
            {
                    string systemDbName = ElvizInstallationUtility.GetEtrmDbName("VizSystem");
                    string sqlServerName = ElvizInstallationUtility.GetSqlServerName();
                    return new SqlConnection(
                        $"Data Source={sqlServerName};Initial Catalog={systemDbName};Trusted_Connection=True;");
            }
        }

        public QaDao()
        {
            string ecmDbName = ElvizInstallationUtility.GetEtrmDbName("VizECM");
            string systemDbName = ElvizInstallationUtility.GetEtrmDbName("VizSystem");
            string reportingDbName = ElvizInstallationUtility.GetEtrmDbName("ReportingDatabase");
            string vizPricesDbName = ElvizInstallationUtility.GetEtrmDbName("VizPrices");
            string sqlServerName = ElvizInstallationUtility.GetSqlServerName();
            this.ecmDbConnection = new SqlConnection(
                $"Data Source={sqlServerName};Initial Catalog={ecmDbName};Trusted_Connection=True;");
            this.systemDbConnection = new SqlConnection(
                $"Data Source={sqlServerName};Initial Catalog={systemDbName};Trusted_Connection=True;");
            this.reportingDbConnection = new SqlConnection(
                $"Data Source={sqlServerName};Initial Catalog={reportingDbName};Trusted_Connection=True;");
        }

        public static DataTable DataTableFromSql(string sqlInstance,string database,string userName,string password, string sql)
        {
            SqlConnection connection = new SqlConnection(
                $"Data Source={sqlInstance};Initial Catalog={database};User ID={userName};Password={password};Trusted_Connection=False;");
            return DataTableFromSql(connection, sql);
        }

        public static DataTable DataTableFromSql(SqlConnection connection, string sql)
        {

            DataTable table = new DataTable();

            SqlCommand command = new SqlCommand(sql, connection);
            command.CommandTimeout = 60;
            try
            {
                connection.Open();

                SqlDataAdapter dataAdapter = new SqlDataAdapter(command);
                dataAdapter.Fill(table);
            }
            catch (Exception ex)
            {
                throw new Exception("Could not conntect to: " + connection.ConnectionString + "\n" + ex.Message);
            }
            finally
            {
                connection.Close();
            }

            return table;
        }


        public static DataTable DataTableFromSql(string dbType,string sql)
        {
            if (string.IsNullOrEmpty(dbType)) throw new ApplicationException("DatabaseName Property has not been initialized");

            string tmpSqlServerName  = "";
            if (dbType.Equals("BcrContractDatabase"))
            {
                tmpSqlServerName = ElvizInstallationUtility.GetEtrmDbName("BcrContractDatabaseServer");
            }
            else
            {
                tmpSqlServerName = ElvizInstallationUtility.GetSqlServerName(); 
            }
            string dbName = ElvizInstallationUtility.GetEtrmDbName(dbType); 

            //SqlConnection connection = new SqlConnection(string.Format("Data Source={0};Initial Catalog={1};User ID={2};Password={3};Trusted_Connection=False;", tmpSqlServerName, dbName, DatabaseReaderUsername, DatabaseReaderPassword));
            SqlConnection connection = new SqlConnection(
                $"Data Source={tmpSqlServerName};Initial Catalog={dbName};Trusted_Connection=True;");
            DataTable table = new DataTable();

            SqlCommand command = new SqlCommand(sql, connection);
            command.CommandTimeout = 90;
            try
            {
                connection.Open();

                SqlDataAdapter dataAdapter = new SqlDataAdapter(command);
                dataAdapter.Fill(table);
            }
            catch (Exception ex)
            {
                throw new Exception("Could not conntect to: " + connection.ConnectionString + "\n" + ex.Message);
            }
            finally
            {
                connection.Close();
            }

            return table;
        }

        public static void UpdateReportingDBRecord(string dbType, string sql)
        {
            if (string.IsNullOrEmpty(dbType)) throw new ApplicationException("DatabaseName Property has not been initialized");

            string sqlServerName = ElvizInstallationUtility.GetSqlServerName();
            string dbName = ElvizInstallationUtility.GetEtrmDbName(dbType);

            SqlConnection connection = new SqlConnection(
                $"Data Source={sqlServerName};Initial Catalog={dbName};User ID={"EcmDbReader"};Password={"EcmDbReader"};Trusted_Connection=False;");

            NonQuery(connection,sql);
        }


        public static void DeleteInstrumentPriceRecordUsingParameters(string productName, string areaName, string reportDate)
        {
            string query = @"Delete FROM [Prices]
                                  where ProdId =
                                  (SELECT Top 1 p.ProdId FROM [Prices] p
                                    join Products prod on prod.ProdId = p.ProdId
                                    join Areas a on a.AreaId= prod.AreaId                                   
                                    where a.AreaName ='" + areaName + @"'
                                    and TradeDate='" + reportDate + @"'
                                    and ProdName = '"+ productName + @"' )";

            NonQuery(VizPricesDbConnection, query);
        }

        public static DataTable GetInstrumentPricesByArea(string executionVenue,  string areaName,string reportDate, string cfdAreaName=null )
        {
            if (cfdAreaName == null) cfdAreaName = "is NULL";
            else cfdAreaName = "= '" + cfdAreaName + "'";
            
            string sqlCommand = @"SELECT * FROM [Prices] p
                                  join Products prod on prod.ProdId = p.ProdId
                                  join Areas a on a.AreaId=prod.AreaId
                                  join ExecutionVenues ev on ev.ExecutionVenueId= prod.ExecutionVenueId
                                  left join Areas c on c.AreaId=prod.CfdBaseAreaId
                                  where a.AreaName ='" + areaName+ @"'
                                  and TradeDate='"+ reportDate+ @"'                             
                                  and ev.ExecutionVenueName = '"+ executionVenue +@"'
                                  and c.AreaName "+ cfdAreaName + @"";

            DataTable pricesTable = DataTableFromSql("VizPrices", sqlCommand);
            return pricesTable;
        }

        public static int GetSpotPricesByArea(string areaName, string reportDate, string resolution = "Day")
        {
            string sqlCommand = string.Empty;
            //query returns 24 records for hourly resolution in timeline between report day 00 AM until 23 (PM)
               sqlCommand = @"SELECT COUNT (*)
                            FROM AreaPriceSeries serie
                            join Areas a on serie.AreaId = a.AreaId
                            join AreaPriceTypes t on t.TypeId = serie.TypeId
                            join AreaPrices prices on prices.AreaPriceSeriesId = serie.AreaPriceSeriesId
                            join AreaPricePeriodTypes period on serie.AreaPricePeriodTypeId= period.AreaPricePeriodTypeId
                            where a.AreaName = '" + areaName + @"'
                            and serie.TypeId='2' 
                            and prices.PeriodStartTimeInTimeZone >= '" + reportDate + @" 00:00'
                            and prices.PeriodStartTimeInTimeZone < '" + reportDate + @" 23:59'
                            and period.AreaPricePeriodTypeName ='" + resolution + @"' ";
                
           // }

            return ReadScalar<int>(VizPricesDbConnection, sqlCommand);
            
        }
        public int[] GetOriginalTransactionIdsFromExternalIds(string[] externalIds)
        {
            SqlConnection connection = this.ecmDbConnection;

            const string sqlCommand = @"select t.Id from transactions t                                        

                                        where t.ExternalId in ('@externalIds')
                                        and  t.Originator=1";

            string commandText = sqlCommand.Replace("@externalIds", string.Join("','", externalIds));

            SqlCommand command = new SqlCommand(commandText, connection);

            IList<int> transactionIds = new List<int>();

            try
            {
                connection.Open();

                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    transactionIds.Add(reader.GetInt32(0));
                }

            }
            finally
            {
                connection.Close();
            }

            return transactionIds.OrderBy(x => x).ToArray();
        }

        public string[] GetExternalIdsFromTransactionIds(int[] transactionIds)
        {
            SqlConnection connection = this.ecmDbConnection;

            const string sqlCommand = @"select t.ExternalId from transactions t
                                        where t.Id in (@transIds)";

            string commandText = sqlCommand.Replace("@transIds", string.Join(",", transactionIds));

            SqlCommand command = new SqlCommand(commandText, connection);

            IList<string> extIds = new List<string>();

            try
            {
                connection.Open();

                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    extIds.Add(reader.GetString(0));
                }

            }
            catch (SqlNullValueException)
            {
                throw new ArgumentException("No ExternalIds for one or several of the TransactionIds : " + string.Join(",", transactionIds));
            }
            finally
            {
                connection.Close();
            }

            return extIds.OrderBy(x => x).ToArray();
        }

        public string GetFilterForEcmWorkspace(string workSpaceName)
        {
            string commandText = @"select FilterName from filters where filterid = (
                                    select top 1[Value] from WSFormSettings
                                where WSId in (select wsid from Workspace where WSName = '" + workSpaceName + @"')
                                and setting = 'filterid')";
            return ReadScalar<string>(this.ecmDbConnection, commandText);
        }

        public void CreateFilterForPortfolioAndSetOptionAllowed(string portfolioName)
        {
            string query = @"DECLARE @filterPortfolioName VARCHAR(255)
                            set @filterPortfolioName='"+portfolioName+ @"'

                             DECLARE @lastFilterId INT
                             DECLARE @portfolioId INT

                            SET @portfolioId=(SELECT PortfolioId FROM Portfolios WHERE PortfolioName=@filterPortfolioName)

                            INSERT INTO Filters VALUES('Trans',@filterPortfolioName,0,'Common','Vizard',-1)
                            SET @lastFilterId =(SELECT SCOPE_IDENTITY())

                            INSERT INTO FilterParts VALUES(@lastFilterId,1,5,'Transaction status = Active, Cleared','FilteredList','Transaction status','StatusId','=',1,'','','1;4')
                            INSERT INTO FilterParts VALUES(@lastFilterId,1,1,'Trade portfolio = '+@filterPortfolioName ,'Comp','Trade portfolio','PortfolioId','=',2,'','AND',@portfolioId)
                            INSERT INTO Portfolio_Role VALUES(2,@portfolioId)
                            ";
                            
            NonQuery(this.ecmDbConnection,query);
        }

        public static bool HasAllTransactionBeenExportedToDwh()
        {
            string query = "select COUNT(*) from VizTransMonReports where Workspace = 'AllTransactions'";

            string sqlServerName = ElvizInstallationUtility.GetSqlServerName();
            string dbName = ElvizInstallationUtility.GetEtrmDbName("VizDatawarehouse");

            SqlConnection connection = new SqlConnection(
                $"Data Source={sqlServerName};Initial Catalog={dbName};User ID={DatabaseReaderUsername};Password={DatabaseReaderPassword};Trusted_Connection=False;");

            int rows = ReadScalar<int>(connection, query);

            if (rows == 0) return false;

            return true;

        }

        public static T ReadScalar<T>(SqlConnection connection, string query)
        {
            SqlCommand command = new SqlCommand(query, connection);
            try
            {
                connection.Open();

                object scalar = command.ExecuteScalar();

                if (scalar == DBNull.Value) return default(T);

                return (T)scalar;
            }
            finally
            {
                connection.Close();
            }
        }

        public static void NonQuery(SqlConnection connection, string query)
        {
            NonQuery(connection,query,null);
        }

        public static void NonQuery(SqlConnection connection, string query,IEnumerable<SqlParameter> parameters)
        {
            SqlCommand command = new SqlCommand(query, connection) { CommandTimeout = 120 };
            if (parameters != null)
            {

                foreach (SqlParameter sqlParameter in parameters)
                {
                    command.Parameters.Add(sqlParameter);
                }
            }
            try
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                }
                finally
                {
                    connection.Close();
                }
        }


        public int? GetLatestExecutionIdForJob(string jobName)
        {
            string query = @"
                            select MAX(j.JobExecutionId) from JobExecutions j
                            join StoredJobs s on j.StoredJobId=s.StoredJobId
                            where s.[Description]='" + jobName + "'";

            return ReadScalar<int?>(this.ecmDbConnection, query);
        }

        public static string MissingRealizedDataStrategy
        {
            get
            {
                string query = "select TOP 1 [MissingRealizedDataStrategy] from [SystemInfo]";
                string missingRealizedDataStrategy = ReadScalar<string>(SystemDbConnection ,query);
                return missingRealizedDataStrategy;
            }
            set
            {

                string nonQuery = ("Update [SystemInfo] SET [MissingRealizedDataStrategy]=" + "'" + value + "'");
                NonQuery(SystemDbConnection, nonQuery)
                ;


            }
        }

        public static bool PamEnabled
        {
            get
            {
                string query = "select TOP 1 [PAMisActive] from [SystemInfo]";
                bool pam = ReadScalar<bool>(SystemDbConnection, query);
                return pam;

             
            }
            set
            {

                string nonQuery = "Update [SystemInfo] SET [PAMisActive]=" + (value ? "1" : "0");
                NonQuery(SystemDbConnection, nonQuery);

            }
        }

        public static bool AutorizationEnabled
        {
            get
            {
                string query = "select TOP 1 [SysUseAuths] from [SystemInfo]";
                int autorization = ReadScalar<int>(SystemDbConnection, query);
                return Convert.ToBoolean(autorization);

            }
            set
            {
                string nonQuery = "Update [SystemInfo] SET[SysUseAuths]=" + (value ? "1" : "0");
                NonQuery(SystemDbConnection, nonQuery);

            }
        }
        public void RevertConfigurationToDefault(string configurationName)
        {
            string query =
                $"UPDATE [ConfigurationSettings] SET [Value]=[DefaultValue] WHERE [Name] = '{configurationName}'";


            NonQuery(this.ecmDbConnection,query);
        }

        public static void SetSysTradeDate(string newSysTradeDate)
        {
            string query =
                $"Update SystemInfo set SysTradeDate = '{newSysTradeDate}'";

            NonQuery(SystemDbConnection, query);
        }

        public void UpdateConfiguration(string configurationName,string configurationValue)
        {
            string query =
                $" UPDATE [ConfigurationSettings] SET [Value]='{configurationValue}'WHERE [Name] = '{configurationName}'";
            
            NonQuery(this.ecmDbConnection,query);
        }

        public static void RevertVolatilitiesToDefault(string pricebookTemplateName)
        {
            string query = string.Format(@" UPDATE [CurveInfo] SET
 
                                            SummerVolatilityImmediate = 0.9,
                                            SummerVolatilityMedium = 0.17,
                                            SummerVolatilityAsymptotic = 0.1,
                                            WinterVolatilityImmediate = 0.9,
                                            WinterVolatilityMedium = 0.17,
                                            WinterVolatilityAsymptotic = 0.1

                                            where CurveInfoid = (SELECT CurveInfoId FROM CurveVenueMap WHERE CurvePriceBookTemplateId =
                                            (select CurvePriceBookTemplateId from CurvePriceBookTemplates t where t.Name = '{0}'))
                                            or CurveInfoId = (SELECT PeakCurveInfoId FROM CurveVenueMap WHERE CurvePriceBookTemplateId =
                                            (select CurvePriceBookTemplateId from CurvePriceBookTemplates t where t.Name = '{0}'))", pricebookTemplateName);
            NonQuery(VizEcmDbConnection, query);
        }

        public static void UpdateLastDateRunForVolatilities(string volatilityJobName, string newLastDateRunFor)
        {
            string query = string.Format(@" UPDATE [VolCorrUpdateDefinitions] SET LastDateRunFor = '{1}' 
                                            where Description = '{0}'", volatilityJobName, newLastDateRunFor);
            NonQuery(VizEcmDbConnection, query);
        }

        public ElvizConfiguration[] GetAllElvizConfigurations()
        {
            return GetElvizConfigurations("");
        }

        public ElvizConfiguration[] GetAllNonDefulatElvizConfigurations()
        {
            const string parameterSql = "WHERE Value<>DefaultValue";
            return GetElvizConfigurations(parameterSql);
        }

        private ElvizConfiguration[] GetElvizConfigurations(string parameterSqlString)
        {

            SqlConnection connection = this.ecmDbConnection;

            string commandText = @"SELECT 
                                         [Name]
                                        ,[Value]
                                        ,[DefaultValue]
                                         FROM [ConfigurationSettings] " + parameterSqlString;



            SqlCommand command = new SqlCommand(commandText, connection);

            IList<ElvizConfiguration> elvizConfigurations = new List<ElvizConfiguration>();

            try
            {
                connection.Open();

                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    string name = reader.GetString(0);
                    string value = reader.IsDBNull(1) ? string.Empty : reader.GetString(1);
                    string defaultValue = reader.IsDBNull(2) ? string.Empty : reader.GetString(2);

                    elvizConfigurations.Add(new ElvizConfiguration(name, value, defaultValue));
                }

            }
            finally
            {
                connection.Close();
            }

            return elvizConfigurations.ToArray();
        }

		public static void ConfigureMutablePosMon(Setup setup)
		{
			string query = @"	UPDATE WSFormSettings 
								SET Value = @aggregateStructuredDeals
								WHERE WSId = (SELECT WSId FROM Workspace WHERE WSName = 'MutablePosMon') AND Setting = 'AggregateStructuredDeals'

								UPDATE FilterParts 
								SET ValueList = @extId 
								WHERE FilterId = (select FilterId from Filters where FilterName = 'MutableExtId') AND FieldName = 'Transactions.ExternalId' ";

			IList<SqlParameter> parameters = new List<SqlParameter>();
			parameters.Add(new SqlParameter("@aggregateStructuredDeals", setup.AggregateStructuredDeals));
			parameters.Add(new SqlParameter("@extId", setup.ExtId));

			NonQuery(VizEcmDbConnection, query, parameters);
		}

		public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // Protected implementation of Dispose pattern. 
        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                this.ecmDbConnection.Close();
                this.systemDbConnection.Close();
                this.ecmDbConnection.Dispose();
                this.systemDbConnection.Dispose();
                this.vizProcesDbConnection.Dispose();
                // Free any other managed objects here. 
                //
            }

            // Free any unmanaged objects here. 
            //
            disposed = true;
        }
    }
}
