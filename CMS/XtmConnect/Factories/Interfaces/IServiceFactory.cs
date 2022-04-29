using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Xtm.Connector.Config.Interfaces;
using Xtm.Connector.Logic.Services.Interface;
using XtmConnect.XtmConnect.Services.Interfaces;

namespace XtmConnect.Factories
{
    public interface IServiceFactory
    {
        IXtmConnector CreateXtmConnector(IXtmConnectorConfiguration xtmConnectorConfig);
        IXtmTranslationProviderService CreateXtmTranslationProviderService();
    }
}