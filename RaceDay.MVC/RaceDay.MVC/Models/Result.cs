namespace RaceDay.MVC.Models
{
    public class ResultModel
    {
        public int ResultId { get; set; }
        public int EnrolmentId { get; set; }
        public string ParticipantName { get; set; }
        public string EventName { get; set; }
        public DateTime EventDate { get; set; }
        public string CategoryName { get; set; }
        public TimeSpan FinishTime { get; set; }
        public int FinishingPosition { get; set; }
        public int TotalFinishers { get; set; }
    }

    public class ResultCreateModel
    {
        public int EnrolmentId { get; set; }
        public TimeSpan FinishTime { get; set; }
        public int FinishingPosition { get; set; }
        public int TotalFinishers { get; set; }
    }
}
