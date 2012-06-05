<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %> 
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SPAssociationControl.ascx.cs" Inherits="SharepointEmails.ControlTemplates.SPAssociationControl" %>
<SharePoint:RenderingTemplate ID="SPAssociationEditTemplate" runat="server">
<Template>
    <asp:UpdatePanel runat="server" ID="updatePanel">
        <ContentTemplate>
     <table style="width: 100%;">
        <tr>
            <td>
               <asp:GridView ID="grd_Asses" runat="server" EnableModelValidation="True" 
                AutoGenerateColumns="False" BackColor="White" BorderColor="#999999" 
                BorderStyle="None" BorderWidth="1px" CellPadding="3" GridLines="Vertical" 
                AutoGenerateSelectButton="True" DataKeyNames="ID" >
                <Columns>
                    <asp:BoundField DataField="ID"/>
                    <asp:BoundField DataField="Name" />
                    <asp:BoundField DataField="Type" />        
                </Columns>
                <FooterStyle BackColor="#CCCCCC" ForeColor="Black" />
                <HeaderStyle BackColor="#000084" Font-Bold="True" ForeColor="White" />
                <PagerStyle BackColor="#999999" ForeColor="Black" HorizontalAlign="Center" />
                <RowStyle BackColor="#EEEEEE" ForeColor="Black" />
                <SelectedRowStyle BackColor="#008A8C" Font-Bold="True" ForeColor="White" />
                <EmptyDataTemplate>No Data Found.</EmptyDataTemplate> 
              </asp:GridView>
            </td>
            <td>
                  <asp:MultiView ID="mv_Main" runat="server" ActiveViewIndex="1">
                      <asp:View ID="v_Displaying" runat="server">
                          
                      </asp:View>
                      <asp:View ID="v_Editing" runat="server">
                         
                      </asp:View>
                 </asp:MultiView>
            </td>
        </tr>    
        <tr>
            <td>
                <asp:Button ID="btn_Delete" runat="server" Text="Delete" />
                <asp:Button ID="btn_Create" runat="server" Text="Create" /> 
            </td>
        </tr>
          <tr>
            <td>
                <asp:Panel ID="p_Create" runat="server" >
                    <asp:Literal ID="Literal1" runat="server">Name : </asp:Literal>
                    <asp:TextBox ID="tb_Name" runat="server"></asp:TextBox>
                    <asp:Literal ID="Literal2" runat="server">Type : </asp:Literal>
                    <asp:DropDownList ID="cb_AssType" runat="server">
                        <asp:ListItem Value="1">By Group</asp:ListItem>
                        <asp:ListItem Value="2">By ID</asp:ListItem>
                    </asp:DropDownList>
                    <asp:MultiView ID="mv_CreateMain" runat="server">
                        <asp:View ID="v_ById" runat="server" >
                        </asp:View>
                        <asp:View ID="v_ByGroup" runat="server">
                        </asp:View>
                    </asp:MultiView>
                    <asp:Button ID="btn_Add" runat="server" Text="Add" />
                </asp:Panel>
            </td>
        </tr>    
    </table>
    </ContentTemplate>
    </asp:UpdatePanel>
    <asp:UpdateProgress runat="server" ID="updateProgress" AssociatedUpdatePanelID="updatePanel">
           <ProgressTemplate>
                Processing...
            </ProgressTemplate>
    </asp:UpdateProgress>
</Template>
</SharePoint:RenderingTemplate>