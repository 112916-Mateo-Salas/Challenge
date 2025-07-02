using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotificacionesService.Models
{
    public class InventoryLog
    {
        public Guid Id { get; set; }

        public string EventType { get; set; }

        public string ProductJson { get; set; }

        public DateTime ReceivedAt { get; set; }

    }
}
