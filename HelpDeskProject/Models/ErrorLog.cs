using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace HelpDeskProject.Models
{
    public class ErrorLog
    {
        [Key]
        public int ErrorID { get; set; }

        [ForeignKey("User")]
        public string UserID { get; set; } 

        public string ErrorDescription { get; set; }

        public int LineNumber { get; set; }

        public string Traceback { get; set; }

        public DateTime Timestamp { get; set; } = DateTime.Now;

        public string BrowserInfo { get; set; }

        public string IPAddress { get; set; }

        public ApplicationUser User { get; set; }
    }
}
