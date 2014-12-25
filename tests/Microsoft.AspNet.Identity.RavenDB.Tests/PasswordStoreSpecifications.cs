namespace Microsoft.AspNet.Identity.RavenDB.Tests
{
    using NUnit.Framework;
    
    [TestFixture]
    public class PasswordStoreSpecifications : UserStoreSpecificationBase
    {
        private IUserPasswordStore<RavenIdentityUser> _passwordStore;

        [SetUp]
        public void SetSpecificationContext()
        {
            _passwordStore = RavenUserStore as IUserPasswordStore<RavenIdentityUser>;

            if (_passwordStore == null)
            {
                Assert.Ignore();
            }

        }

        protected override void SeedDocuments()
        {
            using (var session = RavenDocumentStore.OpenSession())
            {
                session.Store(new RavenIdentityUser() { UserName = "dummy.user.one", Email = "dummy.user@somedomain.com" });
                session.Store(new RavenIdentityUser() { UserName = "dummy.user.two", Email = "dummy.user@somedomain.com" });
                session.SaveChanges();
            }
        }

        
    }
}