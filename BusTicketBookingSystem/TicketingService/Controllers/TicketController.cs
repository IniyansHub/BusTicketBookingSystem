using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
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

        public int getIdFromToken()
        {
            var handler = new JwtSecurityTokenHandler();
            string authHeader = Request.Headers["Authorization"];
            authHeader = authHeader.Replace("bearer ", "");
            var jsonToken = handler.ReadToken(authHeader);
            var tokenS = handler.ReadToken(authHeader) as JwtSecurityToken;
            var id = tokenS.Claims.First(claim => claim.Type == "Id").Value;
            return Convert.ToInt32(id);
        }


        //Display all the available bus details
        [HttpGet]
        [Route("/api/showbuses"), Authorize]
        public IEnumerable<Busdatum> DisplayBusDetails()
        {
            var busDetails = new List<Busdatum>();
            busDetails = context.Busdata.ToList();
            return busDetails;

        }

        
        

        //Display all the booked tickets of the user
        [HttpGet]
        [Route("/api/showtickets"), Authorize]
        public async Task<ActionResult> DisplayBookedTickets()
        {
            var id = getIdFromToken();
            var userFound = await context.Userdata.FirstOrDefaultAsync(x => x.UserId == id);
            if(userFound != null)
            {
                var ticketDetails = context.Ticketdata.Where(x => x.UserId == userFound.UserId);
                return Ok(ticketDetails);
            }
            return BadRequest("Some error occured");
            
        }

        //Endpoint for ticket booking
        [HttpPut]
        [Route("/api/booking"),Authorize]
        public async Task<ActionResult> BookTickets([FromBody] Booking booking)
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
                var id = getIdFromToken();

                var userFound = await context.Userdata.FirstOrDefaultAsync(x => x.UserId == id);

                if (userFound == null) return BadRequest("Some error occured!");

                busFound.TicketCount -= booking.ticketCount;

                var existingTicket = await context.Ticketdata.FirstOrDefaultAsync(x => x.UserId == userFound.UserId && x.BusId == busFound.BusId);

                if (existingTicket != null)
                {
                    existingTicket.TicketCount+=booking.ticketCount;
                }
                else
                {
                    Ticketdatum bookedTicket = new()
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

        //Endpoint for ticket cancellation
        [HttpPut]
        [Route("/api/cancelbooking"),Authorize]
        public async Task<ActionResult> CancelTickets([FromBody] Booking cancelBooking)
        {

            var id = getIdFromToken();
           
            var userFound = await context.Userdata.FirstOrDefaultAsync(x => x.UserId == id);

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
