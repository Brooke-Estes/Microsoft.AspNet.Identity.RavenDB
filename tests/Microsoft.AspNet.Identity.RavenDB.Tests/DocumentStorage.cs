namespace Microsoft.AspNet.Identity.RavenDB.Tests
{
    using NUnit.Framework;
    using Raven.Client;
    using Raven.Client.Document;
    using Raven.Client.Embedded;

    [TestFixture]
    public class DocumentStorage
    {
        protected IDocumentStore DocumentStore { get; private set; }

        [TestFixtureSetUp]
        public void SetDocumentStorageContext()
        {
            DocumentStore = new EmbeddableDocumentStore()
            {
                RunInMemory = true

            }.Initialize();

            RegisterConventions();
            SeedDocument();
        }

        protected void RegisterConventions()
        {
            DocumentStore.Conventions.FindTypeTagName = type => (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(RoleReferenceList<>))
                    ? "IdentityReferences" : DocumentConvention.DefaultTypeTagName(type);
            
        }

        protected virtual void SeedDocument() { }
        
        protected IAsyncDocumentSession AsyncDocumentSession
        {
            get
            {
                var session = DocumentStore.OpenAsyncSession();
                session.Advanced.UseOptimisticConcurrency = true;
                return session;
            }
        }
        
    }
}