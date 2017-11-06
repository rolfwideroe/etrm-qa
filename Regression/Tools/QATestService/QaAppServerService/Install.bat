SET InstallPath=
FOR /F "tokens=*" %%i in ('CD') do SET InstallPath=%%i
sc create "QATestService" binPath= "%InstallPath%\QATestService.exe" start= auto
pause