using NotificacionesService.Data;
using NotificacionesService.Models;
using NotificacionesService.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotificacionesService.Repositories.Impl
{
    public class LogRepositoryImpl : ILogRepository
    {
        private readonly NotificacionDbContext _context;

        public LogRepositoryImpl(NotificacionDbContext notificacionDbContext)
        {
            _context = notificacionDbContext;
        }

        public async Task AddLog(InventoryLog log)
        {
            _context.InventoryLogs.Add(log);
            await _context.SaveChangesAsync();
        }
    }
}
