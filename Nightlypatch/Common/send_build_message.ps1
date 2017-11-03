$smtpServer = “smtp.songnetworks.no”
$msg = new-object Net.Mail.MailMessage
$smtp = new-object Net.Mail.SmtpClient($smtpServer)
$msg.From = "qainstall@bradyplc.com"
$msg.To.Add([string]::join(",", $args[1]))
$msg.Subject = $args[2]
$msg.isBodyhtml = $true
$msg.Body = "
<html><body>
See <a href='http://BERVS-TFS05:8080/tfs/web/UI/Pages/Build/Details.aspx?builduri="+ $args[0] +"'>build history</a> for details.
</body></html>
"
$smtp.Send($msg)