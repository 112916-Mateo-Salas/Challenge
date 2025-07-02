using Microsoft.EntityFrameworkCore;
using NotificacionesService.Consumers;
using NotificacionesService.Data;
using NotificacionesService.Repositories.Impl;
using NotificacionesService.Repositories.Interfaces;

var builder = Host.CreateApplicationBuilder(args);

// Configurar servicios
builder.Services.AddDbContext<NotificacionDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<ILogRepository, LogRepositoryImpl>();
builder.Services.AddHostedService<InventoryConsumer>();

var host = builder.Build();

// Aplicar migraciones (crear tabla si no existe)
using (var scope = host.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<NotificacionDbContext>();
    dbContext.Database.Migrate();
}

await host.RunAsync();
