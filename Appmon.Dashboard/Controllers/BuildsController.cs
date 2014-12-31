using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Http;
using BuildManager.Models;

namespace BuildManager.Controllers
{
    public class BuildsController : ApiController
    {
        // GET api/builds
        public BuildResult Get()
        {
            return DocumentService.GetBuildResult();
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
        }
    }
}
