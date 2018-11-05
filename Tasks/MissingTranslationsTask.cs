using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Extensions.Configuration;

using MimeKit;

using Newtonsoft.Json;

using NLog;

using translate_spa.Clients;
using translate_spa.Models;
using translate_spa.Models.ResponseModels;

namespace translate_spa.Tasks
{
	public class MissingTranslationsTask
	{
		readonly IEnumerable<Translation> _translations;
		readonly ILogger _log;
		readonly IEnumerable<TranslationsEnvironment> _environments;
		readonly string _notificationUrl = Startup.Configuration.GetSection("NotificationSettings:EnvironmentLink").Get<string>();
		readonly string _senderName = Startup.Configuration.GetSection("EmailConfig:SenderName").Get<string>();
		readonly bool _enableEmail = Startup.Configuration.GetSection("NotificationSettings:EnableEmail").Get<bool>();
		readonly bool _enableSlack = Startup.Configuration.GetSection("NotificationSettings:EnableSlack").Get<bool>();

		public MissingTranslationsTask(IEnumerable<Translation> translations, ILogger log)
		{
			_translations = translations;
			_log = log;
			_environments = GetEnvironments();
		}

		public async Task ExecuteAsync()
		{
			var message = GetMessage();

			if (_enableSlack)
			{
				new SlackClient(MessageString()).Send();
			}
			if (_enableEmail)
			{
				new MailClient(MailMessage(message), _log).Send();
			}
		}

		SlackMessage MessageString()
		{
			var slackMessage = new SlackMessage()
			{
				UserName = _senderName,
					Attachments = GetSlackAttachments().ToList(),
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
			var recipients = GetRecipients();

			foreach (var recipient in recipients)
			{
				mailMessage.To.Add(recipient);
			}

			mailMessage.Subject = "Oversættelser mangler";
			var body = new TextPart("plain")
			{
				Text = text,
			};

			// now create the multipart/mixed container to hold the message text and the
			// image attachment
			var multipart = new Multipart("mixed");
			multipart.Add(body);
			var mailAttachments = GetMailAttachments();

			foreach (var attachment in mailAttachments)
			{
				multipart.Add(attachment);
			}

			// now set the multipart/mixed as the message body
			mailMessage.Body = multipart;

			return mailMessage;
		}

		string GetMessage()
		{
			var sb = new StringBuilder();
			foreach (var env in _environments)
			{
				var envTranslations = _translations.Where(x => x.Environment == env);
				sb.Append($"I {env.ToString()} mangler der følgende antal oversættelser:");

				var languages = Enum.GetValues(typeof(Language))
					.Cast<Language>();

				foreach (var lang in languages)
				{
					sb.Append($"\n>{lang.ToString()}: {envTranslations.Count(x => string.IsNullOrEmpty(x.GetByLanguage(lang)))}");
				}

				sb.Append($"\n\n\n");
			}

			return sb.ToString();
		}

		IEnumerable<MimePart> GetMailAttachments()
		{
			var mailAttachments = new List<MimePart>();

			mailAttachments.Add(GetMailAttachment(_translations, $"missing_translations_all.json"));

			foreach (var env in _environments)
			{
				var envTranslations = _translations.Where(x => x.Environment == env);

				mailAttachments.Add(GetMailAttachment(envTranslations, $"missing_translations_{env.ToString().ToLower()}.json"));
			}

			return mailAttachments;
		}

		MimePart GetMailAttachment(IEnumerable<Translation> translations, string filename)
		{
			var translationsContent = JsonConvert.SerializeObject(translations.Select(x => new ResponseTranslation()
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
					FileName = Path.GetFileName(filename),
			};
			return attachment;
		}

		IEnumerable<MailboxAddress> GetRecipients()
		{
			var recipients = Startup.Configuration.GetSection("EmailConfig:Receivers").Get<List<Recipient>>();

			return recipients.Select(x => new MailboxAddress(x.Name, x.Email));
		}

		IEnumerable<SlackAttachment> GetSlackAttachments()
		{
			var attachments = new List<SlackAttachment>();

			foreach (var env in _environments)
			{
				var envTranslations = _translations.Where(x => x.Environment == env);
				if (!envTranslations.Any())
				{
					continue;
				}
				var languages = Enum.GetValues(typeof(Language))
					.Cast<Language>();

				/* if (!languages.Any(lang => envTranslations.Count(x => string.IsNullOrEmpty(x.GetByLanguage(lang))) > 0))
				{
					continue;
				} */

				var attachment = new SlackAttachment()
				{
					Fallback = string.Format("Der mangler '{0}' oversættelser til {1}", envTranslations.Count(), env.ToString()),
						Color = "",
						Pretext = string.Format("Der mangler '{0}' oversættelser til {1}", envTranslations.Count(), env.ToString()),
						AuthorName = env.ToString(),
						AuthorIcon = "https://www.shareicon.net/download/2016/11/22/854967_logo_512x512.png",
						Title = $"{env.ToString()} mangler {envTranslations.Count()} oversættelser",
						TitleLink = string.Format(_notificationUrl, env.ToString().ToLower()),
						Text = "Optælling på manglende oversættelser opdelt i sprog:",
						Fields = languages.Select(lang => new Field()
						{
							Title = lang.ToString(),
								Value = envTranslations.Count(x => string.IsNullOrEmpty(x.GetByLanguage(lang))).ToString(),
								Short = false,
						}).ToList(),

						Footer = "Translations API",
						FooterIcon = "https://www.shareicon.net/download/2016/11/22/854967_logo_512x512.png",
						TimeString = GetEpochTime(),
				};
				attachments.Add(attachment);
			}

			return attachments;
		}

		string GetEpochTime()
		{
			var utcDate = DateTime.Now.ToUniversalTime();
			var baseTicks = 621355968000000000;
			var tickResolution = 10000000;
			var epoch = (utcDate.Ticks - baseTicks) / tickResolution;
			var epochTicks = (epoch * tickResolution) + baseTicks;
			var date = new DateTime(epochTicks, DateTimeKind.Utc);
			return epoch.ToString();
		}
		IEnumerable<TranslationsEnvironment> GetEnvironments()
		{
			var enabledEnvironments = new List<TranslationsEnvironment>();
			var environments = Startup.Configuration.GetSection("NotificationSettings:Environments").Get<string>().Split(',').ToList();
			foreach (var env in environments)
			{
				var couldParse = Enum.TryParse(env, true, out TranslationsEnvironment parsed);
				if (couldParse)
				{
					enabledEnvironments.Add(parsed);
				}
			}
			return enabledEnvironments;
		}
	}
}