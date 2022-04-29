using CMS.Base.Web.UI;
using CMS.Base.Web.UI.ActionsConfig;
using CMS.EventLog;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.UIControls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using Xtm.Connector.Config;
using Xtm.Connector.Exceptions;
using Xtm.Connector.Logic.Services.Interface;
using XtmConnect;
using XtmConnect.Config;
using XtmConnect.Enums;
using XtmConnect.Factories;
using XtmConnect.XtmConnect.Exceptions;
using XtmConnect.XtmConnect.Models;
using XtmConnect.XtmConnect.Services.Interfaces;

[UIElement(Constants.Module.CodeName, Constants.Module.UIElements.XtmTranslationProvider_Edit)]
public partial class XtmTranslationProvider_Edit : CMSPage
{
    private IServiceFactory serviceFactory;
    private IXtmTranslationProviderService xtmTranslationProviderService;

    public XtmTranslationProvider_Edit() : this(new ServiceFactory())
    {
    }

    public XtmTranslationProvider_Edit(IServiceFactory serviceFactory) : this(serviceFactory, serviceFactory.CreateXtmTranslationProviderService())
    {
    }

    public XtmTranslationProvider_Edit(IServiceFactory serviceFactory, IXtmTranslationProviderService xtmTranslationProviderService)
    {
        this.serviceFactory = serviceFactory;
        this.xtmTranslationProviderService = xtmTranslationProviderService;
    }

    protected void CustomerDropDownList_SelectedIndexChanged(object sender, EventArgs e)
    {
        GetXTMTemplates();
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        PasswordTextBox.Attributes["value"] = PasswordTextBox.Text;

        ScriptHelper.RegisterBootstrapTooltip(Page, ".info-icon > i");

        var backToListHeader = new HeaderAction
        {
            Text = GetString("Back to list"),
            Index = -100,
            RedirectUrl = UIContextHelper.GetElementUrl(Constants.Module.CodeName, Constants.Module.UIElements.XtmTranslationProvider_List)
        };

        CurrentMaster.HeaderActions.AddAction(backToListHeader);

        if (!Page.IsPostBack)
        {
            var xtmTranslationProviderIdParam = Request.QueryString["id"];

            bool isEdit = !string.IsNullOrEmpty(xtmTranslationProviderIdParam) && ValidationHelper.IsInteger(xtmTranslationProviderIdParam);

            if (!isEdit)
            {
                xtmTranslationProviderIdHidden.Value = "0";
                DisableCustomerTemplateDropDowns();
                PopulateImportTypeDropdown();
            }
            else
            {
                try
                {
                    xtmTranslationProviderIdHidden.Value = xtmTranslationProviderIdParam;
                    var xtmTranslationProviderId = int.Parse(xtmTranslationProviderIdParam);

                    var xtmProviderFullData = xtmTranslationProviderService.GetXtmTranslationProviderFullData(xtmTranslationProviderId);
                    var xtmProviderDetails = XtmTranslationProviderInfoProvider.GetXtmTranslationProviderInfo(xtmTranslationProviderId);

                    EnableCustomerTemplateDropDowns();
                    EnableCustomerTemplateRefreshButtons();

                    ProviderNameTextBox.Text = xtmProviderFullData.ProviderName;
                    ClientNameTextBox.Text = xtmProviderDetails.XTMClient;
                    UsernameTextBox.Text = xtmProviderDetails.XTMUsername;
                    PasswordTextBox.Text = xtmProviderDetails.XTMPassword;
                    WebserviceURITextBox.Text = xtmProviderDetails.XTMWebserviceURI;

                    ProjectNamePrefixTextBox.Text = xtmProviderDetails.XTMProjectNamePrefix;

                    PopulateImportTypeDropdown();
                    var importType = ImportTypeDropDownList.Items.FindByValue((xtmProviderDetails.TranslationImportType).ToString());
                    ImportTypeDropDownList.ClearSelection();
                    importType.Selected = true;

                    canProvideDeadlineCheckbox.Checked = xtmProviderFullData.DeadlineEnabled;
                    GetXTMCustomers();
                    var customer = CustomerDropDownList.Items.FindByValue(xtmProviderDetails.XTMCustomerID.ToString());

                    // provider's customer is available in XTM
                    if (customer != null)
                    {
                        CustomerDropDownList.ClearSelection();
                        customer.Selected = true;
                    }
                    else // provider has settings with customer that was deleted in  XTM SYSTEM
                    {
                        ShowMessage(MessageTypeEnum.Warning, String.Format(GetString("CustomerDoesNotExists"), xtmProviderDetails.XTMCustomerName), null, null, true);
                        GetXTMTemplates();
                        return;
                    }

                    GetXTMTemplates();
                    if (xtmProviderDetails.XTMTemplateID != 0) // there was a selected template
                    {
                        var template = TemplateDropDownList.Items.FindByValue(xtmProviderDetails.XTMTemplateID.ToString());
                        // provider's template is available in XTM
                        if (template != null)
                        {
                            TemplateDropDownList.ClearSelection();
                            template.Selected = true;
                        }
                        else // provider's template was deleted in XTM
                        {
                            ShowMessage(MessageTypeEnum.Warning, String.Format(GetString("TemplateDoesNotExists"), xtmProviderDetails.XTMTemplateName), null, null, true);
                        }
                    }
                    else
                    {
                        TemplateDropDownList.ClearSelection();
                    }
                }
                catch (Exception ex)
                {
                    EventLogProvider.LogException(Constants.Module.DisplayName, Constants.Module.EventCode.Exception, ex, 0, "Edit xtm translation provider exception");
                    ShowMessage(MessageTypeEnum.Error, GetString("UnknownExceptionError"), null, null, true);
                }
            }
        }
    }

