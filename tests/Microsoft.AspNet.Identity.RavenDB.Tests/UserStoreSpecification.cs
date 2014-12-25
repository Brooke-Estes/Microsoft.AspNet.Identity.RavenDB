namespace Microsoft.AspNet.Identity.RavenDB.Tests
{
    using System.Threading.Tasks;
    using NUnit.Framework;
    using Raven.Client;

    [TestFixture]
    public class UserStoreSpecification : UserStoreSpecificationBase
    {
        private IUserStore<RavenIdentityUser> _userStore;
        
        [SetUp]
        public void SetSpecificationContext()
        {
            _userStore = RavenUserStore as IUserStore<RavenIdentityUser>;

            if (_userStore == null)
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

        [Test]
        public async Task CreateAsync_NewIdentityUser_ShouldPersistUserToDocument() 
        {
            await _userStore.CreateAsync(new RavenIdentityUser() { UserName = "user.one", Email = "user.one@somedomain.com" });

            using (var session = AsyncDocumentSession)
            {
                var user = (await session.Query<RavenIdentityUser>()
                    .SingleOrDefaultAsync(u => u.UserName == "user.one")
                    .ConfigureAwait(false));

                Assert.IsNotNull(user);
            }
            
        }

        [Test]
        public async Task UpdateAsync_ExistingUser_ShouldPersistChangeToDocument() 
        {
            var ravenUser = await _userStore.FindByNameAsync("dummy.user.one");
            ravenUser.UserName = "dummy.user";

            await _userStore.UpdateAsync(ravenUser);

            using (var session = AsyncDocumentSession)
            {
                var user = (await session.LoadAsync<RavenIdentityUser>(ravenUser.Id)
                    .ConfigureAwait(false));

                Assert.IsNotNull(user);
                Assert.IsTrue(user.UserName == "dummy.user");
            }
        }

        [Test]
        public async Task DeleteAsync_ExistingUser_ShouldDeletUserDocument() 
        {
            var ravenUser = await _userStore.FindByNameAsync("dummy.user.two");

            await _userStore.DeleteAsync(ravenUser);

            using (var session = AsyncDocumentSession)
            {
                var users = (await session.Query<RavenIdentityUser>()
                    .ToListAsync()
                    .ConfigureAwait(false));

                Assert.IsNotNull(!users.Contains(ravenUser));
            }
            
        }
        
    }
}