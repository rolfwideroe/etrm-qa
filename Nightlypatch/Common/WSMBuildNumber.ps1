
$reg = [Microsoft.Win32.RegistryKey]::OpenRemoteBaseKey('LocalMachine', $MachineName)         
$regKey= $reg.OpenSubKey("SOFTWARE\\Wow6432Node\\Microsoft\\Windows\\CurrentVersion\\Uninstall\\{2BDDACBE-5326-458F-88B6-EBD7FBBD987D}")           
$BuildNumber=$regkey.GetValue("DisplayVersion")
return $BuildNumber
start-sleep 10
