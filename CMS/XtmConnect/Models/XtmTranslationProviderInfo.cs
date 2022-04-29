using System;
using System.Data;
using System.Runtime.Serialization;
using System.Collections.Generic;

using CMS;
using CMS.DataEngine;
using CMS.Helpers;
using XtmConnect;

[assembly: RegisterObjectType(typeof(XtmTranslationProviderInfo), XtmTranslationProviderInfo.OBJECT_TYPE)]

namespace XtmConnect
{
    /// <summary>
    /// Data container class for <see cref="XtmTranslationProviderInfo"/>.
    /// </summary>
	[Serializable]
    public partial class XtmTranslationProviderInfo : AbstractInfo<XtmTranslationProviderInfo>
    {
        /// <summary>
        /// Object type.
        /// </summary>
        public const string OBJECT_TYPE = "xtmtranslations.xtmtranslationprovider";


        /// <summary>
        /// Type information.
        /// </summary>
#warning "You will need to configure the type info."
        public static readonly ObjectTypeInfo TYPEINFO = new ObjectTypeInfo(typeof(XtmTranslationProviderInfoProvider), OBJECT_TYPE, "XtmConnect.XtmTranslationProvider", "XtmTranslationProviderID", "XtmTranslationProviderLastModified", "XtmTranslationProviderGuid", "XtmTranslationProviderGuid", null, null, null, null, null)
        {
            ModuleName = "XtmConnect",
            TouchCacheDependencies = true,
            DependsOn = new List<ObjectDependency>()
            {
                new ObjectDependency("TranslationProviderID", "cms.translationservice", ObjectDependencyEnum.Required),
            },
        };


        /// <summary>
        /// Xtm translation provider ID.
        /// </summary>
        [DatabaseField]
        public virtual int XtmTranslationProviderID
        {
            get
            {
                return ValidationHelper.GetInteger(GetValue("XtmTranslationProviderID"), 0);
            }
            set
            {
                SetValue("XtmTranslationProviderID", value);
            }
        }


        /// <summary>
        /// Translation provider ID.
        /// </summary>
        [DatabaseField]
        public virtual int TranslationProviderID
        {
            get
            {
                return ValidationHelper.GetInteger(GetValue("TranslationProviderID"), 0);
            }
            set
            {
                SetValue("TranslationProviderID", value);
            }
        }


        /// <summary>
        /// XTM customer ID.
        /// </summary>
        [DatabaseField]
        public virtual long XTMCustomerID
        {
            get
            {
                return ValidationHelper.GetLong(GetValue("XTMCustomerID"), 0);
            }
            set
            {
                SetValue("XTMCustomerID", value);
            }
        }


        /// <summary>
        /// XTM customer name.
        /// </summary>
        [DatabaseField]
        public virtual string XTMCustomerName
        {
            get
            {
                return ValidationHelper.GetString(GetValue("XTMCustomerName"), String.Empty);
            }
            set
            {
                SetValue("XTMCustomerName", value);
            }
        }


        /// <summary>
        /// XTM template ID.
        /// </summary>
        [DatabaseField]
        public virtual long XTMTemplateID
        {
            get
            {
                return ValidationHelper.GetLong(GetValue("XTMTemplateID"), 0);
            }
            set
            {
                SetValue("XTMTemplateID", value, 0);
            }
        }


        /// <summary>
        /// XTM template name.
        /// </summary>
        [DatabaseField]
        public virtual string XTMTemplateName
        {
            get
            {
                return ValidationHelper.GetString(GetValue("XTMTemplateName"), String.Empty);
            }
            set
            {
                SetValue("XTMTemplateName", value, String.Empty);
            }
        }


        /// <summary>
        /// XTM project name prefix.
        /// </summary>
        [DatabaseField]
        public virtual string XTMProjectNamePrefix
        {
            get
            {
                return ValidationHelper.GetString(GetValue("XTMProjectNamePrefix"), String.Empty);
            }
            set
            {
                SetValue("XTMProjectNamePrefix", value, String.Empty);
            }
        }


