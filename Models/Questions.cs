using System.ComponentModel.DataAnnotations;

namespace Meaar5.Models
{
    public class Questions
    {
        [Key]
        
            public int Id { get; set; }

            public int CourseId { get; set; }

            public string ExamType { get; set; } = "";

            public int QuestionNumber { get; set; }

            public string CLO { get; set; } = "";
        public ICollection<Grades> Grades { get; set; }

    }
}
