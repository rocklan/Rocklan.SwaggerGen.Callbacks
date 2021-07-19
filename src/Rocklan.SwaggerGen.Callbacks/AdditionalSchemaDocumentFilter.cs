using Microsoft.OpenApi.Models;

using Swashbuckle.AspNetCore.SwaggerGen;

namespace Rocklan.SwaggerGen.Callbacks
{
    /// <summary>
    /// Adds extra schemas (models) to the swagger page. Useful for callback payload objects
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class AdditionalSchemaDocumentFilter<T> : IDocumentFilter where T : class
    {
        /// <summary>
        /// Generates a schema for the supplied type
        /// </summary>
        /// <param name="swaggerDoc"></param>
        /// <param name="context"></param>
        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            context.SchemaGenerator.GenerateSchema(typeof(T), context.SchemaRepository);
        }
    }
}
