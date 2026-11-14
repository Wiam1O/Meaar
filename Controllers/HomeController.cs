using Meaar5.Data;
using Meaar5.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace Meaar5.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }
        // to return the view for the home page
        public IActionResult HomePage()
        {
            return View();
        }
        public IActionResult Login()
        {
            return View();
        }



        // to return the view for the privacy page
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        //To view the page for the users 
        public IActionResult CreateAccount(string role)
        {
            ViewBag.Role = role;
            return View();
        }



        //To receive the data after Register



        [HttpPost]
        public IActionResult CreateAccount(User newUser, string role)
        {
            if (ModelState.IsValid)
            {
                if (role == "Faculty")
                {
                    var faculty = new FacultyMembers
                    {
                        FirstName = newUser.FirstName,
                        LastName = newUser.LastName,
                        Email = newUser.Email,
                        Password = newUser.Password
                    };

                    _context.FacultyMembers.Add(faculty);
                }
                else if (role == "DepartmentHead")
                {
                    var head = new DepartmentHead
                    {
                        FirstName = newUser.FirstName,
                        LastName = newUser.LastName,
                        Email = newUser.Email,
                        Password = newUser.Password
                    };

                    _context.DepartmentHeads.Add(head);
                }
                else if (role == "Admin")
                {
                    var admin = new Admin
                    {
                        FirstName = newUser.FirstName,
                        LastName = newUser.LastName,
                        Email = newUser.Email,
                        Password = newUser.Password
                    };

                    _context.Admins.Add(admin);
                }

                _context.SaveChanges();

                return RedirectToAction("Login1", "Login1", new { role = role });
            }

            return View(newUser);
        }



        
        public IActionResult Courses()
        {
            var facultyIdStr = HttpContext.Session.GetString("FacultyId");

            int facultyId = 0;

            if (!string.IsNullOrEmpty(facultyIdStr))
            {
                facultyId = int.Parse(facultyIdStr);
            }
            var courses = _context.FacultyCources
                .Include(x => x.Course)
               .Where(x => x.FacultyId == facultyId) .ToList();

            return View(courses);

        }
    }
}
   


    


