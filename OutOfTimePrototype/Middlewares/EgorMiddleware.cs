namespace OutOfTimePrototype.Middlewares
{
    public class EgorMiddleware
    {
        private readonly RequestDelegate _next;

        public EgorMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            httpContext.Response.Headers.Add("Access-Control-Allow-Origin", "http://localhost:3000");
            await _next.Invoke(httpContext);     
        }
    }

    public static class EgorMiddlewareExtensions
    {
        public static IApplicationBuilder UseEgorMiddleware(this IApplicationBuilder applicationBuilder)
        {
            return applicationBuilder.UseMiddleware<EgorMiddleware>();
        }
    }
}
