$filePath = $args[0]

[xml]$res = get-content $filePath
if (($res['test-results'].errors -eq 0) -and ($res['test-results'].failures -eq 0) -and ($res['test-results'].invalid -eq 0)) 
{
	'All tests passed'
}
else
{
	'Failed'
}