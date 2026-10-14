namespace RaceDay.DTOs
{
    public class CreateCategoryDTO
    {
        public string CategoryName { get; set; } = string.Empty;
        public string? Description { get; set; }
    }

    public class UpdateCategoryDTO
    {
        public string CategoryName { get; set; } = string.Empty;
        public string? Description { get; set; }
    }

    public class CategoryResponseDTO
    {
        public int CategoryId { get; set; }
        public int EventId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}