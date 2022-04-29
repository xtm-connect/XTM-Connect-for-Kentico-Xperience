using CMS;
using CMS.DataEngine;
using CMS.EventLog;
using CMS.TranslationServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using XtmConnect.Config;
using XtmConnect.Services;
using XtmConnect.XtmConnect.Services;

[assembly: RegisterModule(typeof(XtmConnect.XtmConnectModule))]
namespace XtmConnect
{
    public class XtmConnectModule : Module
    {
        public XtmConnectModule() : base(Constants.Module.CodeName, isInstallable: true)
        {
        }

        protected override void OnInit()
        {
            EventLogProvider.LogInformation(Constants.Module.DisplayName, "INFORMATION", "XTM Connect module started.");
            GlobalConfiguration.Configuration.Routes.MapHttpRoute(Constants.Module.Web.ApiRoutePrefix, Constants.Module.Web.ApiRoutePrefix + @"/{controller}/{action}"); // enable webapi controller for callbacks
            SecureXtmTranslationProvidersDeletionAndUpdate(); // secures unsafe edition/deletion of XTM tranlsation provider from the "Translation services" application (and/or others forbidden source)
                                                              // (update/deletion of XtmTranslationProvider with corresponding Kentico's translation service should be done in transactional manner with XtmTranslationProviderService class)

            base.OnInit();
        }

        private void SecureXtmTranslationProvidersDeletionAndUpdate()
        {
            TranslationServiceInfo.TYPEINFO.Events.Delete.Before += (sender, args) =>
            {
                try
                {
                    var provider = (TranslationServiceInfo)args.Object;
                    var className = typeof(XtmTranslationService).FullName;
                    if (provider.TranslationServiceClassName == className && XtmTranslationProviderService.IsDeletionLocked)
                    {
                        throw new Exception("Cannot delete XTM translation service on this page. To delete the service, please go to Applications -> Xtm Translation Providers.");
                    }
                }
                finally
                {
                    XtmTranslationProviderService.IsDeletionLocked = true;
                }
            };

            TranslationServiceInfo.TYPEINFO.Events.Update.Before += (sender, args) =>
            {
                try
                {
                    var provider = (TranslationServiceInfo)args.Object;
                    var className = typeof(XtmTranslationService).FullName;
                    if (provider.TranslationServiceClassName == className && XtmTranslationProviderService.IsEditionLocked)
                    {
                        throw new Exception("Cannot edit XTM translation service on this page. To edit the service, please go to Applications -> Xtm Translation Providers.");
                    }
                }
                finally
                {
                    XtmTranslationProviderService.IsEditionLocked = true;
                }
            };
        }
    }
}