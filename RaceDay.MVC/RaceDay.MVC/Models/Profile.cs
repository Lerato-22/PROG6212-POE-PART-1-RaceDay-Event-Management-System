namespace RaceDay.MVC.Models
{
    public class ProfileModel
    {
        public int UserId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string ProfilePictureUrl { get; set; }
        public string Role { get; set; }
    }

    public class ProfileUpdateModel
    {
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public IFormFile ProfilePicture { get; set; }
    }
}
