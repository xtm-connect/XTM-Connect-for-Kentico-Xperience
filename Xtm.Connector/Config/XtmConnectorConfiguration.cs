using CMS.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using Xtm.Connector.Config.Interfaces;
using Xtm.Connector.Exceptions;

namespace Xtm.Connector.Config
{
    public class XtmConnectorConfiguration : IXtmConnectorConfiguration
    {
        public XtmConnectorConfiguration(string client, string userName, string password, string webserviceURI, string integrationKey)
        {
            Client = client;
            UserName = userName;
            Password = password;
            WebserviceURI = webserviceURI;
            IntegrationKey = integrationKey;
            this.Validate();
        }

        public string Client { get; set; }
        public string IntegrationKey { get; set; }
        public string Password { get; set; }
        public string UserName { get; set; }
        public string WebserviceURI { get; set; }

        public void Validate()
        {
            if (string.IsNullOrEmpty(Client))
                throw new InvalidXtmConfigurationException(ResHelper.GetString("ClientRule"));

            if (string.IsNullOrEmpty(UserName))
                throw new InvalidXtmConfigurationException(ResHelper.GetString("UserNameRule"));

            if (string.IsNullOrEmpty(Password))
                throw new InvalidXtmConfigurationException(ResHelper.GetString("PasswordRule"));

            if (string.IsNullOrEmpty(WebserviceURI))
                throw new InvalidXtmConfigurationException(ResHelper.GetString("WebserviceRule"));

            if (!Uri.IsWellFormedUriString(WebserviceURI, UriKind.Absolute))
                throw new InvalidXtmConfigurationException(ResHelper.GetString("EndPointRule"));
        }
    }
}