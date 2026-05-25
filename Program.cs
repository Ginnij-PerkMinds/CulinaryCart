using CulinaryCart.CulinaryBAl;
using CulinaryCart.CulinaryDal;
using CulinaryCart.DbContext;
using Microsoft.EntityFrameworkCore;



var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<CulinaryCartDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<MenuDAL>();
builder.Services.AddScoped<OrderHistoryDAL>();
builder.Services.AddScoped<CartBAL>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "CulinaryCart API v1");
    c.RoutePrefix = string.Empty; // launches Swagger at root URL (http://localhost:<port>/)
});

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();
app.Run();

