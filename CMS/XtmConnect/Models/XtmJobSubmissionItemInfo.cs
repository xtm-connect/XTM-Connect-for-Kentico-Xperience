using System;
using System.Data;
using System.Runtime.Serialization;

using CMS;
using CMS.DataEngine;
using CMS.Helpers;
using XtmConnect;

[assembly: RegisterObjectType(typeof(XtmJobSubmissionItemInfo), XtmJobSubmissionItemInfo.OBJECT_TYPE)]

namespace XtmConnect
{
    /// <summary>
    /// Data container class for <see cref="XtmJobSubmissionItemInfo"/>.
    /// This class can be used to associate Kentico Submisison Items and XTM JOBS without using IntegrationId.
    /// </summary>
	[Serializable]
    public partial class XtmJobSubmissionItemInfo : AbstractInfo<XtmJobSubmissionItemInfo>
    {
        /// <summary>
        /// Object type.
        /// </summary>
        public const string OBJECT_TYPE = "xtmtranslations.xtmjobsubmissionitem";


        /// <summary>
        /// Type information.
        /// </summary>
#warning "You will need to configure the type info."
        public static readonly ObjectTypeInfo TYPEINFO = new ObjectTypeInfo(typeof(XtmJobSubmissionItemInfoProvider), OBJECT_TYPE, "XtmConnect.XtmJobSubmissionItem", "XtmJobSubmissionItemID", "XtmJobSubmissionItemLastModified", "XtmJobSubmissionItemGuid", null, null, null, null, null, null)
        {
            ModuleName = "XtmConnect",
            TouchCacheDependencies = true,
        };


        /// <summary>
        /// Xtm job submission item ID.
        /// </summary>
        [DatabaseField]
        public virtual int XtmJobSubmissionItemID
        {
            get
            {
                return ValidationHelper.GetInteger(GetValue("XtmJobSubmissionItemID"), 0);
            }
            set
            {
                SetValue("XtmJobSubmissionItemID", value);
            }
        }


        /// <summary>
        /// Submission ID.
        /// </summary>
        [DatabaseField]
        public virtual int SubmissionID
        {
            get
            {
                return ValidationHelper.GetInteger(GetValue("SubmissionID"), 0);
            }
            set
            {
                SetValue("SubmissionID", value);
            }
        }


        /// <summary>
        /// Submission item ID.
        /// </summary>
        [DatabaseField]
        public virtual int SubmissionItemID
        {
            get
            {
                return ValidationHelper.GetInteger(GetValue("SubmissionItemID"), 0);
            }
            set
            {
                SetValue("SubmissionItemID", value);
            }
        }


        /// <summary>
        /// XTM project ID.
        /// </summary>
        [DatabaseField]
        public virtual long XTMProjectID
        {
            get
            {
                return ValidationHelper.GetLong(GetValue("XTMProjectID"), 0);
            }
            set
            {
                SetValue("XTMProjectID", value);
            }
        }


        /// <summary>
        /// XTM job ID.
        /// </summary>
        [DatabaseField]
        public virtual long XTMJobID
        {
            get
            {
                return ValidationHelper.GetLong(GetValue("XTMJobID"), 0);
            }
            set
            {
                SetValue("XTMJobID", value);
            }
        }


        /// <summary>
        /// Xtm job submission item guid.
        /// </summary>
        [DatabaseField]
        public virtual Guid XtmJobSubmissionItemGuid
        {
            get
            {
                return ValidationHelper.GetGuid(GetValue("XtmJobSubmissionItemGuid"), Guid.Empty);
            }
            set
            {
                SetValue("XtmJobSubmissionItemGuid", value);
            }
        }


        /// <summary>
        /// Xtm job submission item last modified.
        /// </summary>
        [DatabaseField]
        public virtual DateTime XtmJobSubmissionItemLastModified
        {
            get
            {
                return ValidationHelper.GetDateTime(GetValue("XtmJobSubmissionItemLastModified"), DateTimeHelper.ZERO_TIME);
            }
            set
            {
                SetValue("XtmJobSubmissionItemLastModified", value);
            }
        }


        /// <summary>
        /// Deletes the object using appropriate provider.
        /// </summary>
        protected override void DeleteObject()
        {
            XtmJobSubmissionItemInfoProvider.DeleteXtmJobSubmissionItemInfo(this);
        }


        /// <summary>
        /// Updates the object using appropriate provider.
        /// </summary>
        protected override void SetObject()
        {
            XtmJobSubmissionItemInfoProvider.SetXtmJobSubmissionItemInfo(this);
        }


        /// <summary>
        /// Constructor for de-serialization.
        /// </summary>
        /// <param name="info">Serialization info.</param>
        /// <param name="context">Streaming context.</param>
        protected XtmJobSubmissionItemInfo(SerializationInfo info, StreamingContext context)
            : base(info, context, TYPEINFO)
        {
        }


        /// <summary>
        /// Creates an empty instance of the <see cref="XtmJobSubmissionItemInfo"/> class.
        /// </summary>
        public XtmJobSubmissionItemInfo()
            : base(TYPEINFO)
        {
        }


        /// <summary>
        /// Creates a new instances of the <see cref="XtmJobSubmissionItemInfo"/> class from the given <see cref="DataRow"/>.
        /// </summary>
        /// <param name="dr">DataRow with the object data.</param>
        public XtmJobSubmissionItemInfo(DataRow dr)
            : base(TYPEINFO, dr)
        {
        }
    }
}