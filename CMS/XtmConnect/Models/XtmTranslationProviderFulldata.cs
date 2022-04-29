using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XtmConnect.XtmConnect.Models
{
    // Full information about XtmTranslationProvider - merges XTM module class data with Kenticos Translation service data
    public class XtmTranslationProviderFullData
    {
        // Xtm module provider:
        public XtmTranslationProviderInfo XtmTranslationProvider { get; set; }



        // Kentico's transaltion service data:
        public bool DeadlineEnabled { get; set; }
        public string ProviderName { get; set; }

    }
}