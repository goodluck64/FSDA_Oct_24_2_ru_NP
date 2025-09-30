using System.Text;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Gmail.v1;
using Google.Apis.Gmail.v1.Data;
using Google.Apis.Services;

var myMail = "xrlogic1@gmail.com";
using var fileStream =
	File.OpenRead("secret.json");

var googleClientSecrets = await GoogleClientSecrets.FromStreamAsync(fileStream);
var flow = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
{
	ClientSecrets = googleClientSecrets.Secrets,
	Scopes = [GmailService.Scope.GmailReadonly]
});

var codeReceiver = new LocalServerCodeReceiver();
var app = new AuthorizationCodeInstalledApp(flow, codeReceiver);
var credentials = await app.AuthorizeAsync(myMail, CancellationToken.None);

if (app.ShouldRequestAuthorizationCode(credentials.Token))
{
	await credentials.RefreshTokenAsync(CancellationToken.None);
}

using var gmailService = new GmailService(new BaseClientService.Initializer
{
	HttpClientInitializer = credentials,
	ApplicationName = "MailApp",
});


var labelsRequest = gmailService.Users.Labels.List("me");
var labelsResponse = await labelsRequest.ExecuteAsync();
var inboxLabel = labelsResponse.Labels.Single(l => l.Name == "INBOX");
var request = gmailService.Users.Messages.List("me");
var messagesResponse = await request.ExecuteAsync();

Console.WriteLine(inboxLabel.MessagesTotal);

if (messagesResponse.Messages is
    {
	    Count: 0
    })
{
	Console.WriteLine("No messages.");
}
else
{
	foreach (var message in messagesResponse.Messages.Take(3))
	{
		Console.WriteLine("----------------------");
		var messageWrapper = await gmailService.Users.Messages.Get("me", message.Id).ExecuteAsync();

		if (messageWrapper is not null)
		{
			Console.WriteLine(messageWrapper.Payload.Body.ToString());
		}
		
	}
}