using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Appmon.Dashboard.Models
{
    public class PerformanceResultList
    {
        public PerformanceResult results { get; set; }
        public string id { get; set; }
        //public string _rid { get; set; }
        //public int _ts { get; set; }
        //public string _self { get; set; }
        //public string _etag { get; set; }
        //public string _attachments { get; set; }
    }

    public class PerformanceResult
    {
        public double EndUserAverageResponseTime { get; set; }
        public int EndUserCallCount { get; set; }
        public int HttpDispatcherAverageCallTime { get; set; }
        public int HttpDispatcherCallCount { get; set; }
        public double ApplicationAverageResponseTime { get; set; }
    }
}