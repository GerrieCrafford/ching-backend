using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Ching.Utilities;

public class NullableSchemaFilter : ISchemaFilter
{
    /// <summary>
    /// Add to model.Required all properties where nullable is false
    /// </summary>
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        schema.Properties
            .Where(x => !x.Value.Nullable && !schema.Required.Contains(x.Key))
            .Select(x => x.Key)
            .ToList()
            .ForEach(key => schema.Required.Add(key));
    }
}
