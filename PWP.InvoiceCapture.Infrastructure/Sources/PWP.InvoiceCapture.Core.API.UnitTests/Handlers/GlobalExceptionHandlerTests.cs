using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PWP.InvoiceCapture.Core.API.Handlers;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.Core.API.UnitTests.Handlers
{
    [ExcludeFromCodeCoverage]
    [TestClass]
    public class GlobalExceptionHandlerTests
    {
        public GlobalExceptionHandlerTests()
        {
            target = new GlobalExceptionHandler();
        }

        [TestMethod]
        public void InvokeAsync_WhenContextIsNull_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsExceptionAsync<ArgumentNullException>(() => target.InvokeAsync(null));
        }

        [TestMethod]
        public async Task InvokeAsync_WhenExceptionIsOperationCanceledException_ShouldContainNoContentStatusCode()
        {
            var context = CreateHttpContext(new OperationCanceledException());

            await target.InvokeAsync(context);

            Assert.AreEqual(context.Response.StatusCode, (int)HttpStatusCode.NoContent);
        }

        [TestMethod]
        public async Task Invoke_WhenExceptionIsNotOperationCanceledException_ShouldContainInternalServerErrorStatusCode()
        {
            var context = CreateHttpContext(new Exception());

            var target = new GlobalExceptionHandler();

            await target.InvokeAsync(context);

            Assert.AreEqual(context.Response.StatusCode, (int)HttpStatusCode.InternalServerError);
        }

        private HttpContext CreateHttpContext(Exception exception)
        {
            var context = new DefaultHttpContext();

            context.Features.Set<IExceptionHandlerPathFeature>(new ExceptionHandlerFeature() { Error = exception });
            context.Response.Body = new MemoryStream();

            return context;
        }

        private readonly GlobalExceptionHandler target;
    }
}
