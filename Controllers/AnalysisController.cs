using Meaar5.Data;
using Meaar5.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace Meaar5.Controllers
{
    public class AnalysisController : Controller
    {
        
        private readonly ApplicationDbContext _context;

        // Constructor to inject the database context
        public AnalysisController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET method to display the CLO management page for a specific course
        [HttpGet]
        public async Task<IActionResult> CLO(int courseId)
        {
            var course = await _context.Courses.FindAsync(courseId);
            if (course == null) return NotFound();

            var existingClos = await _context.CourseCLO
                .Where(c => c.CourseId == courseId)
                .Include(c => c.CLO)
                .ToListAsync();

            ViewBag.Course = course;
            ViewBag.CourseId = courseId;

            return View(existingClos);
        }

        // Save method to add new CLOs for a course
        [HttpPost]
        public async Task<IActionResult> Save(int courseId, List<CLOInputModel> clos)
        {
            if (clos != null && clos.Any())
            {
                foreach (var item in clos)
                {
                    // Ignore empty entries
                    if (string.IsNullOrWhiteSpace(item.CLOCode) && string.IsNullOrWhiteSpace(item.Description))
                        continue;

                    // إنشاء سجل جديد في جدول CLO
                    var newClo = new CLO
                    {
                        CLOCode = item.CLOCode ?? "",
                        Description = item.Description ?? "",
                        Type = item.ProgramAlignment ?? ""

                    };

                    _context.CLO.Add(newClo);
                    await _context.SaveChangesAsync();

                    // ربط الـ CLO بالكورس في جدول CourseCLO
                    var courseClo = new CourseCLO
                    {
                        CourseId = courseId,
                        CLOId = newClo.CLOId
                    };
                    _context.CourseCLO.Add(courseClo);
                }
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("CLO", new { courseId = courseId });
        }

        // Delete method to remove a CLO 
        [HttpPost]
        public async Task<IActionResult> Delete(int id, int courseId)
        {
            var courseCLO = await _context.CourseCLO
                .Include(cc => cc.CLO)
                .FirstOrDefaultAsync(cc => cc.CourseCLOId == id);

            if (courseCLO != null)
            {
                if (courseCLO.CLO != null)
                    _context.CLO.Remove(courseCLO.CLO);

                _context.CourseCLO.Remove(courseCLO);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("CLO", new { courseId = courseId });
        }




        // GET method to display the analysis page for a specific section and exam type
        public IActionResult Analysis(int sectionId, string examType)
        {
            var section = _context.FacultyCources
                .FirstOrDefault(x => x.Id == sectionId);

            if (section == null)
                return NotFound();

            int courseId = section.CourseId;

            ViewBag.Section = section.Section;
            ViewBag.SectionId = sectionId;
            ViewBag.CourseId = courseId;

            var exams = _context.Grades
                .Join(_context.Questions,
                      g => g.QuestionId,
                      q => q.Id,
                      (grade, question) => new
                      {
                          question.CourseId,
                          question.ExamType,
                          grade.SectionId
                      })
                .Where(x => x.CourseId == courseId && x.SectionId == sectionId)
                .Select(x => x.ExamType)
                .Distinct()
                .ToList();

            ViewBag.Exams = exams;

            if (string.IsNullOrEmpty(examType))
                return View();

            ViewBag.SelectedExam = examType;

            var data = _context.Grades
                .Join(_context.Questions,
                      g => g.QuestionId,
                      q => q.Id,
                      (grade, question) => new
                      {
                          question.CourseId,
                          question.ExamType,
                          CLOId = question.CLO,
                          grade.Score,
                          grade.SectionId
                      })
                .Where(x => x.CourseId == courseId &&
                            x.ExamType == examType &&
                            x.SectionId == sectionId)
                .ToList(); 

            if (!data.Any())
            {
                ViewBag.NoData = true;
                return View();
            }

            var result = data
                .Select(x =>
                {
                    int id;
                    string cloName = "Unknown";

                    if (int.TryParse(x.CLOId, out id))
                    {
                        var clo = _context.CLO.FirstOrDefault(c => c.CLOId == id);
                        if (clo != null)
                            cloName = clo.CLOCode + " - " + clo.Description; // ✨ الشكل الاحترافي
                    }

                    return new
                    {
                        CLO = cloName,
                        x.Score
                    };
                })
                .GroupBy(x => x.CLO)
                .Select(g => new
                {
                    CLO = g.Key,
                    Percentage = (g.Sum(x => x.Score) * 100.0) / g.Count(),
                    Status = (g.Sum(x => x.Score) * 100.0) / g.Count() >= 70
                             ? "Achieved"
                             : "Not Achieved"
                })
                .ToList();

            ViewBag.Result = result;

            return View();
        }
        // GET method to display the all exam analysis for a specific section
        public IActionResult AllAnalysis(int sectionId)
        {
            var section = _context.FacultyCources
                .FirstOrDefault(x => x.Id == sectionId);

            if (section == null)
                return NotFound();

            int courseId = section.CourseId;

            ViewBag.Section = section.Section;
            ViewBag.SectionId = sectionId;
            ViewBag.IsTotal = true; 

            var data = _context.Grades
                .Join(_context.Questions,
                      g => g.QuestionId,
                      q => q.Id,
                      (grade, question) => new
                      {
                          question.CourseId,
                          CLOId = question.CLO,
                          grade.Score,
                          grade.SectionId
                      })
                .Where(x => x.CourseId == courseId &&
                            x.SectionId == sectionId)
                .ToList();

            if (!data.Any())
            {
                ViewBag.NoData = true;
                return View("Analysis"); 
            }

            var result = data
                .Select(x =>
                {
                    int id;
                    string cloName = "Unknown";

                    if (int.TryParse(x.CLOId, out id))
                    {
                        var clo = _context.CLO.FirstOrDefault(c => c.CLOId == id);
                        if (clo != null)
                            cloName = clo.CLOCode + " - " + clo.Description;
                    }

                    return new
                    {
                        CLO = cloName,
                        x.Score
                    };
                })
                .GroupBy(x => x.CLO)
                .Select(g => new
                {
                    CLO = g.Key,
                    Percentage = (g.Sum(x => x.Score) * 100.0) / g.Count(),
                    Status = (g.Sum(x => x.Score) * 100.0) / g.Count() >= 70
                             ? "Achieved"
                             : "Not Achieved"
                })
                .ToList();

            ViewBag.Result = result;

            return View("Analysis"); 
        }


        // Input model to capture CLO details from the form
        public class CLOInputModel
        {
            public string? CLOCode { get; set; }
            public string? Description { get; set; }
            public string? ProgramAlignment { get; set; }

        }
    }
}
