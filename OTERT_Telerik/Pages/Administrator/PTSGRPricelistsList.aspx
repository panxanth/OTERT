<%@ Page Title="" Language="C#" MasterPageFile="~/Masters/Inside.Master" Culture="el-GR" AutoEventWireup="true" CodeBehind="PTSGRPricelistsList.aspx.cs" Inherits="OTERT.Pages.Administrator.PTSGRPricelistsList" %>

<asp:Content ID="Content1" ContentPlaceHolderID="PHTitle" runat="server"><% =pageTitle %></asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="PHHead" runat="server">
    <script type="text/javascript">
        function addValue(txtTarget, txtVal) {
            document.getElementById(txtTarget).value += txtVal;
        }
    </script>
</asp:Content>

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
        <telerik:RadGrid ID="gridMain" runat="server" AutoGenerateColumns="false" AllowPaging="true" AllowCustomPaging="true" PageSize="10" Skin="Metro"
            OnNeedDataSource="gridMain_NeedDataSource" 
            OnUpdateCommand="gridMain_UpdateCommand"
            OnItemCreated="gridMain_ItemCreated" 
            OnDeleteCommand="gridMain_DeleteCommand"
            OnInsertCommand="gridMain_InsertCommand" 
            OnItemDataBound="gridMain_ItemDataBound" >
            <GroupingSettings CaseSensitive="false" />
            <MasterTableView DataKeyNames="ID" CommandItemDisplay="Top" InsertItemPageIndexAction="ShowItemOnCurrentPage" NoMasterRecordsText="Δεν υπάρχουν ακόμη εγγραφές" Name="Master">
                <CommandItemSettings AddNewRecordText="Προσθήκη νέας εγγραφής" RefreshText="Ανανέωση" />
                <PagerStyle PageSizeLabelText=" Εγγραφές ανά σελίδα:" PagerTextFormat=" {4} <strong>{5}</strong> εγγραφές σε <strong>{1}</strong> σελίδες " AlwaysVisible="true" />
                <Columns>
                    <telerik:GridEditCommandColumn EditText="Επεξεργασία" HeaderStyle-Width="20px" ItemStyle-HorizontalAlign="Center" />
                    <telerik:GridBoundColumn DataField="ID" HeaderText="Α/Α" ReadOnly="true" ForceExtractValue="Always" ConvertEmptyStringToNull="true" HeaderStyle-Font-Bold="true" />
                    <telerik:GridBoundColumn DataField="Name" HeaderText="Τίτλος" HeaderStyle-Font-Bold="true" >
                        <ColumnValidationSettings EnableRequiredFieldValidation="true">
                            <RequiredFieldValidator ForeColor="Red" ErrorMessage="Το πεδίο είναι υποχρεωτικό!" />
                        </ColumnValidationSettings>
                    </telerik:GridBoundColumn>
                    <telerik:GridNumericColumn DataField="InstallationCost" HeaderText="Κόστος Εγκατάστασης" DataType="System.Decimal" NumericType="Number" DecimalDigits="5" HeaderStyle-Font-Bold="true">
                        <ColumnValidationSettings EnableRequiredFieldValidation="true">
                            <RequiredFieldValidator ForeColor="Red" ErrorMessage="Το πεδίο είναι υποχρεωτικό!" />
                        </ColumnValidationSettings>
                    </telerik:GridNumericColumn>
                    <telerik:GridNumericColumn DataField="ChargesPerMonth" HeaderText="Μηνιαίο Κόστος" DataType="System.Decimal" NumericType="Number" DecimalDigits="5" HeaderStyle-Font-Bold="true">
                        <ColumnValidationSettings EnableRequiredFieldValidation="true">
                            <RequiredFieldValidator ForeColor="Red" ErrorMessage="Το πεδίο είναι υποχρεωτικό!" />
                        </ColumnValidationSettings>
                    </telerik:GridNumericColumn>
                    <telerik:GridNumericColumn DataField="ChargesPerDay" HeaderText="Ημερήσιο Κόστος" DataType="System.Decimal" NumericType="Number" DecimalDigits="5" HeaderStyle-Font-Bold="true">
                        <ColumnValidationSettings EnableRequiredFieldValidation="true">
                            <RequiredFieldValidator ForeColor="Red" ErrorMessage="Το πεδίο είναι υποχρεωτικό!" />
                        </ColumnValidationSettings>
                    </telerik:GridNumericColumn>
                    <telerik:GridNumericColumn DataField="MSNPerMonth" HeaderText="Μηνιαίο Κόστος MSN" DataType="System.Decimal" NumericType="Number" DecimalDigits="5" DefaultInsertValue="0" HeaderStyle-Font-Bold="true" />
                    <telerik:GridNumericColumn DataField="MSNPerDay" HeaderText="Ημερήσιο Κόστος MSN" DataType="System.Decimal" NumericType="Number" DecimalDigits="5" DefaultInsertValue="0" HeaderStyle-Font-Bold="true" />
                    <telerik:GridCheckBoxColumn DataField="IsChargePerMonth" HeaderText="Χρέωση ανά Μήνα" DataType="System.Boolean" HeaderStyle-Font-Bold="true" />
                    <telerik:GridButtonColumn UniqueName="btnDelete" ConfirmText="Να διαγραφεί αυτός ο Τιμοκατάλογος ΠΤΣ;" ConfirmDialogType="RadWindow" ConfirmTitle="Διαγραφή" ButtonType="FontIconButton" HeaderTooltip="Διαγραφή" CommandName="Delete" HeaderStyle-Width="20px" ItemStyle-HorizontalAlign="Center" />
                </Columns>
                <EditFormSettings>
                    <EditColumn UpdateText="Ενημέρωση" InsertText="Εισαγωγή" CancelText="Ακύρωση" />                          
                </EditFormSettings>
            </MasterTableView>
        </telerik:RadGrid>
        <telerik:RadWindowManager RenderMode="Lightweight" ID="RadWindowManager1" runat="server" />
    </div>
</asp:Content>