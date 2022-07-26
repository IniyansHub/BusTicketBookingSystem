using Microsoft.AspNetCore.Mvc;
using TicketManagerService.Receiver;

namespace TicketManagerService.Controllers
{
    public class TicketManagerController : Controller
    {
        [HttpGet]
        [Route("/api/print")]
        public ActionResult printer()
        {
            return Ok(""+Receive.Receiver);
        }

    }
}
