using Meaar5.Data;
using Meaar5.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Meaar5.Controllers
{
    public class ManageCourcesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ManageCourcesController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult ManageCources()
        {
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Index(int? FacultyId, string college, string department)
        {
            // default values
            college ??= "College of Computer Science and Engineering";
            department ??= "Cybersecurity";

            var faculty = _context.FacultyMembers.ToList();

            if (!(college == "College of Computer Science and Engineering" &&
                  department == "Cybersecurity"))
            {
                faculty = new List<FacultyMembers>();
            }

            ViewBag.FacultyMembers = faculty;
            ViewBag.Courses = _context.Courses.ToList();

            ViewBag.Colleges = new List<string>
    {
        "College of Computer Science and Engineering",
        "College of Business",
        "College of Medicine"
    };

            ViewBag.Departments = new List<string>
    {
        "Cybersecurity",
        "Information Systems",
        "Artificial Intelligence"
    };

            ViewBag.SelectedCollege = college;
            ViewBag.SelectedDepartment = department;

            if (FacultyId != null)
            {
                var assigned = _context.FacultyCources
                    .Include(x => x.Course)
                    .Where(x => x.FacultyId == FacultyId)
                    .ToList();

                var doc = _context.FacultyMembers
                    .FirstOrDefault(x => x.FacultyId == FacultyId);

                ViewBag.DoctorName = doc == null ? "" : $"{doc.FirstName} {doc.LastName}";

                return View("ManageCources", assigned);
            }

            return View("ManageCources", new List<FacultyCources>());
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Assign(int FacultyId, int CourseId, string Section)
        {
            if (FacultyId == 0)
                return RedirectToAction("Index");

            bool exists = _context.FacultyCources
                .Any(x => x.FacultyId == FacultyId &&
                          x.CourseId == CourseId &&
                          x.Section == Section);

            if (!exists)
            {
                _context.FacultyCources.Add(new FacultyCources
                {
                    FacultyId = FacultyId,
                    CourseId = CourseId,
                    Section = Section
                });

                _context.SaveChanges();
            }

            return RedirectToAction("Index", new { FacultyId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Remove(int id, int FacultyId)
        {
            var assignment = _context.FacultyCources.Find(id);

            if (assignment != null)
            {
                _context.FacultyCources.Remove(assignment);
                _context.SaveChanges();
            }

            return RedirectToAction("Index", new { FacultyId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RemoveSelected(int FacultyId, List<int> selectedIds)
        {
            if (selectedIds == null || selectedIds.Count == 0)
                return RedirectToAction("Index", new { FacultyId });

            var rows = _context.FacultyCources
                .Where(x => selectedIds.Contains(x.Id))
                .ToList();

            _context.FacultyCources.RemoveRange(rows);
            _context.SaveChanges();

            return RedirectToAction("Index", new { FacultyId });
        }

        public IActionResult ViewReports(int? courseId, string college, string department)
        {
            // Default selected values
            college ??= "College of Computer Science and Engineering";
            department ??= "Cybersecurity";

            var courses = _context.Courses.ToList();

            var facultyCourses = _context.FacultyCources
                .Include(f => f.FacultyMember)
                .Include(f => f.Course)
                .Where(f => courseId == null || f.CourseId == courseId)
                .ToList();

           
            if (!(college == "College of Computer Science and Engineering" &&
                  department == "Cybersecurity"))
            {
                facultyCourses = new List<FacultyCources>();
                courses = new List<CourcesData>();
            }

            ViewBag.Courses = courses;
            ViewBag.SelectedCourseId = courseId;

            ViewBag.Colleges = new List<string>
    {
        "College of Computer Science and Engineering",
        "College of Business",
        "College of Medicine"
    };

            ViewBag.Departments = new List<string>
    {
        "Cybersecurity",
        "Information Systems",
        "Artificial Intelligence"
    };

            ViewBag.SelectedCollege = college;
            ViewBag.SelectedDepartment = department;

            return View(facultyCourses);
        }



        public IActionResult SubmitReport(int id)
        {
            var item = _context.FacultyCources.Find(id);

            if (item != null)
            {
                item.IsSubmitted = true;
                _context.SaveChanges();
            }

            return RedirectToAction("ViewReports");
        }


    }
}