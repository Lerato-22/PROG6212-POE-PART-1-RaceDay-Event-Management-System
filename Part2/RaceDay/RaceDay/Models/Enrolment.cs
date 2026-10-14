using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RaceDay.Models
{
    public class Enrolment
    {
        [Key]
        public int EnrolmentId { get; set; }

        [Required]
        public int ParticipantId { get; set; }

        [Required]
        public int EventId { get; set; }

        [Required]
        public int CategoryId { get; set; }

        [Required]
        [MaxLength(20)]
        [RegularExpression("^(Pending|Confirmed|Cancelled)$", ErrorMessage = "Status must be Pending, Confirmed, or Cancelled.")]
        public string Status { get; set; } = "Pending";

        public DateTime EnrolledAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        [ForeignKey("ParticipantId")]
        public User? Participant { get; set; }

        [ForeignKey("EventId")]
        public Event? Event { get; set; }

        [ForeignKey("CategoryId")]
        public Category? Category { get; set; }

        public Result? Result { get; set; }
    }
}
