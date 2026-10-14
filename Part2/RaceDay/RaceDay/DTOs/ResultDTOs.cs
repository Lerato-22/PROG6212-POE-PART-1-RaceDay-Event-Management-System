namespace RaceDay.DTOs
{
    public class CreateResultDTO
    {
        public int EnrolmentId { get; set; }
        public string FinishTime { get; set; } = string.Empty;
        public int FinishingPosition { get; set; }
        public int TotalFinishers { get; set; }
    }

    public class UpdateResultDTO
    {
        public string FinishTime { get; set; } = string.Empty;
        public int FinishingPosition { get; set; }
        public int TotalFinishers { get; set; }
    }

    public class ResultResponseDTO
    {
        public int ResultId { get; set; }
        public int EnrolmentId { get; set; }
        public string ParticipantName { get; set; } = string.Empty;
        public string EventName { get; set; } = string.Empty;
        public DateTime EventDate { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string FinishTime { get; set; } = string.Empty;
        public int FinishingPosition { get; set; }
        public int TotalFinishers { get; set; }
        public DateTime RecordedAt { get; set; }
    }
}
