using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using Microsoft.AspNetCore.Http;
using CulinaryCart.CulinaryCartDAL.Repositories;
using CulinaryCart.CulinaryCartBAL.Models.DTO;

public class FormDataOperationFilter : IOperationFilter
{
    private readonly IServiceProvider _serviceProvider;

    public FormDataOperationFilter(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        if (context.ApiDescription.RelativePath.Contains("AddMenu") ||
            context.ApiDescription.RelativePath.Contains("UpdateMenu"))
        {
            using var scope = _serviceProvider.CreateScope();
            var categoryDal = scope.ServiceProvider.GetRequiredService<CategoryDAL>();
            var dietDal = scope.ServiceProvider.GetRequiredService<DietDAL>();

            var categories = categoryDal.GetAllCategories().Select(c => c.CategoryName).ToList();
            var diets = dietDal.GetAllDietPreferences().Select(d => d.Diet).ToList();

            // Clear default body
            operation.RequestBody = new OpenApiRequestBody
            {
                Content =
                {
                    ["multipart/form-data"] = new OpenApiMediaType
                    {
                        Schema = new OpenApiSchema
                        {
                            Type = "object",
                            Properties =
                            {
                                ["FoodItemName"] = new OpenApiSchema { Type = "string" },
                                ["Price"] = new OpenApiSchema { Type = "number", Format = "decimal" },
                                ["Offers"] = new OpenApiSchema { Type = "string" },
                                ["ImageFile"] = new OpenApiSchema { Type = "string", Format = "binary" },
                                ["CategoryName"] = new OpenApiSchema
                                {
                                    Type = "string",
                                    Enum = categories.Select(c => new Microsoft.OpenApi.Any.OpenApiString(c)).ToList<Microsoft.OpenApi.Any.IOpenApiAny>()
                                },
                                ["DietaryPreferenceName"] = new OpenApiSchema
                                {
                                    Type = "string",
                                    Enum = diets.Select(d => new Microsoft.OpenApi.Any.OpenApiString(d)).ToList<Microsoft.OpenApi.Any.IOpenApiAny>()
                                }
                            },
                            Required = new HashSet<string>
                            {
                                "FoodItemName","Price","Offers","ImageFile","CategoryName","DietaryPreferenceName"
                            }
                        }
                    }
                }
            };
        }
    }
}