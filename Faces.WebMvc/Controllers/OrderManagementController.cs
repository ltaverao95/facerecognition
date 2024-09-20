namespace Faces.WebMvc.Controllers
{
    using Faces.WebMvc.Core.Repositories;
    using Microsoft.AspNetCore.Mvc;

    public class OrderManagementController : Controller
    {
        private readonly ILogger<OrderManagementController> logger;
        private readonly IOrdersRepository ordersRepository;

        public OrderManagementController(ILogger<OrderManagementController> logger, IOrdersRepository ordersRepository)
        {
            this.logger = logger;
            this.ordersRepository = ordersRepository;
        }

        public async Task<IActionResult> Index()
        {
            var orders = await this.ordersRepository.GetOrders();

            return View(orders);
        }

        [Route("/Details/{orderId}")]
        public async Task<IActionResult> Details(string orderId)
        {
            var order = await this.ordersRepository.GetByOrderId(orderId);

            return View(order);
        }
    }
}
