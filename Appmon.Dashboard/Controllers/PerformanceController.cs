using System.Threading.Tasks;
using System.Web.Http;
using Appmon.Dashboard.Models;
using Newtonsoft.Json;

namespace Appmon.Dashboard.Controllers
{
    public class PerformanceController : ApiController
    {
        //
        // GET: /Performance/
        public PerformanceResult Get()
        {
            return (PerformanceResult)JsonConvert.DeserializeObject(DocumentService.GetResult("PerfMonCollection"), typeof(PerformanceResult));
        }
	}
}