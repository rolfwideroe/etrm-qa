  DECLARE @maxId INT

  IF NOT EXISTS (SELECT * FROM SettlementRules WHERE Name='Once -10 BD / +0 CD')
  BEGIN
	SET @maxId=(SELECT MAX(SettlementRuleId) FROM SettlementRules)+1
	INSERT INTO SettlementRules VALUES(@maxId,'Once -10 BD / +0 CD',1,0,-10)
  END

  IF NOT EXISTS (SELECT * FROM SettlementRules WHERE Name='Daily +0 BD / +0 CD')
  BEGIN
	SET @maxId=(SELECT MAX(SettlementRuleId) FROM SettlementRules)+1
	INSERT INTO SettlementRules VALUES(@maxId,'Daily +0 BD / +0 CD',5,0,0)
  END 

  IF NOT EXISTS (SELECT * FROM SettlementRules WHERE Name='Monthly +1 BD / +19 CD')
  BEGIN
	SET @maxId=(SELECT MAX(SettlementRuleId) FROM SettlementRules)+1
	INSERT INTO SettlementRules VALUES(@maxId,'Monthly +1 BD / +19 CD',3,19,1)
  END 

  IF NOT EXISTS (SELECT * FROM SettlementRules WHERE Name='Monthly +1 BD / +0 CD with Holidays')
  BEGIN
	  SET @maxId=(SELECT MAX(SettlementRuleId) FROM SettlementRules)+1
	  INSERT INTO SettlementRules VALUES(@maxId,'Monthly +1 BD / +0 CD with Holidays',3,0,1)
	  INSERT INTO [SettlementRuleCalendars] VALUES(@maxId,5)
  END