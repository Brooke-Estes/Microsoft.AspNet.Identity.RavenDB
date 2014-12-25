namespace Microsoft.AspNet.Identity.RavenDB.Tests
{
    using NUnit.Framework;

    public class UserStoreSpecificationBase : RavenDocumentStorage
    {
        protected RavenUserStore<RavenIdentityUser> RavenUserStore { get; set; }

        [TestFixtureSetUp]
        public void SetUserStoreContext()
        {
            RavenUserStore = new RavenUserStore<RavenIdentityUser>(AsyncDocumentSession);
            SeedDocuments();
        }

        protected virtual void SeedDocuments() {}
    }
}
