using MailKit.Net.Smtp;

using Microsoft.Extensions.Configuration;

using MimeKit;

namespace translate_spa.Clients
{
    public class MailClient
    {
        private readonly string _url = Startup.Configuration.GetSection("EmailConfig")["ServerAddress"];
        private readonly string _username = Startup.Configuration.GetSection("EmailConfig")["UserName"];
        private readonly string _password = Startup.Configuration.GetSection("EmailConfig")["Password"];
        private readonly int _port = Startup.Configuration.GetSection("EmailConfig:Port").Get<int>();
        private readonly MimeMessage _message;

        public MailClient(MimeMessage message)
        {
            _message = message;
        }

        public void Send()
        {
            using (var client = new SmtpClient())
            {
                // For demo-purposes, accept all SSL certificates (in case the server supports STARTTLS)
                client.ServerCertificateValidationCallback = (s, c, h, e) => true;

                client.Connect(_url, _port, false);

                // Note: only needed if the SMTP server requires authentication
                client.Authenticate(_username, _password);

                client.Send(_message);
                client.Disconnect(true);
            }
        }
    }
}