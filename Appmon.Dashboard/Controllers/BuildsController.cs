using System.Threading.Tasks;
using System.Web.Http;
using Appmon.Dashboard.Models;
using Newtonsoft.Json;

namespace Appmon.Dashboard.Controllers
{
    public class BuildController : ApiController
    {
        // GET api/builds
        public BuildResult Get()
        {            
            return (BuildResult)JsonConvert.DeserializeObject(DocumentService.GetResult("ErmsBuildsCollection"), typeof(BuildResult));
        }

        // GET api/builds/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/builds
        public void Post()
        {
            Task pushBuilds = DocumentService.PushBuilds();
        }

        // PUT api/builds/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/builds/5
        public void Delete(int id)
        {
            // No implemented.
        }
    }
}
