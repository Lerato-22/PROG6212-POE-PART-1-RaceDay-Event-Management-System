namespace RaceDay.DTOs
{
    public class CreateEventDTO
    {
        public string EventName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime EventDate { get; set; }
        public string Location { get; set; } = string.Empty;
        public decimal Distance { get; set; }
        public string EventType { get; set; } = string.Empty;
    }

    public class UpdateEventDTO
    {
        public string EventName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime EventDate { get; set; }
        public string Location { get; set; } = string.Empty;
        public decimal Distance { get; set; }
        public string EventType { get; set; } = string.Empty;
    }

    public class EventResponseDTO
    {
        public int EventId { get; set; }
        public int OrganiserId { get; set; }
        public string OrganizerName { get; set; } = string.Empty;
        public string EventName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime EventDate { get; set; }
        public string Location { get; set; } = string.Empty;
        public decimal Distance { get; set; }
        public string EventType { get; set; } = string.Empty;
        public string? BannerImageUrl { get; set; }
        public DateTime CreatedAt { get; set; }
        public int TotalEnrolments { get; set; }
        public List<CategoryResponseDTO> Categories { get; set; } = new List<CategoryResponseDTO>();
    }
}
