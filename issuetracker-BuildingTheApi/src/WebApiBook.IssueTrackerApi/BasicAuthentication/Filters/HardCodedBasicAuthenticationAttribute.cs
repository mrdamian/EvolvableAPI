using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;

namespace WebApiBook.IssueTrackerApi.BasicAuthentication.Filters
{
    public class HardCodedBasicAuthenticationAttribute : BasicAuthenticationAttribute
    {
        protected override async Task<IPrincipal> AuthenticateAsync(string userName, string password, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested(); // Unfortunately, UserManager doesn't support CancellationTokens.
            IPrincipal principal = null;
            if (userName == "Ivan" && password == "Test")
            {
                GenericIdentity myIdentity = new GenericIdentity(userName);
                principal = new GenericPrincipal(myIdentity, null);
                return principal;
            }

            return null;
        }
    }
}
