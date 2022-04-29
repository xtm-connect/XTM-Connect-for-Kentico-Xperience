using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xtm.Connector.Models
{
    public class Project
    {
        public long? Id { get; set; }
        public string IntegrationId { get; set; }
        public string Name { get; set; }
        public List<Job> Jobs { get; set; }
    }
}
