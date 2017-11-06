@echo off
xcopy /Y  "\\NETVS-DEPLOY.nortest.bradyplc.com\INSTALLATIONS\QASCRIPTS\Common\Functions.ps1" C:\ElvizPatchScripts\
xcopy /Y  "\\NETVS-DEPLOY.nortest.bradyplc.com\INSTALLATIONS\QASCRIPTS\Common\ElvizClient.ps1" C:\ElvizPatchScripts\
PowerShell -ExecutionPolicy Bypass -command C:\ElvizPatchScripts\ElvizClient.ps1
