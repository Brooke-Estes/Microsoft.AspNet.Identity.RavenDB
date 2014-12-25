namespace Microsoft.AspNet.Identity.RavenDB.Tests
{
    using NUnit.Framework;
    using Raven.Client;
    using Raven.Client.Embedded;

    public class RavenDocumentStorage
    {
        protected IDocumentStore RavenDocumentStore { get; private set; }

        protected IAsyncDocumentSession AsyncDocumentSession
        {
            get { return CreateAsyncSession(); }
        }

        [TestFixtureSetUp]
        protected void SetStorageContext()
        {
            RavenDocumentStore = new EmbeddableDocumentStore()
            {
                RunInMemory = true
            };

            RavenDocumentStore.Initialize();
            RegisterConventions();
        }

        protected virtual void RegisterConventions() { }

        private IAsyncDocumentSession CreateAsyncSession()
        {
            var session = RavenDocumentStore.OpenAsyncSession();
            session.Advanced.UseOptimisticConcurrency = true;
            return session;
        }

       
    }
}