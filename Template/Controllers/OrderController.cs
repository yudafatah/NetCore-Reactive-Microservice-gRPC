using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Plain.RabbitMQ;
using Template.DTO;
using Template.Repositories.Interfaces;

namespace Template.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderDetailsProvider orderDetailsProvider;
        private readonly IPublisher publisher;

        public OrderController(IOrderDetailsProvider orderDetailsProvider, IPublisher publisher)
        {
            this.orderDetailsProvider = orderDetailsProvider;
            this.publisher = publisher;
        }

        [HttpPost]
        public void Post([FromBody] OrderDetail model)
        {
            // Order insert to database
            publisher.Publish(JsonConvert.SerializeObject(model), "report.order", null);
        }
    }
}
