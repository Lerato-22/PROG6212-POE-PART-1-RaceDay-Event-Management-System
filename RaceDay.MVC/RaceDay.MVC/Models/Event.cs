using System.Text.Json.Serialization;

namespace RaceDay.MVC.Models
{
    public class EventModel
    {
        [JsonPropertyName("eventId")]
        public int EventId { get; set; }

        [JsonPropertyName("eventName")]
        public string Name { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("eventDate")]
        public DateTime Date { get; set; }

        [JsonPropertyName("location")]
        public string Location { get; set; }

        [JsonPropertyName("distance")]
        public decimal Distance { get; set; }

        [JsonPropertyName("eventType")]
        public string EventType { get; set; }

        [JsonPropertyName("bannerImageUrl")]
        public string BannerImageUrl { get; set; }

        public string BannerImage { get; set; } = "";

        public int TotalEnrolments { get; set; }
    }

    public class EventCreateModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }
        public string Location { get; set; }
        public decimal Distance { get; set; }
        public string EventType { get; set; }
        public string BannerImage { get; set; } = "";
    }
}