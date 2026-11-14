using Meaar5.Data;
using Meaar5.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using static System.Collections.Specialized.BitVector32;
namespace Meaar5.Controllers
{


    public class GradesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public GradesController(ApplicationDbContext context)
        {
            _context = context;
        }
        // GET method to display the file upload page for a specific course, exam type, and section
        public IActionResult Upload(int courseId, string examType, int sectionId)
        {
            ViewBag.CourseId = courseId;
            ViewBag.SectionId = sectionId;
            ViewBag.ExamType = examType;

            var clos = _context.CourseCLO
                .Where(c => c.CourseId == courseId)
                .Select(c => c.CLO)
                .ToList();

            ViewBag.CLOs = clos;

            var questions = _context.Questions
                .Where(q => q.CourseId == courseId && q.ExamType == examType)
                .ToList();

            if (questions.Any())
            {
                ViewBag.QuestionCount = questions.Count;
                ViewBag.SavedQuestions = questions;
            }
            else
            {
                ViewBag.QuestionCount = null;
            }

            return View();
        }

        // POST method to handle the file upload and extract question information for a specific course, exam type, and section
        [HttpPost]
        public IActionResult Upload(IFormFile file, string examType, int courseId, int sectionId)
        {
            if (file == null)
            {
                ViewBag.Error = "Please upload a file";
                return View();
            }

            var extension = Path.GetExtension(file.FileName);

            if (extension != ".xlsx" && extension != ".xls")
            {
                ViewBag.Error = "Upload Excel file only";
                return View();
            }

            var folderPath = Path.Combine("wwwroot", "files");
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var filePath = Path.Combine(folderPath, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                file.CopyTo(stream);
            }

            HttpContext.Session.SetString("FilePath", filePath);
            using (var package = new ExcelPackage(new FileInfo(filePath)))
            {
                var worksheet = package.Workbook.Worksheets[0];

                int questionCount = 0;

                for (int col = 1; col <= worksheet.Dimension.End.Column; col++)
                {
                    var header = worksheet.Cells[1, col].Text.Trim().ToLower();

                    if (header.Contains("num") && header.Contains("question"))
                    {
                        int.TryParse(worksheet.Cells[2, col].Text, out questionCount);
                        break;
                    }
                }

                ViewBag.QuestionCount = questionCount;
            }

            //  CLOs
            var clos = _context.CourseCLO
                .Where(c => c.CourseId == courseId)
                .Select(c => c.CLO)
                .ToList();

            ViewBag.CLOs = clos;

            ViewBag.CourseId = courseId;
            ViewBag.SectionId = sectionId;
            ViewBag.ExamType = examType;

            return View();
        }

        // POST method to save the classification of questions and their corresponding grades for a specific course, exam type, and section
        [HttpPost]
        public IActionResult SaveClassification(List<QuestionInputModel> questions, int courseId, string examType, int sectionId)
        {
            var oldQuestions = _context.Questions
                .Where(q => q.CourseId == courseId && q.ExamType == examType)
                .ToList();

            _context.Questions.RemoveRange(oldQuestions);
            _context.SaveChanges();

            // ✅ حفظ الأسئلة الجديدة
            foreach (var q in questions)
            {
                var question = new Questions
                {
                    CourseId = courseId,
                    ExamType = examType,
                    QuestionNumber = q.QuestionNumber,
                    CLO = q.CLO
                };

                _context.Questions.Add(question);
            }

            _context.SaveChanges();

            // after saving questions, we need to save grades from the uploaded file
            var filePath = HttpContext.Session.GetString("FilePath");
            if (!string.IsNullOrEmpty(filePath))
            {
                using (var package = new ExcelPackage(new FileInfo(filePath)))
                {
                    var worksheet = package.Workbook.Worksheets[0];

                    SaveGrades(worksheet, courseId, examType, sectionId);
                }
            }

            return RedirectToAction("Analysis", "Analysis", new { sectionId = sectionId, examType = examType });
        }
        // Method to save grades from the uploaded Excel file for a specific course, exam type, and section
        public void SaveGrades(ExcelWorksheet worksheet, int courseId, string examType, int sectionId)
        {
            var questionIds = _context.Questions
     .Where(q => q.CourseId == courseId &&
                 q.ExamType.ToLower() == examType.ToLower())
     .Select(q => q.Id)
     .ToList();
            var oldGrades = _context.Grades
     .Where(g => g.SectionId == sectionId &&
                 _context.Questions
                     .Any(q => q.Id == g.QuestionId &&
                               q.ExamType.ToLower() == examType.ToLower()))
     .ToList();

            _context.Grades.RemoveRange(oldGrades);
            _context.SaveChanges();

            for (int row = 2; row <= worksheet.Dimension.End.Row; row++)
            {
                int studentId = row;

                for (int col = 11; col <= worksheet.Dimension.End.Column; col++)
                {
                    int questionNumber = col - 10;

                    var question = _context.Questions.FirstOrDefault(q =>
                        q.CourseId == courseId &&
                        q.ExamType.ToLower() == examType.ToLower() &&
                        q.QuestionNumber == questionNumber);

                    if (question == null) continue;

                    // قراءة 
                    int score = 0;
                    int.TryParse(worksheet.Cells[row, col].Text, out score);

                    _context.Grades.Add(new Grades
                    {
                        StudentId = studentId,
                        QuestionId = question.Id,
                        Score = score,
                        SectionId = sectionId
                    });
                }
            }

            _context.SaveChanges();
        }

    } }
// Input model to represent the question number and its corresponding CLO for saving classifications
public class QuestionInputModel
    {
        public int QuestionNumber { get; set; }
        public string CLO { get; set; }
    }
