namespace AdminService.Models
{
    public class CancelBooking
    {
        public int UserId { get; set; }
        public int BusId { get; set; }
        public int TicketCount { get; set; }
    }
}
