<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Import Namespace="Microsoft.SharePoint.ApplicationPages" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="aaa" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WebSettings.aspx.cs" Inherits="SharepointEmails.Layouts.SharepointEmails.WebSettings" DynamicMasterPageFile="~masterurl/default.master" %>

<asp:Content ID="PageHead" ContentPlaceHolderID="PlaceHolderAdditionalPageHead" runat="server">

</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="PlaceHolderMain" runat="server">
    <asp:Label ID="lbl_Message" runat="server" EnableViewState="false" />
    <asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl="/Lists/HiddenTemplatesList">Alert templates</asp:HyperLink><br/>
    <asp:HyperLink ID="HyperLink2" runat="server" NavigateUrl="/HiddenXsltTemplates">Xslt templates library</asp:HyperLink>
    Settings:
    <asp:Table ID="Table1" runat="server">
        <asp:TableHeaderRow></asp:TableHeaderRow>
        <asp:TableRow>
            <asp:TableCell>Disable : </asp:TableCell>
            <asp:TableCell>
                <asp:CheckBox ID="cb_Disabled" runat="server" />
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow>
            <asp:TableCell>Disable also in child webs : </asp:TableCell>
            <asp:TableCell>
                <asp:CheckBox ID="cb_IncludeChilds" runat="server" />
            </asp:TableCell>
        </asp:TableRow>
    </asp:Table>
     <asp:Button ID="btn_Save" runat="server" Text="Save"/>
    <asp:Button ID="btn_Exit" runat="server" Text="Exit" /><br/>
  
</asp:Content>

<asp:Content ID="PageTitle" ContentPlaceHolderID="PlaceHolderPageTitle" runat="server">
SharePointEmails web settings
</asp:Content>

<asp:Content ID="PageTitleInTitleArea" ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server" >

</asp:Content>
