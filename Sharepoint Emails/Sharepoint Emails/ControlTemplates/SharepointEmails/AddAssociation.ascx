<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %> 
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AddAssociation.ascx.cs" Inherits="SharepointEmails.ControlTemplates.SharepointEmails.AddAssociation" %>
<asp:Label ID="Label1" runat="server" Text="Label">Name:</asp:Label>
<asp:TextBox ID="tb_Name" runat="server"></asp:TextBox>
<asp:Label ID="Label2" runat="server" Text="Label"></asp:Label>
<asp:DropDownList ID="cb_Types" runat="server" AutoPostBack="True" 
    onselectedindexchanged="cb_Types_SelectedIndexChanged">
</asp:DropDownList><br />
<asp:MultiView ID="MultiView1" runat="server" ActiveViewIndex="0">
    <asp:View ID="v_ByType" runat="server">
        <asp:Label ID="Label4" runat="server" Text="Label"></asp:Label>
        <asp:DropDownList ID="cb_ItemTypes" runat="server">
        </asp:DropDownList>
    </asp:View>
    <asp:View ID="v_ById" runat="server">
        <asp:Label ID="Label3" runat="server" Text="Label">Item ID</asp:Label>
        <asp:TextBox ID="tb_ItemId" runat="server"></asp:TextBox>
    </asp:View>   
</asp:MultiView><br/>
<asp:ValidationSummary ID="ValidationSummary1" runat="server" />