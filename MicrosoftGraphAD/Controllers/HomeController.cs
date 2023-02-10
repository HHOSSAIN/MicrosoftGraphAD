using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Graph;
using Microsoft.Identity.Web;
using MicrosoftGraphAD.Models;
using System.Diagnostics;

namespace MicrosoftGraphAD.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ITokenAcquisition _tokenAcquisition;

        public HomeController(ILogger<HomeController> logger, ITokenAcquisition tokenAcquisition)
        {
            _logger = logger;
            _tokenAcquisition = tokenAcquisition;
        }

        public IActionResult Index()
        {
            var token = _tokenAcquisition.GetAccessTokenForUserAsync(new string[] { "user.read", "presence.read", "mailboxsettings.read", "mail.read" }).Result;
            GraphServiceClient graphClient = new GraphServiceClient("https://graph.microsoft.com/v1.0",
                new DelegateAuthenticationProvider(
                        request =>
                        {
                            request.Headers.Authorization = new System.Net.Http.Headers
                            .AuthenticationHeaderValue("bearer", token);
                            return Task.CompletedTask;
                        })
                );
            return View();
        }

        [Authorize]
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