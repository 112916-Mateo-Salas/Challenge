using NotificacionesService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotificacionesService.Repositories.Interfaces
{
    public interface ILogRepository
    {
        Task AddLog(InventoryLog log);
    }
}
