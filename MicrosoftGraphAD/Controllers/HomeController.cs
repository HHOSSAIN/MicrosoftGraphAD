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
        private readonly GraphServiceClient _graphServiceClient;

        public HomeController(ILogger<HomeController> logger, ITokenAcquisition tokenAcquisition, GraphServiceClient graphServiceClient)
        {
            _logger = logger;
            _tokenAcquisition = tokenAcquisition;
            _graphServiceClient = graphServiceClient;
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

        public async Task<bool> CallGraphAPI() //InitialiseAction
        {
            #region get all groups of org
            var groups = await _graphServiceClient.Groups.Request().Top(999)
               .GetAsync();
            Console.WriteLine($"groups= {groups}");
            //List<string> dls = new List<string>();

            Console.WriteLine($"Group Count= {groups.Count()}");
            foreach (var group in groups)
            {
                //checking if the group is a DL
                //if (group.GroupTypes.Count() == 0 && group.MailEnabled == true)
                {
                    //dls.Add(group.Id);
                    Console.WriteLine("emptyyyyy");
                    Console.WriteLine($"Group Id: {group.Id}");
                    Console.WriteLine($"Group Display Name: {group.DisplayName}");
                    Console.WriteLine($"Group Description: {group.Description}"); //groupTypes
                    Console.WriteLine($"Group Types: {group.GroupTypes}");

                }
                Console.WriteLine($"mailEnabled: {group.MailEnabled}");
            }
            //Console.WriteLine($"DL count = {dls.Count()}");
            #endregion

            return true;
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}