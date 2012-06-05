<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Import Namespace="Microsoft.SharePoint.ApplicationPages" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WebSettings.aspx.cs" Inherits="SharepointEmails.Layouts.SharepointEmails.WebSettings" DynamicMasterPageFile="~masterurl/default.master" %>
<%@ Register TagPrefix="my" TagName="AllAss" Src="~/_controltemplates/SharepointEmails/AllAssociations.ascx" %>
<%@ Register TagPrefix="my" TagName="AddAss" Src="~/_controltemplates/SharepointEmails/AddAssociation.ascx" %>

<asp:Content ID="PageHead" ContentPlaceHolderID="PlaceHolderAdditionalPageHead" runat="server">

</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="PlaceHolderMain" runat="server">
    <asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl="~/_layouts/SharepointEmails/AddAssociation.aspx">Add association</asp:HyperLink>
    <my:AllAss ID="asd" runat="server" ser></my:AllAss><br/>
    <my:AddAss ID="asasdd" runat="server" ser></my:AddAss>
</asp:Content>

<asp:Content ID="PageTitle" ContentPlaceHolderID="PlaceHolderPageTitle" runat="server">
SharePoint Emails Web Settings
</asp:Content>

<asp:Content ID="PageTitleInTitleArea" ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server" >
SharePoint Emails Web Settings
</asp:Content>
