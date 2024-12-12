using Microsoft.AspNetCore.Diagnostics;
using System.Net.Mime;
using System.Net;
using System.Text.Json;
using ElasticSearch.API.Models;

namespace ElasticSearch.API.Middleware;

static public class ConfigureExceptionHandlerExtension
{
    public static void ConfigureExceptionHandler<T>(this WebApplication application, ILogger<T> logger)
    {
        application.UseExceptionHandler(builder =>
        {
            builder.Run(async context =>
            {
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                context.Response.ContentType = MediaTypeNames.Application.Json;

                var contextFeature = context.Features.Get<IExceptionHandlerFeature>();

                if (contextFeature != null)
                {
                    logger.LogError(contextFeature.Error.Message);

                    ServiceResult serviceResult = new();

                    serviceResult.InitError(contextFeature.Error);

                    await context.Response.WriteAsync(JsonSerializer.Serialize(serviceResult));
                }
            });
        });
    }
}
