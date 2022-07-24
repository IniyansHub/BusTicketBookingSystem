namespace AdminService.Models
{
    public class BusDetailsEditor
    {
        public int BusId { get; set; }
        public string? BusName { get; set; }
        public string? BusType { get; set; }
        public string? BusRoute { get; set; }
        public DateTime? ArrivalTime { get; set; }
        public int? TicketCount { get; set; }
    }
}
