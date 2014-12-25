namespace Microsoft.AspNet.Identity.RavenDB
{
    public class RavenIdentityUserLogin
    {
        public string LoginProvider { get; set; }
        public string ProviderKey { get; set; }
        public string UserId { get; set; } 
    }
}