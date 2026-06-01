using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using Microsoft.OpenApi.Any;
using System.Collections.Generic;
using System.Linq;

public class ShowMenuPaginationOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        // Apply only to ShowMenu endpoint
        if (context.ApiDescription.RelativePath.Contains("ShowMenu"))
        {
            // PageNumber parameter
            var pageNumberParam = operation.Parameters.FirstOrDefault(p => p.Name == "PageNumber");
            if (pageNumberParam != null)
            {
                pageNumberParam.Schema = new OpenApiSchema
                {
                    Type = "integer",
                    Default = new OpenApiInteger(1),
                    Minimum = 1
                };
            }

            // PageSize parameter with dropdown values
            var pageSizeParam = operation.Parameters.FirstOrDefault(p => p.Name == "PageSize");
            if (pageSizeParam != null)
            {
                pageSizeParam.Schema = new OpenApiSchema
                {
                    Type = "integer",
                    Enum = new List<IOpenApiAny>
                    {
                        new OpenApiInteger(10),
                        new OpenApiInteger(20),
                        new OpenApiInteger(50)
                    },
                    Default = new OpenApiInteger(10)
                };
            }
        }
    }
}
