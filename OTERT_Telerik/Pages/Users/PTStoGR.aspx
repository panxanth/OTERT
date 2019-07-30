<%@ Page Title="" Language="C#" MasterPageFile="~/Masters/Inside.Master" AutoEventWireup="true" CodeBehind="PTStoGR.aspx.cs" Inherits="OTERT.Pages.Administrator.PTStoGR" %>

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
        function RequestStart(sender, args) {
            var re = new RegExp("\.btnPrint$|\.lnkDownload$|\.btnDownArrow$|\.btnUpArrow$", "ig");
            if (args.get_eventTarget().match(re)) { args.set_enableAjax(false); }
        }
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
        <ClientEvents OnRequestStart="RequestStart" />
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
            OnDetailTableDataBind="gridMain_DetailTableDataBind" 
            OnItemCommand="gridMain_ItemCommand" >
            <MasterTableView DataKeyNames="ID" CommandItemDisplay="Top" InsertItemPageIndexAction="ShowItemOnCurrentPage" NoMasterRecordsText="Δεν υπάρχουν ακόμη εγγραφές" Name="Master">
                <CommandItemSettings AddNewRecordText="Προσθήκη νέας εγγραφής" RefreshText="Ανανέωση" />
                <PagerStyle PageSizeLabelText=" Εγγραφές ανά σελίδα:" PagerTextFormat=" {4} <strong>{5}</strong> εγγραφές σε <strong>{1}</strong> σελίδες " AlwaysVisible="true" />
                <DetailTables>
                    <telerik:GridTableView DataKeyNames="ID" Width="100%" runat="server" CommandItemDisplay="Top" Name="TasksDetails" Caption="Παραγγελίες" NoDetailRecordsText="Δεν υπάρχουν Παραγγελίες">
                        <CommandItemSettings AddNewRecordText="Προσθήκη νέας εγγραφής" RefreshText="Ανανέωση" />
                        <Columns>
                            <telerik:GridEditCommandColumn EditText="Επεξεργασία" UniqueName="EditCommandColumn1" HeaderStyle-Width="20px" ItemStyle-HorizontalAlign="Center" />
                            <telerik:GridBoundColumn DataField="ID" HeaderText="Α/Α" ReadOnly="true" ForceExtractValue="Always" ConvertEmptyStringToNull="true" />
                            <telerik:GridBoundColumn DataField="OrderID" HeaderText="Παραγγελία" ReadOnly="true" Visible="false" />
                            <telerik:GridBoundColumn UniqueName="RegNo" DataField="RegNo" HeaderText="Αριθμός Πρωτοκόλλου" >
                                <ColumnValidationSettings EnableRequiredFieldValidation="true" RequiredFieldValidator-ForeColor="Red" RequiredFieldValidator-ErrorMessage="Το πεδίο είναι υποχρεωτικό!" />
                            </telerik:GridBoundColumn>
                            <telerik:GridDateTimeColumn UniqueName="OrderDate" DataField="OrderDate" HeaderText="Ημ/νία Παραγγελίας" DataType="System.DateTime" PickerType="DatePicker" >
                                <ColumnValidationSettings EnableRequiredFieldValidation="true" RequiredFieldValidator-ForeColor="Red" RequiredFieldValidator-ErrorMessage="Το πεδίο είναι υποχρεωτικό!" />
                            </telerik:GridDateTimeColumn>
                            <telerik:GridTemplateColumn DataField="CustomerID" HeaderText="Πελάτης" >
                                <ItemTemplate>
                                    <asp:Label Text='<% #Eval("Customer.NameGR") %>' runat="server" /> 
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <telerik:RadDropDownList runat="server" ID="ddlCustomers" RenderMode="Lightweight" DropDownHeight="200" Width="500px" AutoPostBack="true" CausesValidation="false" OnSelectedIndexChanged="ddlCustomers_SelectedIndexChanged" />
                                </EditItemTemplate>
                            </telerik:GridTemplateColumn>
                            <telerik:GridTemplateColumn HeaderText="Αιτούμενη Θέση" UniqueName="RequestedPositionID" DataField="RequestedPositionID" AllowFiltering="false">
                                <ItemTemplate>
                                    <asp:Label Text='<% #Eval("RequestedPosition.NameGR") %>' runat="server" /> 
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <telerik:RadDropDownList runat="server" ID="ddlRequestedPosition" RenderMode="Lightweight" DropDownHeight="200" Width="500px" AutoPostBack="true" CausesValidation="false" OnSelectedIndexChanged="ddlRequestedPosition_SelectedIndexChanged" />
                                </EditItemTemplate>
                            </telerik:GridTemplateColumn>

                            <telerik:GridBoundColumn UniqueName="TechnicalSupport" DataField="TechnicalSupport" HeaderText="Τεχνική Υποστήριξη" Visible="false" ReadOnly="true" />
                            <telerik:GridBoundColumn UniqueName="TelephoneNumber" DataField="TelephoneNumber" HeaderText="Αριθμός Τηλεφώνου" Visible="false" />
                            <telerik:GridBoundColumn UniqueName="InvoceComments" DataField="InvoceComments" HeaderText="Πρόσωπο Επικοινωνίας" Visible="false" />

                            <telerik:GridTemplateColumn HeaderText="Είδος Γραμμής" UniqueName="LineTypeID" DataField="LineTypeID" AllowFiltering="false">
                                <ItemTemplate>
                                    <asp:Label Text='<% #Eval("LineType.Name") %>' runat="server" /> 
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <telerik:RadDropDownList runat="server" ID="ddlLineType" RenderMode="Lightweight" DropDownHeight="200" Width="500px" AutoPostBack="true" CausesValidation="false" OnSelectedIndexChanged="ddlLineType_SelectedIndexChanged" />
                                </EditItemTemplate>
                            </telerik:GridTemplateColumn>
                            <telerik:GridCheckBoxColumn UniqueName="Internet" DataField="Internet" HeaderText="Internet" Visible="false" DataType="System.Boolean" />
                            <telerik:GridCheckBoxColumn UniqueName="MSN" DataField="MSN" HeaderText="MSN" Visible="false" DataType="System.Boolean" />
                            <telerik:GridBoundColumn HeaderText="Τύπος Έργου" DataField="JobsID" Visible="false" ReadOnly="true" />
                            <telerik:GridBoundColumn HeaderText="Απόσταση" DataField="DistanceID" Visible="false" ReadOnly="true" />
                            <telerik:GridBoundColumn HeaderText="Δορυφόρος" DataField="SateliteID" Visible="false" ReadOnly="true" />
                            <telerik:GridDateTimeColumn UniqueName="DateTimeStartOrder" DataField="DateTimeStartOrder" HeaderText="Προγραμματισμένη Ημ/νία Έναρξης" Visible="false" DataType="System.DateTime" PickerType="DateTimePicker" ReadOnly="true" >
                                <ColumnValidationSettings EnableRequiredFieldValidation="true" RequiredFieldValidator-ForeColor="Red" RequiredFieldValidator-ErrorMessage="Το πεδίο είναι υποχρεωτικό!" />
                            </telerik:GridDateTimeColumn>
                            <telerik:GridDateTimeColumn UniqueName="DateTimeEndOrder" DataField="DateTimeEndOrder" HeaderText="Προγραμματισμένη Ημ/νία Λήξης" Visible="false" DataType="System.DateTime" PickerType="DateTimePicker" ReadOnly="true" >
                                <ColumnValidationSettings EnableRequiredFieldValidation="true" RequiredFieldValidator-ForeColor="Red" RequiredFieldValidator-ErrorMessage="Το πεδίο είναι υποχρεωτικό!" />
                            </telerik:GridDateTimeColumn>
                            <telerik:GridBoundColumn DataField="DateTimeDurationOrder" HeaderText="Προγραμματισμένη Διάρκεια" Visible="false" ReadOnly="true" >
                                <ColumnValidationSettings EnableRequiredFieldValidation="true" RequiredFieldValidator-ForeColor="Red" RequiredFieldValidator-ErrorMessage="Το πεδίο είναι υποχρεωτικό!" />
                            </telerik:GridBoundColumn>
                            <telerik:GridDateTimeColumn DataField="DateTimeStartActual" HeaderText="Ημ/νία Υλοποίησης (Έναρξη)" DataType="System.DateTime" PickerType="DateTimePicker" />
                            <telerik:GridDateTimeColumn DataField="DateTimeEndActual" HeaderText="Ημ/νία Υλοποίησης (Λήξη)" Visible="false" DataType="System.DateTime" PickerType="DateTimePicker" />
                            <telerik:GridBoundColumn DataField="DateTimeDurationActual" HeaderText="Διάρκεια Υλοποίησης" Visible="false" />
                            <telerik:GridBoundColumn DataField="CostCalculated" HeaderText="Προϋπολογιζόμενο Κόστος (€)" Visible="false" ReadOnly="true" />
                            <telerik:GridCheckBoxColumn DataField="InstallationCharges" HeaderText="Κόστος Εγγατάστησης" Visible="false" ReadOnly="true" DataType="System.Boolean" />
                            <telerik:GridCheckBoxColumn DataField="MonthlyCharges" HeaderText="Μηνιαία Τέλη" Visible="false" ReadOnly="true" DataType="System.Boolean" />
                            <telerik:GridBoundColumn DataField="CallCharges" HeaderText="Κόστος Κλήσεων" Visible="false" ReadOnly="true" >
                                <ColumnValidationSettings EnableRequiredFieldValidation="true" RequiredFieldValidator-ForeColor="Red" RequiredFieldValidator-ErrorMessage="Το πεδίο είναι υποχρεωτικό!" />
                            </telerik:GridBoundColumn>

                            
                            
                            <telerik:GridTemplateColumn DataField="AddedCharges" UniqueName="AddedCharges" HeaderText="Επιπρόσθετα Τέλη" Visible="false" >
                                <ItemTemplate>
                                    <asp:Label Text='<% #Eval("AddedCharges") %>' runat="server" /> 
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox ID="txtAddedCharges" Text='<% #Bind("AddedCharges") %>' runat="server" OnTextChanged="txtAddedCharges_TextChanged" AutoPostBack="true" />
                                </EditItemTemplate>
                            </telerik:GridTemplateColumn>
                            <telerik:GridBoundColumn DataField="CostActual" HeaderText="Ποσό Είσπραξης (€)" Visible="false" />
                            <telerik:GridDateTimeColumn DataField="PaymentDateOrder" HeaderText="Ημ/νία Εντολής Τιμολόγησης" Visible="false" DataType="System.DateTime" PickerType="DatePicker" />
                            <telerik:GridDateTimeColumn DataField="PaymentDateCalculated" HeaderText="Προγραμματισμένη Ημ/νία Είσπραξης" Visible="false" DataType="System.DateTime" PickerType="DatePicker" />
                            <telerik:GridDateTimeColumn DataField="PaymentDateActual" HeaderText="Πραγματική Ημ/νία Είσπραξης" Visible="false" DataType="System.DateTime" PickerType="DatePicker" />
                            <telerik:GridCheckBoxColumn DataField="IsForHelpers" HeaderText="Ενημέρωση ΚΕΤ" DataType="System.Boolean" Visible="false" ReadOnly="true" />
                            <telerik:GridCheckBoxColumn DataField="IsLocked" HeaderText="Κλειδωμένο Έργο" Visible="false" DataType="System.Boolean" />
                            <telerik:GridCheckBoxColumn DataField="IsCanceled" HeaderText="Ακυρωμένο Έργο" Visible="false" DataType="System.Boolean" />
                            <telerik:GridBoundColumn DataField="CancelPrice" HeaderText="Όνομα" Visible="false" ReadOnly="true" >
                                <ColumnValidationSettings EnableRequiredFieldValidation="true" RequiredFieldValidator-ForeColor="Red" RequiredFieldValidator-ErrorMessage="Το πεδίο είναι υποχρεωτικό!" />
                            </telerik:GridBoundColumn>
                            <telerik:GridBoundColumn DataField="Comments" HeaderText="Παρατηρήσεις" Visible="false" />
                            
                            <telerik:GridButtonColumn UniqueName="btnDelete2" ConfirmText="Να διαγραφεί αυτή η Παραγγελία;" ConfirmDialogType="RadWindow" ConfirmTitle="Διαγραφή" ButtonType="FontIconButton" HeaderTooltip="Διαγραφή" CommandName="Delete" HeaderStyle-Width="20px" ItemStyle-HorizontalAlign="Center" />
                        </Columns>
                    </telerik:GridTableView>
                </DetailTables>
                <DetailTables>
                    <telerik:GridTableView DataKeyNames="ID" Width="100%" runat="server" CommandItemDisplay="Top" Name="AttachedFiles" Caption="Συνοδευτικά Αρχεία" NoDetailRecordsText="Δεν υπάρχουν Συνοδευτικά Αρχεία">
                        <CommandItemSettings AddNewRecordText="Προσθήκη νέου αρχείου" RefreshText="Ανανέωση" />
                        <Columns>
                            <telerik:GridBoundColumn SortExpression="TaskID" HeaderText="TaskID" DataField="TaskID" UniqueName="TaskID" ReadOnly="true" Visible="false">
                            </telerik:GridBoundColumn>
                            <telerik:GridBoundColumn HeaderText="Όνομα" DataField="FileName" UniqueName="FileName" Visible="false">
                                <ColumnValidationSettings EnableRequiredFieldValidation="true" RequiredFieldValidator-ForeColor="Red" RequiredFieldValidator-ErrorMessage="Το πεδίο είναι υποχρεωτικό!" />
                            </telerik:GridBoundColumn>
                            <telerik:GridTemplateColumn HeaderText="Αρχείο" DataField="FilePath" UniqueName="FilePath">
                                <ItemTemplate>
                                    <asp:HyperLink runat="server" Text='<% #Eval("FileName") %>' NavigateUrl='<% #Eval("FilePath") %>' Target="_blank" />
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <telerik:RadAsyncUpload RenderMode="Lightweight" ID="uplFile" AllowedFileExtensions="doc,docx,xls,xlsx,zip,rar,pdf,msg" runat="server" OnFileUploaded="uplFile_FileUploaded">
                                    </telerik:RadAsyncUpload>
                                </EditItemTemplate>
                            </telerik:GridTemplateColumn>
                            <telerik:GridBoundColumn HeaderText="Ημ/νία Καταχώρησης" DataField="DateStamp" UniqueName="DateStamp" DataType="System.DateTime" ReadOnly="true">
                            </telerik:GridBoundColumn>
                            <telerik:GridButtonColumn UniqueName="btnDeleteFile" ConfirmText="Να διαγραφεί αυτό το Αρχείο;" ConfirmDialogType="RadWindow" ConfirmTitle="Διαγραφή" ButtonType="FontIconButton" HeaderTooltip="Διαγραφή" CommandName="Delete" HeaderStyle-Width="20px" ItemStyle-HorizontalAlign="Center" />
                        </Columns>
                    </telerik:GridTableView>
                </DetailTables>
                <Columns>
                    <telerik:GridEditCommandColumn EditText="Επεξεργασία" HeaderStyle-Width="20px" ItemStyle-HorizontalAlign="Center" />
                    <telerik:GridBoundColumn DataField="ID" HeaderText="Α/Α" ReadOnly="true" ForceExtractValue="Always" ConvertEmptyStringToNull="true" />
                    <telerik:GridBoundColumn UniqueName="RegNo" DataField="RegNo" HeaderText="Αριθμός Πρωτοκόλλου" >
                        <ColumnValidationSettings EnableRequiredFieldValidation="true" RequiredFieldValidator-ForeColor="Red" RequiredFieldValidator-ErrorMessage="&nbsp;&nbsp;&nbsp;Το πεδίο είναι υποχρεωτικό!" />
                    </telerik:GridBoundColumn>
                    <telerik:GridTemplateColumn HeaderText="Χώρα" UniqueName="CountryID" DataField="CountryID" AllowFiltering="false">
                        <ItemTemplate>
                            <asp:Label Text='<% #Eval("Event.Place.Country.NameGR") %>' runat="server" /> 
                        </ItemTemplate>
                        <EditItemTemplate>
                            <telerik:RadDropDownList runat="server" ID="ddlCountry" RenderMode="Lightweight" DropDownHeight="200" Width="550" AutoPostBack="true" CausesValidation="false" OnSelectedIndexChanged="ddlCountry_SelectedIndexChanged" />
                        </EditItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn HeaderText="Πελάτης (Πάροχος)" UniqueName="Customer1ID" DataField="Customer1ID" AllowFiltering="false">
                        <ItemTemplate>
                            <asp:Label Text='<% #Eval("Customer1.NameGR") %>' runat="server" /> 
                        </ItemTemplate>
                        <EditItemTemplate>
                            <telerik:RadDropDownList runat="server" ID="ddlCustomer1" RenderMode="Lightweight" DropDownHeight="200" Width="550" AutoPostBack="true" CausesValidation="false" OnSelectedIndexChanged="ddlCustomer1_SelectedIndexChanged" />
                        </EditItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn UniqueName="EventID" DataField="EventID" HeaderText="Γεγονός" >
                        <ItemTemplate>
                            <asp:Label Text='<% #Eval("Event.NameGR") %>' runat="server" /> 
                        </ItemTemplate>
                        <EditItemTemplate>
                            <telerik:RadDropDownList runat="server" ID="ddlEvent" RenderMode="Lightweight" DropDownHeight="200" Width="550" AutoPostBack="true" CausesValidation="false" OnSelectedIndexChanged="ddlEvent_SelectedIndexChanged" />
                        </EditItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridCheckBoxColumn DataField="IsLocked" HeaderText="Κλειδωμένο Έργο" Visible="false" DataType="System.Boolean" />
                    <telerik:GridTemplateColumn UniqueName="btnPrintColumn" HeaderText="" AllowFiltering="false">
                        <ItemStyle HorizontalAlign="Center"></ItemStyle>
                        <ItemTemplate>
                            <asp:Button ID="btnPrint" runat="server" Text="Εκτύπωση" CommandName="invPrint"></asp:Button>
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridButtonColumn UniqueName="btnDelete" ConfirmText="Να διαγραφεί αυτή η Παραγγελία;" ConfirmDialogType="RadWindow" ConfirmTitle="Διαγραφή" ButtonType="FontIconButton" HeaderTooltip="Διαγραφή" CommandName="Delete" HeaderStyle-Width="20px" ItemStyle-HorizontalAlign="Center" />
                </Columns>
                <EditFormSettings>
                    <EditColumn UpdateText="Ενημέρωση" InsertText="Εισαγωγή" CancelText="Ακύρωση" />                          
                </EditFormSettings>
            </MasterTableView>
        </telerik:RadGrid>
        <telerik:RadWindowManager RenderMode="Lightweight" ID="RadWindowManager1" runat="server" />
    </div>
</asp:Content>