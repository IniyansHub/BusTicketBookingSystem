using System;
using System.Collections.Generic;

namespace TicketManagerService.Models
{
    public partial class Ticketdatum
    {
        public int TicketId { get; set; }
        public int BusId { get; set; }
        public int UserId { get; set; }
        public int TicketCount { get; set; }
    }
}
