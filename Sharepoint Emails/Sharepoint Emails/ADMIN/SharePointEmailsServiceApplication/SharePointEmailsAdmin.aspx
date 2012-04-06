<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Import Namespace="Microsoft.SharePoint.ApplicationPages" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SharePointEmailsAdmin.aspx.cs" Inherits="SharepointEmails.Layouts.SharepointEmailsServiceApplication.SharePointEmailsAdmin" MasterPageFile="~/_admin/admin.master" %>

<%@ Assembly Name="Microsoft.SharePoint.ApplicationPages.Administration, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c"%>

<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="wssawc" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="wssuc" TagName="LinksTable" src="/_controltemplates/LinksTable.ascx" %>
<%@ Register TagPrefix="wssuc" TagName="InputFormSection" src="/_controltemplates/InputFormSection.ascx" %>
<%@ Register TagPrefix="wssuc" TagName="InputFormControl" src="/_controltemplates/InputFormControl.ascx" %>
<%@ Register TagPrefix="wssuc" TagName="LinkSection" src="/_controltemplates/LinkSection.ascx" %>
<%@ Register TagPrefix="wssuc" TagName="ButtonSection" src="/_controltemplates/ButtonSection.ascx" %>
<%@ Register TagPrefix="wssuc" TagName="ActionBar" src="/_controltemplates/ActionBar.ascx" %>
<%@ Register TagPrefix="wssuc" TagName="ToolBar" src="/_controltemplates/ToolBar.ascx" %>
<%@ Register TagPrefix="wssuc" TagName="ToolBarButton" src="/_controltemplates/ToolBarButton.ascx" %>
<%@ Register TagPrefix="wssuc" TagName="Welcome" src="/_controltemplates/Welcome.ascx" %>

<asp:Content ID="Content1" contentplaceholderid="PlaceHolderPageTitle" runat="server">
    Test Service Admin Page
</asp:content>

<asp:Content ID="Content2" contentplaceholderid="PlaceHolderPageTitleInTitleArea" runat="server">
    Test Service Admin Page
</asp:Content>

<asp:content ID="Content3" contentplaceholderid="PlaceHolderPageDescription" runat="server">
    <asp:Literal ID="m_pathPageDescription" runat="server" />
</asp:content>

<asp:content ID="Content4" contentplaceholderid="PlaceHolderMain" runat="server">
    <table width="100%" class="propertysheet" cellspacing="0" cellpadding="0" border="0"> <tr> <td class="ms-descriptionText"> <asp:Label ID="LabelMessage" Runat="server" EnableViewState="False" class="ms-descriptionText"/> </td> </tr> <tr> <td class="ms-error"><asp:Label ID="LabelErrorMessage" Runat="server" EnableViewState="False" /></td> </tr> <tr> <td class="ms-descriptionText"> <asp:ValidationSummary ID="ValSummary" HeaderText="<%$SPHtmlEncodedResources:spadmin, ValidationSummaryHeaderText%>" DisplayMode="BulletList" ShowSummary="True" runat="server"> </asp:ValidationSummary> </td> </tr> </table>

    <P>
        <span style="font-size:140%"><asp:HyperLink ID="m_linkToSettingsPage" runat="server"/></span><br />
        Test Service Admin Page Description
    </P>
    <P>
        <span style="font-size:140%"><asp:HyperLink ID="m_linkToTrustedLocations" runat="server"/></span><br />
        Trusted Location
    </P>
    <P>
        <span style="font-size:140%"><asp:HyperLink ID="m_linkToTrustedDataProviders" runat="server"/></span><br />
        Trusted Data Provider
    </P>
    <P>
        <span style="font-size:140%"><asp:HyperLink ID="m_linkToTrustedDcls" runat="server"/></span><br />
        Trusted Data Connection Library
    </P>
    <P>
        <span style="font-size:140%"><asp:HyperLink ID="m_linkToUdfs" runat="server"/></span><br />
        User Defined Library
    </P>

</asp:content>
