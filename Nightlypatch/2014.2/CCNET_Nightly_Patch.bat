 @echo off
set version=2014.2
ECHO CCNET Nightly patch is  starting on %ElvizApplServer%
Del C:\ElvizPatchScripts\CCNETDeploy.ps1
set DeployServer=NETVS-DEPLOY
xcopy /Y  "\\%DeployServer%\INSTALLATIONS\QASCRIPTS\%version%\CCNETDeploy.ps1" C:\ElvizPatchScripts\
xcopy /Y  "\\%DeployServer%\INSTALLATIONS\QASCRIPTS\Common\Functions.ps1" C:\ElvizPatchScripts\

MSG.exe /Time 60 * Contract Clearer will be Upgraded on this machine, please save ur work and logoff.
echo waiting 2 minutes to run nightly patch...
c:\util\sleep.exe 120
PowerShell -command C:\ElvizPatchScripts\CCNETDeploy.ps1

rem sc stop "ElvizNasdaqOMXadapter"
rem sc config "ElvizNasdaqOMXadapter" start= disabled



