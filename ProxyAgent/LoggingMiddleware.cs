// Copyright (c) Microsoft. All rights reserved.

using Microsoft.AspNetCore.Http;
using Microsoft.Azure.IoTSolutions.ReverseProxy.Diagnostics;
using Microsoft.Azure.IoTSolutions.ReverseProxy.Runtime;
using Microsoft.Extensions.Primitives;
using System;
using System.Threading.Tasks;

namespace Microsoft.Azure.IoTSolutions.ReverseProxy
{
    public class LoggingMiddleware
    {
        private readonly RequestDelegate next;
        private readonly ILogger log;
        private readonly IConfig config;

        public LoggingMiddleware(
            RequestDelegate next,
            IConfig config,
            ILogger log)
        {
            this.log = log;
            this.config = config;
            this.next = next ?? throw new ArgumentNullException(nameof(next));
        }

        public Task Invoke(HttpContext context)
        {
            try
            {

                if (context.Request.Headers.TryGetValue(
                    config.CorrelationHeader, out StringValues correlationId))
                {
                    context.TraceIdentifier = correlationId;
                }

                // add correlationID to response header
                context.Response.OnStarting(() =>
                {
                    context.Response.Headers.Add(
                        config.CorrelationHeader,
                        new[] { context.TraceIdentifier });

                    return Task.CompletedTask;
                });

                return next(context);
            }
            catch (Exception e)
            {
                throw new Exception(
                    "Error including correlation Id in response header", e);
            }
        }
    }
}
