<%@ Page Title="" Language="C#" MasterPageFile="~/Masters/Inside.Master" AutoEventWireup="true" CodeBehind="InvoiceCreate.aspx.cs" Inherits="OTERT.Pages.Invoices.InvoiceCreate" %>

<asp:Content ID="Content1" ContentPlaceHolderID="PHTitle" runat="server"><% =pageTitle %></asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="PHHead" runat="server"></asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="PHContent" runat="server">
    <telerik:RadScriptManager ID="RadScriptManager1" runat="server">
        <Scripts>
            <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.Core.js" />
            <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.jQuery.js" />
            <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.jQueryInclude.js" />
        </Scripts>
    </telerik:RadScriptManager>
    <script type="text/javascript">
        //Put your JavaScript code here.
    </script>
    <telerik:RadAjaxManager ID="RadAjaxManager1" runat="server">
        <AjaxSettings>
            <telerik:AjaxSetting AjaxControlID="gridMain">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="gridMain" LoadingPanelID="RadAjaxLoadingPanel1" />
                    <telerik:AjaxUpdatedControl ControlID="RadWindowManager1" />
                </UpdatedControls>
            </telerik:AjaxSetting>
        </AjaxSettings>
    </telerik:RadAjaxManager>
    <telerik:RadAjaxLoadingPanel ID="RadAjaxLoadingPanel1" runat="server" Height="75px" Width="75px" Transparency="25" InitialDelayTime="500" />
    <div>
        <telerik:RadDatePicker RenderMode="Lightweight" ID="dpDate" Width="50%" runat="server" DateInput-Label="Επιλέξτε ημερομηνία: " Culture="el-GR">
        </telerik:RadDatePicker>
        <br /><br />
        <telerik:RadGrid ID="gridJobs" runat="server" AutoGenerateColumns="false" Skin="Metro" Width="50%"
            OnNeedDataSource="gridJobs_NeedDataSource">
            <MasterTableView DataKeyNames="ID" CommandItemDisplay="Top" CommandItemStyle-HorizontalAlign="Right" InsertItemPageIndexAction="ShowItemOnCurrentPage" NoMasterRecordsText="Δεν υπάρχουν Μεταδόσεις για τη συγκεκριμένη ημέρα">
                <CommandItemTemplate>
                    <asp:Button ID="btnSelectAll" Text="Επιλογή Όλων" runat="server" OnClick="btnSelectAll_Click" />
                    <asp:Button ID="btnDeSelectAll" Text="Καθαρισμός" runat="server" OnClick="btnDeSelectAll_Click" />
                </CommandItemTemplate>   
                <CommandItemSettings RefreshText="Ανανέωση" ShowAddNewRecordButton="false" />
                <Columns>
                    <telerik:GridTemplateColumn>
                        <ItemTemplate>
                            <asp:CheckBox ID="chk" runat="server" />
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridBoundColumn DataField="ID" HeaderText="ID" ReadOnly="true" ForceExtractValue="Always" ConvertEmptyStringToNull="true" />
                    <telerik:GridBoundColumn DataField="Name" HeaderText="Τίτλος" ReadOnly="true" />
                </Columns>
            </MasterTableView>
        </telerik:RadGrid>
        <br /><br />
        <asp:Button ID="btnShow" runat="server" Text="Προβολή" OnClick="btnShow_Click" />
        <telerik:RadWindowManager RenderMode="Lightweight" ID="RadWindowManager1" runat="server" />
    </div>
</asp:Content>