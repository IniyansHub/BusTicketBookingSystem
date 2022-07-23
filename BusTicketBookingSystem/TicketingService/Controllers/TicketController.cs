using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TicketingService.Models;

namespace TicketingService.Controllers
{
    public class TicketController : Controller
    {
        private readonly busticketdbContext context;
        public TicketController(busticketdbContext dbcontext)
        {
            context = dbcontext;
        }

        public string getEmailFromToken()
        {
            var principal = HttpContext.User;

            if (principal?.Claims != null)
            {
                foreach (var claim in principal.Claims)
                {
                    return claim.Value;
                }

            }

            return "";
        }

        [HttpGet]
        [Route("/api/showbuses"), Authorize]
        public IEnumerable<Busdatum> displayBusDetails()
        {
            var busDetails = new List<Busdatum>();
            busDetails = context.Busdata.ToList();
            return busDetails;

        }

        [HttpGet]
        [Route("/api/showalltickets"), Authorize]
        public IEnumerable<Ticketdatum> displayAllBookedTickets()
        {
            var ticketDetails = new List<Ticketdatum>();
            ticketDetails = context.Ticketdata.ToList();
            return ticketDetails;

        }

        [HttpGet]
        [Route("/api/showtickets"), Authorize]
        public async Task<ActionResult> displayBookedTickets()
        {
            var userEmail = getEmailFromToken();
            var userFound = await context.Userdata.FirstOrDefaultAsync(x => x.EmailId == userEmail);
            var ticketDetails = context.Ticketdata.Where(x => x.UserId == userFound.UserId);
            return Ok(ticketDetails);

        }


        [HttpPut]
        [Route("/api/booking"), Authorize(Roles = "1")]
        public async Task<ActionResult> bookTickets([FromBody] Booking booking)
        {
            var busFound = await context.Busdata.FirstOrDefaultAsync(x => x.BusId == booking.busId);
            if(busFound == null)
            {
                return StatusCode(404,"Service not available!");
            }
            else if(busFound.TicketCount<booking.ticketCount)
            {
                return StatusCode(401, "Insufficient tickets. Please select the valid amount of tickets available");
            }
            else
            {
                var userEmail = getEmailFromToken();

                var userFound = await context.Userdata.FirstOrDefaultAsync(x => x.EmailId == userEmail);

                busFound.TicketCount -= booking.ticketCount;

                var existingTicket = await context.Ticketdata.FirstOrDefaultAsync(x => x.UserId == userFound.UserId && x.BusId == busFound.BusId);

                if (existingTicket != null)
                {
                    existingTicket.TicketCount+=booking.ticketCount;
                }
                else
                {
                    Ticketdatum bookedTicket = new Ticketdatum()
                    {
                        BusId = busFound.BusId,
                        UserId = userFound.UserId,
                        TicketCount = booking.ticketCount
                    };

                    context.Ticketdata.Add(bookedTicket);
                }

                await context.SaveChangesAsync();
            }

            return Ok("Ticket booked successfully ");

        }

        [HttpPut]
        [Route("/api/cancelbooking"), Authorize(Roles = "1")]
        public async Task<ActionResult> cancelTickets([FromBody] Booking cancelBooking)
        {

            var userEmail = getEmailFromToken();
           

            var userFound = await context.Userdata.FirstOrDefaultAsync(x => x.EmailId == userEmail);

            var busFound = await context.Busdata.FirstOrDefaultAsync(x => x.BusId == cancelBooking.busId);

            var existingTicket = await context.Ticketdata.FirstOrDefaultAsync(x => x.UserId == userFound.UserId && x.BusId == busFound.BusId);

            if (existingTicket == null)
            {

                return StatusCode(404, "No bookings found for this account!");
            }
            else if (existingTicket.TicketCount < cancelBooking.ticketCount)
            {
                return StatusCode(401, "Tickets selected for cancellation is greater than the booked tickets");
            }
            else
            {
                busFound.TicketCount += cancelBooking.ticketCount;

                if (existingTicket.TicketCount - cancelBooking.ticketCount == 0)
                {
                    context.Ticketdata.Remove(existingTicket);
                }
                else
                {
                    existingTicket.TicketCount -= cancelBooking.ticketCount;
                }

                await context.SaveChangesAsync();
            }

            return Ok("Ticket Cancellation successful ");

        }

    }
}
