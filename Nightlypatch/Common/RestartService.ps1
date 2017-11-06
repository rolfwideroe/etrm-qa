
Echo "[INFO]:Killing Elviz Processes on the application server..."
$processes_names=@("Viz.Integration.Core.MessageQueueListener.exe","Viz.Integration.Core.FileWatcher.exe","Viz.MiddleWare.Priceboard.WinService.exe","Viz.MiddleWare.ETRMWinService.exe")
$Processes_objects = @()
$services_names=@("Elviz Priceboard Service","Elviz Curve Server Services","Elviz WCF Publishing Service","Elviz File Watching Service","Elviz Message Queue Listener Service")
FOREACH($process_name in $processes_names){
    $process_name = get-wmiobject win32_process | where {$_.Name -like "$process_name"}
    if($process_name -ne $null){
        $Processes_objects=$Processes_objects + $process_name
    }
}

FOREACH($Process_object in $Processes_objects){
    $obj_name=$Process_object.Name
    Echo "[INFO]:Terminating $obj_name "
    $terminate =$Process_object.Terminate()
}
FOREACH ($Process_object in $Processes_objects)
{
    $s= Get-Process  -Id $Process_object.ProcessId -ErrorAction SilentlyContinue
    if($s)
    {			
       $obj_name=$Process_object.Name
       $Obj_PID=$Process_object.ProcessId
       Echo "[INFO]:Killing  $obj_name which has PID:$Obj_PID"
       & taskkill -PID $Process_object.ProcessId | Out-Null
  }
}

FOREACH ($Process_object in $Processes_objects)
{
$PID_Exist= Get-Process  -Id $Process_object.ProcessId -ErrorAction SilentlyContinue
$PID_Exist_name= $Process_object.Name
    if($PID_Exist)
    {	
        Echo "[WARN]:Cannot kill the process $PID_Exist_name with PID number $PID_Exist"
    }
}
FOREACH ($service_name in $services_names)
{

	Echo " Stop $service_name"
	Stop-Service -name "$service_name"
	$Viz_ETRM_Services = get-service -display "$service_name" -ErrorAction SilentlyContinue 
	$ServiceStatus = $Viz_ETRM_Services.Status
	IF($ServiceStatus -eq "Running"){
	    Echo "[INFO]:$service_name status is : $ServiceStatus "
	}ELSE{

	    Echo "[INFO]:$service_name status is : $ServiceStatus "

	}
}
FOREACH ($service_name in $services_names)
{
	start-sleep 5
	Echo " Start $service_name"
	Start-Service -name "$service_name"
	$Viz_ETRM_Services = get-service -display "$service_name" -ErrorAction SilentlyContinue 
	$ServiceStatus = $Viz_ETRM_Services.Status
	IF($ServiceStatus -eq "Running"){
	    Echo "[INFO]:$service_name status is : $ServiceStatus "
	}ELSE{

	    Echo "[INFO]:$service_name status is : $ServiceStatus "

	}
}
