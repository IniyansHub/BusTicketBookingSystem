using AdminService.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AdminService.Controllers
{
    public class AdminController : Controller
    {
        private readonly busticketdbContext context;

        public AdminController(busticketdbContext _dbcontext)
        {
            context = _dbcontext;
        }

        [HttpGet]
        [Route("/api/showalltickets"),Authorize(Roles="1")]
        public IEnumerable<Ticketdatum> DisplayBookedTickets()
        {
            return context.Ticketdata.ToList();

        }

        [HttpPut]
        [Route("/api/admin/cancelbooking"),Authorize(Roles = "1")]
        public async Task<ActionResult> CancelTickets([FromBody] CancelBooking cancelBooking)
        {


            var busFound = await context.Busdata.FirstOrDefaultAsync(x => x.BusId == cancelBooking.BusId);

            if (busFound == null) return NotFound("Not bookings found for this account!");

            var existingTicket = await context.Ticketdata.FirstOrDefaultAsync(x => x.UserId == cancelBooking.UserId && x.BusId == cancelBooking.BusId);

            if (existingTicket == null)
            {

                return StatusCode(404, "No bookings found for this account!");
            }
            else if (existingTicket.TicketCount < cancelBooking.TicketCount)
            {
                return StatusCode(401, "Tickets selected for cancellation is greater than the booked tickets");
            }
            else
            {
                busFound.TicketCount += cancelBooking.TicketCount;

                if (existingTicket.TicketCount - cancelBooking.TicketCount == 0)
                {
                    context.Ticketdata.Remove(existingTicket);
                }
                else
                {
                    existingTicket.TicketCount -= cancelBooking.TicketCount;
                }

                await context.SaveChangesAsync();
            }

            return Ok("Ticket Cancellation successful ");

        }

        [HttpPost]
        [Route("api/admin/addbus"), Authorize(Roles = "1")]
        public async Task<ActionResult> AddBus(Busdatum bus)
        {
            if(bus == null)
            {
                return  BadRequest("Please enter valid details");
            }

            context.Busdata.Add(bus);
            await context.SaveChangesAsync();

            return Ok("New Bus details added");
        }

        [HttpDelete]
        [Route("api/admin/deletebus/{busId}"),Authorize(Roles = "1")]
        public async Task<ActionResult> RemoveBusDetails(int busId)
        {
            var busFound =await context.Busdata.FirstOrDefaultAsync(x=>x.BusId==busId);

            if (busFound == null)
            {
                return BadRequest("No buses found with this Id:" + busId);
            }
            var ticketsMappedWithThisId = context.Ticketdata.Where(x => x.BusId == busId);

            if (ticketsMappedWithThisId != null)
            {
                foreach (var tickets in ticketsMappedWithThisId)
                {
                    context.Ticketdata.Remove(tickets);
                }

                context.Busdata.Remove(busFound);
            }
            else
            {
                context.Busdata.Remove(busFound);
            }
            await context.SaveChangesAsync();
            return Ok("Bus details removed successfully");
        }

        [HttpPut]
        [Route("/api/admin/editbus/{busId}"),Authorize(Roles = "1")]
        public async Task<ActionResult<Busdatum>> EditBusDetails(int busId,BusDetailsEditor busEdit)
        {
            var busFound = await context.Busdata.FirstOrDefaultAsync(x => x.BusId==busId);

            if (busFound == null)
            {
                return NotFound("No buses found with this Id:" + busId);
            }

            if (busEdit.BusName != null)
            {
                busFound.BusName = busEdit.BusName;
            }else if (busEdit.BusType != null)
            {
                busFound.BusType = busEdit.BusType;
            }
            else if (busEdit.BusRoute != null)
            {
                busFound.BusRoute = busEdit.BusRoute;
            }
            else if (busEdit.ArrivalTime != null)
            {
                busFound.ArrivalTime = (DateTime)busEdit.ArrivalTime;
            }
            else if (busEdit.TicketCount != null)
            {
                busFound.TicketCount = (int)busEdit.TicketCount;
            }
            await context.SaveChangesAsync();
            return Ok("Bus details updated successfully!");
        }
    }
}
