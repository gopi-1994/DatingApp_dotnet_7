using System.Net;
using System.Text.Json;
using API.Errors;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace API.Middleware{
    public class ExceptionMiddleware{
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;
        private readonly IHostEnvironment _env;

        public ExceptionMiddleware(RequestDelegate next,ILogger<ExceptionMiddleware> logger, IHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;
        }
        public async Task InvokeAsync(HttpContext context){
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,ex.Message);
                context.Response.ContentType= "applications/json";
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                var Response = _env.IsDevelopment()
                ? new API_Exceptions(context.Response.StatusCode,ex.Message,ex.StackTrace.ToString())
                : new API_Exceptions(context.Response.StatusCode,ex.Message,"Internal Server error");
                var options = new JsonSerializerOptions{PropertyNamingPolicy = JsonNamingPolicy.CamelCase};
                var json = JsonSerializer.Serialize(Response,options);
                await context.Response.WriteAsync(json);
                
            }

        }
    }
}