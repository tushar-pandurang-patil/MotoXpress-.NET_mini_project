using Microsoft.AspNetCore.Identity;

namespace MotoXpress3.Data
{
    public class ApplicationUser : IdentityUser
    {
        public string? Name { get; set; }  
        public String? ProfilePicture { get; set; }
    }
}
