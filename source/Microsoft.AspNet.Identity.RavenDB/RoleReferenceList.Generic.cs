namespace Microsoft.AspNet.Identity.RavenDB
{
    using System.Collections.Generic;

    /// <summary>
    /// A generic class that expects an implementation of Identity's <see cref="IRole"/>.
    /// 
    /// The idea is to keep the roles, and any other reference data, in a single document.
    /// A good candidate for a reference list is data that is potentially heavier on the read operations 
    /// with infrequent inserts and updates.
    /// </summary>
    /// <typeparam name="TRole">The type of the role.</typeparam>
    public class RoleReferenceList<TRole>
        where TRole : IRole
    {
        public RoleReferenceList(string documentKey)
        {
            Id = documentKey;
        }

        public string Id { get; private set; }

        public List<TRole> Roles { get; set; }

    }
}