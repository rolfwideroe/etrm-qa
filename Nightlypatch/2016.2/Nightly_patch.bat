 @echo off
set version=2016.2
ECHO Nightly patch is  starting on %ElvizApplServer%
Del C:\ElvizPatchScripts\QADeploy.ps1
Del C:\ElvizPatchScripts\Functions.ps1
Del C:\ElvizPatchScripts\Powershell_launcher.bat
set DeployServer=NETVS-DEPLOY
xcopy /Y  "\\%DeployServer%\INSTALLATIONS\QASCRIPTS\Common\Powershell_launcher.bat" C:\ElvizPatchScripts\
xcopy /Y  "\\%DeployServer%\INSTALLATIONS\QASCRIPTS\%version%\QADeploy.ps1" C:\ElvizPatchScripts\
xcopy /Y  "\\%DeployServer%\INSTALLATIONS\QASCRIPTS\Common\Functions.ps1" C:\ElvizPatchScripts\
xcopy /Y  "\\%DeployServer%\INSTALLATIONS\QASCRIPTS\Common\password.txt" C:\ElvizPatchScripts\
ECHO Enable QA_DEPLOY to run after rebooting....
schtasks /Change /ru bradyplc\qainstall /rp Isetitup2  /tn Run_QADEPLOY /ENABLE
sc config "Brady.ETRM" start= demand
sc config "Brady.ETRM.File.Watching" start= demand
sc config "Brady.ETRM.Message.Queue.Listener" start= demand
sc config "Brady.ETRM.Priceboard" start= demand
sc config "Brady.ETRM.WCF.Publishing" start= demand
MSG.exe /Server:%ElvizApplClient% /Time 120 * This machine will be patched in 2 minutes, please save your work and logoff.
MSG.exe /Time 120 * This machine will be patched in 2 minutes, please save your work and logoff.
\\%DeployServer%\INSTALLATIONS\QASCRIPTS\util\sleep.exe 120
shutdown.exe -r -f -t 0