    protected void ReloadCustomersButtonClick(object sender, EventArgs e)
    {
        GetXTMCustomers();
        if (CustomerDropDownList.SelectedItem != null)
        {
            GetXTMTemplates();
            ShowMessage(MessageTypeEnum.Information, GetString("CustomersAndTemplates"), null, null, true);
        }
        else
        {
            ShowMessage(MessageTypeEnum.Warning, GetString("NoCustomersFound"), null, null, true);
        }
    }

    protected void ReloadTemplatesButtonClick(object sender, EventArgs e)
    {
        GetXTMTemplates();
    }

    protected void SaveButtonClick(object sender, EventArgs e)
    {
        if (!CheckPermissions(Constants.Module.CodeName, Constants.Module.Permissions.Modify))
        {
            ShowMessage(MessageTypeEnum.Error, GetString("NotAllowed"), null, null, true);
        }

        if (TestConnection())
        {
            try
            {
                var xtmTranslationProviderFulldata = new XtmTranslationProviderFullData()
                {
                    XtmTranslationProvider = new XtmTranslationProviderInfo()
                    {
                        XtmTranslationProviderID = int.Parse(xtmTranslationProviderIdHidden.Value),
                        XTMClient = ClientNameTextBox.Text,
                        XTMUsername = UsernameTextBox.Text,
                        XTMPassword = PasswordTextBox.Text,
                        XTMWebserviceURI = WebserviceURITextBox.Text,

                        XTMCustomerID = long.Parse(CustomerDropDownList.SelectedValue),
                        XTMCustomerName = CustomerDropDownList.SelectedItem.Text,

                        XTMTemplateID = !string.IsNullOrEmpty(TemplateDropDownList.SelectedValue) ? long.Parse(TemplateDropDownList.SelectedValue) : 0,
                        XTMTemplateName = TemplateDropDownList.SelectedItem.Text,

                        XTMProjectNamePrefix = ProjectNamePrefixTextBox.Text,
                        TranslationImportType = int.Parse(ImportTypeDropDownList.SelectedValue)
                    },
                    DeadlineEnabled = canProvideDeadlineCheckbox.Checked,
                    ProviderName = ProviderNameTextBox.Text
                };

                var resultID = xtmTranslationProviderService.CreateOrUpdateProvider(xtmTranslationProviderFulldata);

                bool isNew = xtmTranslationProviderIdHidden.Value == "0";
                var actionName = isNew ? GetString("Added") : GetString("Updated");

                xtmTranslationProviderIdHidden.Value = resultID.ToString();
                ShowMessage(MessageTypeEnum.Confirmation, String.Format(GetString("SuccesfullyAction"), actionName) + ProviderNameTextBox.Text, null, null, true);
            }
            catch (XtmModuleException ex)
            {
                ShowMessage(MessageTypeEnum.Error, "Error: " + ex.Message, null, null, true);
            }
            catch (Exception ex)
            {
                EventLogProvider.LogException(Constants.Module.DisplayName, Constants.Module.EventCode.Exception, ex, 0, "Save xtm translation provider exception");
                ShowMessage(MessageTypeEnum.Error, GetString("UnknownExceptionError"), null, null, true);
            }
        }
    }

    protected void TestConnectionClick(object sender, EventArgs e)
    {
        TestConnection();
    }

    private void DisableCustomerTemplateDropDowns()
    {
        CustomerDropDownList.Enabled = false;
        TemplateDropDownList.Enabled = false;
    }

    private void DisableCustomerTemplateRefreshButtons()
    {
        ReloadCustomersLinkButton.Enabled = false;
        ReloadTemplatesLinkButton.Enabled = false;
    }

    private void EnableCustomerTemplateDropDowns()
    {
        CustomerDropDownList.Enabled = true;
        TemplateDropDownList.Enabled = true;
    }

    private void EnableCustomerTemplateRefreshButtons()
    {
        ReloadCustomersLinkButton.Enabled = true;
        ReloadTemplatesLinkButton.Enabled = true;
    }

