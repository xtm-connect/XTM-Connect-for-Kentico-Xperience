using CMS.DataEngine;
using CMS.Helpers;
using CMS.TranslationServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Web;
using XtmConnect.Services;
using XtmConnect.XtmConnect.Exceptions;
using XtmConnect.XtmConnect.Models;
using XtmConnect.XtmConnect.Services.Interfaces;

namespace XtmConnect.XtmConnect.Services
{
    internal class XtmTranslationProviderService : IXtmTranslationProviderService
    {
        internal static bool IsDeletionLocked = true;
        internal static bool IsEditionLocked = true;

        /// <summary>
        /// Creates or updates XtmTranslationProvider with it's corresponding Kentico's Translation Service in a transactional manner
        /// </summary>
        /// <param name="xtmTranslationProviderFulldata"></param>
        /// <returns></returns>
        public int CreateOrUpdateProvider(XtmTranslationProviderFullData xtmTranslationProviderFulldata)
        {
            var xtmTranslationProvider = xtmTranslationProviderFulldata.XtmTranslationProvider;

            bool isNew = xtmTranslationProvider.XtmTranslationProviderID == 0;

            using (CMSTransactionScope trans = new CMSTransactionScope())
            {
                if (isNew) // add
                {
                    var codeName = xtmTranslationProviderFulldata.ProviderName.Replace(" ", "");

                    if (TranslationServiceInfoProvider.GetTranslationServices().Where(x => x.TranslationServiceName == xtmTranslationProviderFulldata.ProviderName).Any())
                        throw new XtmModuleException(ResHelper.GetString("ProviderNameTaken"));

                    if (TranslationServiceInfoProvider.GetTranslationServices().Where(x => x.TranslationServiceName == codeName).Any())
                        throw new XtmModuleException(ResHelper.GetString("ProviderDiffrentName"));

                    if (!ValidationHelper.IsCodeName(codeName))
                        throw new XtmModuleException(ResHelper.GetString("ProviderNameRule"));

                    var kenticoTranslationService = new TranslationServiceInfo()
                    {
                        TranslationServiceAssemblyName = typeof(XtmTranslationService).Assembly.GetName().Name,
                        TranslationServiceClassName = typeof(XtmTranslationService).FullName,
                        TranslationServiceName = codeName,
                        TranslationServiceDisplayName = xtmTranslationProviderFulldata.ProviderName,
                        TranslationServiceSupportsDeadline = true,
                        TranslationServiceSupportsCancel = true,
                        TranslationServiceEnabled = true,
                        TranslationServiceIsMachine = false,
                        TranslationServiceSupportsInstructions = false,
                        TranslationServiceSupportsPriority = false,
                        TranslationServiceSupportsStatusUpdate = false,
                    };

                    TranslationServiceInfoProvider.SetTranslationServiceInfo(kenticoTranslationService);
                    var newId = kenticoTranslationService.TranslationServiceID;

                    xtmTranslationProvider.TranslationProviderID = newId;
                    xtmTranslationProvider.XtmTranslationProviderGuid = Guid.NewGuid();
                    xtmTranslationProvider.XtmTranslationProviderLastModified = DateTime.Now;

                    XtmTranslationProviderInfoProvider.SetXtmTranslationProviderInfo(xtmTranslationProvider);
                }
                else // update
                {
                    var oldXtmProvider = XtmTranslationProviderInfoProvider.GetXtmTranslationProviders().Where(x => x.XtmTranslationProviderID == xtmTranslationProvider.XtmTranslationProviderID).Single();

                    var kenticoTranslationService = TranslationServiceInfoProvider.GetTranslationServices()
                         .Where(x => x.TranslationServiceID == oldXtmProvider.TranslationProviderID).Single();

                    if (kenticoTranslationService.TranslationServiceDisplayName != xtmTranslationProviderFulldata.ProviderName)
                    {
                        var codeName = xtmTranslationProviderFulldata.ProviderName.Replace(" ", "");

                        if (TranslationServiceInfoProvider.GetTranslationServices().Where(x => x.TranslationServiceName == xtmTranslationProviderFulldata.ProviderName).Any())
                            throw new XtmModuleException(ResHelper.GetString("ProviderNameTaken"));

                        if (TranslationServiceInfoProvider.GetTranslationServices().Where(x => x.TranslationServiceName == codeName).Any())
                            throw new XtmModuleException(ResHelper.GetString("ProviderDiffrentName"));

                        if (!ValidationHelper.IsCodeName(codeName))
                            throw new XtmModuleException(ResHelper.GetString("ProviderNameRule"));

                        kenticoTranslationService.TranslationServiceDisplayName = xtmTranslationProviderFulldata.ProviderName;
                        kenticoTranslationService.TranslationServiceName = codeName;
                    }

                    kenticoTranslationService.TranslationServiceSupportsDeadline = xtmTranslationProviderFulldata.DeadlineEnabled;

                    IsEditionLocked = false;
                    try
                    {
                        TranslationServiceInfoProvider.SetTranslationServiceInfo(kenticoTranslationService);
                    }
                    catch
                    {
                        IsEditionLocked = true;
                        throw;
                    }

                    oldXtmProvider.XTMClient = xtmTranslationProvider.XTMClient;
                    oldXtmProvider.XTMUsername = xtmTranslationProvider.XTMUsername;
                    oldXtmProvider.XTMPassword = xtmTranslationProvider.XTMPassword;
                    oldXtmProvider.XTMWebserviceURI = xtmTranslationProvider.XTMWebserviceURI;

                    oldXtmProvider.XTMCustomerID = xtmTranslationProvider.XTMCustomerID;
                    oldXtmProvider.XTMCustomerName = xtmTranslationProvider.XTMCustomerName;

                    oldXtmProvider.XTMTemplateID = xtmTranslationProvider.XTMTemplateID;
                    oldXtmProvider.XTMTemplateName = xtmTranslationProvider.XTMTemplateName;

                    oldXtmProvider.TranslationImportType = xtmTranslationProvider.TranslationImportType;

                    oldXtmProvider.XTMProjectNamePrefix = xtmTranslationProvider.XTMProjectNamePrefix;
                    XtmTranslationProviderInfoProvider.SetXtmTranslationProviderInfo(oldXtmProvider);
                }
                trans.Commit();
            }
            return xtmTranslationProvider.XtmTranslationProviderID;
        }

