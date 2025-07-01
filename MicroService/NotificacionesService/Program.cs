using Microsoft.EntityFrameworkCore;
using NotificacionesService;
using NotificacionesService.Consumers;
using NotificacionesService.Data;
using NotificacionesService.Repositories.Impl;
using NotificacionesService.Repositories.Interfaces;

var builder = Host.CreateApplicationBuilder(args);



var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services.AddDbContext<NotificacionDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))); // 

        services.AddScoped<ILogRepository, LogRepositoryImpl>();
        services.AddHostedService<InventoryConsumer>();
    })
    .Build();

using (var scope = host.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<NotificacionDbContext>();
    dbContext.Database.Migrate(); //Esto sirve para que cada vez que se use el microservice se actualice la base de datos
}

await host.RunAsync();

