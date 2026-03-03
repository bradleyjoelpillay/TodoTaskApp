
using Microsoft.AspNetCore.Identity;

namespace TaskApp.Domain.Entities
{
    public sealed class AppUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