        public void DeleteProvider(int xtmTranslatoinProviderID)
        {
            using (CMSTransactionScope trans = new CMSTransactionScope())
            {
                var xtmProvider = XtmTranslationProviderInfoProvider.GetXtmTranslationProviders()
                    .Where(x => x.XtmTranslationProviderID == xtmTranslatoinProviderID).Single();
                var kenticoTranslationService = TranslationServiceInfoProvider.GetTranslationServices()
                     .Where(x => x.TranslationServiceID == xtmProvider.TranslationProviderID).Single();

                XtmTranslationProviderInfoProvider.DeleteXtmTranslationProviderInfo(xtmProvider);

                IsDeletionLocked = false;
                try
                {
                    TranslationServiceInfoProvider.DeleteTranslationServiceInfo(kenticoTranslationService);
                }
                catch
                {
                    IsDeletionLocked = true;
                    throw;
                }

                trans.Commit();
            }
        }

        public IEnumerable<XtmTranslationProviderFullData> GetAllXtmTranslationProviderFullData()
        {
            var xtmProviders = XtmTranslationProviderInfoProvider.GetXtmTranslationProviders().ToList();
            var kenticoTranslationServices = TranslationServiceInfoProvider.GetTranslationServices().ToList();

            var allXtmProvidersFullData = xtmProviders.Select(prov =>
            {
                return new XtmTranslationProviderFullData()
                {
                    XtmTranslationProvider = prov,
                    ProviderName = kenticoTranslationServices.Single(x => x.TranslationServiceID == prov.TranslationProviderID).TranslationServiceDisplayName,
                    DeadlineEnabled = kenticoTranslationServices.Single(x => x.TranslationServiceID == prov.TranslationProviderID).TranslationServiceSupportsDeadline
                };
            }).ToList();
            return allXtmProvidersFullData;
        }

        public XtmTranslationProviderFullData GetXtmTranslationProviderFullData(int xtmTranslationProviderId)
        {
            var xtmProvider = XtmTranslationProviderInfoProvider.GetXtmTranslationProviderInfo(xtmTranslationProviderId);
            var kenticoTranslationService = TranslationServiceInfoProvider.GetTranslationServiceInfo(xtmProvider.TranslationProviderID);

            var xtmProviderFullData = new XtmTranslationProviderFullData()
            {
                ProviderName = kenticoTranslationService.TranslationServiceDisplayName,
                DeadlineEnabled = kenticoTranslationService.TranslationServiceSupportsDeadline,
                XtmTranslationProvider = xtmProvider
            };

            return xtmProviderFullData;
        }

        public XtmTranslationProviderFullData GetXtmTranslationProviderFullDataByTranslationService(int translationServiceId)
        {
            var xtmProvider = XtmTranslationProviderInfoProvider.GetXtmTranslationProviders().Where(x => x.TranslationProviderID == translationServiceId).Single();
            var kenticoTranslationService = TranslationServiceInfoProvider.GetTranslationServiceInfo(translationServiceId);

            var xtmProviderFullData = new XtmTranslationProviderFullData()
            {
                ProviderName = kenticoTranslationService.TranslationServiceDisplayName,
                DeadlineEnabled = kenticoTranslationService.TranslationServiceSupportsDeadline,
                XtmTranslationProvider = xtmProvider
            };

            return xtmProviderFullData;
        }
    }
}