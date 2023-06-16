using Microsoft.AspNetCore.Mvc;

namespace PortableHttpServer.Controllers
{
    public sealed class ConvertController : Controller
    {
        private readonly ILogger<ConvertController> _logger;
        private readonly Config _config;

        public ConvertController(
            ILogger<ConvertController> logger,
            Config config
        )
        {
            _logger = logger;
            _config = config;
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
