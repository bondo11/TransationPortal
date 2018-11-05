/* using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.NodeServices;
using Microsoft.AspNetCore.Razor.Language;
using Microsoft.Extensions.Logging;

using RazorLight;

using translate_spa.ErrorHandling;
using translate_spa.Logger;
using translate_spa.Models;
using translate_spa.Models.ViewModels;

namespace BaseupWeb.Tasks
{
	public class MakePdf
	{
		private static readonly string CurrentDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
		private readonly ILogger _logger;
		private readonly User _user;
		private readonly INodeServices _nodeServices;
		public MakePdf(INodeServices nodeServices, User user)
		{
			_nodeServices = nodeServices;
			_logger = new CustomLogger().Execute();
			_user = user;
		}

		public async Task Execute()
		{
			var pdfData = await _nodeServices.InvokeAsync<byte[]>("./pdf", await UserPdfContentAsync());

			var pdf = new MemoryStream(pdfData);
			var relativePath = Path.Combine("ProfilePdfs", _user.Id);
			var completePath = Path.Combine(CurrentDir, relativePath);
			if (!Directory.Exists(completePath))
			{
				_logger.LogDebug($"Creating directory {completePath}");
				Directory.CreateDirectory(completePath);
			}

			relativePath = Path.Combine(relativePath, "cv.pdf");
			completePath = Path.Combine(CurrentDir, relativePath);
			using(var stream = new FileStream(completePath, FileMode.Create))
			{
				pdf.CopyTo(stream);
			}
		}

		private async Task<string> GenerateAsync(string url)
		{
			var hc = new HttpClient();
			var htmlContent = await hc.GetStringAsync(url);
			return htmlContent;
		}

		private async Task<HtmlToPdfDocument> GenerateAsync()
		{
			var doc = new HtmlToPdfDocument()
			{
				GlobalSettings = {
						ColorMode = ColorMode.Color,
						Orientation = Orientation.Landscape,
						PaperSize = PaperKind.A4Plus,
					},
					Objects = {
						new ObjectSettings()
						{
							PagesCount = true,
								HtmlContent = await GenerateAsync("https://bold.dk"),
								WebSettings = { DefaultEncoding = "utf-8" },
								HeaderSettings = { FontSize = 9, Right = "Page [page] of [toPage]", Line = true, Spacing = 2.812 }
						}
					}
			};
			return doc;
		}

		async Task<string> UserPdfContentAsync()
		{
			var engine = new RazorLightEngineBuilder()
				.UseFilesystemProject(Path.Combine(Directory.GetCurrentDirectory(), "Views", "Pdf"))
				.UseMemoryCachingProvider()
				.Build();
			var pdfModel = new PdfUserViewModel(_user);
			var template = Path.Combine(Directory.GetCurrentDirectory(), "Views", "Pdf", "user.cshtml");

			try
			{
				var result = await engine.CompileRenderAsync(template, pdfModel);
				return result;
			}
			catch (Exception exception)
			{
				new CustomException(CustomExceptionType.PDFGeneration, "could not create user html", exception);
				return "";
			}
		}
	}
} */