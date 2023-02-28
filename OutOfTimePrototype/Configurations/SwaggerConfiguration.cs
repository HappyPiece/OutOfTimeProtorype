using Microsoft.OpenApi.Models;
using OutOfTimePrototype.DTO;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace OutOfTimePrototype.Configurations
{
    public class CustomModelDocumentFilter<T> : IDocumentFilter where T : class
    {
        public void Apply(OpenApiDocument openApiDocument, DocumentFilterContext context)
        {
            context.SchemaGenerator.GenerateSchema(typeof(T), context.SchemaRepository);
        }
    }
}
