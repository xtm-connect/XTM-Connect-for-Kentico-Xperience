using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xtm.Connector.Exceptions
{
    public class XtmConnectionException : XtmConnectorException
    {
        public XtmConnectionException(string message) : base(message) { }
        public XtmConnectionException(string message, Exception innerException) : base(message, innerException) { }
    }
}
