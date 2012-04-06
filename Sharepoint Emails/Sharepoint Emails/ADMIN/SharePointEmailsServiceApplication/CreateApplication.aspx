<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Import Namespace="Microsoft.SharePoint.ApplicationPages" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CreateApplication.aspx.cs" Inherits="SharepointEmails.Layouts.SharepointEmailsServiceApplication.CreateApplication" MasterPageFile="~/_layouts/dialog.master" %>

<%@ Assembly Name="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c"%>
<%@ Register TagPrefix="wssuc" TagName="LinksTable" src="/_controltemplates/LinksTable.ascx" %>
<%@ Register TagPrefix="wssuc" TagName="InputFormSection" src="/_controltemplates/InputFormSection.ascx" %>
<%@ Register TagPrefix="wssuc" TagName="InputFormControl" src="/_controltemplates/InputFormControl.ascx" %>
<%@ Register TagPrefix="wssuc" TagName="LinkSection" src="/_controltemplates/LinkSection.ascx" %>
<%@ Register TagPrefix="wssuc" TagName="ButtonSection" src="/_controltemplates/ButtonSection.ascx" %>
<%@ Register TagPrefix="wssuc" TagName="ActionBar" src="/_controltemplates/ActionBar.ascx" %>
<%@ Register TagPrefix="wssuc" TagName="ToolBar" src="/_controltemplates/ToolBar.ascx" %>
<%@ Register TagPrefix="wssuc" TagName="ToolBarButton" src="/_controltemplates/ToolBarButton.ascx" %>
<%@ Register TagPrefix="wssuc" TagName="Welcome" src="/_controltemplates/Welcome.ascx" %>
<%@ Register TagPrefix="wssuc" TagName="IisWebServiceApplicationPoolSection" src="~/_admin/IisWebServiceApplicationPoolSection.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="PlaceHolderDialogHeaderPageTitle" runat="server">
    <asp:Literal ID="CreateASAppTitle" Text="Create New SharePointEmails Service Application" runat="server" />
</asp:Content>

<asp:Content ID="Content2" contentplaceholderid="PlaceHolderDialogDescription" runat="server">
    <asp:Literal ID="CreateASAppDesc" Text="Specify the name, application pool, and default for this Application." runat="server"/>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderId="PlaceHolderDialogBodyMainSection" runat="server">    
    <TABLE border="0" cellspacing="0" cellpadding="0" width="100%" class="ms-authoringcontrols">
        <wssuc:InputFormSection ID="InputFormSection1"
            Title="<%$Resources:xlsrv, ManagementUI_ServiceAppNameLabel%>"
            runat="server">
            <Template_InputFormControls>
                <wssuc:InputFormControl ID="InputFormControl1" LabelText="" LabelAssociatedControlID="TextBoxAppName" runat="server">
                    <Template_control>
                        <wssawc:InputFormTextBox title="<%$Resources:xlsrv, ManagementUI_ServiceAppNameLabel%>" class="ms-input" ID="TextBoxAppName" Columns="35" Runat="server" MaxLength=256 />
                        <wssawc:InputFormRequiredFieldValidator ID="AppNameValidator" 
                            ControlToValidate="TextBoxAppName" 
                            ErrorMessage="<%$Resources:xlsrv, ManagementUI_RequiredFieldErrorMessage%>"
                            width='300px'
                            Runat="server"/>
                        <wssawc:InputFormCustomValidator ID="UniqueNameValidator" 
                            ControlToValidate="TextBoxAppName"
                            ErrorMessage="<%$Resources:xlsrv, ManagementUI_DuplicateNameErrorMessage%>" 
                            OnServerValidate="ValidateUniqueName" 
                            runat="server" />
                    </Template_control>
                </wssuc:InputFormControl>
            </Template_InputFormControls>
        </wssuc:InputFormSection>

        <wssuc:IisWebServiceApplicationPoolSection id="AppPoolSection" runat="server" />

        <wssuc:InputFormSection ID="InputFormSection2"
            Title="<%$Resources:xlsrv, ManagementUI_DefaultLabel%>"
            Description="<%$Resources:xlsrv, ManagementUI_DefaultDescription%>"
            runat="server">
            <Template_InputFormControls>
                <wssuc:InputFormControl ID="InputFormControl2" LabelText="" LabelAssociatedControlID="CheckBoxDefault" runat="server">
                    <Template_control>
                        <asp:CheckBox Checked="True" ID="CheckBoxDefault" Text="<%$Resources:xlsrv, ManagementUI_DefaultCheckboxDescription%>" Runat="server" />
                    </Template_control>
                </wssuc:InputFormControl>
            </Template_InputFormControls>
        </wssuc:InputFormSection>

        <SharePoint:FormDigest ID="FormDigest1" runat=server/>        
</asp:Content>