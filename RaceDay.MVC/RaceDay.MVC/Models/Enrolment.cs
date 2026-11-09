using System.Text.Json.Serialization;

namespace RaceDay.MVC.Models
{
    public class EnrolmentModel
    {
        [JsonPropertyName("enrolmentId")]
        public int EnrolmentId { get; set; }

        [JsonPropertyName("eventId")]
        public int EventId { get; set; }

        [JsonPropertyName("eventName")]
        public string EventName { get; set; }

        [JsonPropertyName("eventDate")]
        public DateTime EventDate { get; set; }

        [JsonPropertyName("categoryId")]
        public int CategoryId { get; set; }

        [JsonPropertyName("categoryName")]
        public string CategoryName { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("participantId")]
        public int ParticipantId { get; set; }

        [JsonPropertyName("participantName")]
        public string ParticipantName { get; set; }
    }

    public class EnrolmentCreateModel
    {
        public int EventId { get; set; }
        public int CategoryId { get; set; }
    }
}