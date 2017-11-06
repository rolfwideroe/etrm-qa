$smtpServer = “smtp.songnetworks.no”
$msg = new-object Net.Mail.MailMessage
$smtp = new-object Net.Mail.SmtpClient($smtpServer)
$msg.From = "qainstall@bradyplc.com"
$msg.To.Add([string]::join(",", $args[1]))
$msg.Subject = "ERROR:Nightly integration tests failed"
$msg.isBodyhtml = $true
$msg.Priority = [System.Net.Mail.MailPriority]::High
$msg.Body = "
<html><body>
<h1>Integration tests failed</h1> 
See <a href='http://BERVS-TFS05:8080/tfs/web/UI/Pages/Build/Details.aspx?builduri="+ $args[0] +"'>build history</a> for details.
</body></html>
"
$smtp.Send($msg)