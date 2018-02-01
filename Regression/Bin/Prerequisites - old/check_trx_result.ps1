
$filePath = $args[0]
[xml] $res = [xml](get-content $filePath)




if (($res.TestRun.ResultSummary.Counters.error -eq 0) -and ($res.TestRun.ResultSummary.Counters.failed -eq 0) -and ($res.TestRun.ResultSummary.Counters.inconclusive -eq 0)) 
{
	'All tests passed'
}
else
{
    'Failed'

}