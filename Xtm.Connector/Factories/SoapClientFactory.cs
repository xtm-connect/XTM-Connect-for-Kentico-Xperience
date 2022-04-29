using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Xtm.Connector.Config.Interfaces;
using Xtm.Connector.Factories.Interfaces;
using Xtm.Connector.XtmWebService;

namespace Xtm.Connector.Factories
{
    internal class SoapClientFactory : ISoapClientFactory
    {
        IXtmConnectorConfiguration xtmConnectorConfig;
        public SoapClientFactory(IXtmConnectorConfiguration xtmConnectorConfig)
        {
            this.xtmConnectorConfig = xtmConnectorConfig;
        }
        public ProjectManagerMTOMWebServiceClient CreateXTMWebService()
        {
            var url = xtmConnectorConfig.WebserviceURI;
            bool isHttps = url.ToLower().Contains("https");
            bool isMTOM = url.ToLower().Contains("xop") || url.ToLower().Contains("mtom");
            // MTOM is described within XTM webservice URL in 2 ways:
            // https://www.xtm-cloud.com/project-manager-gui/services/v2/XTMProjectManagerMTOMWebService
            // https://www.xtm-cloud.com/project-manager-gui/services/v2/projectmanager/xop/XTMWebService
            var binding = new BasicHttpBinding();
            {
                binding.Security = new BasicHttpSecurity() { Mode = isHttps ? BasicHttpSecurityMode.Transport : BasicHttpSecurityMode.None };
                binding.MessageEncoding = isMTOM ? WSMessageEncoding.Mtom : WSMessageEncoding.Text;
            }
            binding.MaxReceivedMessageSize = 100*1000000; // 100MB
            binding.MaxBufferSize = 100 * 1000000; // 100MB
            var client = new ProjectManagerMTOMWebServiceClient(binding, new EndpointAddress(url));
            return client;
        }
    }
}
