using Meaar5.Data;
using Meaar5.Models;
using Microsoft.AspNetCore.Mvc;
using Rotativa.AspNetCore;
using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Meaar5.Controllers
{
    public class ReportsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReportsController(ApplicationDbContext context)
        {
            _context = context;
        }

        //  method to generate the report for a specific section, but it will show data for all sections of the same course
        public IActionResult CoursesReport(int sectionId)
        {
            var section = _context.FacultyCources
                .FirstOrDefault(x => x.Id == sectionId);

            if (section == null)
                return NotFound();

            int courseId = section.CourseId;
            ViewBag.SectionId = sectionId;

            var sectionIds = _context.FacultyCources
                .Where(x => x.CourseId == courseId)
                .Select(x => x.Id)
                .ToList();

            var course = _context.Courses
                .FirstOrDefault(c => c.CourseId == courseId);

            ViewBag.CourseName = course?.CourseName;
            ViewBag.CourseCode = course?.CourseCode;

            ViewBag.Section = "All Sections";

            var instructor = _context.FacultyMembers
                .FirstOrDefault(f => f.FacultyId == section.FacultyId);

            ViewBag.InstructorName = instructor != null
                ? instructor.FirstName + " " + instructor.LastName
                : "Unknown";

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
                            sectionIds.Contains(x.SectionId))
                .ToList();

            if (!data.Any())
            {
                ViewBag.NoData = true;
                return View();
            }

            var report = data
                .GroupBy(x => x.CLOId)
                .Join(_context.CLO,
                      g => g.Key,
                      c => c.CLOId.ToString(),
                      (g, clo) => new
                      {
                          Description = clo.Description,
                          Type = clo.Type,
                          Assessment = "Exams",
                          Target = 70,
                          Actual = (g.Sum(x => x.Score) * 100.0) / g.Count(),
                          Status = (g.Sum(x => x.Score) * 100.0) / g.Count() >= 70
                                   ? "Achieved"
                                   : "Not Achieved"
                      })
                .ToList();

            ViewBag.Report = report;

            return View();
        }
        private List<dynamic> GetReportData(int courseId)
        {
            var sectionIds = _context.FacultyCources
                .Where(x => x.CourseId == courseId)
                .Select(x => x.Id)
                .ToList();

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
                            sectionIds.Contains(x.SectionId))
                .ToList();

            if (!data.Any())
                return new List<dynamic>();

            var report = data
     .GroupBy(x => x.CLOId)  
     .GroupJoin(_context.CLO,
 g => int.Parse(g.Key), c => c.CLOId,
         (g, cloGroup) => new { g, cloGroup })
     .SelectMany(x => x.cloGroup.DefaultIfEmpty(),
         (x, clo) => new
         {
             Description = clo != null ? clo.Description : "N/A",
             Type = clo != null ? clo.Type : "N/A",
             Assessment = "Exams",
             Target = 70,

             Actual = x.g.Any()
                 ? (x.g.Sum(i => i.Score) * 100.0) / x.g.Count()
                 : 0,

             Status = x.g.Any() && ((x.g.Sum(i => i.Score) * 100.0) / x.g.Count() >= 70)
                 ? "Achieved"
                 : "Not Achieved"
         })
     .ToList<dynamic>();

            return report;
        }
        [HttpPost]
        public IActionResult DownloadPdf(int sectionId, string college, string department, string semester, string program, string date, string overall)
        {
            var section = _context.FacultyCources
                .FirstOrDefault(x => x.Id == sectionId);

            if (section == null)
                return NotFound();

            int courseId = section.CourseId;

            var course = _context.Courses
                .FirstOrDefault(c => c.CourseId == courseId);

            var instructor = _context.FacultyMembers
                .FirstOrDefault(f => f.FacultyId == section.FacultyId);

            ViewBag.CourseName = course?.CourseName;
            ViewBag.CourseCode = course?.CourseCode;
            ViewBag.Section = "All Sections";
            ViewBag.InstructorName = instructor?.FirstName + " " + instructor?.LastName;
            ViewBag.College = college;
            ViewBag.Department = department;
            ViewBag.Semester = semester;
            ViewBag.Program = program;
            ViewBag.Date = date;
            ViewBag.Overall = overall;


            ViewBag.Report = GetReportData(courseId);

            return new ViewAsPdf("PdfTemplate", new
            {
                CourseName = ViewBag.CourseName,
                CourseCode = course?.CourseCode,
                InstructorName = ViewBag.InstructorName,
                College = ViewBag.College,
                Department = ViewBag.Department,
                Semester = ViewBag.Semester,
                Program = ViewBag.Program,
                Overall = ViewBag.Overall,
                Date = ViewBag.Date,

                Report = ViewBag.Report
            })
            {
                FileName = "CourseReport.pdf",
                PageSize = Rotativa.AspNetCore.Options.Size.A4,

                CustomSwitches = "--enable-local-file-access --dpi 300 --margin-top 0 --margin-bottom 0 --margin-left 0 --margin-right 0 --zoom 1.1"

            };


        }
        
        
        [HttpPost]
        public IActionResult SubmitReport(int sectionId)
        {
            var section = _context.FacultyCources
                .FirstOrDefault(s => s.Id == sectionId);

            if (section == null)
                return NotFound();

            // تحقق من الدرجات
            var hasGrades = _context.Grades
                .Any(g => g.SectionId == sectionId);

            if (!hasGrades)
            {
                TempData["Error"] = "You must have a CourseReport !";
                return RedirectToAction("Courses", "Home");
            }

          
            section.IsSubmitted = true;
            _context.SaveChanges();

            TempData["Success"] = "Report submitted successfully!";
            return RedirectToAction("Courses" ,"Home");
        }
        public IActionResult DownloadReport(int sectionId)
        {
            var section = _context.FacultyCources
                .FirstOrDefault(x => x.Id == sectionId);

            if (section == null)
                return NotFound();

            if (!section.IsSubmitted)
                return BadRequest("Report not submitted yet");

            int courseId = section.CourseId;

            var course = _context.Courses
                .FirstOrDefault(c => c.CourseId == courseId);

            var instructor = _context.FacultyMembers
                .FirstOrDefault(f => f.FacultyId == section.FacultyId);

            var report = GetReportData(courseId);

            return new ViewAsPdf("PdfTemplate", new
            {
                CourseName = course?.CourseName,
                CourseCode = course?.CourseCode,
                InstructorName = instructor?.FirstName + " " + instructor?.LastName,
                Section = section.Section,
                Report = report,

                // أضيفيها عشان ما يطلع error
                College = "CCSE",
                Department = "Cybersecurity",
                Semester = "First",
                Program = "IS",
                Date = DateTime.Now.ToString("yyyy-MM-dd"),
                 Overall = 80
            })
            {
                FileName = $"Report_{course?.CourseCode}.pdf"
            };
        }
    }

}