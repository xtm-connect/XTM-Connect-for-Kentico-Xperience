<%@ Page Language="C#" AutoEventWireup="true" Inherits="XtmTranslationProvider_Edit" Title="Edit XTM translation provider"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" CodeBehind="XtmTranslationProvider_Edit.aspx.cs" %>


<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>
<%@ Register Namespace="CMS.UIControls.UniGridConfig" TagPrefix="ug" Assembly="CMS.UIControls" %>

<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <div class="cms-bootstrap">
        <cms:MessagesPlaceHolder ID="plcMess" runat="server" Visible="false" />

        <div class="form-horizontal">

            <%--<cms:FormCategoryHeading runat="server" ID="FormCategoryHeading2" Level="3" IsAnchor="true" Text="XTM Translation Provider: ADD" EnableViewState="false" />--%>
            <asp:HiddenField ID="xtmTranslationProviderIdHidden" runat="server" />

            <div id="general">
                <cms:FormCategoryHeading runat="server" ID="headGeneral" Level="4" IsAnchor="true" Text="General" EnableViewState="false" />
                <div class="form-group">
                    <div class="editing-form-label-cell">
                        <cms:FormLabel CssClass="control-label" runat="server" EnableViewState="false"
                            DisplayColon="true" Text="Provider name" ShowRequiredMark="True" AssociatedControlID="ProviderNameTextBox" />
                    </div>
                    <div class="editing-form-value-cell">
                        <cms:CMSTextBox ID="ProviderNameTextBox" runat="server" />
                        <span class="info-icon">
                            <cms:CMSIcon ToolTip="Provider name must be unique" runat="server" CssClass="icon-question-circle" EnableViewState="false" aria-hidden="true" data-html="true" />
                        </span>
                        <asp:RequiredFieldValidator ID="rfvProviderNameTextBox" runat="server" ErrorMessage="Provider name cannot be blank" ControlToValidate="ProviderNameTextBox" ForeColor="Red"></asp:RequiredFieldValidator>
                    </div>
                </div>
            </div>

            <div id="connection">
                <cms:FormCategoryHeading runat="server" ID="headConnection" Level="4" IsAnchor="true" Text="Connection" EnableViewState="false" />
                <cms:FormCategoryHeading CssClass="col-md-offset-1" runat="server" ID="headLogin" Level="5" IsAnchor="true" Text="Login details" EnableViewState="false" />

                <div class="form-group">
                    <div class="editing-form-label-cell">
                        <cms:FormLabel CssClass="control-label" runat="server" EnableViewState="false"
                            DisplayColon="true" Text="Company" ShowRequiredMark="True" AssociatedControlID="ClientNameTextBox" />
                    </div>
                    <div class="editing-form-value-cell control-group-inline-forced">
                        <cms:CMSTextBox ID="ClientNameTextBox" ToolTip="Name of your company in XTM" runat="server" />
                        <span class="info-icon">
                            <cms:CMSIcon ToolTip="Name of your company in XTM" runat="server" CssClass="icon-question-circle" EnableViewState="false" aria-hidden="true" data-html="true" />
                        </span>
                        <asp:RequiredFieldValidator ID="rfvClientNameTextBox" Display="dynamic" runat="server" ErrorMessage="Company cannot be blank" ControlToValidate="ClientNameTextBox" ForeColor="Red"></asp:RequiredFieldValidator>
                        <asp:RequiredFieldValidator ValidationGroup="connectionConfigValidationGroup" ID="rfvClientNameTextBox2" Display="dynamic" runat="server" ErrorMessage="Company cannot be blank" ControlToValidate="ClientNameTextBox" ForeColor="Red"></asp:RequiredFieldValidator>
                    </div>
                </div>

                <div class="form-group">
                    <div class="editing-form-label-cell">
                        <cms:FormLabel CssClass="control-label" runat="server" EnableViewState="false"
                            DisplayColon="true" Text="Username" ShowRequiredMark="True" AssociatedControlID="UsernameTextBox" />
                    </div>
                    <div class="editing-form-value-cell control-group-inline-forced">
                        <cms:CMSTextBox ID="UsernameTextBox" runat="server" />
                        <span class="info-icon">
                            <cms:CMSIcon ToolTip="User login in XTM" runat="server" CssClass="icon-question-circle" EnableViewState="false" aria-hidden="true" />
                        </span>
                        <asp:RequiredFieldValidator ID="rfvUsernameTextBox" Display="dynamic" runat="server" ErrorMessage="Username name cannot be blank" ControlToValidate="UsernameTextBox" ForeColor="Red"></asp:RequiredFieldValidator>
                        <asp:RequiredFieldValidator ValidationGroup="connectionConfigValidationGroup" ID="rfvUsernameTextBox2" Display="dynamic" runat="server" ErrorMessage="Username name cannot be blank" ControlToValidate="UsernameTextBox" ForeColor="Red"></asp:RequiredFieldValidator>
                    </div>
                </div>

                <div class="form-group">
                    <div class="editing-form-label-cell">
                        <cms:FormLabel CssClass="control-label" runat="server" EnableViewState="false"
                            DisplayColon="true" Text="Password" ShowRequiredMark="True" AssociatedControlID="PasswordTextBox" />
                    </div>
                    <div class="editing-form-value-cell">
                        <cms:CMSTextBox ID="PasswordTextBox" TextMode="Password" runat="server" />
                        <span class="info-icon">
                            <cms:CMSIcon ToolTip="User password in XTM" runat="server" CssClass="icon-question-circle" EnableViewState="false" aria-hidden="true" data-html="true" />
                        </span>
                        <asp:RequiredFieldValidator ID="rfvPasswordTextBox" Display="dynamic" runat="server" ErrorMessage="Password cannot be blank" ControlToValidate="PasswordTextBox" ForeColor="Red"></asp:RequiredFieldValidator>
                        <asp:RequiredFieldValidator ValidationGroup="connectionConfigValidationGroup" ID="rfvPasswordTextBox2" Display="dynamic" runat="server" ErrorMessage="Password cannot be blank" ControlToValidate="PasswordTextBox" ForeColor="Red"></asp:RequiredFieldValidator>
                    </div>
                </div>

                <cms:FormCategoryHeading CssClass="col-md-offset-1" runat="server" ID="headWebservice" Level="5" IsAnchor="true" Text="Address" EnableViewState="false" />
                <div class="form-group">
                    <div class="editing-form-label-cell">
                        <cms:FormLabel CssClass="control-label" runat="server" EnableViewState="false"
                            DisplayColon="true" Text="Webservice URI" ShowRequiredMark="True" AssociatedControlID="WebserviceURITextBox" />
                    </div>
                    <div class="editing-form-value-cell">
                        <cms:CMSTextBox ID="WebserviceURITextBox" runat="server" />
                        <span class="info-icon">
                            <cms:CMSIcon ToolTip="URI address of XTM webservice - e.g. https://www.xtm-cloud.com/project-manager-gui/services/v2/projectmanager/XTMWebService" runat="server" CssClass="icon-question-circle" EnableViewState="false" aria-hidden="true" data-html="true" />
                        </span>
                        <asp:RequiredFieldValidator ID="rfvWebserviceURITextBox" Display="dynamic" runat="server" ErrorMessage="Webservice URI cannot be blank" ControlToValidate="WebserviceURITextBox" ForeColor="Red"></asp:RequiredFieldValidator>
                        <asp:RequiredFieldValidator ValidationGroup="connectionConfigValidationGroup" ID="rfvWebserviceURITextBox2" Display="dynamic" runat="server" ErrorMessage="Webservice URI cannot be blank" ControlToValidate="WebserviceURITextBox" ForeColor="Red"></asp:RequiredFieldValidator>
                    </div>
                </div>
                <div class="form-group">
                    <div class="editing-form-value-cell editing-form-value-cell-offset">
                        <asp:Button ValidationGroup="connectionConfigValidationGroup" CssClass="btn btn-secondary" ID="TestConnectionButton" runat="server" Text="Test connection" OnClick="TestConnectionClick" />
                    </div>
                </div>
            </div>

            <div id="basicSettings">
                <cms:FormCategoryHeading runat="server" ID="basicSettingsHead" Level="4" IsAnchor="true" Text="Basic settings" EnableViewState="false" />
                <div class="form-group">
                    <div class="editing-form-label-cell">
                        <cms:FormLabel CssClass="control-label" runat="server" EnableViewState="false"
                            DisplayColon="true" Text="Customer" ShowRequiredMark="True" AssociatedControlID="CustomerDropDownList" />
                    </div>
                    <div class="editing-form-value-cell control-group-inline-forced">
                        <cms:CMSDropDownList ID="CustomerDropDownList" runat="server"
                            OnSelectedIndexChanged="CustomerDropDownList_SelectedIndexChanged"
                            AutoPostBack="true" />
                        <span class="info-icon">
                            <cms:CMSIcon ToolTip="XTM customer for whom projects will be created" runat="server" CssClass="icon-question-circle" EnableViewState="false" aria-hidden="true" data-html="true" />
                        </span>
                        <cms:CMSAccessibleLinkButton Enabled="false" ValidationGroup="connectionConfigValidationGroup" OnClick="ReloadCustomersButtonClick" ToolTip="Click to get customers from XTM" runat="server" ID="ReloadCustomersLinkButton" IconCssClass="icon-rotate-double-right cms-icon-80" CssClass="btn-icon" />
                        <asp:RequiredFieldValidator Display="dynamic" ID="rfvCustomerDropDownList" runat="server" ErrorMessage="Please select the customer" ControlToValidate="CustomerDropDownList" ForeColor="Red"></asp:RequiredFieldValidator>
                    </div>
                </div>

                <div class="form-group">
                    <div class="editing-form-label-cell">
                        <cms:FormLabel CssClass="control-label" runat="server" EnableViewState="false"
                            DisplayColon="true" Text="Template" ShowRequiredMark="False" AssociatedControlID="TemplateDropDownList" />
                    </div>
                    <div class="editing-form-value-cell control-group-inline-forced">
                        <cms:CMSDropDownList ID="TemplateDropDownList" runat="server"
                            AutoPostBack="true" />
                        <span class="info-icon">
                            <cms:CMSIcon ToolTip="XTM template for created project" runat="server" CssClass="icon-question-circle" EnableViewState="false" aria-hidden="true" data-html="true" />
                        </span>
                        <cms:CMSAccessibleLinkButton Enabled="false" ValidationGroup="connectionConfigValidationGroup" OnClick="ReloadTemplatesButtonClick" ToolTip="Click to get templates from XTM" runat="server" ID="ReloadTemplatesLinkButton" IconCssClass="icon-rotate-double-right cms-icon-80" CssClass="btn-icon" />
                        <%--<asp:RequiredFieldValidator Display="dynamic" ID="rfvTemplateDropDownList" runat="server" ErrorMessage="Please select the template" ControlToValidate="TemplateDropDownList" ForeColor="Red"></asp:RequiredFieldValidator>--%>
                    </div>
                </div>
                <div class="form-group">
                    <div class="editing-form-label-cell">
                        <cms:FormLabel CssClass="control-label" runat="server" EnableViewState="false"
                            DisplayColon="true" Text="Import type" ShowRequiredMark="True" AssociatedControlID="ImportTypeDropDownList" />
                    </div>
                    <div class="editing-form-value-cell control-group-inline-forced">
                        <cms:CMSDropDownList ID="ImportTypeDropDownList" runat="server"
                            AutoPostBack="true" />
                        <span class="info-icon">
                            <cms:CMSIcon ToolTip="Please define when the translated content will be imported back to the system. Check project completion - content imported only when whole project is finished; Check job completion - content imported when single job is done" runat="server" CssClass="icon-question-circle" EnableViewState="false" aria-hidden="true" data-html="true" />
                        </span>
                        <asp:RequiredFieldValidator Display="dynamic" ID="rfvImportTypeDropDownList" runat="server" ErrorMessage="Please select the import type" ControlToValidate="ImportTypeDropDownList" ForeColor="Red"></asp:RequiredFieldValidator>

                    </div>
                </div>
                <div class="form-group">
                    <div class="editing-form-label-cell">
                        <cms:FormLabel CssClass="control-label" runat="server" EnableViewState="false"
                            DisplayColon="true" Text="Enable setting deadline" AssociatedControlID="canProvideDeadlineCheckbox" />
                    </div>
                    <div class="editing-form-value-cell control-group-inline-forced">
                        <cms:CMSCheckBox ID="canProvideDeadlineCheckbox" runat="server" />
                        <span class="info-icon">
                            <cms:CMSIcon ToolTip="When checked, translation submitter must provide deadline when submitting content for translation." runat="server" CssClass="icon-question-circle" EnableViewState="false" aria-hidden="true" data-html="true" />
                        </span>
                    </div>
                </div>

                  <div class="form-group">
                    <div class="editing-form-label-cell">
                        <cms:FormLabel CssClass="control-label" runat="server" EnableViewState="false"
                            DisplayColon="true" Text="Project name prefix" AssociatedControlID="ProjectNamePrefixTextBox" />
                    </div>
                    <div class="editing-form-value-cell">
                        <cms:CMSTextBox ID="ProjectNamePrefixTextBox" runat="server" />
                        <span class="info-icon">
                            <cms:CMSIcon ToolTip="Prefix string that will be added when creating project in XTM" runat="server" CssClass="icon-question-circle" EnableViewState="false" aria-hidden="true" data-html="true" />
                        </span>
                    </div>
                </div>

            </div>

            <div class="form-group">
                <div class="editing-form-value-cell editing-form-value-cell-offset">
                    <cms:FormSubmitButton runat="server" ID="saveButton" EnableViewState="false"
                        OnClick="SaveButtonClick" Text="Save" />
                </div>
            </div>

        </div>
    </div>

</asp:Content>

