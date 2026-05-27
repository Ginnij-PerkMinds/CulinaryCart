using CulinaryCart.CulinaryBAl;
using CulinaryCart.CulinaryBAL;
using CulinaryCart.CulinaryDal;
using CulinaryCart.DbContext;
using CulinaryCart.Middleware;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        //Preventing serialization issues with circular references:
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.WriteIndented = true; 
    });


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<CulinaryCartDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<MenuDAL>();
builder.Services.AddScoped<CategoryDAL>();
builder.Services.AddScoped<DietDAL>();
builder.Services.AddScoped<OrderHistoryDAL>();

builder.Services.AddScoped<MenuBAL>();
builder.Services.AddScoped<CartBAL>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "CulinaryCart API v1");
    c.RoutePrefix = "swagger";
});

app.UseGlobalExceptionMiddleware();

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();
app.Run();

