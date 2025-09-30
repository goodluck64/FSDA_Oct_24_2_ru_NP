using System.Net;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using MimeKit.Text;

using var smtpClient = new SmtpClient();

smtpClient.MessageSent += (o, eventArgs) =>
{
    Console.WriteLine($"Message sent. Response: {eventArgs.Response}");
};

var sender = new MailboxAddress("Alex", "skiba_al@itstep.edu.az");

await smtpClient.ConnectAsync("smtp-mail.outlook.com", 587, SecureSocketOptions.StartTls);

await smtpClient.AuthenticateAsync(new NetworkCredential
{
    UserName = sender.Address,
    Password = File.ReadAllText("password")
});

var message = new MimeMessage();

message.To.Add(new MailboxAddress(string.Empty, "sadir_fe29@itstep.edu.az"));
message.To.Add(new MailboxAddress(string.Empty, "abbas_fr65@itstep.edu.az"));
message.From.Add(sender);

var textPart = new TextPart(TextFormat.Plain);

textPart.Text = "Hello from FSDA_Oct_24_2_ru";

message.Sender = sender;
message.Body = textPart;
message.Subject = "SMTP Test";

await smtpClient.SendAsync(message);