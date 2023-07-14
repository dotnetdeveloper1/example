using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using System.Threading;

namespace PWP.InvoiceCapture.Core.API.UnitTests.Fakes
{
    [ExcludeFromCodeCoverage]
    internal class FakeHttpContext : HttpContext
    {
        public override ConnectionInfo Connection => throw new NotSupportedException();
        public override IFeatureCollection Features => throw new NotSupportedException();
        public override IDictionary<object, object> Items { get; set; }
        public override HttpRequest Request => throw new NotSupportedException();
        public override CancellationToken RequestAborted { get; set; }
        public override IServiceProvider RequestServices { get; set; }
        public override HttpResponse Response => throw new NotSupportedException();
        public override ISession Session { get; set; }
        public override string TraceIdentifier { get; set; }
        public override ClaimsPrincipal User { get; set; }
        public override WebSocketManager WebSockets => throw new NotSupportedException();
        public override void Abort() => throw new NotSupportedException();
    }
}
