using System.Threading.Tasks;
using System.Web.Http;
using Appmon.Dashboard.Models;
using Newtonsoft.Json;

namespace Appmon.Dashboard.Controllers
{
    public class AppSummaryController : ApiController
    {
        public ApplicationSummary Get()
        {
            return (ApplicationSummary)JsonConvert.DeserializeObject(DocumentService.GetResult("ApplicationSummarycollection"), typeof(ApplicationSummary));
        }
    }
}
