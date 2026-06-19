
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;

public class UserResponseSchemaFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        if (context.Type == typeof(CulinaryCart.CulinaryCartBAL.Models.DTO.UserDto))
        {
            schema.Properties = new Dictionary<string, OpenApiSchema>
            {
                ["UserId"] = new OpenApiSchema { Type = "integer", Format = "int32" },
                ["Name"] = new OpenApiSchema { Type = "string" },
                ["EmailId"] = new OpenApiSchema { Type = "string" },
                ["PhoneNo"] = new OpenApiSchema { Type = "string" },
                ["ProfilePic"] = new OpenApiSchema { Type = "string" },
                ["IsActive"] = new OpenApiSchema { Type = "boolean" },
                ["IsAdmin"] = new OpenApiSchema { Type = "boolean" },
                ["CreatedAt"] = new OpenApiSchema { Type = "string", Format = "date-time" },
                ["UpdatedAt"] = new OpenApiSchema { Type = "string", Format = "date-time" },

                // Flattened address fields
                ["HouseNo"] = new OpenApiSchema { Type = "string" },
                ["Locality"] = new OpenApiSchema { Type = "string" },
                ["Landmark"] = new OpenApiSchema { Type = "string" },
                ["City"] = new OpenApiSchema { Type = "string" },
                ["District"] = new OpenApiSchema { Type = "string" },
                ["Pincode"] = new OpenApiSchema { Type = "string" },
                ["State"] = new OpenApiSchema { Type = "string" }
            };

            schema.Required = new HashSet<string>
            {
                "UserId","Name","EmailId","PhoneNo","IsActive","IsAdmin","CreatedAt"
            };
        }
    }
}
