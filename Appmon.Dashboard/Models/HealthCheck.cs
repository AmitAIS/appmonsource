using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Appmon.Dashboard.Models
{
    public class HealthCheckResult
    {
        public string ApplicationName { get; set; }
        public string Attribute { get; set; }
        public string Uri { get; set; }
        public string Health { get; set; }
    }

    public class HealthCheckResults
    {
        public List<HealthCheckResult> results { get; set; }
        public string id { get; set; }
        //public string _rid { get; set; }
        //public int _ts { get; set; }
        //public string _self { get; set; }
        //public string _etag { get; set; }
        //public string _attachments { get; set; }
    }
}