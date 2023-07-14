using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using PWP.InvoiceCapture.Core.Telemetry;
using PWP.InvoiceCapture.Core.Utilities;
using PWP.InvoiceCapture.DocumentAggregation.API.Constants;
using PWP.InvoiceCapture.DocumentAggregation.Business.Contract.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.DocumentAggregation.API.Middlewares
{
    public class EmailDocumentUploadMiddleware
    {
        public EmailDocumentUploadMiddleware(RequestDelegate next, IInvoiceDocumentService invoiceDocumentService, IEmailService emailService, ITelemetryClient telemetryClient)
        {
            Guard.IsNotNull(next, nameof(next));
            Guard.IsNotNull(invoiceDocumentService, nameof(invoiceDocumentService));
            Guard.IsNotNull(emailService, nameof(emailService));
            Guard.IsNotNull(telemetryClient, nameof(telemetryClient));

            this.invoiceDocumentService = invoiceDocumentService;
            this.emailService = emailService;
            this.telemetryClient = telemetryClient;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                if (context.Request.Method == HttpMethod.Post.Method)
                {
                    var form = await context.Request.ReadFormAsync();

                    var to = GetTo(form);
                    var from = GetFrom(form);
                    var attachments = GetAttachedFiles(form);

                    context.Response.StatusCode = await TryUploadDocumentAsync(from, to, attachments);
                }
                else
                {
                    context.Response.StatusCode = StatusCodes.Status200OK;
                }
            }
            catch (Exception exception)
            {
                //Status code 5xx will execute retry for web hook.
                context.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
                telemetryClient.TrackException(exception);
            }
        }

        private IEnumerable<IFormFile> GetAttachedFiles(IFormCollection formCollection)
        {
            var emailContentAttachments = formCollection.FirstOrDefault(form => form.Key == contentIds);
            if (string.IsNullOrWhiteSpace(emailContentAttachments.Value))
            {
                return formCollection.Files;
            }

            var contentAttachments = JsonConvert.DeserializeObject<Dictionary<string, string>>(emailContentAttachments.Value);

            var contentAttachmentNames = contentAttachments.Select(content => content.Value).ToList();

            return formCollection.Files.Where(file => !contentAttachmentNames.Any(name => name == file.Name));
        }

        private async Task<int> TryUploadDocumentAsync(string from, string to, IEnumerable<IFormFile> files)
        {
            if (string.IsNullOrWhiteSpace(to) || string.IsNullOrWhiteSpace(from) || files == null)
            {
                //TODO: We can reply to sender to notify about unsupported form files or other issues.
                return StatusCodes.Status200OK;
            }

            var validFiles = files.Where(file => FileIsValid(file));

            if (!validFiles.Any())
            {
                //TODO: We can reply to sender to notify about unsupported form files or other issues.
                return StatusCodes.Status200OK;
            }

            foreach (var file in validFiles)
            {
                using (var fileStream = file.OpenReadStream())
                {
                    await invoiceDocumentService.SaveEmailDocumentAsync(to, from, file.FileName, fileStream, CancellationToken.None);
                }
            }

            return StatusCodes.Status200OK;
        }

        private string GetTo(IFormCollection form)
        {
            form.TryGetValue("to", out StringValues value);

            var to = value.FirstOrDefault();

            to = emailService.FindEmail(to);

            return to;
        }

        private string GetFrom(IFormCollection form)
        {
            form.TryGetValue("from", out StringValues value);

            var from = value.FirstOrDefault();

            from = emailService.FindEmail(from);

            return from;
        }

        private bool FileIsValid(IFormFile file)
        {
            if (file == null)
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(file.FileName))
            {
                return false;
            }

            if (!FileExtensions.AllowedExtensions.Contains(Path.GetExtension(file.FileName).ToLower()))
            {
                return false;
            }

            return true;
        }

        private readonly IInvoiceDocumentService invoiceDocumentService;
        private readonly IEmailService emailService;
        private readonly ITelemetryClient telemetryClient;

        private const string contentIds = "content-ids";
    }
}
