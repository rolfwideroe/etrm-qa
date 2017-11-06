$smtpServer = “smtp.songnetworks.no”
$msg = new-object Net.Mail.MailMessage
$smtp = new-object Net.Mail.SmtpClient($smtpServer)
$msg.From = "qainstall@bradyplc.com"
$msg.To.Add([string]::join(",", $args[1]))
$msg.Subject = "WARNING:Nightly integration tests have not been run"
$msg.isBodyhtml = $true
$msg.Priority = [System.Net.Mail.MailPriority]::High
$msg.Body = "
<html><body>
<h1>Integration tests have not been run</h1> 
See <a href='http://BERVS-TFS05:8080/tfs/web/UI/Pages/Build/Details.aspx?builduri="+ $args[0] +"'>build history</a> for details.
</body></html>
"
$smtp.Send($msg)