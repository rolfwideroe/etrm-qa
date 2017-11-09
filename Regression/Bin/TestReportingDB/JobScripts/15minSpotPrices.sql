

-- This script will populate Transactions.LastUpdatedUTC based on Transactions.NotedTime, and then set LastUpdatedUTC column to NOT NULL

-- drop existing funtion if any
IF object_id(N'[ConvertfromUTCtoCET]', N'FN') IS NOT NULL
    DROP FUNCTION ConvertfromUTCtoCET
GO

-- create function
CREATE FUNCTION [dbo].[ConvertfromUTCtoCET] (@UTCDate AS DATETIME2(7))
RETURNS DATETIME2(7)
AS
BEGIN

DECLARE @DstStart datetime2(7)
DECLARE @DstEnd datetime2(7)
DECLARE @CetDate datetime2(7)

SELECT @DstStart = DATEADD(hour, 1,DATEADD(day, DATEDIFF(day, 0, '31/Mar' + CAST(YEAR(@UTCDate) AS varchar)) - 
        (DATEDIFF(day, 6, '31/Mar' + CAST(YEAR(@UTCDate) AS varchar)) % 7), 0)),
    @DstEnd = DATEADD(hour, 1,DATEADD(day, DATEDIFF(day, 0, '31/Oct' + CAST(YEAR(@UTCDate) AS varchar)) - 
        (DATEDIFF(day, 6, '31/Oct' + CAST(YEAR(@UTCDate) AS varchar)) % 7), 0))



SELECT @CetDate = CASE WHEN @UTCDate <= @DstEnd AND @UTCDate >= @DstStart
    THEN DATEADD(hour, +2, @UTCDate)
    ELSE DATEADD(hour, +1, @UTCDate) END



RETURN @CetDate
END

GO 


Delete from AreaPrices
Delete from AreaPriceSeries


  IF NOT Exists (SELECT * FROM [AreaPriceTypes] WHERE TypeName='NP_IntradayAuction')
  BEGIN
       INSERT INTO [AreaPriceTypes] VALUES ('NP_IntradayAuction',NULL)
  END


  DECLARE @areaId INT
  DECLARE @typeId INT
  DECLARE @currencyId INT 
  DECLARE @sourceId INT
  SET @areaId= (SELECT AreaId FROM Areas WHERE AreaName='NO1')
  SET @typeId= (SELECT TypeId FROM AreaPriceTypes WHERE TypeName='NP_IntradayAuction')
  SET @currencyId=(SELECT CurrencyId FROM Currencies WHERE ISO_Currency='EUR')
  SET @sourceId=(SELECT SourceId FROM DataSources WHERE SourceName='Nordpool')
  INSERT INTO [AreaPriceSeries] VALUES (@areaId,15,@typeId, @currencyId, @sourceId,'Central Europe Standard Time','NO1-IntraDay-15min','NO1-IntraDay-15min')

  DECLARE @areaPriceSeriesId INT
  SET @areaPriceSeriesId=(SELECT TOP 1 AreaPriceSeriesId FROM AreaPriceSeries ORDER BY AreaPriceSeriesId DESC)

  

  DECLARE @startDate DATETIME
  DECLARE @stopDate DATETIME
  DECLARE @indexDate DATETIME

  SET @startDate='2010-12-31 23:00'
  SET @stopDate='2011-12-31 23:00'


  SET @indexDate=@startDate

  WHILE @indexDate<@stopDate
  BEGIN

         INSERT INTO AreaPrices VALUES(@areaPriceSeriesId,Round(rand()*100,2),@indexDate,dbo.ConvertfromUTCtoCET(@indexDate))
      SET @indexDate=DATEADD(MINUTE,15,@indexDate)

  END

  GO



  
-- drop function
IF object_id(N'[ConvertfromUTCtoCET]', N'FN') IS NOT NULL
    DROP FUNCTION ConvertfromUTCtoCET
GO
  

