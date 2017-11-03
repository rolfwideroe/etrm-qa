                                                                        
 #$reg = [Microsoft.Win32.RegistryKey]::OpenRemoteBaseKey('LocalMachine', $MachineName)         
 #$regKey= $reg.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Uninstall\\{F03EAA70-F8A9-4BA2-8287-A7BF71DD5E9C}")           
 #$BuildNumber=$regkey.GetValue("DisplayVersion")
 
 If( Test-Path "C:\ElvizClient\Tools\ElvizVersion.txt") {
 	$BuildNumber= Get-Content "C:\ElvizClient\Tools\ElvizVersion.txt"
}else{
 	$BuildNumber=" Elviz Client is not patched correctly"
}
 return $BuildNumber