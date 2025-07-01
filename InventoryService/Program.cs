using InventoryService.Context;
using InventoryService.Repositories.Impl;
using InventoryService.Repositories.Interfaces;
using InventoryService.Response;
using InventoryService.Services.Impl;
using InventoryService.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<InventoryContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IProductRepository, ProductRepositoryImpl>();

builder.Services.AddScoped<IProductService, ProductServiceImpl>();

//builder.Services.AddTransient<ExceptionHandlerMiddleware>();


var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<InventoryContext>();
    dbContext.Database.Migrate(); //Esto sirve para que cada vez que se use el microservice se actualice la base de datos
}

app.UseMiddleware<ExceptionHandlerMiddleware>(); // Para mi manejo de excepciones no controladas de respuesta.

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors(options =>
{
    options.AllowAnyHeader()
    .AllowAnyMethod()
    .AllowAnyOrigin();
});

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
