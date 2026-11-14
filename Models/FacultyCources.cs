using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Meaar5.Models
{
    public class FacultyCources
    {
        
        [Key]
        public int Id { get; set; }          // Primary Key للجدول نفسه

        [Required]
        public int FacultyId { get; set; }   
        public int CourseId { get; set; }    // FK → Courses

        // Navigation Properties (عشان EF يعرف العلاقة)
        [ForeignKey("FacultyId")]
        public FacultyMembers FacultyMember { get; set; }
        public CourcesData Course { get; set; }
        public string Section { get; set; } = "";


        public bool IsSubmitted { get; set; } = false;


    }
}

