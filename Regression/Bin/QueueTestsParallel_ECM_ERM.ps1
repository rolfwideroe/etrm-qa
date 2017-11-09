
Workflow TestWorkFlow
{   
    param ($scriptPath) 

  
    parallel
    {

        InlineScript
        {
              $executingScriptDirectory = $Using:scriptPath
            & "$executingScriptDirectory\Prerequisites\NUnit\bin\nunit-console.exe" "$executingScriptDirectory\EcmBatch\TestEcmBatch.dll" "/xml:$executingScriptDirectory\EcmBatch\TestResult.xml" "/exclude:DwhExportOnly"
		}
        InlineScript
        {
              $executingScriptDirectory = $Using:scriptPath       
            & "$executingScriptDirectory\Prerequisites\NUnit\bin\nunit-console.exe" "$executingScriptDirectory\ErmBatch\TestErmBatch.dll" "/xml:$executingScriptDirectory\ErmBatch\TestResult.xml" "/exclude:ERM_New,DwhExportOnly"     

		}
		        InlineScript
        {
              $executingScriptDirectory = $Using:scriptPath       
            & "$executingScriptDirectory\Prerequisites\NUnit\bin\nunit-console.exe" "$executingScriptDirectory\TestReportingDB\TestReportingDB.dll" "/xml:$executingScriptDirectory\TestReportingDB\TestResult.xml"    

		}
    }
}

$scriptPath = split-path $MyInvocation.MyCommand.Definition -Parent 


TestWorkflow $scriptPath

