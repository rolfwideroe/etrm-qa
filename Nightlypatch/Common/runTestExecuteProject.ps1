Param($Param1)
function Run-LocalProcess ($cmd, $Parameters) {
    $Process = New-Object System.Diagnostics.Process
    $Process.StartInfo = New-Object System.Diagnostics.ProcessStartInfo
    $ExitCode = $false      
    $Process.StartInfo.FileName = $cmd
    $Process.StartInfo.Arguments = $Parameters
    $Process.StartInfo.UseShellExecute = $shell
    $Process.StartInfo.WindowStyle = 1; #Window style is hidden.  Comment out this line to show output in separate console
    $null = $Process.Start()
    $Process.WaitForExit() #Waits for the process to complete.   
    $ExitCode = $Process.ExitCode
    $Process.Dispose()    
    return $ExitCode
}

$PROGRAMFILESx86=${env:PROGRAMFILES(x86)}
$TestExecute=$PROGRAMFILESx86+'\SmartBear\TestExecute 9\Bin\TestExecute.exe'

            $cmdLine=$TestExecute
			$parameter="\\Z400BER02\Shared\RunFromCMD\RunTestFromCMD\RunTestFromCMD.pjs  /r /e"
			$executable=Run-LocalProcess "$cmdLine" "$parameter"

IF ($executable -eq 0){
    Echo "     Succeded  exitCode:$executable"
}ElSE{
	Echo "     [ERROR]:Failure "
	Echo "     Exited with ExitCode $executable"
	Echo "     $cmdLine $parameter"
				
}

start-sleep 120