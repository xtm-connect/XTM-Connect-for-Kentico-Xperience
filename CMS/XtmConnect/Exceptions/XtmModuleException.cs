using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XtmConnect.XtmConnect.Exceptions
{
    internal class XtmModuleException : Exception
    {
        public XtmModuleException(string message) : base(message) { }
    }
}