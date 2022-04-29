using System;
using System.Data;

using CMS.Base;
using CMS.DataEngine;
using CMS.Helpers;

namespace XtmConnect
{
    /// <summary>
    /// Class providing <see cref="XtmTranslationProviderInfo"/> management.
    /// WARNING: USING THIS CLASS DIRECTLY MIGHT CAUSE UNINTENDED SIDE EFFECTS, CONSIDER USING "XtmTranslationProviderService" for CRUD operations instead.
    /// </summary>
    public partial class XtmTranslationProviderInfoProvider : AbstractInfoProvider<XtmTranslationProviderInfo, XtmTranslationProviderInfoProvider>
    {
        /// <summary>
        /// Creates an instance of <see cref="XtmTranslationProviderInfoProvider"/>.
        /// </summary>
        public XtmTranslationProviderInfoProvider()
            : base(XtmTranslationProviderInfo.TYPEINFO)
        {
        }


        /// <summary>
        /// Returns a query for all the <see cref="XtmTranslationProviderInfo"/> objects.
        /// </summary>
        public static ObjectQuery<XtmTranslationProviderInfo> GetXtmTranslationProviders()
        {
            return ProviderObject.GetObjectQuery();
        }


        /// <summary>
        /// Returns <see cref="XtmTranslationProviderInfo"/> with specified ID.
        /// </summary>
        /// <param name="id"><see cref="XtmTranslationProviderInfo"/> ID.</param>
        public static XtmTranslationProviderInfo GetXtmTranslationProviderInfo(int id)
        {
            return ProviderObject.GetInfoById(id);
        }


        /// <summary>
        /// Returns <see cref="XtmTranslationProviderInfo"/> with specified name.
        /// </summary>
        /// <param name="name"><see cref="XtmTranslationProviderInfo"/> name.</param>
        public static XtmTranslationProviderInfo GetXtmTranslationProviderInfo(string name)
        {
            return ProviderObject.GetInfoByCodeName(name);
        }


        /// <summary>
        /// Sets (updates or inserts) specified <see cref="XtmTranslationProviderInfo"/>.
        /// </summary>
        /// <param name="infoObj"><see cref="XtmTranslationProviderInfo"/> to be set.</param>
        public static void SetXtmTranslationProviderInfo(XtmTranslationProviderInfo infoObj)
        {
            ProviderObject.SetInfo(infoObj);
        }


        /// <summary>
        /// Deletes specified <see cref="XtmTranslationProviderInfo"/>.
        /// </summary>
        /// <param name="infoObj"><see cref="XtmTranslationProviderInfo"/> to be deleted.</param>
        public static void DeleteXtmTranslationProviderInfo(XtmTranslationProviderInfo infoObj)
        {
            ProviderObject.DeleteInfo(infoObj);
        }


        /// <summary>
        /// Deletes <see cref="XtmTranslationProviderInfo"/> with specified ID.
        /// </summary>
        /// <param name="id"><see cref="XtmTranslationProviderInfo"/> ID.</param>
        public static void DeleteXtmTranslationProviderInfo(int id)
        {
            XtmTranslationProviderInfo infoObj = GetXtmTranslationProviderInfo(id);
            DeleteXtmTranslationProviderInfo(infoObj);
        }
    }
}