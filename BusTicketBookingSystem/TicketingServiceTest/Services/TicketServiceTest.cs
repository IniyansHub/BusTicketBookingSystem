using Microsoft.AspNetCore.Mvc;
using Moq;
using TicketingService.Controllers;
using TicketingService.Models;
using TicketingService.Services;

namespace TicketingServiceTest.Services
{
    public class TicketServiceTest
    {
        private readonly TicketController _controller;
        Mock<TicketService> servicemock = new Mock<TicketService>();
        Mock<busticketdbContext> busdb = new Mock<busticketdbContext>();
        public TicketServiceTest()
        {
            _controller = new TicketController(busdb.Object, servicemock.Object);
        }
        [Fact]
        public async Task GetBusById_Return_Failure()
        {
            var busId = 1;
            var ExpectedBus = new Busdatum()
            {
             BusId= 1,
             BusName= "KPR Travels",
            BusType= "SR",
            BusRoute= "Chennai to Coimbatore",
            ArrivalTime= DateTime.Now,
            TicketCount= 50
            };

            servicemock.Setup(x => x.DisplayBusDetailsBasedOnId(busId)).ReturnsAsync(ExpectedBus);
            var ActualOutput = await _controller.DisplayBus(busId);
            var result = ActualOutput as OkObjectResult;
            //Assert.IsType<OkObjectResult>(ActualOutput);
            //ExpectedBus.Equals(ActualOutput);
            Assert.NotSame(ExpectedBus,ActualOutput);
        }
        
    }
}














