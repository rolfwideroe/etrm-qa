 @echo off
set version=2012.1
set versionNum=2012.1
ECHO Nightly patch is  starting on %ElvizApplServer%
Del C:\ElvizPatchScripts\QADeploy.ps1
Del C:\ElvizPatchScripts\Powershell_launcher.bat
set DeployServer=BERSVDEPLOY
xcopy /Y  "\\%DeployServer%\INSTALLATIONS\QASCRIPTS\%version%\warning_message.ps1" C:\ElvizPatchScripts\
xcopy /Y  "\\%DeployServer%\INSTALLATIONS\QASCRIPTS\%version%\Powershell_launcher.bat" C:\ElvizPatchScripts\
xcopy /Y  "\\%DeployServer%\INSTALLATIONS\QASCRIPTS\%version%\QADeploy.ps1" C:\ElvizPatchScripts\
xcopy /Y  "\\%DeployServer%\INSTALLATIONS\QASCRIPTS\%version%\Functions.ps1" C:\ElvizPatchScripts\
ECHO Enable QA_DEPLOY to run after rebooting....
schtasks /Change /ru viz\qainstall /rp installqa  /tn Run_QADEPLOY /ENABLE
MSG.exe /Time 120 * This machine will be patched in 2 minutes, please save your work and logoff.
MSG.exe /Server:%ElvizApplClient% /Time 120 * This machine will be patched in 2 minutes, please save your work and logoff.
c:\util\sleep.exe 120
shutdown.exe -r -f -t 0

