using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XtmConnect.XtmConnect.Models
{
    public class XtmSubmissionTicket
    {
        public string ProjectName { get;  set; }
        public string ProjectIntegrationId { get; set; }
        public long? ProjectId { get; set; }
    }
}