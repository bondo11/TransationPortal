using System;
using System.Net;

using Newtonsoft.Json;

using translate_spa.Models;

namespace translate_spa.Clients
{
    public class SlackClient
    {
        public static readonly Uri DefaultWebHookUri = new Uri(Startup.Configuration.GetSection("Slack")["Uri"]);

        readonly SlackMessage _message;
        public SlackClient(SlackMessage message)
        {
            _message = message;
        }

        public void Send()
        {
            using(var webClient = new WebClient())
            {
                webClient.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
                var request = System.Text.Encoding.UTF8.GetBytes("payload=" + JsonConvert.SerializeObject(_message));
                var response = webClient.UploadData(DefaultWebHookUri, "POST", request);
            }
        }
    }
}