namespace userscan.Models
{
    public class Contact
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string FullAddress { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Cell { get; set; }
        public DateTime RegistrationDate { get; set; }
        public int Age { get; set; }
        public string ImageUrl { get; set; }
    }
}
