using System.Net;
using Microsoft.AspNetCore.Diagnostics;

namespace OutOfTimePrototype.Middlewares.ExceptionMiddleware;

public static class ExceptionMiddlewareExtensions
{
    public static void ConfigureExceptionHandler(this IApplicationBuilder app, ILogger logger)
    {
        app.UseExceptionHandler(appError =>
        {
            appError.Run(async context =>
            {
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                context.Response.ContentType = "application/json";

                var contextFeature = context.Features.Get<IExceptionHandlerFeature>();

                if (contextFeature != null)
                {
                    logger.LogError("Something went wrong: {Cfe}", contextFeature.Error);
                    await context.Response.WriteAsync(new ErrorDetails
                    (
                        context.Response.StatusCode,
                        "Internal Server Error"
                    ).ToString());
                }
            });
        });
    }

    public static void ConfigureCustomExceptionMiddleware(this WebApplication app)
    {
        app.UseMiddleware<ExceptionMiddleware>();
    }
}