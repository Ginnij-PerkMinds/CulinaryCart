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
        using (var scope = _serviceProvider.CreateScope())
        {
            var categoryDAL = scope.ServiceProvider.GetRequiredService<CategoryDAL>();
            var dietDAL = scope.ServiceProvider.GetRequiredService<DietDAL>();

            // For AddMenuRequest DTO
            if (context.Type == typeof(AddMenuRequest))
            {
                var categories = categoryDAL.GetAllCategories();
                if (schema.Properties.ContainsKey("CategoryId"))
                {
                    schema.Properties["CategoryId"].Enum = categories
                        .Select(c => new OpenApiInteger(c.CategoryId))
                        .Cast<IOpenApiAny>()
                        .ToList();
                }

                var diets = dietDAL.GetAllDietPreferences();
                if (schema.Properties.ContainsKey("DietId"))
                {
                    schema.Properties["DietId"].Enum = diets
                        .Select(d => new OpenApiInteger(d.DietId))
                        .Cast<IOpenApiAny>()
                        .ToList();
                }
            }
        }
    }

}

