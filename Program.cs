using CulinaryCart.CulinaryCartBAL.Repositories;
using CulinaryCart.CulinaryCartDAL.DbContext;
using CulinaryCart.CulinaryCartDAL.Repositories;
using CulinaryCart.CulinaryFAL;
using CulinaryCart.Filters;
using CulinaryCart.Middleware;
using Microsoft.OpenApi;
using Microsoft.OpenApi.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;


internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers()
            .AddJsonOptions(options =>
            {
                //Preventing serialization issues with circular references:
                options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
                options.JsonSerializerOptions.WriteIndented = true;
            });


        builder.Services.AddEndpointsApiExplorer();

        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Version = "v1",
                Title = "CulinaryCart API",
                Description = "API for managing menu items, categories, dietary preferences, and order history in the CulinaryCart application.",
            });

            c.OperationFilter<FileUploadOperationFilter>();

            c.SchemaFilter<DropdownSchemaFilter>();

            c.OperationFilter<ShowMenuOperationFilter>();

            c.OperationFilter<FormDataOperationFilter>();

            c.OperationFilter<ShowMenuPaginationOperationFilter>();
        });

        builder.Services.AddDbContext<CulinaryCartDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

        builder.Services.AddScoped<MenuDAL>();
        builder.Services.AddScoped<CategoryDAL>();
        builder.Services.AddScoped<DietDAL>();
        builder.Services.AddScoped<OrderHistoryDAL>();

        builder.Services.AddScoped<MenuBAL>();
        builder.Services.AddScoped<CartBAL>();

        builder.Services.AddScoped<IImageFAL, ImageFAL>();


        var app = builder.Build();

        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "CulinaryCart API v1");
            c.RoutePrefix = "swagger";

        });

        app.UseGlobalExceptionMiddleware();

        app.UseStaticFiles();

        app.UseHttpsRedirection();
        app.UseAuthorization();

        app.MapControllers();
        app.Run();
    }
}