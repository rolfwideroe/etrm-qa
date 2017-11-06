		declare @workspace varchar(50)
		set @workspace='El-European-EUR-PosMon-EUR-MtM'
		declare @reportDate Date
		set @reportDate='2011-11-01'
		
			SELECT a.Instrument,a.DelType,a.PriceBasis,a.Loadtype,a.CurSource, a.Mktprice-b.Mktprice as MktPriceDiff,a.Net_PL-b.Net_PL as PLDiff,a.Net_PL as CognagPL,b.Net_PL as BrandyPL
				  ,a.Bkprice-b.Bkprice as BPDiff
			  FROM QADatawareHouse_Reg132.dbo.[VizPosMonData] a,QADatawareHouse_Reg122.dbo.[VizPosMonData] b
 			  
			  WHERE a.ReportID=(
								SELECT [ReportID]
								FROM QADatawareHouse_Reg132.dbo.[VizPosMonReports]
								WHERE [ReportDate]=@reportDate
								AND [Workspace]=@workspace
								AND [MonitorTitle]=@workspace
							  )
							  
			  and b.ReportID=(
								SELECT [ReportID]
								FROM QADatawareHouse_Reg122.dbo.[VizPosMonReports]
								WHERE [ReportDate]=@reportDate
								AND [Workspace]=@workspace
								AND [MonitorTitle]=@workspace
							  )
			 and a.Instrument=b.Instrument
			 and a.Loadtype=b.Loadtype
			 and a.PriceBasis=b.PriceBasis
			 and a.DelType=b.DelType
			 and a.CurSource=b.CurSource
			-- and (ABS(round(a.Mktprice-b.Mktprice,6))>0 or ABS(round(a.Net_PL -b.Net_PL,6))>0)
			 
			 order by a.Instrument