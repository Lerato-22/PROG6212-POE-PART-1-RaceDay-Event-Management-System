using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RaceDay.Models
{
    public class Event
    {
        [Key]
        public int EventId { get; set; }

        [Required]
        public int OrganiserId { get; set; }

        [Required]
        [MaxLength(200)]
        public string EventName { get; set; } = string.Empty;

        [MaxLength(2000)]
        public string? Description { get; set; }

        [Required]
        public DateTime EventDate { get; set; }

        [Required]
        [MaxLength(300)]
        public string Location { get; set; } = string.Empty;

        [Required]
        [Range(0.1, 1000.0, ErrorMessage = "Distance must be between 0.1 and 1000 km.")]
        public decimal Distance { get; set; }

        [Required]
        [MaxLength(20)]
        public string EventType { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string? BannerImageUrl { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        [ForeignKey("OrganiserId")]
        public User? Organiser { get; set; }
        public ICollection<Category> Categories { get; set; } = new List<Category>();
        public ICollection<Enrolment> Enrolments { get; set; } = new List<Enrolment>();
    }
}
