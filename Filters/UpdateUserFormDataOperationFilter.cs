using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;

public class UpdateUserFormDataOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        // Apply only to User/update endpoint
        if (context.ApiDescription.RelativePath.Contains("User/UpdateUser"))
        {
            operation.RequestBody = new OpenApiRequestBody
            {
                Content =
                {
                    ["multipart/form-data"] = new OpenApiMediaType
                    {
                        Schema = new OpenApiSchema
                        {
                            Type = "object",
                            Properties = new Dictionary<string, OpenApiSchema>
                            {
                                ["Name"] = new OpenApiSchema { Type = "string", Description = "User's full name" },
                                ["Password"] = new OpenApiSchema { Type = "string", Description = "New password" },
                                ["EmailId"] = new OpenApiSchema { Type = "string", Description = "Email address" },
                                ["PhoneNo"] = new OpenApiSchema { Type = "string", Description = "Phone number" },
                                ["ProfilePic"] = new OpenApiSchema { Type = "string", Format = "binary", Description = "Upload profile picture file" },                          
                                ["HouseNo"] = new OpenApiSchema { Type = "string" },
                                ["Locality"] = new OpenApiSchema { Type = "string" },
                                ["Landmark"] = new OpenApiSchema { Type = "string" },
                                ["City"] = new OpenApiSchema { Type = "string" },
                                ["District"] = new OpenApiSchema { Type = "string" },
                                ["Pincode"] = new OpenApiSchema { Type = "string" },
                                ["State"] = new OpenApiSchema { Type = "string" },
                                ["IsActive"] = new OpenApiSchema { Type = "boolean" },
                                ["IsAdmin"] = new OpenApiSchema { Type = "boolean" }
                            },
                            Required = new HashSet<string> { "Name", "EmailId" }
                        }
                    }
                }
            };
        }
    }
}



