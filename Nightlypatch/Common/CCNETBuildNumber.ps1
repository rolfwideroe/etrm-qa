                                                                        

 If( Test-Path "C:\Elviz\CCNET\Tools\CCNETVersion.txt") {
 	$BuildNumber= Get-Content "C:\Elviz\CCNET\Tools\CCNETVersion.txt"
}else{
 	$BuildNumber=" Contract clearer is not patched correctly"
}
 return $BuildNumber