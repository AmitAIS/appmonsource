using System.Threading.Tasks;
using System.Web.Http;
using Appmon.Dashboard.Models;
using Newtonsoft.Json;

namespace Appmon.Dashboard.Controllers
{
    public class HealthCheckController : ApiController
    {
        public HealthCheckResults Get()
        {
            return (HealthCheckResults)JsonConvert.DeserializeObject(DocumentService.GetResult("DependencyHealthResultsCollection"), typeof(HealthCheckResults));
        }
    }
}
