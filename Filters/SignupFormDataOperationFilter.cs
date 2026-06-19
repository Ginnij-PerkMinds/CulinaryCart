using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

public class SignupFormDataOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        // Only apply to Auth/signup endpoint
        if (context.ApiDescription.RelativePath.Contains("Auth/signup"))
        {
            operation.RequestBody = new OpenApiRequestBody
            {
                Content =
                {
                    ["application/json"] = new OpenApiMediaType
                    {
                        Schema = new OpenApiSchema
                        {
                            Type = "object",
                            Properties = new Dictionary<string, OpenApiSchema>
                            {
                                ["Name"] = new OpenApiSchema { Type = "string" },
                                ["Email"] = new OpenApiSchema { Type = "string" },
                                ["Password"] = new OpenApiSchema { Type = "string" },   
                            },
                            Required = new HashSet<string>
                            {
                                "Name","Email","Password"
                            }
                        }
                    }
                }
            };
        }
    }
}
