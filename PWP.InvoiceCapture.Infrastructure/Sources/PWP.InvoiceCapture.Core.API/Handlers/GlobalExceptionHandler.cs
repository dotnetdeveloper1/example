using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using PWP.InvoiceCapture.Core.Utilities;
using System;
using System.Net;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.Core.API.Handlers
{
    public class GlobalExceptionHandler
    {
        public Task InvokeAsync(HttpContext context)
        {
            Guard.IsNotNull(context, nameof(context));

            var feature = context.Features.Get<IExceptionHandlerPathFeature>();
            var exception = feature.Error;
            var exceptionType = exception.GetType();

            var status = exceptionType == typeof(OperationCanceledException) ?
                HttpStatusCode.NoContent :
                HttpStatusCode.InternalServerError;

            context.Response.StatusCode = (int)status;

            return Task.CompletedTask;
        }
    }
}
