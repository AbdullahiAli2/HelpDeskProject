using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace HelpDeskProject.Models
{
    public class Comment
    {
        [Key]
        public int CommentID { get; set; }
        public string CommentText { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [ForeignKey("Ticket")]
        public int TicketID { get; set; }
        public Ticket Ticket { get; set; }

        [ForeignKey("User")]
        public string UserID { get; set; } // Assuming IdentityUser's ID is of type string
        public ApplicationUser User { get; set; }
    }
}
