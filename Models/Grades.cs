using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Meaar5.Models
{
    public class Grades
    {
     
        public int Id { get; set; }

        public int StudentId { get; set; }
        public int QuestionId { get; set; }
        public int Score { get; set; }
        public int SectionId { get; set; }
        [ForeignKey("QuestionId")]
        public Questions Question { get; set; }
    }
}
