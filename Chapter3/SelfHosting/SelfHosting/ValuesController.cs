using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace SelfHosting
{
    public class ValuesController : ApiController
    {

        public IEnumerable<string> Get()
        {
            return new string[] { "sfewf", "valuegwegwegweg2" };
        }
    }
}
