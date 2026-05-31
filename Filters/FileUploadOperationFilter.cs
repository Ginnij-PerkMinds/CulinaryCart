using Microsoft.AspNetCore.Http;
using Microsoft.OpenApi; 
using Swashbuckle.AspNetCore.SwaggerGen;    
using System.Linq;

namespace CulinaryCart.Filters
{
    public class FileUploadOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var fileUploadParams = context.ApiDescription.ParameterDescriptions
                .Where(p => p.Type == typeof(IFormFile));

            if (fileUploadParams.Any())
            {
                operation.RequestBody = new OpenApiRequestBody
                {
                    Content =
                    {
                        ["multipart/form-data"] = new OpenApiMediaType
                        {
                            Schema = new OpenApiSchema
                            {
                                Type = Microsoft.OpenApi.JsonSchemaType.Object,
                                Properties =
                                {
                                    ["file"] = new OpenApiSchema
                                    {
                                        Type = Microsoft.OpenApi.JsonSchemaType.String,
                                        Format = "binary"
                                    }
                                }
                            }
                        }
                    }
                };
            }
        }
    }
}
