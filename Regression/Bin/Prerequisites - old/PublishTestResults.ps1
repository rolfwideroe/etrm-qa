
$publish = $args[0]
$publishbuild =  $args[1]
$publishresultsfile = $args[2]
$teamproject  = $args[3]
$platform = 'Any CPU'
$flavor = 'Release'

#$publish = "http://bervs-tfs05:8080/tfs/bradyenergy"
#$publishbuild =  "Internal_Regression_2015.1_v2015.1.1.134_.1"
#$publishresultsfile = "C:\Builds\Elviz\Internal_Regression_2015.1\bin\TestWCFCurveService\TestResults.trx"
#$teamproject  = "Elviz"
#$platform = 'Any CPU'
#$flavor = "Release"


$msTest='C:\Program Files (x86)\Microsoft Visual Studio 12.0\Common7\IDE\MSTest.exe'


if(test-path $publishresultsfile)
{
$s=& $msTest /publish:$publish /publishbuild:$publishbuild /publishresultsfile:$publishresultsfile /teamproject:$teamproject /platform:$platform /flavor:$flavor |Out-String 


    if ($s.Contains('Publish completed successfully') )
    {
	    'OK'
    }
    else
    {
	    $s
    }

}
else
{
'Trx file was not avaialble'
}



