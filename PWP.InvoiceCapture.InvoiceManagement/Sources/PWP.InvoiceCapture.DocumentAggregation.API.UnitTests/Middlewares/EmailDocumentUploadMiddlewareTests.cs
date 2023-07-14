using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PWP.InvoiceCapture.Core.Telemetry;
using PWP.InvoiceCapture.DocumentAggregation.API.Middlewares;
using PWP.InvoiceCapture.DocumentAggregation.Business.Contract.Models;
using PWP.InvoiceCapture.DocumentAggregation.Business.Contract.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.DocumentAggregation.API.UnitTests.Controllers
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class EmailDocumentUploadMiddlewareTests
    {
        [TestInitialize]
        public void Initialize()
        {
            mockRepository = new MockRepository(MockBehavior.Strict);
            invoiceDocumentServiceMock = mockRepository.Create<IInvoiceDocumentService>();
            emailServiceMock = mockRepository.Create<IEmailService>();
            telemetryClientMock = mockRepository.Create<ITelemetryClient>();
            next = (HttpContext context) => Task.CompletedTask;
            target = new EmailDocumentUploadMiddleware(next, invoiceDocumentServiceMock.Object, emailServiceMock.Object, telemetryClientMock.Object);
        }

        [TestCleanup]
        public void Cleanup()
        {
            mockRepository.VerifyAll();
        }

        [TestMethod]
        public void Instance_WhenNextDelegateIsNull_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new EmailDocumentUploadMiddleware(null, invoiceDocumentServiceMock.Object, emailServiceMock.Object, telemetryClientMock.Object));
        }

        [TestMethod]
        public void Instance_WhenInvoiceDocumentServiceIsNull_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new EmailDocumentUploadMiddleware(next, null, emailServiceMock.Object, telemetryClientMock.Object));
        }

        [TestMethod]
        public void Instance_WhenEmailServiceIsNull_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new EmailDocumentUploadMiddleware(next, invoiceDocumentServiceMock.Object, null, telemetryClientMock.Object));
        }

        [TestMethod]
        public void Instance_WhenTelemetryClientIsNull_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new EmailDocumentUploadMiddleware(next, invoiceDocumentServiceMock.Object, emailServiceMock.Object, null));
        }

        [TestMethod]
        [DataRow("GET")]
        [DataRow("OPTIONS")]
        [DataRow("PUT")]
        [DataRow("DELETE")]
        [DataRow("HEAD")]
        [DataRow("TRACE")]
        public async Task InvokeAsyncAsync_WhenNotSupportedRequest_ShouldReturnOK(string method)
        {
            var context = CreateContext(method);

            await target.InvokeAsync(context);

            Assert.IsTrue(context.Response.StatusCode == StatusCodes.Status200OK);
        }

        [TestMethod]
        public async Task InvokeAsyncAsync_WhenHttpPostRequestWIthWrongFiles_ShouldReturnOK()
        {
            emailServiceMock
                .Setup(emailService => emailService.FindEmail(toEmail))
                .Returns(toEmail);

            emailServiceMock
                .Setup(emailService => emailService.FindEmail(fromEmail))
                .Returns(fromEmail);

            var context = CreateContextWithWrongFile();

            await target.InvokeAsync(context);

            Assert.IsTrue(context.Response.StatusCode == StatusCodes.Status200OK);
        }

        [TestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public async Task InvokeAsyncAsync_WhenHttpPostRequest_ShouldReturnOK(bool withContentAttachments)
        {
            emailServiceMock
                .Setup(emailService => emailService.FindEmail(toEmail))
                .Returns(toEmail);

            emailServiceMock
                .Setup(emailService => emailService.FindEmail(fromEmail))
                .Returns(fromEmail);

            invoiceDocumentServiceMock
                .Setup(documentService => documentService.SaveEmailDocumentAsync(toEmail, fromEmail, fileName1, It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new UploadedDocument());

            invoiceDocumentServiceMock
                .Setup(documentService => documentService.SaveEmailDocumentAsync(toEmail, fromEmail, fileName2, It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new UploadedDocument());

            var context = withContentAttachments ? CreateContextWithContentAttachments(HttpMethod.Post.Method) : CreateContext(HttpMethod.Post.Method);

            await target.InvokeAsync(context);

            Assert.IsTrue(context.Response.StatusCode == StatusCodes.Status200OK);
        }

        private HttpContext CreateContext(string httpRequestMethod)
        {
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers.Add("Content-Type", "multipart/form-data");
            httpContext.Request.Method = httpRequestMethod;

            var file1 = new FormFile(new MemoryStream(Encoding.UTF8.GetBytes(fileName1)), 0, 0, fileName1, fileName1);
            var file2 = new FormFile(new MemoryStream(Encoding.UTF8.GetBytes(fileName2)), 0, 0, fileName2, fileName2);

            httpContext.Request.Form = new FormCollection(
                new Dictionary<string, StringValues>() 
                {
                    { "to", new StringValues(toEmail) },
                    { "from", new StringValues(fromEmail) }
                }, 
                new FormFileCollection { file1, file2 }
            );
            
            return httpContext;
        }

        private HttpContext CreateContextWithContentAttachments(string httpRequestMethod)
        {
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers.Add("Content-Type", "multipart/form-data");
            httpContext.Request.Method = httpRequestMethod;

            var file1 = new FormFile(new MemoryStream(Encoding.UTF8.GetBytes(fileName1)), 0, 0, fileName1, fileName1);
            var file2 = new FormFile(new MemoryStream(Encoding.UTF8.GetBytes(fileName2)), 0, 0, fileName2, fileName2);
            var file3 = new FormFile(new MemoryStream(Encoding.UTF8.GetBytes(fileName2)), 0, 0, fileNameContent1, fileNameContent1);
            var file4 = new FormFile(new MemoryStream(Encoding.UTF8.GetBytes(fileName2)), 0, 0, fileNameContent2, fileNameContent2);

            httpContext.Request.Form = new FormCollection(
                new Dictionary<string, StringValues>()
                {
                    { "to", new StringValues(toEmail) },
                    { "from", new StringValues(fromEmail) },
                    { "content-ids", new StringValues($"{{\"34854934-019c-4f67-88eb-922545d601dd\":\"{fileNameContent1}\",\"94b2ccfd-05c4-4368-bf8a-8fac46b2ef7a\":\"{fileNameContent2}\"}}") }
                },
                new FormFileCollection { file1, file2, file3, file4 }
            );

            return httpContext;
        }

        private HttpContext CreateContextWithWrongFile()
        {
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers.Add("Content-Type", "multipart/form-data");
            httpContext.Request.Method = HttpMethod.Post.Method;

            var file1 = new FormFile(new MemoryStream(Encoding.UTF8.GetBytes(wrongFileName)), 0, 0, wrongFileName, wrongFileName);

            httpContext.Request.Form = new FormCollection(
                new Dictionary<string, StringValues>()
                {
                    { "to", new StringValues(toEmail) },
                    { "from", new StringValues(fromEmail) }
                },
                new FormFileCollection { file1 }
            );

            return httpContext;
        }

        private Mock<IInvoiceDocumentService> invoiceDocumentServiceMock;
        private Mock<IEmailService> emailServiceMock;
        private Mock<ITelemetryClient> telemetryClientMock;
        private EmailDocumentUploadMiddleware target;
        private MockRepository mockRepository;
        private RequestDelegate next;

        private const string toEmail = "toEmail@email.com";
        private const string fromEmail = "fromEmail@email.com";
        private const string fileName1 = "PdfSample.pdf";
        private const string fileName2 = "0.png";
        private const string fileNameContent1 = "content1";
        private const string fileNameContent2 = "content2";
        private const string wrongFileName = "text.txt";
    }
}
