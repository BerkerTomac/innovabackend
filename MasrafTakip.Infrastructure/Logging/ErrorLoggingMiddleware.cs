using Microsoft.AspNetCore.Http;
using Serilog;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MasrafTakip.Infrastructure.Logging
{
    public class ErrorLoggingMiddleware
    {
        private readonly RequestDelegate _next;

        public ErrorLoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                var userId = context.User?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                var errorCode = context.Response?.StatusCode.ToString() ?? "500"; 
                var errorTime = DateTime.UtcNow;
                var errorMessage = ex.Message;

                Log.ForContext("UserId", userId)
                   .ForContext("ErrorCode", errorCode)
                   .ForContext("ErrorTime", errorTime)
                   .ForContext("ErrorMessage", errorMessage)
                   .Error(ex, "An error occurred");

                context.Response.StatusCode = 500; 
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(Newtonsoft.Json.JsonConvert.SerializeObject(new { error = errorMessage }));
            }
        }
    }
}
