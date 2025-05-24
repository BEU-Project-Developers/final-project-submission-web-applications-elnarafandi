using Microsoft.AspNetCore.Identity;

namespace Fashion.Models
{
    public class AppUser:IdentityUser
    {
        public string FullName { get; set; }
    }
}
