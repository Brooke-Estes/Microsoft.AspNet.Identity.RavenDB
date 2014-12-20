namespace Microsoft.AspNet.Identity.RavenDB
{
    /// <summary>
    /// An identity role that implements the <see cref="IRole"/> interface.
    /// </summary>
    public class RavenIdentityRole : IRole
    {
        public RavenIdentityRole(string id)
        {
            Id = id;
        }

        public string Id { get; private set; }

        public string Name { get; set; }
    }
}