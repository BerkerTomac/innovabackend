using Microsoft.AspNetCore.Identity;

namespace MasrafTakip.Infrastructure.Identity
{
    public class ApplicationUser : IdentityUser
    {
        public string Name { get; set; }
    }
}
