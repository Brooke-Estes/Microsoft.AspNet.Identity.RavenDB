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
        public virtual DateTime? LockoutEndDateUtc { get; set; }
        public virtual bool LockoutEnabled { get; set; }
        public virtual int AccessFailedCount { get; set; }
        public virtual List<string> Roles { get; set; }
         
    }
}