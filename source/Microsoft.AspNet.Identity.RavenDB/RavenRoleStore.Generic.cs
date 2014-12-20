namespace Microsoft.AspNet.Identity.RavenDB
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Raven.Client;

    /// <summary>
    /// A generic class that exposes Identity's <see cref="IRoleStore{TRole}"/> interface.
    /// 
    /// This implementation uses the <see cref="RoleReferenceList{TRole}"/> class for persisting roles to RavenDB.
    /// </summary>
    /// <typeparam name="TRole">The type of the role.</typeparam>
    public class RavenRoleStore<TRole> : IRoleStore<TRole>
        where TRole : IRole
    {
        private IAsyncDocumentSession _documentSession;
        private readonly string _documentKey;
        private bool _disposed;
        
        public RavenRoleStore(IAsyncDocumentSession documentSession, string documentKey)
        {
            if (documentSession == null)
            {
                throw new ArgumentNullException("documentSession");
            }

            _documentSession = documentSession;

            _documentKey = documentKey;

        }
       
        #region Methods

        public void ThrowIfDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(GetType().Name);
            }
        }

        #endregion

        #region Implementation of IRoleStore<TRole>

        public async Task CreateAsync(TRole role)
        {
            ThrowIfDisposed();

            if (role == null)
            {
                throw new ArgumentNullException("role");
            }

            var document = await _documentSession.LoadAsync<RoleReferenceList<TRole>>(_documentKey)
                .ConfigureAwait(false);
            document.Roles.Add(role);

            await _documentSession.SaveChangesAsync();
        }

        public async Task UpdateAsync(TRole role)
        {
            ThrowIfDisposed();

            if (role == null)
            {
                throw new ArgumentNullException("role");
            }

            await _documentSession.SaveChangesAsync();
        }

        public async  Task DeleteAsync(TRole role)
        {
            ThrowIfDisposed();

            if (role == null)
            {
                throw new ArgumentNullException("role");
            }

            var document = await _documentSession.LoadAsync<RoleReferenceList<TRole>>(_documentKey)
                .ConfigureAwait(false);
            document.Roles.RemoveAll(r => r.Id == role.Id && r.Name == role.Name);

            await _documentSession.SaveChangesAsync();

        }

        public async Task<TRole> FindByIdAsync(string roleId)
        {
            ThrowIfDisposed();

            var document = await _documentSession.LoadAsync<RoleReferenceList<TRole>>(_documentKey)
                .ConfigureAwait(false);

            return document.Roles.FirstOrDefault(role => role.Id == roleId);
        }

        public async  Task<TRole> FindByNameAsync(string roleName)
        {
            ThrowIfDisposed();

            var document = await _documentSession.LoadAsync<RoleReferenceList<TRole>>(_documentKey)
                .ConfigureAwait(false);

            return document.Roles.FirstOrDefault(role => role.Name == roleName);
        }

        #endregion

        #region Implementation of IDisposable

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void Dispose(bool disposing)
        {
            if (disposing && _documentSession != null)
            {
                _documentSession.Dispose();
            }

            _disposed = true;
            _documentSession = null;
        }

        #endregion
    }
         
    
}