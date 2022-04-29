using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using XtmConnect.XtmConnect.Models;

namespace XtmConnect.XtmConnect.Services.Interfaces
{
    public interface IXtmTranslationProviderService
    {
        int CreateOrUpdateProvider(XtmTranslationProviderFullData xtmTranslationProviderFulldata);
        void DeleteProvider(int xtmTranslatoinProviderID);
        XtmTranslationProviderFullData GetXtmTranslationProviderFullData(int xtmTranslationProviderId);
        IEnumerable<XtmTranslationProviderFullData> GetAllXtmTranslationProviderFullData();

        XtmTranslationProviderFullData GetXtmTranslationProviderFullDataByTranslationService(int translationServiceId);

    }
}