using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using Microsoft.OpenApi.Any;
using CulinaryCart.CulinaryCartDAL.Repositories;
using CulinaryCart.CulinaryCartBAL.Models.DTO;

public class DropdownSchemaFilter : ISchemaFilter
{
    private readonly IServiceProvider _serviceProvider;

    public DropdownSchemaFilter(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        // Create a scope so scoped services can be resolved safely
        using var scope = _serviceProvider.CreateScope();
        var categoryDal = scope.ServiceProvider.GetRequiredService<CategoryDAL>();
        var dietDal = scope.ServiceProvider.GetRequiredService<DietDAL>();

        if (context.Type == typeof(AddMenuRequest) ||
            context.Type == typeof(UpdateMenuRequest) ||
            context.Type == typeof(ShowMenuFilterRequest))  
        {
            var categories = categoryDal.GetAllCategories()
                .Select(c => c.CategoryName)
                .ToList();

            if (schema.Properties.ContainsKey("CategoryName"))
            {
                schema.Properties["CategoryName"].Enum = categories
                    .Select(c => new OpenApiString(c))
                    .Cast<IOpenApiAny>()
                    .ToList();
            }

            var diets = dietDal.GetAllDietPreferences()
                .Select(d => d.Diet)
                .ToList();

            if (schema.Properties.ContainsKey("DietaryPreferenceName"))
            {
                schema.Properties["DietaryPreferenceName"].Enum = diets
                    .Select(d => new OpenApiString(d))
                    .Cast<IOpenApiAny>()
                    .ToList();
            }
        }
    }
}