    private void GetXTMCustomers()
    {
        try
        {
            IXtmConnector xtmConnector = serviceFactory.CreateXtmConnector(new XtmConnectorConfiguration(ClientNameTextBox.Text, UsernameTextBox.Text, PasswordTextBox.Text, WebserviceURITextBox.Text, Constants.Module.KenticoXtmIntegrationKey));
            var customers = xtmConnector.GetCustomers();

            CustomerDropDownList.Items.Clear();
            CustomerDropDownList.Items.AddRange(customers.Select(x => new ListItem() { Enabled = true, Selected = false, Text = x.Name, Value = x.Id.ToString() }).ToArray());
            CustomerDropDownList.DataBind();

            TemplateDropDownList.Items.Clear();
            TemplateDropDownList.ClearSelection();
            ShowMessage(MessageTypeEnum.Information, GetString("CustomersObtained"), null, null, true);
        }
        catch (XtmConnectorException ex)
        {
            ShowMessage(MessageTypeEnum.Error, GetString("ErrorGettingCustomers") + ex.Message, null, null, true);
        }
        catch (Exception ex)
        {
            EventLogProvider.LogException(Constants.Module.DisplayName, Constants.Module.EventCode.Exception, ex, 0, "Xtm botain customeres exception");
            ShowMessage(MessageTypeEnum.Error, GetString("UnknownExceptionError"), null, null, true);
        }
    }

    private void GetXTMTemplates()
    {
        try
        {
            var selectedCustomer = CustomerDropDownList.SelectedItem;
            if (selectedCustomer == null)
            {
                ShowMessage(MessageTypeEnum.Warning, GetString("SelectCustomer"), null, null, true);
                return;
            }

            TemplateDropDownList.Items.Clear();

            IXtmConnector xtmConnector = serviceFactory.CreateXtmConnector(new XtmConnectorConfiguration(ClientNameTextBox.Text, UsernameTextBox.Text, PasswordTextBox.Text, WebserviceURITextBox.Text, Constants.Module.KenticoXtmIntegrationKey));
            var templates = xtmConnector.GetTemplates(long.Parse(CustomerDropDownList.SelectedValue));

            TemplateDropDownList.Items.Add(new ListItem() { Enabled = true, Selected = false, Text = string.Empty, Value = string.Empty });
            TemplateDropDownList.Items.AddRange(templates.Select(x => new ListItem() { Enabled = true, Selected = false, Text = x.Name, Value = x.Id.ToString() }).ToArray());
            TemplateDropDownList.DataBind();

            ShowMessage(MessageTypeEnum.Information, GetString("TemplatesObtained"), null, null, true);
        }
        catch (XtmConnectorException ex)
        {
            ShowMessage(MessageTypeEnum.Error, GetString("ErrorGettingTemplates") + ex.Message, null, null, true);
        }
        catch (Exception ex)
        {
            EventLogProvider.LogException(Constants.Module.DisplayName, Constants.Module.EventCode.Exception, ex, 0, "Xtm obtain templates exception");
            ShowMessage(MessageTypeEnum.Error, GetString("UnknownExceptionError"), null, null, true);
        }
    }

    private void PopulateImportTypeDropdown()
    {
        var optionsList = new List<ListItem>();
        optionsList.Add(new ListItem() { Enabled = true, Selected = true, Text = GetString("SelectImportType"), Value = string.Empty }); // add empty item
        optionsList.AddRange(Enum.GetValues(typeof(TranslationImportType)).Cast<TranslationImportType>().Select(enumValue =>
        {
            return new ListItem() { Enabled = true, Selected = false, Text = enumValue.ToStringRepresentation(), Value = ((int)enumValue).ToString() };
        }).ToList());

        ImportTypeDropDownList.Items.AddRange(optionsList.ToArray());
        ImportTypeDropDownList.DataBind();
    }

    private bool TestConnection()
    {
        bool testSuccess = false;

        try
        {
            IXtmConnector xtmConnector = serviceFactory.CreateXtmConnector(new XtmConnectorConfiguration(ClientNameTextBox.Text, UsernameTextBox.Text, PasswordTextBox.Text, WebserviceURITextBox.Text, Constants.Module.KenticoXtmIntegrationKey));
            xtmConnector.TestConnection();
            ShowMessage(MessageTypeEnum.Confirmation, GetString("ConnectionSuccessful"), null, null, true);
            EnableCustomerTemplateDropDowns();
            EnableCustomerTemplateRefreshButtons();
            testSuccess = true;
        }
        catch (XtmConnectorException ex)
        {
            ShowMessage(MessageTypeEnum.Error, GetString("ConnectionError") + ex.Message, null, null, true);
        }
        catch (Exception ex)
        {
            EventLogProvider.LogException(Constants.Module.DisplayName, Constants.Module.EventCode.Exception, ex, 0, "Xtm test connection exception");
            ShowMessage(MessageTypeEnum.Error, GetString("UnknownExceptionError"), null, null, true);
        }
        finally
        {
            if (!testSuccess)
            {
                DisableCustomerTemplateDropDowns();
                DisableCustomerTemplateRefreshButtons();
            }
        }
        return testSuccess;
    }
}