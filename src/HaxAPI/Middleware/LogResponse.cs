namespace HaxAPI.Middleware
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Logging;

    public class LogResponse
    {
        private readonly RequestDelegate next;
        private readonly ILogger<LogResponse> logger;
        private Func<string, Exception, string> defaultformatter = (state, exception) => state;

        public LogResponse(RequestDelegate next, ILogger<LogResponse> logger)
        {
            this.next = next;
            this.logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            var bodyStream = context.Response.Body;

            var responseBodyStream = new MemoryStream();
            context.Response.Body = responseBodyStream;

            await this.next(context);

            responseBodyStream.Seek(0, SeekOrigin.Begin);
            var responseBody = new StreamReader(responseBodyStream).ReadToEnd();
            this.logger.Log(LogLevel.Information, 1, $"RESPONSE LOG: {responseBody}", null, this.defaultformatter);
            responseBodyStream.Seek(0, SeekOrigin.Begin);
            await responseBodyStream.CopyToAsync(bodyStream);
        }
    }
}
