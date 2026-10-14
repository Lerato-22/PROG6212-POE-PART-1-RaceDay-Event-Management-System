namespace RaceDay.DTOs
{
    public class CreateEnrolmentDTO
    {
        public int EventId { get; set; }
        public int CategoryId { get; set; }
    }

    public class UpdateEnrolmentStatusDTO
    {
        public string Status { get; set; } = string.Empty;
    }

    public class EnrolmentResponseDTO
    {
        public int EnrolmentId { get; set; }
        public int ParticipantId { get; set; }
        public string ParticipantName { get; set; } = string.Empty;
        public int EventId { get; set; }
        public string EventName { get; set; } = string.Empty;
        public DateTime EventDate { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime EnrolledAt { get; set; }
    }
}
