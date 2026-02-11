using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace ClientContactManager.Models
{
    [Index(nameof(ClientCode), IsUnique = true)]
    public class Client
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [Display(Name = "Name")]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(6, MinimumLength = 6)]
        [Display(Name = "Client Code")]
        public string ClientCode { get; set; } = string.Empty;

        // Navigation property
        public ICollection<ClientContact> ClientContacts { get; set; } = new List<ClientContact>();

        // Computed property
        public int LinkedContactsCount => ClientContacts?.Count ?? 0;
    }
}
