namespace Meaar5.Models
{
    public class CLOReportViewModel
    {
        
            public string Description { get; set; }
            public string Type { get; set; }
            public string AssessmentMethod { get; set; } = "Exams";
            public double Target { get; set; } = 70;
            public double Actual { get; set; }
            public string? Comment { get; set; }
        
    }
}
