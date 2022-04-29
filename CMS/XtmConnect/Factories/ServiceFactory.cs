using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Xtm.Connector.Config.Interfaces;
using Xtm.Connector.Logic.Services;
using Xtm.Connector.Logic.Services.Interface;
using XtmConnect.Factories;
using XtmConnect.XtmConnect.Services;
using XtmConnect.XtmConnect.Services.Interfaces;

namespace XtmConnect.Factories
{
    internal class ServiceFactory : IServiceFactory
    {
        public IXtmConnector CreateXtmConnector(IXtmConnectorConfiguration xtmConnectorConfig)
        {
            return new XtmConnector(xtmConnectorConfig);
        }

        public IXtmTranslationProviderService CreateXtmTranslationProviderService()
        {
            return new XtmTranslationProviderService();
        }
    }
}