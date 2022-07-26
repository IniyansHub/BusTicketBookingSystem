using Microsoft.EntityFrameworkCore;
using TicketingService.Models;

namespace TicketingService.Services
{
    public class TicketService
    {
        private readonly busticketdbContext context;

        public TicketService()
        {

        }
        public TicketService(busticketdbContext dbcontext)
        {
            context = dbcontext;
        }

        public virtual async Task<object> DisplayBusDetailsBasedOnId(int id)
        {
            var busDetail = await context.Busdata.FirstOrDefaultAsync(x => x.BusId == id);
            if (busDetail == null)
            {
                return null;
            }

            return busDetail;

        }
    }
}
