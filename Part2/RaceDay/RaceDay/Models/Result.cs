using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RaceDay.Models
{
    public class Result
    {
        [Key]
        public int ResultId { get; set; }

        [Required]
        public int EnrolmentId { get; set; }

        [Required]
        [MaxLength(20)]
        public string FinishTime { get; set; } = string.Empty;

        [Required]
        [Range(1, 100000, ErrorMessage = "Finishing position must be a positive number.")]
        public int FinishingPosition { get; set; }

        [Required]
        [Range(1, 100000, ErrorMessage = "Total finishers must be a positive number.")]
        public int TotalFinishers { get; set; }

        public DateTime RecordedAt { get; set; } = DateTime.UtcNow;

        // Navigation property
        [ForeignKey("EnrolmentId")]
        public Enrolment? Enrolment { get; set; }
    }
}
