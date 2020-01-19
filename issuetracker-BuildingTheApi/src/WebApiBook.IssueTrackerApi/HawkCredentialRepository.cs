using HawkNet;
using HawkNet.WebApi;
using System.Threading.Tasks;

namespace WebApiBook.IssueTrackerApi
{
    public class HawkCredentialRepository : IHawkCredentialRepository
    {
        public Task<HawkCredential> GetCredentialsAsync(string id)
        {
            return Task.FromResult(new HawkCredential
            {
                Id = "dh37fgj492je",
                Key = "werxhqb98rpaxn39848xrunpaw3489ruxnpa98w4rxn",
                Algorithm = "sha256",
                User = "steve"
            });
        }
    }
}
