
$filePath = $args[0]
[xml] $res = [xml](get-content $filePath)


$res.TestRun.name="Test"

$res.Save($filePath)

