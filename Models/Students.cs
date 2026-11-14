using System.ComponentModel.DataAnnotations;

namespace Meaar5.Models
{
    public class Students
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; } = "";
    }
}