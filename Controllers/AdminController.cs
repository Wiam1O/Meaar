using Meaar5.Models;
using Meaar5.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Meaar5.Data;
using System.Linq;



namespace Meaar5.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        // GET method to display the form for adding courses and the list of existing courses
        public IActionResult Addcources()
        {
            var courses = _context.Courses.ToList();
            return View(courses);
        }

        // POST method to add a new course
        [HttpPost]
        public IActionResult AddCourse(CourcesData course)
        {
            if (ModelState.IsValid)
            {
                var existingCourse = _context.Courses
                    .FirstOrDefault(c => c.CourseCode == course.CourseCode);

                if (existingCourse != null)
                {
                    ModelState.AddModelError("CourseCode", "This course already exists.");
                    var courses = _context.Courses.ToList();
                    return View("Addcources", courses);
                }

                _context.Courses.Add(course);
                _context.SaveChanges();

                return RedirectToAction("Addcources");
            }

            var list = _context.Courses.ToList();
            return View("Addcources", list);
        }
        // GET method to delete a course by its ID
        public IActionResult Delete(int id)
        {
            var course = _context.Courses.Find(id);

            if (course != null)
            {
                _context.Courses.Remove(course);
                _context.SaveChanges();
            }

            return RedirectToAction("Addcources");
            var model = _context.Courses.ToList();
            return View(model);
        }
    }
}