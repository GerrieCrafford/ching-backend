using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Ching.Utilities;

public class NullableSchemaFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        if (schema.Type == "object")
        {
            foreach (var openApiSchema in schema.Properties)
            {
                if (!openApiSchema.Value.Nullable)
                {
                    schema.Required.Add(openApiSchema.Key);
                }
            }
        }
    }
}