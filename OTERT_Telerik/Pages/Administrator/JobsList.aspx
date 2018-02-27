<%@ Page Title="" Language="C#" MasterPageFile="~/Masters/Inside.Master" AutoEventWireup="true" CodeBehind="JobsList.aspx.cs" Inherits="OTERT.Pages.Administrator.JobsList" %>

<asp:Content ID="Content1" ContentPlaceHolderID="PHTitle" runat="server"><% =pageTitle %></asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="PHHead" runat="server">
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
            OnItemDataBound="gridMain_ItemDataBound"
            OnDetailTableDataBind="gridMain_DetailTableDataBind" >
            <MasterTableView DataKeyNames="ID" CommandItemDisplay="Top" InsertItemPageIndexAction="ShowItemOnCurrentPage" NoMasterRecordsText="Δεν υπάρχουν ακόμη εγγραφές" Name="Master">
                <CommandItemSettings AddNewRecordText="Προσθήκη νέας εγγραφής" RefreshText="Ανανέωση" />
                <PagerStyle PageSizeLabelText=" Εγγραφές ανά σελίδα:" PagerTextFormat=" {4} <strong>{5}</strong> εγγραφές σε <strong>{1}</strong> σελίδες " />
                <DetailTables>
                    <telerik:GridTableView DataKeyNames="ID" Width="100%" runat="server" CommandItemDisplay="Top" Name="FormulaDetails" Caption="Τύποι Υπολγισμού Κόστους">
                        <CommandItemSettings AddNewRecordText="Προσθήκη νέας εγγραφής" RefreshText="Ανανέωση" />
                        <Columns>
                            <telerik:GridEditCommandColumn EditText="Επεξεργασία" UniqueName="EditCommandColumn1" HeaderStyle-Width="20px" ItemStyle-HorizontalAlign="Center" />
                            <telerik:GridBoundColumn SortExpression="JobsID" HeaderText="JobsID" DataField="JobsID" UniqueName="JobsID" ReadOnly="true" Visible="false">
                            </telerik:GridBoundColumn>
                            <telerik:GridBoundColumn HeaderText="Α/Α" DataField="ID" UniqueName="ID" ReadOnly="true">
                            </telerik:GridBoundColumn>
                            <telerik:GridBoundColumn HeaderText="Προϋπόθεση" DataField="Condition" UniqueName="Condition" />
                            <telerik:GridBoundColumn HeaderText="Τύπος" DataField="Formula" UniqueName="Formula">
                                <ColumnValidationSettings EnableRequiredFieldValidation="true">
                                    <RequiredFieldValidator ForeColor="Red" ErrorMessage="Το πεδίο είναι υποχρεωτικό!" />
                                </ColumnValidationSettings>
                            </telerik:GridBoundColumn>
                            <telerik:GridButtonColumn UniqueName="btnDelete2" ConfirmText="Να διαγραφεί αυτός ο Τύπος;" ConfirmDialogType="RadWindow" ConfirmTitle="Διαγραφή" ButtonType="FontIconButton" HeaderTooltip="Διαγραφή" CommandName="Delete" HeaderStyle-Width="20px" ItemStyle-HorizontalAlign="Center" />
                        </Columns>
                    </telerik:GridTableView>
                </DetailTables>
                <DetailTables>
                    <telerik:GridTableView DataKeyNames="ID" Width="100%" runat="server" CommandItemDisplay="Top" Name="CancelDetails" Caption="Κόστη Ακυρώσεων">
                        <CommandItemSettings AddNewRecordText="Προσθήκη νέας εγγραφής" RefreshText="Ανανέωση" />
                        <Columns>
                            <telerik:GridEditCommandColumn EditText="Επεξεργασία" UniqueName="EditCommandColumn1" HeaderStyle-Width="20px" ItemStyle-HorizontalAlign="Center" />
                            <telerik:GridBoundColumn SortExpression="JobsID" HeaderText="JobsID" DataField="JobsID" UniqueName="JobsID" ReadOnly="true" Visible="false">
                            </telerik:GridBoundColumn>
                            <telerik:GridBoundColumn HeaderText="Α/Α" DataField="ID" UniqueName="ID" ReadOnly="true">
                            </telerik:GridBoundColumn>
                            <telerik:GridBoundColumn HeaderText="Ποσά Ακύρωσης" DataField="Price" UniqueName="Price">
                                <ColumnValidationSettings EnableRequiredFieldValidation="true">
                                    <RequiredFieldValidator ForeColor="Red" ErrorMessage="Το πεδίο είναι υποχρεωτικό!" />
                                </ColumnValidationSettings>
                            </telerik:GridBoundColumn>
                            <telerik:GridButtonColumn UniqueName="btnDelete3" ConfirmText="Να διαγραφεί αυτό το Κόστος Ακύρωσης;" ConfirmDialogType="RadWindow" ConfirmTitle="Διαγραφή" ButtonType="FontIconButton" HeaderTooltip="Διαγραφή" CommandName="Delete" HeaderStyle-Width="20px" ItemStyle-HorizontalAlign="Center" />
                        </Columns>
                    </telerik:GridTableView>
                </DetailTables>
                <Columns>
                    <telerik:GridEditCommandColumn EditText="Επεξεργασία" HeaderStyle-Width="20px" ItemStyle-HorizontalAlign="Center" />
                    <telerik:GridBoundColumn DataField="ID" HeaderText="Α/Α" ReadOnly="true" ForceExtractValue="Always" ConvertEmptyStringToNull="true" />
                    <telerik:GridBoundColumn DataField="Name" HeaderText="Όνομα" >
                        <ColumnValidationSettings EnableRequiredFieldValidation="true">
                            <RequiredFieldValidator ForeColor="Red" ErrorMessage="Το πεδίο είναι υποχρεωτικό!" />
                        </ColumnValidationSettings>
                    </telerik:GridBoundColumn>
                    <telerik:GridTemplateColumn UniqueName="Sale" DataField="Type" HeaderText="Κατηγορία Έκπτωσης" >
                        <ItemTemplate>
                            <asp:Label Text='<% #Eval("Sale.Name") %>' runat="server" />
                        </ItemTemplate>
                        <EditItemTemplate>
                            <telerik:RadDropDownList runat="server" ID="ddlSale" RenderMode="Lightweight" DropDownHeight="200" AutoPostBack="true" CausesValidation="false" OnSelectedIndexChanged="ddlSale_SelectedIndexChanged" />
                        </EditItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridBoundColumn DataField="MinimumTime" HeaderText="Ελάχιστος χρόνος χρέωσης" />
                    <telerik:GridBoundColumn DataField="InvoiceCode" HeaderText="Κωδικός Έργου (για τιμολόγιο)" />
                    <telerik:GridButtonColumn UniqueName="btnDelete" ConfirmText="Να διαγραφεί αυτή η Κατηγορία Έργου;" ConfirmDialogType="RadWindow" ConfirmTitle="Διαγραφή" ButtonType="FontIconButton" HeaderTooltip="Διαγραφή" CommandName="Delete" HeaderStyle-Width="20px" ItemStyle-HorizontalAlign="Center" />
                </Columns>
                <EditFormSettings>
                    <EditColumn UpdateText="Ενημέρωση" InsertText="Εισαγωγή" CancelText="Ακύρωση" />                          
                </EditFormSettings>
            </MasterTableView>
        </telerik:RadGrid>
        <telerik:RadWindowManager RenderMode="Lightweight" ID="RadWindowManager1" runat="server" />
    </div>
</asp:Content>