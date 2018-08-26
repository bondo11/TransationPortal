using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using MimeKit;

using Newtonsoft.Json;

using NLog;

using translate_spa.Clients;
using translate_spa.Models;
using translate_spa.Models.ResponseModels;
using translate_spa.MongoDB.DbBuilder;
using translate_spa.Repositories;
using translate_spa.Tasks;

namespace translate_spa.Tasks
{
	public class MissingTranslationsTask
	{
		readonly IEnumerable<Translation> _translations;
		readonly ILogger _log;
		public MissingTranslationsTask(IEnumerable<Translation> translations, ILogger log)
		{
			_translations = translations;
			_log = log;
		}

		public async Task ExecuteAsync()
		{
			var message = GetMessage();
			new SlackClient(MessageString(message)).Send();
			new MailClient(MailMessage(message), _log).Send();
		}

		SlackMessage MessageString(string text)
		{
			var slackMessage = new SlackMessage()
			{
				Text = text,
			};

			return slackMessage;
		}
		MimeMessage MailMessage(string text)
		{
			var mailMessage = new MimeMessage()
			{
				Sender = new MailboxAddress("TranslationsPortal", "translate@bon.do")
			};

			mailMessage.From.Add(new MailboxAddress("TranslationsPortal", "translate@bon.do"));
			mailMessage.To.Add(new MailboxAddress("Morten Bondo", "mbh@esignatur.dk"));
			mailMessage.To.Add(new MailboxAddress("Jacob Rosca", "jhr@esignatur.dk"));
			mailMessage.Subject = "Oversættelser mangler";
			var body = new TextPart("plain")
			{
				Text = text,
			};

			var translationsContent = JsonConvert.SerializeObject(_translations.Select(x => new ResponseTranslation()
			{
				KEY = x.Key,
					DA = x.Da,
					EN = x.En,
					SV = x.Sv,
					NB = x.Nb,
			}));
			var byteArray = Encoding.UTF8.GetBytes(translationsContent);

			// create an image attachment for the file located at path
			var attachment = new MimePart("text", "json")
			{
				Content = new MimeContent(new MemoryStream(byteArray), ContentEncoding.Default),
					ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
					ContentTransferEncoding = ContentEncoding.Base64,
					FileName = Path.GetFileName("missing_translations.json")
			};

			// now create the multipart/mixed container to hold the message text and the
			// image attachment
			var multipart = new Multipart("mixed");
			multipart.Add(body);
			multipart.Add(attachment);

			// now set the multipart/mixed as the message body
			mailMessage.Body = multipart;

			return mailMessage;
		}

		string GetMessage()
		{
			var sb = new StringBuilder();
			sb.Append("Der mangler følgende oversættelser:");
			sb.Append($"\n>Da: {_translations.Count(x => string.IsNullOrEmpty(x.Da))}");
			sb.Append($"\n>En: {_translations.Count(x => string.IsNullOrEmpty(x.En))}");
			sb.Append($"\n>Sv: {_translations.Count(x => string.IsNullOrEmpty(x.Sv))}");
			sb.Append($"\n>Nb: {_translations.Count(x => string.IsNullOrEmpty(x.Nb))}");

			return sb.ToString();
		}
	}
}