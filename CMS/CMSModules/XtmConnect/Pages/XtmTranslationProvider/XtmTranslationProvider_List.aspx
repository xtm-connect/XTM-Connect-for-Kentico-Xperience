<%@ Page Language="C#" AutoEventWireup="true" Inherits="XtmTranslationProvider_List" Title="XTM translation providers list"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" CodeBehind="XtmTranslationProvider_List.aspx.cs" %>


<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>
<%@ Register Namespace="CMS.UIControls.UniGridConfig" TagPrefix="ug" Assembly="CMS.UIControls" %>

<%@ Register Src="~/CMSAdminControls/UI/UniSelector/UniSelector.ascx" TagName="UniSelector" TagPrefix="cms" %>

<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <div class="cms-bootstrap">
        <%--<asp:ScriptManager ID="manScript" runat="server" ScriptMode="Release" EnableViewState="false" />--%>

        <div id="filters">
            <cms:UIPlaceHolder runat="server" Id="FilterSection" Visible="true">
                <div class="form-horizontal">
                    <div class="form-group">
                        <div class="editing-form-label-cell">
                            <cms:FormLabel CssClass="control-label" runat="server" EnableViewState="false"
                                DisplayColon="true" Text="Filter" ShowRequiredMark="False" AssociatedControlID="FilterTextBox" />
                        </div>
                        <div class="editing-form-value-cell">
                            <cms:CMSTextBox ID="FilterTextBox" runat="server" />
                        </div>
                    </div>
                    <div class="form-group">
                        <div class="editing-form-value-cell editing-form-value-cell-offset">
                            <asp:Button CssClass="btn btn-actions" ID="ResetFilterButton" runat="server" Text="Reset" OnClick="ResetFilterButtonClick" />
                            <asp:Button CssClass="btn btn-primary" ID="SearchFilterButton" runat="server" Text="Search" OnClick="SearchFilterButtonClick" />
                        </div>
                    </div>
                </div>
            </cms:UIPlaceHolder>
        </div>
    <cms:UniGrid ID="XtmTranslationProvidersGrid" runat="server" OrderBy="XtmTranslationProviderID" FilterLimit="1" ZeroRowsText="No translation providers found.">
        <GridActions>
            <ug:Action Name="edit" Caption="$General.Edit$" FontIconClass="icon-edit" FontIconStyle="allow" CommandArgument="XtmTranslationProviderID" />
            <ug:Action Name="delete" Caption="$General.Delete$" FontIconClass="icon-bin" FontIconStyle="Critical" Confirmation="$General.ConfirmDelete$" CommandArgument="XtmTranslationProviderID" />
        </GridActions>
        <GridColumns>
            <ug:Column Source="XtmTranslationProviderID" Caption="XtmTranslationProviderID" Visible="false" />
            <ug:Column Source="ProviderName" Caption="Name" />
            <ug:Column Source="XTMCustomerName" Caption="Customer" />
            <ug:Column Source="XTMTemplateName" Caption="Template" />
            <ug:Column Source="XTMProjectNamePrefix" Caption="Project prefix" />
            <ug:Column Source="TranslationImportType" Caption="Translation flow" />
        </GridColumns>
    </cms:UniGrid>
    </div>
</asp:Content>

