using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace HelpDeskProject.Models
{
    public class ActivityLog
    {
        [Key]
        public int LogID { get; set; }

        public string Action { get; set; }

        public DateTime Timestamp { get; set; } = DateTime.Now;

        public string BrowserInfo { get; set; }

        public string IPAddress { get; set; }

        [ForeignKey("User")]
        public string UserID { get; set; }

        public ApplicationUser User { get; set; }
    }
}
