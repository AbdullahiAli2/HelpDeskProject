using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace HelpDeskProject.Models
{
    public class Ticket
    {
        [Key]
        public int TicketID { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; }

        public string Description { get; set; }

        [Required]
        public string Status { get; set; }

        [Required]
        public string Priority { get; set; }

        [ForeignKey("CreatedBy")]
        public string CreatedById { get; set; }

        public ApplicationUser CreatedBy { get; set; }

        public DateTime CreatedOn { get; set; } = DateTime.Now;
    }
}
