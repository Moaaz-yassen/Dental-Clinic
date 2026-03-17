using System.Diagnostics;
using Dental_Clinic.Data;
using Dental_Clinic.Models;
using Dental_Clinic.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Dental_Clinic.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly IQRCodeService _qrCodeService;
        private readonly IConfiguration _configuration;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context, IQRCodeService qrCodeService, IConfiguration configuration)
        {
            _logger = logger;
            _context = context;
            _qrCodeService = qrCodeService;
            _configuration = configuration;
        }

        public async Task<IActionResult> Index()
        {
            var cases = await _context.TreatmentCases.ToListAsync();
            // Generate QR Code for the homepage using the configured ClinicUrl
            var baseUrl = _configuration.GetValue<string>("ClinicUrl") ?? $"{this.Request.Scheme}://{this.Request.Host}{this.Request.PathBase}";
            var qrCodeUrl = _qrCodeService.GenerateQRCode(baseUrl);

            var vm = new HomeViewModel
            {
                Cases = cases,
                QRCodeBase64 = qrCodeUrl
            };

            return View(vm);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
