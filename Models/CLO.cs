using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Meaar5.Models
{
    public class CLO
    {
        [Key]
        public int CLOId { get; set; }

        [Required]
        public string CLOCode { get; set; } = "";

        [Required]
        public string Description { get; set; } = "";
        public string Type { get; set; } = "";



    }
}