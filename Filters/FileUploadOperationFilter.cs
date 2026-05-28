using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Linq;
using System.Collections.Generic;   

namespace CulinaryCart.Filters
{
    public class FileUploadOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var fileParams = context.ApiDescription.ParameterDescriptions
                .Where(p => p.Type == typeof(Microsoft.AspNetCore.Http.IFormFile));

            if (fileParams.Any())
            {
                var properties = new Dictionary<string, IOpenApiSchema>();

                foreach (var p in fileParams)
                {
                    properties[p.Name] = new OpenApiSchema
                    {
                        Type = Microsoft.OpenApi.JsonSchemaType.String, 
                        Format = "binary"
                    };
                }

                operation.RequestBody = new OpenApiRequestBody
                {
                    Content =
                    {
                        ["multipart/form-data"] = new OpenApiMediaType
                        {
                            Schema = new OpenApiSchema
                            {
                                Type = Microsoft.OpenApi.JsonSchemaType.Object, 
                                Properties = properties,   
                                Required = fileParams.Select(p => p.Name).ToHashSet()
                            }
                        }
                    }
                };
            }
        }
    }
}

