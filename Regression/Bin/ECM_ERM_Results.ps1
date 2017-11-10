param
(
  [string] $testname
)

$scriptPath = split-path $MyInvocation.MyCommand.Definition -Parent
#$filePath = "C:\TFS\Development\QA\Regression\Bin\TestResult.xml"

$filePath = $scriptPath + "\" + $testname + "\TestResult.xml"
[xml] $res = [xml](get-content $filePath)

$failed= $res.'test-results'.GetAttribute("failures")
$errors = $res.'test-results'.GetAttribute("errors")
$inconclusive = $res.'test-results'.GetAttribute("inconclusive")


echo $scriptPath

if (($failed -eq 0) -and ($errors -eq 0) -and ($inconclusive -eq 0)) 
{
     Echo "All tests passed"   
     $ExitCode =  0        
}
else
{     
   Write-Error -Message ("Test finished with errors. Failed= $failed. Errors= $errors")   
   $ExitCode = 1
}
return $ExitCode