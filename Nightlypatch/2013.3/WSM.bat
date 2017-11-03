@echo off
set version=2014.1
set DeployServer=NETVS-DEPLOY
set MsiName=WSM%version%.msi
ECHO Nightly patch is  starting on %ElvizApplServer%
echo %MsiName%
Del C:\%MsiName%
xcopy /Y  "\\%DeployServer%\INSTALLATIONS\DEP_TEMP\WSM\%version%\%MsiName%" C:\
MsiExec /X  "{2BDDACBE-5326-458F-88B6-EBD7FBBD987D}" /qn

msiexec /i "C:\%MsiName%" /quiet INSTALLDIR=C:\Elviz IS_SQLSERVER_SERVER=BERSV-SQL8QA IS_SQLSERVER_USERNAME=EcmDbUser IS_SQLSERVER_PASSWORD=EcmDbUser IS_SQLSERVER_DATABASE=QAWSM133 PORT_NUMBER=8009 BUYATC=1 AUTOREFRESH=1 BUYEC=0 USEFAILEDCURVES=1 USENONSYNCEDCURVES=1

