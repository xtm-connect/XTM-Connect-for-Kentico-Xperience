using System;
using System.Data;

using CMS.Base;
using CMS.DataEngine;
using CMS.Helpers;

namespace XtmConnect
{
    /// <summary>
    /// Class providing <see cref="XtmJobSubmissionItemInfo"/> management.
    /// </summary>
    public partial class XtmJobSubmissionItemInfoProvider : AbstractInfoProvider<XtmJobSubmissionItemInfo, XtmJobSubmissionItemInfoProvider>
    {
        /// <summary>
        /// Creates an instance of <see cref="XtmJobSubmissionItemInfoProvider"/>.
        /// </summary>
        public XtmJobSubmissionItemInfoProvider()
            : base(XtmJobSubmissionItemInfo.TYPEINFO)
        {
        }


        /// <summary>
        /// Returns a query for all the <see cref="XtmJobSubmissionItemInfo"/> objects.
        /// </summary>
        public static ObjectQuery<XtmJobSubmissionItemInfo> GetXtmJobSubmissionItems()
        {
            return ProviderObject.GetObjectQuery();
        }


        /// <summary>
        /// Returns <see cref="XtmJobSubmissionItemInfo"/> with specified ID.
        /// </summary>
        /// <param name="id"><see cref="XtmJobSubmissionItemInfo"/> ID.</param>
        public static XtmJobSubmissionItemInfo GetXtmJobSubmissionItemInfo(int id)
        {
            return ProviderObject.GetInfoById(id);
        }


        /// <summary>
        /// Sets (updates or inserts) specified <see cref="XtmJobSubmissionItemInfo"/>.
        /// </summary>
        /// <param name="infoObj"><see cref="XtmJobSubmissionItemInfo"/> to be set.</param>
        public static void SetXtmJobSubmissionItemInfo(XtmJobSubmissionItemInfo infoObj)
        {
            ProviderObject.SetInfo(infoObj);
        }


        /// <summary>
        /// Deletes specified <see cref="XtmJobSubmissionItemInfo"/>.
        /// </summary>
        /// <param name="infoObj"><see cref="XtmJobSubmissionItemInfo"/> to be deleted.</param>
        public static void DeleteXtmJobSubmissionItemInfo(XtmJobSubmissionItemInfo infoObj)
        {
            ProviderObject.DeleteInfo(infoObj);
        }


        /// <summary>
        /// Deletes <see cref="XtmJobSubmissionItemInfo"/> with specified ID.
        /// </summary>
        /// <param name="id"><see cref="XtmJobSubmissionItemInfo"/> ID.</param>
        public static void DeleteXtmJobSubmissionItemInfo(int id)
        {
            XtmJobSubmissionItemInfo infoObj = GetXtmJobSubmissionItemInfo(id);
            DeleteXtmJobSubmissionItemInfo(infoObj);
        }
    }
}