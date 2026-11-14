using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Meaar5.Models
{
    // Junction table: links a Course to its CLOs
    public class CourseCLO
    {
        [Key]
        public int CourseCLOId { get; set; }

        [Required]
        public int CourseId { get; set; }

        [Required]
        public int CLOId { get; set; }

        // Navigation properties
        [ForeignKey("CourseId")]
        public CourcesData Courase { get; set; }

        [ForeignKey("CLOId")]
        public CLO? CLO { get; set; }
    }
}