using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xtm.Connector.XtmWebService;

namespace Xtm.Connector.Factories.Interfaces
{
    internal interface ISoapClientFactory
    {
        ProjectManagerMTOMWebServiceClient CreateXTMWebService();
    }
}
