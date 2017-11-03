@echo off
xcopy /Y  "\\BERSVDEPLOY\INSTALLATIONS\QASCRIPTS\Common\Functions.ps1" C:\ElvizPatchScripts\
xcopy /Y  "\\BERSVDEPLOY\INSTALLATIONS\QASCRIPTS\Common\WSM_Deploy.ps1" C:\ElvizPatchScripts\
PowerShell -ExecutionPolicy Bypass -command C:\ElvizPatchScripts\WSM_Deploy.ps1


