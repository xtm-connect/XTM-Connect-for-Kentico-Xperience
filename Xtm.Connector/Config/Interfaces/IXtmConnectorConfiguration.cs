using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xtm.Connector.Config.Interfaces
{
    public interface IXtmConnectorConfiguration
    {
        string Client { get; set; }
        string UserName { get; set; }
        string Password { get; set; }
        string WebserviceURI { get; set; }
        void Validate();
        string IntegrationKey { get; set; }
    }
}
