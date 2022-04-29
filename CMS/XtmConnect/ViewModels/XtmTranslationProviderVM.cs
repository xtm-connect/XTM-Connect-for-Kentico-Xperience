using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XtmConnect.ViewModels
{
    internal class XtmTranslationProviderVM
    {
        public int XtmTranslationProviderID { get; set; }
        public string ProviderName { get; set; }
        public string XTMCustomerName { get; set; }
        public string XTMTemplateName { get; set; }
        public string XTMProjectNamePrefix { get; set; }
        public string TranslationImportType { get; set; }
        //string XTMWebserviceURI;
        //string XTMClient;
        //string XTMUsername;
        //string XTMPassword;
    }
}