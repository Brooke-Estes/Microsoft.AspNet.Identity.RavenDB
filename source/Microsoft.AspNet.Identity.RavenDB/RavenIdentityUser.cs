namespace Microsoft.AspNet.Identity.RavenDB
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// A user entity that implements the <see cref="IUser"/> interface.
    /// </summary>
    public class RavenIdentityUser : IUser
    {
        public string Id { get; private set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public bool EmailConfirmed { get; set; }
        public string PasswordHash { get; set; }
        public string SecurityStamp { get; set; }
        public DateTime? LockoutEndDateUtc { get; set; }
        public bool LockoutEnabled { get; set; }
        public int AccessFailedCount { get; set; }
        public List<string> Roles { get; set; }
        public List<RavenIdentityUserLogin> Logins { get; set; }
        public List<RavenIdentityUserClaim> Claims { get; set; } 

    }
}