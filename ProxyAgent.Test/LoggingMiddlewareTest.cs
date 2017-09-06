// Copyright (c) Microsoft. All rights reserved.

using Microsoft.AspNetCore.Http;
using Microsoft.Azure.IoTSolutions.ReverseProxy;
using Microsoft.Azure.IoTSolutions.ReverseProxy.Diagnostics;
using Microsoft.Azure.IoTSolutions.ReverseProxy.Runtime;
using Moq;
using System.Threading.Tasks;
using Xunit;
using System;

namespace ProxyAgent.Test
{
    public class LoggingMiddlewareTest
    {
        [Fact]
        public Task AddsLoggingCorrelationId()
        {
            const string correlationHeader = "X-Correlation-ID";

            // Arrange
            var next = new Mock<RequestDelegate>();
            var log = new Mock<ILogger>();

            var config = new Mock<IConfig>();
            config.SetupGet(x => x.CorrelationHeader)
                .Returns(correlationHeader);

            var request = new Mock<HttpRequest>();
            var requestHeaders = new Mock<IHeaderDictionary>();
            var response = new Mock<HttpResponse>();
            var context = new Mock<HttpContext>();
            context.SetupGet(x => x.Request).Returns(request.Object);
            context.SetupGet(x => x.Request.Headers).Returns(requestHeaders.Object);
            context.SetupGet(x => x.Response).Returns(response.Object);
            context.SetupGet(x => x.TraceIdentifier).Returns(Guid.NewGuid().ToString());

            var target = new LoggingMiddleware(
                next.Object,
                config.Object,
                log.Object);

            // Act
            target.Invoke(context.Object).Wait();

            // Assert
            return Task.CompletedTask;
        }
    }
}
