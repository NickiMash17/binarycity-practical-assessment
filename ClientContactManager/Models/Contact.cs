using System.ComponentModel.DataAnnotations;

namespace ClientContactManager.Models
{
    public class Contact
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [Display(Name = "Name")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Surname is required")]
        [Display(Name = "Surname")]
        public string Surname { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        // Navigation property
        public ICollection<ClientContact> ClientContacts { get; set; } = new List<ClientContact>();

        // Computed property
        public int LinkedClientsCount => ClientContacts?.Count ?? 0;

        // Display full name in [Surname] [Name] format
        public string FullName => $"{Surname} {Name}";
    }
}
