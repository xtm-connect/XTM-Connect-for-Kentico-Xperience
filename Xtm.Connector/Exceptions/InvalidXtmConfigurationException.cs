using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xtm.Connector.Exceptions
{
    public class InvalidXtmConfigurationException : XtmConnectorException
    {
        public InvalidXtmConfigurationException(string message) : base(message) { }
    }
}
