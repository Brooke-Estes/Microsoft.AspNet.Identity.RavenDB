namespace Microsoft.AspNet.Identity.RavenDB
{
    public class RavenIdentityUserClaim
    {
        public string UserId { get; set; }
        public string ClaimType { get; set; }
        public string ClaimValue { get; set; }
    }
}