using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace AsyncAspNet.Controllers
{
    public class HomeController : Controller
    {
        public async Task<ActionResult> Index()
        {
            using (var client = new HttpClient())
            {
                // using async/await in ASP.NET, we're not blocking thread from doing other work
                // that means that the ASP.NET application can process more things
                // it's kind of a relief for the server
                // in ASP.NET apps it's best practice to call ConfigureAwait(false), not so in WPF or other application types
                // reason for this is it becomes a lot quicker because it will pick 
                // one of the threads in the thread pool instead of trying to get back to the one that was used first
                var httpMessage = await client.GetAsync("http://www.filipekberg.se/rss/").ConfigureAwait(false);
                var content = await httpMessage.Content.ReadAsStringAsync().ConfigureAwait(false);
                return Content(content);
            }
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}