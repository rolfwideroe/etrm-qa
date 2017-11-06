$DeploymentServer="NETVS-DEPLOY"
$Target=$env:computername
$PackageName= "Elviz 2016.2 on $Target"  # change the package name
Echo " -Running $PackageName  from $DeploymentServer"
ECHO " -Target Server: $Target"
Invoke-Command -ComputerName NETVS-DEPLOY -ScriptBlock {
param(
    $Target,$PackageName
)
Set-Location "C:\Program Files (x86)\Admin Arsenal\PDQ Deploy\"; 
PDQDeploy.exe Deploy -Package "$PackageName" -Target "$Target"} -ArgumentList $Target,$PackageName

Start-Sleep 30

#OLD COMMAND
#$ComputerName= $env:computername
#Invoke-Command -ComputerName NETVS-DEPLOY -ScriptBlock {Set-Location "C:\Program Files (x86)\Admin Arsenal\PDQ Deploy\";  
#PDQDeploy.exe Deploy -Package "Elviz 2016.1 on NETVS-ISDEV" -Target "$ComputerName"}
#
#