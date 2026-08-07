using CulinaryCart.CulinaryCartBAL.Models.DTO;
using CulinaryCart.CulinaryCartBAL.Repositories;
using CulinaryCart.CulinaryCartDAL.DbContext;
using CulinaryCart.CulinaryCartDAL.Repositories;
using CulinaryCart.CulinaryFAL;
using CulinaryCart.Filters;
using CulinaryCart.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Annotations;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Text.Json.Serialization;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
        // jwt authentication added
        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = builder.Configuration["Jwt:Issuer"],
                ValidAudience = builder.Configuration["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Secret Key is missing from configuration."))) // Safely reads your secure 2026 key!
            };
        });


        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowLocalHost",
                policy => policy.WithOrigins("http://localhost:5209","http://localhost:4200")
                                .AllowAnyHeader()
                                .AllowAnyMethod());
        });

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
                Description = "API for Managing Menu Items, Categories, Dietary Preferences, Cart Procedures and Order History in the CulinaryCart application.",
            });

            // 🔑 Add JWT Bearer security scheme
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Enter 'Bearer' followed by your JWT token.\nExample: Bearer eyJhbGciOiJIUzI1NiIs..."
            });

            c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });

            c.OperationFilter<FileUploadOperationFilter>();

            c.SchemaFilter<DropdownSchemaFilter>();

            c.OperationFilter<ShowMenuOperationFilter>();

            c.OperationFilter<FormDataOperationFilter>();
            c.OperationFilter<SignupFormDataOperationFilter>();
            c.OperationFilter<UpdateUserFormDataOperationFilter>();
            c.SchemaFilter<UserResponseSchemaFilter>();
            c.OperationFilter<ShowMenuPaginationOperationFilter>();

        });

        builder.Services.AddDbContext<CulinaryCartDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
                   .LogTo(Console.WriteLine, LogLevel.Information)   // ✅ logs SQL to console
                   .EnableSensitiveDataLogging());

        builder.Services.AddScoped<MenuDAL>();
        builder.Services.AddScoped<CategoryDAL>();
        builder.Services.AddScoped<DietDAL>();
        builder.Services.AddScoped<OrderHistoryDAL>();
        builder.Services.AddScoped<UserDAL>();
        builder.Services.AddScoped<PromocodeDAL>();
        builder.Services.AddScoped<ChargeDAL>();
        builder.Services.AddScoped<RefundDAL>();
        

        builder.Services.AddScoped<MenuBAL>();
        builder.Services.AddScoped<CartBAL>();
        builder.Services.AddScoped<UserBAL>();
        builder.Services.AddScoped<PromocodeBAL>();
        builder.Services.AddScoped<ChargeBAL>();
        builder.Services.AddScoped<OrdersBAL>();
        builder.Services.AddScoped<RefundsBAL>();
        builder.Services.AddScoped<MyOrdersBAL>();

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

        // Use CORS
        app.UseCors("AllowLocalHost");

        app.UseHttpsRedirection();
        app.UseAuthentication();   // <-- added 24-06
        app.UseAuthorization();
        //app.UseMiddleware<TokenValidationMiddleware>();
        app.MapControllers();
        app.Run();
    }
}