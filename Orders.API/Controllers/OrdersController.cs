namespace Orders.API.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Orders.Core.Persistence.Repository;

    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly ILogger<OrdersController> logger;
        private readonly IOrderRepository orderRepository;

        public OrdersController(ILogger<OrdersController> logger, IOrderRepository orderRepository)
        {
            this.logger = logger;
            this.orderRepository = orderRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync() 
        {
            try
            {
                var data = await this.orderRepository.GetAllOrdersAsync();

                return Ok(data);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Error getting all orders");
                throw;
            }
        }

        [HttpGet]
        [Route("{orderId}", Name = "GetOrderById")]
        public async Task<IActionResult> GetOrderById(string orderId)
        {
            try
            {
                var data = await this.orderRepository.GetOrderByIdAsync(Guid.Parse(orderId));
                if (data == null)
                {
                    return NotFound();
                }

                return Ok(data);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Error getting order by id");
                throw;
            }
        }
    }
}