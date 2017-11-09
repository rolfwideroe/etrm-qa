
Workflow TestWorkFlow
{   
    param ($scriptPath) 

  
    parallel
    {

        InlineScript
        {
             $executingScriptDirectory = $Using:scriptPath
            & "$executingScriptDirectory\Prerequisites\NUnit\bin\nunit-console.exe" "$executingScriptDirectory\EcmBatchCustomConfig\TestEcmCustomConfig.dll" "/xml:$executingScriptDirectory\EcmBatchCustomConfig\TestResult.xml" "/exclude:DwhExportOnly"
        }
        InlineScript
        {
            $executingScriptDirectory = $Using:scriptPath
             echo $scriptPath        
            & "$executingScriptDirectory\Prerequisites\NUnit\bin\nunit-console.exe" "$executingScriptDirectory\ErmBatchCustomConfig\TestErmCustomConfig.dll" "/xml:$executingScriptDirectory\ErmBatchCustomConfig\TestResult.xml" "/exclude:ERM_New,DwhExportOnly"     
        }
    }
}

$scriptPath = split-path $MyInvocation.MyCommand.Definition -Parent 


TestWorkflow $scriptPath