        /// <summary>
        /// Translation import type.
        /// </summary>
        [DatabaseField]
        public virtual int TranslationImportType
        {
            get
            {
                return ValidationHelper.GetInteger(GetValue("TranslationImportType"), 0);
            }
            set
            {
                SetValue("TranslationImportType", value);
            }
        }


        /// <summary>
        /// XTM webservice URI.
        /// </summary>
        [DatabaseField]
        public virtual string XTMWebserviceURI
        {
            get
            {
                return ValidationHelper.GetString(GetValue("XTMWebserviceURI"), String.Empty);
            }
            set
            {
                SetValue("XTMWebserviceURI", value);
            }
        }


        /// <summary>
        /// XTM username.
        /// </summary>
        [DatabaseField]
        public virtual string XTMUsername
        {
            get
            {
                return ValidationHelper.GetString(GetValue("XTMUsername"), String.Empty);
            }
            set
            {
                SetValue("XTMUsername", value);
            }
        }


        /// <summary>
        /// XTM password.
        /// </summary>
        [DatabaseField]
        public virtual string XTMPassword
        {
            get
            {
                return ValidationHelper.GetString(GetValue("XTMPassword"), String.Empty);
            }
            set
            {
                SetValue("XTMPassword", value);
            }
        }


        /// <summary>
        /// XTM client.
        /// </summary>
        [DatabaseField]
        public virtual string XTMClient
        {
            get
            {
                return ValidationHelper.GetString(GetValue("XTMClient"), String.Empty);
            }
            set
            {
                SetValue("XTMClient", value);
            }
        }


        /// <summary>
        /// Xtm translation provider guid.
        /// </summary>
        [DatabaseField]
        public virtual Guid XtmTranslationProviderGuid
        {
            get
            {
                return ValidationHelper.GetGuid(GetValue("XtmTranslationProviderGuid"), Guid.Empty);
            }
            set
            {
                SetValue("XtmTranslationProviderGuid", value);
            }
        }


        /// <summary>
        /// Xtm translation provider last modified.
        /// </summary>
        [DatabaseField]
        public virtual DateTime XtmTranslationProviderLastModified
        {
            get
            {
                return ValidationHelper.GetDateTime(GetValue("XtmTranslationProviderLastModified"), DateTimeHelper.ZERO_TIME);
            }
            set
            {
                SetValue("XtmTranslationProviderLastModified", value);
            }
        }


        /// <summary>
        /// Deletes the object using appropriate provider.
        /// </summary>
        protected override void DeleteObject()
        {
            XtmTranslationProviderInfoProvider.DeleteXtmTranslationProviderInfo(this);
        }


        /// <summary>
        /// Updates the object using appropriate provider.
        /// </summary>
        protected override void SetObject()
        {
            XtmTranslationProviderInfoProvider.SetXtmTranslationProviderInfo(this);
        }


        /// <summary>
        /// Constructor for de-serialization.
        /// </summary>
        /// <param name="info">Serialization info.</param>
        /// <param name="context">Streaming context.</param>
        protected XtmTranslationProviderInfo(SerializationInfo info, StreamingContext context)
            : base(info, context, TYPEINFO)
        {
        }


        /// <summary>
        /// Creates an empty instance of the <see cref="XtmTranslationProviderInfo"/> class.
        /// </summary>
        public XtmTranslationProviderInfo()
            : base(TYPEINFO)
        {
        }


        /// <summary>
        /// Creates a new instances of the <see cref="XtmTranslationProviderInfo"/> class from the given <see cref="DataRow"/>.
        /// </summary>
        /// <param name="dr">DataRow with the object data.</param>
        public XtmTranslationProviderInfo(DataRow dr)
            : base(TYPEINFO, dr)
        {
        }
    }
}