using InventoryService.Context;
using InventoryService.Messaging.Impl;
using InventoryService.Messaging.Interfaces;
using InventoryService.Repositories.Impl;
using InventoryService.Repositories.Interfaces;
using InventoryService.Response;
using InventoryService.Services.Impl;
using InventoryService.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;

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

builder.Services.AddScoped<IRabbitPublisher, RabbitPublisher>();


//builder.Services.AddTransient<ExceptionHandlerMiddleware>();

builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.ListenAnyIP(80); // HTTP en el contenedor
    // No pongas HTTPS en contenedores si no hay certificado
});


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

app.MapPost("/enviarMensaje", async (IServiceProvider serviceProvider, string name, string lastname, string phonenumber) =>
{
    var factory = new ConnectionFactory()
    {
        HostName = "rabbitmq",
        Port = 5672,
        UserName = "guest",
        Password = "guest"
    };
    using var connection = await factory.CreateConnectionAsync();
    using var channel = await connection.CreateChannelAsync();
    await channel.QueueDeclareAsync(queue:"amines_queue",durable:true,exclusive:false,autoDelete:false,arguments:null);
    var message = new {Name = name, Lastname = lastname,  Phonenumber = phonenumber};
    var body = System.Text.Json.JsonSerializer.SerializeToUtf8Bytes(message);
    await channel.BasicPublishAsync(exchange:"",
        routingKey:"amines_queue",
        body:body);
    return Results.Ok("Message sent successfully");
});

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
