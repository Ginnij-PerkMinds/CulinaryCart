using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using CulinaryCart.CulinaryCartDAL.Repositories;

namespace CulinaryCart.Filters;

public class ShowMenuOperationFilter : IOperationFilter
{
    private readonly IServiceProvider _serviceProvider;

    public ShowMenuOperationFilter(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        if (context.ApiDescription.RelativePath.Contains("ShowMenu"))
        {
            using var scope = _serviceProvider.CreateScope();
            var categoryDal = scope.ServiceProvider.GetRequiredService<CategoryDAL>();
            var dietDal = scope.ServiceProvider.GetRequiredService<DietDAL>();

            var categories = categoryDal.GetAllCategories()
                .Select(c => c.CategoryName)
                .ToList();

            var diets = dietDal.GetAllDietPreferences()
                .Select(d => d.Diet)
                .ToList();

            // Update CategoryName parameter
            var categoryParam = operation.Parameters.FirstOrDefault(p => p.Name == "CategoryName");
            if (categoryParam != null)
            {
                categoryParam.Schema.Enum = categories
                    .Select(c => new Microsoft.OpenApi.Any.OpenApiString(c))
                    .Cast<Microsoft.OpenApi.Any.IOpenApiAny>()
                    .ToList();
            }

            // Update DietaryPreferenceName parameter
            var dietParam = operation.Parameters.FirstOrDefault(p => p.Name == "DietaryPreferenceName");
            if (dietParam != null)
            {
                dietParam.Schema.Enum = diets
                    .Select(d => new Microsoft.OpenApi.Any.OpenApiString(d))
                    .Cast<Microsoft.OpenApi.Any.IOpenApiAny>()
                    .ToList();
            }
        }
    }
}