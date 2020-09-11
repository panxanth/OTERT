<%@ Page Title="" Language="C#" MasterPageFile="~/Masters/Inside.Master" Culture="el-GR" AutoEventWireup="true" CodeBehind="CountryPricelistsList.aspx.cs" Inherits="OTERT.Pages.Administrator.CountryPricelistsList" %>

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
                    <telerik:GridBoundColumn DataField="ID" HeaderText="Α/Α" ReadOnly="true" ForceExtractValue="Always" ConvertEmptyStringToNull="true" />
                    <telerik:GridTemplateColumn HeaderText="Πάροχος" UniqueName="CustomerID" DataField="CustomerID" AllowFiltering="false">
                        <ItemTemplate>
                            <asp:Label Text='<% #Eval("Customer.NameGR") %>' runat="server" /> 
                        </ItemTemplate>
                        <EditItemTemplate>
                            <telerik:RadDropDownList runat="server" ID="ddlCustomer" RenderMode="Lightweight" DropDownHeight="200" Width="550" AutoPostBack="true" CausesValidation="false" OnSelectedIndexChanged="ddlCustomer_SelectedIndexChanged" />
                        </EditItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn UniqueName="LineTypeID" DataField="LineTypeID" HeaderText="Είδος Γραμμής" >
                        <ItemTemplate>
                            <asp:Label Text='<% #Eval("LineType.Name") %>' runat="server" /> 
                        </ItemTemplate>
                        <EditItemTemplate>
                            <telerik:RadDropDownList runat="server" ID="ddlLineType" RenderMode="Lightweight" DropDownHeight="200" Width="550" AutoPostBack="true" CausesValidation="false" OnSelectedIndexChanged="ddlLineType_SelectedIndexChanged" />
                        </EditItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridNumericColumn DataField="InstallationCost" HeaderText="Κόστος Εγκατάστασης" DataType="System.Decimal" NumericType="Number">
                        <ColumnValidationSettings EnableRequiredFieldValidation="true">
                            <RequiredFieldValidator ForeColor="Red" ErrorMessage="Το πεδίο είναι υποχρεωτικό!" />
                        </ColumnValidationSettings>
                    </telerik:GridNumericColumn>
                    <telerik:GridNumericColumn DataField="MonthlyCharges" HeaderText="Μηνιαίο Κόστος" DataType="System.Decimal" NumericType="Number">
                        <ColumnValidationSettings EnableRequiredFieldValidation="true">
                            <RequiredFieldValidator ForeColor="Red" ErrorMessage="Το πεδίο είναι υποχρεωτικό!" />
                        </ColumnValidationSettings>
                    </telerik:GridNumericColumn>
                    <telerik:GridCheckBoxColumn DataField="PaymentIsForWholeMonth" HeaderText="Χρέωση ανά Μήνα" Visible="false" DataType="System.Boolean" />
                    <telerik:GridNumericColumn DataField="Internet" HeaderText="Κόστος Internet" DataType="System.Decimal" NumericType="Number">
                        <ColumnValidationSettings EnableRequiredFieldValidation="true">
                            <RequiredFieldValidator ForeColor="Red" ErrorMessage="Το πεδίο είναι υποχρεωτικό!" />
                        </ColumnValidationSettings>
                    </telerik:GridNumericColumn>
                    <telerik:GridNumericColumn DataField="MSN" HeaderText="Κόστος MSN" DataType="System.Decimal" NumericType="Number">
                        <ColumnValidationSettings EnableRequiredFieldValidation="true">
                            <RequiredFieldValidator ForeColor="Red" ErrorMessage="Το πεδίο είναι υποχρεωτικό!" />
                        </ColumnValidationSettings>
                    </telerik:GridNumericColumn>
                    <telerik:GridButtonColumn UniqueName="btnDelete" ConfirmText="Να διαγραφεί αυτός ο Τιμοκατάλογος Παρόχου Εξωτερικού;" ConfirmDialogType="RadWindow" ConfirmTitle="Διαγραφή" ButtonType="FontIconButton" HeaderTooltip="Διαγραφή" CommandName="Delete" HeaderStyle-Width="20px" ItemStyle-HorizontalAlign="Center" />
                </Columns>
                <EditFormSettings>
                    <EditColumn UpdateText="Ενημέρωση" InsertText="Εισαγωγή" CancelText="Ακύρωση" />                          
                </EditFormSettings>
            </MasterTableView>
        </telerik:RadGrid>
        <telerik:RadWindowManager RenderMode="Lightweight" ID="RadWindowManager1" runat="server" />
    </div>
</asp:Content>