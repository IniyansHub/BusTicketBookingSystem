using System;
using System.Collections.Generic;

namespace TicketingService.Models
{
    public partial class Busdatum
    {
        public int BusId { get; set; }
        public string BusName { get; set; } = null!;
        public string BusType { get; set; } = null!;
        public string BusRoute { get; set; } = null!;
        public DateTime ArrivalTime { get; set; }
        public int TicketCount { get; set; }
    }
}
