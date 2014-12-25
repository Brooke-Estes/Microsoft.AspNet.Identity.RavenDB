namespace Microsoft.AspNet.Identity.RavenDB
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using Raven.Client;

    /// <summary>
    /// A generic class the exposes Identity's <see cref="IUserStore{TUser}"/> interface.
    /// 
    /// This implementation also exposes the complete gamut of Identity's core interfaces
    /// for providing an exhaustive set of membership store features.
    /// </summary>
    public class RavenUserStore<TUser> : IUserStore<TUser>, IUserSecurityStampStore<TUser>, IUserPasswordStore<TUser>, IUserEmailStore<TUser>, IUserRoleStore<TUser>,
        IUserLockoutStore<TUser, string>, IUserLoginStore<TUser>, IUserClaimStore<TUser>
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

        #region Implementation of IUserLockoutStore<TUser, TKey>

        public Task<DateTimeOffset> GetLockoutEndDateAsync(TUser user)
        {
            ThrowIfDisposed();

            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            return Task.FromResult<DateTimeOffset>(user.LockoutEndDateUtc.HasValue
                ? new DateTimeOffset(DateTime.SpecifyKind(user.LockoutEndDateUtc.Value, DateTimeKind.Utc)) : default(DateTimeOffset));


        }

        public Task SetLockoutEndDateAsync(TUser user, DateTimeOffset lockoutEnd)
        {
            ThrowIfDisposed();

            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            user.LockoutEndDateUtc = ((lockoutEnd == DateTimeOffset.MinValue) ? null : new DateTime?(lockoutEnd.UtcDateTime));
            return Task.FromResult<int>(0);

        }

        public Task<int> IncrementAccessFailedCountAsync(TUser user)
        {
            ThrowIfDisposed();

            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            user.AccessFailedCount++;
            return Task.FromResult<int>(user.AccessFailedCount);
        }

        public Task ResetAccessFailedCountAsync(TUser user)
        {
            ThrowIfDisposed();

            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            user.AccessFailedCount = 0;
            return Task.FromResult<int>(0);
        }

        public Task<int> GetAccessFailedCountAsync(TUser user)
        {
            ThrowIfDisposed();

            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            return Task.FromResult<int>(user.AccessFailedCount);
        }

        public Task<bool> GetLockoutEnabledAsync(TUser user)
        {
            ThrowIfDisposed();

            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            return Task.FromResult<bool>(user.LockoutEnabled);
        }

        public Task SetLockoutEnabledAsync(TUser user, bool enabled)
        {
            ThrowIfDisposed();

            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            user.LockoutEnabled = true;
            return Task.FromResult<int>(0);
        }

        #endregion

        #region Implementation of IUserLoginStore<TUser>

        public Task AddLoginAsync(TUser user, UserLoginInfo login)
        {
            ThrowIfDisposed();

            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            if (login == null)
            {
                throw new ArgumentNullException("login");
            }

            user.Logins = user.Logins ?? new List<RavenIdentityUserLogin>();

            user.Logins.Add(new RavenIdentityUserLogin()
            {
                LoginProvider = login.LoginProvider,
                ProviderKey = login.ProviderKey,
                UserId = user.Id
            });

            return Task.FromResult<int>(0);
        }

        public Task RemoveLoginAsync(TUser user, UserLoginInfo login)
        {
            ThrowIfDisposed();

            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            if (login == null)
            {
                throw new ArgumentNullException("login");
            }

            var userLogin = user.Logins.FirstOrDefault(l =>
                l.LoginProvider == login.LoginProvider && l.ProviderKey == login.ProviderKey);

            if (userLogin != null)
            {
                user.Logins.Remove(userLogin);
            }

            return Task.FromResult<int>(0);
        }

        public Task<IList<UserLoginInfo>> GetLoginsAsync(TUser user)
        {
            ThrowIfDisposed();

            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            var logins = user.Logins
                .Select(l => new UserLoginInfo(l.LoginProvider, l.ProviderKey))
                .ToList();

            return Task.FromResult<IList<UserLoginInfo>>(logins);
        }

        public async Task<TUser> FindAsync(UserLoginInfo login)
        {
            // todo: create and impelement the appropriate index for querying on the login
            ThrowIfDisposed();

            if (login == null)
            {
                throw new ArgumentNullException("login");
            }

            var user = await _documentSession.Query<TUser>()
                .Where(u => u.Logins.Any(l => l.LoginProvider == login.LoginProvider && l.ProviderKey == login.ProviderKey))
                .SingleOrDefaultAsync()
                .ConfigureAwait(false);

            return user;

        }

        #endregion

        #region Implementation of IUserClaimStore<TUser>

        public Task<IList<Claim>> GetClaimsAsync(TUser user)
        {
            ThrowIfDisposed();

            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            var claims = user.Claims
                .Select(c => new Claim(c.ClaimType, c.ClaimValue))
                .ToList();

            return Task.FromResult<IList<Claim>>(claims);
        }

        public Task AddClaimAsync(TUser user, Claim claim)
        {
            ThrowIfDisposed();

            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            if (claim == null)
            {
                throw new ArgumentNullException("claim");
            }

            user.Claims = user.Claims ?? new List<RavenIdentityUserClaim>();
            user.Claims.Add(new RavenIdentityUserClaim()
            {
                UserId = user.Id,
                ClaimType = claim.Type,
                ClaimValue = claim.Value
            });

            return Task.FromResult<int>(0);
        }

        public Task RemoveClaimAsync(TUser user, Claim claim)
        {
            ThrowIfDisposed();

            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            if (claim == null)
            {
                throw new ArgumentNullException("claim");
            }

            var userClaim = user.Claims.SingleOrDefault(c => c.ClaimType == claim.Type && c.ClaimValue == claim.Value);

            if (userClaim != null)
            {
                user.Claims.Remove(userClaim);
            }

            return Task.FromResult<int>(0);
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
