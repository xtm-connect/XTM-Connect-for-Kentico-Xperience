using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xtm.Connector.Exceptions
{
    /// <summary>
    /// Used as base type of exception in XTM Connector - always includes message (e.g. for user notification).
    /// </summary>
    public class XtmConnectorException : Exception
    {
        public XtmConnectorException(string message) : base(message) { }
        public XtmConnectorException(string message, Exception innerException) : base(message, innerException) { }
    }
}
