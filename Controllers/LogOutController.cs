using Microsoft.AspNetCore.Mvc;

namespace Meaar5.Controllers
{
    public class LogOutController : Controller
    {
        [HttpPost]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();


            return RedirectToAction("Login1", "Login1");
        }
    }
}
