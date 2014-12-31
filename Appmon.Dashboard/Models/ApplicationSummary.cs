using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Appmon.Dashboard.Models
{
    public class ApplicationSummary
    {
        public ApplicationResult results { get; set; }
        public string id { get; set; }
        //public string _rid { get; set; }
        //public int _ts { get; set; }
        //public string _self { get; set; }
        //public string _etag { get; set; }
        //public string _attachments { get; set; }
    }

    public class NameValue
    {
        public string Name { get; set; }
        public string metric_value { get; set; }
    }

    public class ApplicationResult
    {
        public string StartDateTime { get; set; }
        public string EndDateTime { get; set; }
        public List<NameValue> ThresholdValues { get; set; }
    }
}