namespace Microsoft.AspNet.Identity.RavenDB.Tests
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using NUnit.Framework;

    public class RoleStoreSpecifications : DocumentStorage
    {
        private const string DocumentKey = "reference/roles";

        private IRoleStore<RavenIdentityRole> _roleStore;
        
        [SetUp]
        public void SetSpecificationContext() 
        {
            if (_roleStore == null)
            {
                _roleStore = new RavenRoleStore<RavenIdentityRole>(AsyncDocumentSession, DocumentKey);
            }
            
        }

        protected override void SeedDocument()
        {
            using (var session = DocumentStore.OpenSession())
            {
                var rolesReferenceList = new RoleReferenceList<RavenIdentityRole>("reference/roles")
                {
                    Roles = new List<RavenIdentityRole>()
                    {
                        new RavenIdentityRole("roles/author") { Name = "Author" },
                        new RavenIdentityRole("roles/editor") { Name = "Editor" }
                    }
                };

                session.Store(rolesReferenceList);
                session.SaveChanges();  

            }
        }

        [Test]
        public async Task CreateAsync_WhenCreatingaNewRole_ShouldSaveToTheReferenceListDocument()
        {
            await _roleStore.CreateAsync(new RavenIdentityRole("roles/moderator") { Name = "Moderator" });

            using (var session = AsyncDocumentSession)
            {
                var role = (await session.LoadAsync<RoleReferenceList<RavenIdentityRole>>(DocumentKey)
                    .ConfigureAwait(false))
                    .Roles.SingleOrDefault(r => r.Id == "roles/moderator");

                Assert.IsNotNull(role);
            }
        }

        [Test]
        public async Task UpdateAsync_WhenUpdatingAnExistingRole_ShouldSaveChangesToTheReferenceListDocument()
        {
            var storeRole = await _roleStore.FindByIdAsync("roles/author");
            storeRole.Name = "Content Author";
            await _roleStore.UpdateAsync(storeRole);

            using (var session = AsyncDocumentSession)
            {
                var role = (await session.LoadAsync<RoleReferenceList<RavenIdentityRole>>(DocumentKey)
                    .ConfigureAwait(false))
                    .Roles.SingleOrDefault(r => r.Id == "roles/author");

                Assert.IsNotNull(role);
                Assert.IsTrue(role.Name == "Content Author");
            }
        }

        [Test]
        public async Task DeleteAsync_WhenDeletingAnExistingRole_ShouldRemoveFromTheReferenceListDocument()
        {
            var storeRole = await _roleStore.FindByIdAsync("roles/editor");
            await _roleStore.DeleteAsync(storeRole);

            using (var session = AsyncDocumentSession)
            {
                var roles = (await session.LoadAsync<RoleReferenceList<RavenIdentityRole>>(DocumentKey)
                  .ConfigureAwait(false))
                  .Roles.ToList();

                Assert.IsTrue(!roles.Contains(storeRole));
            }
        }  

    }
}   