namespace Faces.WebMvc.Controllers
{
    using Faces.WebMvc.Core.Models.Order.Requests;
    using Faces.WebMvc.Core.Services;
    using Faces.WebMvc.Models;
    using Microsoft.AspNetCore.Mvc;
    using System.Diagnostics;

    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IRegisterOrderService registerOrderService;

        public HomeController(ILogger<HomeController> logger, IRegisterOrderService registerOrderService)
        {
            _logger = logger;
            this.registerOrderService = registerOrderService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult RegisterOrder()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RegisterOrder(OrderViewModel model)
        {
            try
            {
                using var uploadedFile = model.File.OpenReadStream();

                var orderId = await this.registerOrderService.Register(model.File.OpenReadStream(), new RegisterOrderRequest
                {
                    PictureUrl = model.File.FileName,
                    UserEmail = model.UserEmail
                });

                ViewData["OrderId"] = orderId;
                return View("Thanks");
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Error");
                throw;
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}