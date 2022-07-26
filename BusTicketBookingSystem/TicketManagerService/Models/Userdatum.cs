using System;
using System.Collections.Generic;

namespace TicketManagerService.Models
{
    public partial class Userdatum
    {
        public int UserId { get; set; }
        public string EmailId { get; set; } = null!;
        public string Password { get; set; } = null!;
        public sbyte IsAdmin { get; set; }
    }
}
