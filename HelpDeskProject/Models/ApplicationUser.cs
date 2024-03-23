using Microsoft.AspNetCore.Identity;
using Microsoft.Build.Framework;
using System.Net.Sockets;

namespace HelpDeskProject.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Gender { get; set; }

        public string FulllName => $"{FirstName} {MiddleName} {LastName}";
             

    }
}
