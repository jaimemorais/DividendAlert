using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace DividendAlertApi.Middlewares
{
    public class ExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;

        private readonly ILogger<ExceptionHandlerMiddleware> _logger;

        public ExceptionHandlerMiddleware(RequestDelegate next, ILogger<ExceptionHandlerMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next.Invoke(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }


        private async Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            _logger.LogCritical(ex, "Critical Exception");

            var response = context.Response;
            response.StatusCode = (int)HttpStatusCode.InternalServerError;

            // TODO await response.WriteAsync("Critical exception : " + (context.User.Identity.IsAuthenticated ? ex.ToString() : "Check logs."));
            await response.WriteAsync("DividendAlertApi Critical exception : " + ex.ToString());

        }

    }
}
