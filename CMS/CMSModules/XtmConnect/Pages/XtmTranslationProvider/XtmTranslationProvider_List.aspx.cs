using CMS.Base.Web.UI;
using CMS.Base.Web.UI.ActionsConfig;
using CMS.EventLog;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.TranslationServices;
using CMS.UIControls;
using System;
using System.Data;
using System.Linq;
using System.Web.UI.WebControls;
using Xtm.Connector.Exceptions;
using XtmConnect;
using XtmConnect.Config;
using XtmConnect.Enums;
using XtmConnect.Factories;
using XtmConnect.ViewModels;
using XtmConnect.XtmConnect.Helpers;
using XtmConnect.XtmConnect.Services.Interfaces;

[UIElement(Constants.Module.CodeName, Constants.Module.UIElements.XtmTranslationProvider_List)]
public partial class XtmTranslationProvider_List : CMSPage
{
    private IXtmTranslationProviderService xtmTranslationProviderService;

    public XtmTranslationProvider_List() : this(new ServiceFactory())
    {
    }

    public XtmTranslationProvider_List(IServiceFactory serviceFactory) : this(serviceFactory.CreateXtmTranslationProviderService())
    {
    }

    public XtmTranslationProvider_List(IXtmTranslationProviderService xtmTranslationProviderService)
    {
        this.xtmTranslationProviderService = xtmTranslationProviderService;
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        var newTranslationProviderAction = new HeaderAction
        {
            Text = GetString("AddProvider"),
            RedirectUrl = UIContextHelper.GetElementUrl("XtmConnect", Constants.Module.UIElements.XtmTranslationProvider_Edit)
        };
        CurrentMaster.HeaderActions.AddAction(newTranslationProviderAction);

        FillXtmTranslationProvidersGrid();
        XtmTranslationProvidersGrid.OnAction += XtmTranslationProvidersGrid_OnAction;
    }

    protected void ResetFilterButtonClick(object sender, EventArgs e)
    {
        FilterTextBox.Text = string.Empty;
        FillXtmTranslationProvidersGrid();
    }

    protected void SearchFilterButtonClick(object sender, EventArgs e)
    {
        FillXtmTranslationProvidersGrid();
    }

    protected void XtmTranslationProvidersGrid_OnAction(string actionName, object actionArgument)
    {
        if (actionName == Constants.Module.Action.Edit)
        {
            if (!CheckPermissions(Constants.Module.CodeName, Constants.Module.Permissions.Modify))
                ShowMessage(MessageTypeEnum.Error, GetString("NotAllowed"), null, null, true);

            int xtmTranslationProviderId = ValidationHelper.GetInteger(actionArgument, 0);
            string editUrl = UIContextHelper.GetElementUrl(Constants.Module.CodeName, Constants.Module.UIElements.XtmTranslationProvider_Edit);
            editUrl = URLHelper.AddParameterToUrl(editUrl, "id", xtmTranslationProviderId.ToString());
            URLHelper.Redirect(editUrl);
        }
        if (actionName == Constants.Module.Action.Delete)
        {
            if (!CheckPermissions(Constants.Module.CodeName, Constants.Module.Permissions.Modify))
                ShowMessage(MessageTypeEnum.Error, GetString("NotAllowed"), null, null, true);

            int xtmTranslationProviderId = ValidationHelper.GetInteger(actionArgument, 0);

            try
            {
                var xtmProviderToDelete = XtmTranslationProviderInfoProvider.GetXtmTranslationProviderInfo(xtmTranslationProviderId);
                var kenticoProvider = TranslationServiceInfoProvider.GetTranslationServiceInfo(xtmProviderToDelete.TranslationProviderID);

                xtmTranslationProviderService.DeleteProvider(xtmTranslationProviderId);
                ShowMessage(MessageTypeEnum.Confirmation, String.Format(GetString("SuccesfullDeleted"), kenticoProvider.TranslationServiceDisplayName), null, null, true);
            }
            catch (XtmConnectorException ex)
            {
                ShowMessage(MessageTypeEnum.Error, GetString("DeletionError") + ex.Message, null, null, true);
            }
            catch (Exception ex)
            {
                EventLogProvider.LogException(Constants.Module.DisplayName, Constants.Module.EventCode.Exception, ex, 0, "Xtm translation provider deletion exception");
                ShowMessage(MessageTypeEnum.Error, GetString("UnknownExceptionError"), null, null, true);
            }
            finally
            {
                FillXtmTranslationProvidersGrid();
            }
        }
    }

    private void FillXtmTranslationProvidersGrid()
    {
        var filterValue = FilterTextBox.Text;

        Func<XtmTranslationProviderVM, bool> filterConditions;

        if (string.IsNullOrEmpty(filterValue)) // filter textbox is empty
            filterConditions = x => true; // do not create any filter on collection
        else
        {
            filterConditions = x => x.XTMCustomerName.Contains(filterValue) || x.XTMTemplateName.Contains(filterValue) || x.XTMProjectNamePrefix.Contains(filterValue)
            || x.TranslationImportType.Contains(filterValue) || x.ProviderName.Contains(filterValue);
        }

        var xtmProviders = xtmTranslationProviderService.GetAllXtmTranslationProviderFullData();
        var providersListVM = xtmProviders.Select(prov => new XtmTranslationProviderVM()
        {
            XtmTranslationProviderID = prov.XtmTranslationProvider.XtmTranslationProviderID,
            ProviderName = prov.ProviderName,
            XTMCustomerName = prov.XtmTranslationProvider.XTMCustomerName,
            XTMTemplateName = prov.XtmTranslationProvider.XTMTemplateName,
            XTMProjectNamePrefix = prov.XtmTranslationProvider.XTMProjectNamePrefix,
            TranslationImportType = ((TranslationImportType)prov.XtmTranslationProvider.TranslationImportType).ToStringRepresentation(),
        }).Where(filterConditions).ToList();
        XtmTranslationProvidersGrid.DataSource = DataSetHelper.PrepareDataSet(providersListVM);
        XtmTranslationProvidersGrid.ReloadData();

        // if there are no providers to display, hide filter
        // hide only only if there was no filter applied; otherwise if there are ...
        // ... any providers and you filter out all of them, you cannot cancel the filter because it's hidden
        if (!providersListVM.Any() && string.IsNullOrEmpty(FilterTextBox.Text))
            FilterSection.Visible = false;
    }
}