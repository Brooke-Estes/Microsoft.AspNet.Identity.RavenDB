namespace Microsoft.AspNet.Identity.RavenDB
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Raven.Client;

    /// <summary>
    /// A generic class the exposes Identity's <see cref="IUserStore{TUser}"/> interface.
    /// 
    /// This implementation also exposes the complete gamut of Identity's core interfaces
    /// for working with more advanced memberhip features.
    /// </summary>
    public class RavenUserStore<TUser> : IUserStore<TUser>, IUserSecurityStampStore<TUser>, IUserPasswordStore<TUser>, IUserEmailStore<TUser>, IUserRoleStore<TUser>
        where TUser : RavenIdentityUser
    {
        private IAsyncDocumentSession _documentSession;
        private bool _disposed;

        public RavenUserStore(IAsyncDocumentSession documentSession)
        {
            if (documentSession == null)
            {
                throw new ArgumentNullException("documentSession");
            }

            _documentSession = documentSession;
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

        #region Implementation of IUserStore<TUser>

        public async Task CreateAsync(TUser user)
        {
            ThrowIfDisposed();

            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            await _documentSession.StoreAsync(user);
            await _documentSession.SaveChangesAsync();
        }

        public async Task UpdateAsync(TUser user)
        {
            ThrowIfDisposed();

            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            await _documentSession.SaveChangesAsync();
        }

        public async Task DeleteAsync(TUser user)
        {
            ThrowIfDisposed();

            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            _documentSession.Delete(user);
            await _documentSession.SaveChangesAsync();
        }

        public async Task<TUser> FindByIdAsync(string userId)
        {
            ThrowIfDisposed();

            return await _documentSession.LoadAsync<TUser>(userId.ToString())
                .ConfigureAwait(false);
        }

        public async Task<TUser> FindByNameAsync(string userName)
        {
            ThrowIfDisposed();

            return await _documentSession.Query<TUser>().FirstOrDefaultAsync(user => user.UserName == userName)
                .ConfigureAwait(false);
        }

        #endregion

        #region Implementation of IUserSecurityStampStore<TUser>

        public Task SetSecurityStampAsync(TUser user, string stamp)
        {
            ThrowIfDisposed();

            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            user.SecurityStamp = stamp;

            return Task.FromResult<int>(0);
        }

        public Task<string> GetSecurityStampAsync(TUser user)
        {
            ThrowIfDisposed();

            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            return Task.FromResult(user.SecurityStamp);

        }

        #endregion

        #region Implementation of IUserPasswordStore<TUser>

        public Task SetPasswordHashAsync(TUser user, string passwordHash)
        {
            ThrowIfDisposed();

            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            user.PasswordHash = passwordHash;

            return Task.FromResult<int>(0);
        }

        public Task<string> GetPasswordHashAsync(TUser user)
        {
            ThrowIfDisposed();

            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            return Task.FromResult<string>(user.PasswordHash);
        }

        public Task<bool> HasPasswordAsync(TUser user)
        {
            ThrowIfDisposed();

            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            return Task.FromResult<bool>(user.PasswordHash != null);
        }

        #endregion

        #region Implementation of IUserEmailStore<TUser>

        public Task SetEmailAsync(TUser user, string email)
        {
            ThrowIfDisposed();

            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            user.Email = email;

            return Task.FromResult<int>(0);
        }

        public Task<string> GetEmailAsync(TUser user)
        {
            ThrowIfDisposed();

            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            return Task.FromResult<string>(user.Email);
        }

        public Task<bool> GetEmailConfirmedAsync(TUser user)
        {
            ThrowIfDisposed();

            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            return Task.FromResult<bool>(user.EmailConfirmed);
        }

        public Task SetEmailConfirmedAsync(TUser user, bool confirmed)
        {
            ThrowIfDisposed();

            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            user.EmailConfirmed = confirmed;

            return Task.FromResult<int>(0);
        }

        public async  Task<TUser> FindByEmailAsync(string email)
        {
            ThrowIfDisposed();

            if (string.IsNullOrWhiteSpace(email))
            {
                throw new ArgumentNullException("email");
            }

            var userDocument = await _documentSession.Query<TUser>().FirstOrDefaultAsync(user => user.Email == email)
                .ConfigureAwait(false);

            return userDocument;

        }

        #endregion

        #region Implementation of IUserRoleStore<TUser>

        public Task AddToRoleAsync(TUser user, string roleName)
        {
            ThrowIfDisposed();

            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            var roles = user.Roles ?? new List<string>();

            roles.Add(roleName);

            return Task.FromResult<int>(0);
        }

        public Task RemoveFromRoleAsync(TUser user, string roleName)
        {
            ThrowIfDisposed();

            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            user.Roles.Remove(roleName);

            return Task.FromResult<int>(0);
        }

        public Task<IList<string>> GetRolesAsync(TUser user)
        {
            ThrowIfDisposed();

            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            return Task.FromResult<IList<string>>(user.Roles);
        }

        public Task<bool> IsInRoleAsync(TUser user, string roleName)
        {
            ThrowIfDisposed();

            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            return Task.FromResult<bool>(user.Roles.Contains(roleName));
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