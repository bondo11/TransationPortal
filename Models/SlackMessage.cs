using System.Collections.Generic;

using Newtonsoft.Json;

namespace translate_spa.Models
{
    public class SlackMessage
    {
        public SlackMessage()
        {
            this.Attachments = new List<SlackAttachment>();
        }

        [JsonProperty("attachments")]
        public List<SlackAttachment> Attachments { get; set; }

        [JsonProperty("channel")]
        public string Channel { get; set; }

        [JsonProperty("username")]
        public string UserName { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("icon_emoji")]
        public string Icon
        {
            get { return ":computer:"; }
        }

    }
}