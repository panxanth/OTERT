<%@ Page Title="" Language="C#" MasterPageFile="~/Masters/Inside.Master" Culture="el-GR" AutoEventWireup="true" CodeBehind="PTStoGR.aspx.cs" Inherits="OTERT.Pages.Administrator.PTStoGR" %>

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
        function UpdateTo(ctlFromID, ctlToID) {
            var ctlFrom = $find(ctlFromID);
            var ctlTo = $find(ctlToID);
            var dateFrom = ctlFrom.get_selectedDate();
            ctlTo.set_selectedDate(dateFrom);
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
        <telerik:RadGrid ID="gridMain" runat="server" AutoGenerateColumns="false" MasterTableView-AllowPaging="true" MasterTableView-AllowCustomPaging="true" MasterTableView-PageSize="10" Skin="Metro"
            AllowFilteringByColumn="True" PagerStyle-AlwaysVisible="true" MasterTableView-AllowSorting="true" MasterTableView-AllowCustomSorting="true" RetainExpandStateOnRebind="true"
            OnNeedDataSource="gridMain_NeedDataSource" 
            OnUpdateCommand="gridMain_UpdateCommand"
            OnItemCreated="gridMain_ItemCreated" 
            OnDeleteCommand="gridMain_DeleteCommand"
            OnInsertCommand="gridMain_InsertCommand" 
            OnItemDataBound="gridMain_ItemDataBound"
            OnDetailTableDataBind="gridMain_DetailTableDataBind"
            OnItemCommand="gridMain_ItemCommand" >
            <MasterTableView DataKeyNames="ID" CommandItemDisplay="Top" InsertItemPageIndexAction="ShowItemOnCurrentPage" AllowFilteringByColumn="True" NoMasterRecordsText="Δεν υπάρχουν ακόμη εγγραφές" Name="Master">
                <CommandItemSettings AddNewRecordText="Προσθήκη νέας εγγραφής" RefreshText="Ανανέωση" />
                <PagerStyle PageSizeLabelText=" Εγγραφές ανά σελίδα:" PagerTextFormat=" {4} <strong>{5}</strong> εγγραφές σε <strong>{1}</strong> σελίδες " AlwaysVisible="true" />
                <DetailTables>
                    <telerik:GridTableView DataKeyNames="ID" Width="100%" runat="server" CommandItemDisplay="Top" AllowFilteringByColumn="False" Name="Tasks2Details" Caption="Πάροχοι" NoDetailRecordsText="Δεν υπάρχουν Πάροχοι">
                    <CommandItemSettings AddNewRecordText="Προσθήκη νέας εγγραφής" RefreshText="Ανανέωση" />
                    <DetailTables>
                        <telerik:GridTableView EnableHierarchyExpandAll="true" DataKeyNames="ID" Width="100%" runat="server" CommandItemDisplay="Top" AllowFilteringByColumn="False" Name="TasksDetails" Caption="Παραγγελίες" NoDetailRecordsText="Δεν υπάρχουν Παραγγελίες">
                            <CommandItemSettings AddNewRecordText="Προσθήκη νέας εγγραφής" RefreshText="Ανανέωση" />
                            <Columns>
                                <telerik:GridEditCommandColumn EditText="Επεξεργασία" UniqueName="EditCommandColumn1" HeaderStyle-Width="20px" ItemStyle-HorizontalAlign="Center" />                   
                                <%--
                                <telerik:GridTemplateColumn UniqueName="btnPrintOrderColumn" HeaderText="" AllowFiltering="false">
                                    <ItemStyle HorizontalAlign="Center"></ItemStyle>
                                    <ItemTemplate>
                                        <asp:ImageButton ID="btnPrintOrder" runat="server" ImageUrl="~/Images/print.png" CommandName="printOrder" ToolTip="Εκτύπωση Παραγγελίας" />
                                    </ItemTemplate>
                                </telerik:GridTemplateColumn>
                                --%>
                                <telerik:GridBoundColumn DataField="ID" HeaderText="Α/Α" ReadOnly="true" ForceExtractValue="Always" ConvertEmptyStringToNull="true" HeaderStyle-Wrap="false" HeaderStyle-Font-Bold="true" />
                                <telerik:GridBoundColumn DataField="OrderPTSGR2ID" HeaderText="Πάροχος" ReadOnly="true" Visible="false" HeaderStyle-Font-Bold="true" />
                                <telerik:GridBoundColumn UniqueName="RegNo" DataField="RegNo" HeaderText="Αριθμός Πρωτοκόλλου" HeaderStyle-Font-Bold="true" >
                                    <ColumnValidationSettings EnableRequiredFieldValidation="true" RequiredFieldValidator-ForeColor="Red" RequiredFieldValidator-ErrorMessage="Το πεδίο είναι υποχρεωτικό!" />
                                </telerik:GridBoundColumn>
                                <telerik:GridBoundColumn UniqueName="InvoiceProtocol" DataField="InvoiceProtocol" HeaderText="Πρωτόκολλο Τιμολόγησης" HeaderStyle-Font-Bold="true" />
                                <telerik:GridDateTimeColumn UniqueName="OrderDate" DataField="OrderDate" HeaderText="Ημ/νία Παραγγελίας" DataType="System.DateTime" PickerType="DatePicker" HeaderStyle-Font-Bold="true" >
                                    <ColumnValidationSettings EnableRequiredFieldValidation="true" RequiredFieldValidator-ForeColor="Red" RequiredFieldValidator-ErrorMessage="Το πεδίο είναι υποχρεωτικό!" />
                                </telerik:GridDateTimeColumn>
                                <telerik:GridTemplateColumn DataField="CustomerID" HeaderText="Πελάτης" HeaderStyle-Font-Bold="true" >
                                    <ItemTemplate>
                                        <asp:Label Text='<% #Eval("Customer.NameGR") %>' runat="server" /> 
                                    </ItemTemplate>
                                    <EditItemTemplate>
                                        <telerik:RadDropDownList runat="server" ID="ddlCustomers" RenderMode="Lightweight" DropDownHeight="200" Width="500px" AutoPostBack="true" CausesValidation="false" OnSelectedIndexChanged="ddlCustomers_SelectedIndexChanged" />
                                    </EditItemTemplate>
                                </telerik:GridTemplateColumn>
                                <telerik:GridTemplateColumn HeaderText="Αιτούμενη Θέση" UniqueName="RequestedPositionID" DataField="RequestedPositionID" AllowFiltering="false" HeaderStyle-Font-Bold="true" >
                                    <ItemTemplate>
                                        <asp:Label Text='<% #Eval("RequestedPosition.NameGR") %>' runat="server" /> 
                                    </ItemTemplate>
                                    <EditItemTemplate>
                                        <telerik:RadDropDownList runat="server" ID="ddlRequestedPosition" RenderMode="Lightweight" DropDownHeight="200" Width="500px" AutoPostBack="true" CausesValidation="false" OnSelectedIndexChanged="ddlRequestedPosition_SelectedIndexChanged" />
                                    </EditItemTemplate>
                                </telerik:GridTemplateColumn>
                                <telerik:GridBoundColumn UniqueName="TelephoneNumber" DataField="TelephoneNumber" HeaderText="Αριθμοδότηση" HeaderStyle-Font-Bold="true" />
                                <telerik:GridBoundColumn UniqueName="CorrespondentName" DataField="CorrespondentName" HeaderText="Ονομ/νυμο Ανταποκριτή" HeaderStyle-Font-Bold="true" />
                                <telerik:GridTemplateColumn HeaderText="Εμπορικό Πρόγραμμα" UniqueName="PTSRPricelistID" DataField="PTSRPricelistID" AllowFiltering="false" HeaderStyle-Font-Bold="true" >
                                    <ItemTemplate>
                                        <asp:Label Text='<% #Eval("PTSRPricelist.Name") %>' runat="server" /> 
                                    </ItemTemplate>
                                    <EditItemTemplate>
                                        <telerik:RadDropDownList runat="server" ID="ddlPTSRPricelist" RenderMode="Lightweight" DropDownHeight="200" Width="500px" AutoPostBack="true" CausesValidation="false" OnSelectedIndexChanged="ddlPTSRPricelist_SelectedIndexChanged" />
                                    </EditItemTemplate>
                                </telerik:GridTemplateColumn>
                                <telerik:GridTemplateColumn HeaderText="MSN" UniqueName="MSNCount" DataField="MSNCount" AllowFiltering="false" HeaderStyle-Font-Bold="true" >
                                    <ItemTemplate>
                                        <asp:Label Text='<% #Eval("MSNCount") %>' runat="server" /> 
                                    </ItemTemplate>
                                    <EditItemTemplate>
                                        <telerik:RadDropDownList runat="server" ID="ddlMSNCount" RenderMode="Lightweight" DropDownHeight="200" Width="500px" AutoPostBack="true" CausesValidation="false" OnSelectedIndexChanged="ddlMSNCount_SelectedIndexChanged" />
                                    </EditItemTemplate>
                                </telerik:GridTemplateColumn>
                                <telerik:GridBoundColumn UniqueName="MSN1" DataField="MSN1" HeaderText="MSN1" HeaderStyle-Font-Bold="true" Visible="false" />
                                <telerik:GridBoundColumn UniqueName="MSN2" DataField="MSN2" HeaderText="MSN2" HeaderStyle-Font-Bold="true" Visible="false" />
                                <telerik:GridDateTimeColumn UniqueName="DateTimeStartActual" DataField="DateTimeStartActual" HeaderText="Ημ/νία Υλοποίησης (Έναρξη)" DataType="System.DateTime" PickerType="DateTimePicker" HeaderStyle-Font-Bold="true"  />
                                <telerik:GridDateTimeColumn UniqueName="DateTimeEndActual" DataField="DateTimeEndActual" HeaderText="Ημ/νία Υλοποίησης (Λήξη)" Visible="false" DataType="System.DateTime" PickerType="DateTimePicker" />
                                <telerik:GridBoundColumn DataField="DateTimeDurationActual" HeaderText="Διάρκεια Υλοποίησης" Visible="false" />
                                <telerik:GridBoundColumn DataField="CostCalculated" HeaderText="Προϋπολογιζόμενο Κόστος (€)" Visible="false" ReadOnly="true" />
                                <telerik:GridTemplateColumn DataField="CallCharges" UniqueName="CallCharges" HeaderText="Χρέωση Κλήσεων (5%)" Visible="false" >
                                    <ItemTemplate>
                                        <asp:Label Text='<% #Eval("CallCharges") %>' runat="server" /> 
                                    </ItemTemplate>
                                    <EditItemTemplate>
                                        <asp:TextBox ID="txtCallCharges" Text='<% #Bind("CallCharges") %>' runat="server" OnTextChanged="txtCallCharges_TextChanged" AutoPostBack="true" />
                                    </EditItemTemplate>
                                </telerik:GridTemplateColumn>
                                <telerik:GridTemplateColumn DataField="AddedCharges" UniqueName="AddedCharges" HeaderText="Λοιπές Χρεώσεις" Visible="false" >
                                    <ItemTemplate>
                                        <asp:Label Text='<% #Eval("AddedCharges") %>' runat="server" /> 
                                    </ItemTemplate>
                                    <EditItemTemplate>
                                        <asp:TextBox ID="txtAddedCharges" Text='<% #Bind("AddedCharges") %>' runat="server" OnTextChanged="txtAddedCharges_TextChanged" AutoPostBack="true" />
                                    </EditItemTemplate>
                                </telerik:GridTemplateColumn>
                                <telerik:GridTemplateColumn DataField="InvoiceCost" UniqueName="InvoiceCost" HeaderText="Κόστος Εγκατάστασης + Πάγεια (€)    " Visible="false" >
                                    <ItemTemplate>
                                        <asp:Label Text='<% #Eval("InvoiceCost") %>' runat="server" /> 
                                    </ItemTemplate>
                                    <EditItemTemplate>
                                        <asp:TextBox ID="txtInvoiceCost" Text='<% #Bind("InvoiceCost") %>' runat="server" OnTextChanged="txtInvoiceCost_TextChanged" AutoPostBack="true" />
                                    </EditItemTemplate>
                                </telerik:GridTemplateColumn>
                                <telerik:GridBoundColumn DataField="CostActual" HeaderText="Ποσό Τιμολογίου (€)" Visible="false" />
                                <telerik:GridDateTimeColumn DataField="PaymentDateActual" HeaderText="Ημ/νία Τιμολόγησης" Visible="false" DataType="System.DateTime" PickerType="DatePicker" />
                                <telerik:GridCheckBoxColumn DataField="IsLocked" HeaderText="Κλειδωμένο Έργο" Visible="false" DataType="System.Boolean" />
                                <telerik:GridTemplateColumn DataField="IsCanceled" UniqueName="chkIsCanceled" HeaderText="Ακυρωμένο Έργο" Visible="false" DataType="System.Boolean" >
                                    <ItemTemplate>
                                        <asp:Label Text='<% #Eval("IsCanceled") %>' runat="server" /> 
                                    </ItemTemplate>
                                    <EditItemTemplate>
                                        <asp:CheckBox ID="chkIsCanceled" runat="server" />&nbsp;&nbsp;&nbsp;
                                        <asp:Label ID="lblCancelationMsg" runat="server" Text="Δεν υπάρχουν Ποσά Ακύρωσης για τη συγκεκριμένη Κατηγορία Έργου. Το συνολικό κόστος θα μηδενιστεί!" ForeColor="Red" Font-Bold="true" Visible="false" />
                                        <telerik:RadDropDownList runat="server" ID="ddlCancelationPrices" RenderMode="Lightweight" AutoPostBack="true" CausesValidation="false" Width="230" Visible="false" OnSelectedIndexChanged="ddlCancelationPrices_SelectedIndexChanged" OnPreRender="ddlCancelationPrices_PreRender" />&nbsp;&nbsp;&nbsp;
                                        <asp:Button ID="btnCancelationOK" runat="server" Text="Συμφωνώ" Visible="false" OnClick="btnCancelationOK_Click" />&nbsp;&nbsp;&nbsp;
                                        <asp:Button ID="btnCancelationCancel" runat="server" Text="Άκυρο" Visible="false" OnClick="btnCancelationCancel_Click" />
                                    </EditItemTemplate>
                                </telerik:GridTemplateColumn>
                                <telerik:GridBoundColumn DataField="Comments" HeaderText="Παρατηρήσεις" Visible="false" />
                                <telerik:GridButtonColumn UniqueName="btnDelete3" ConfirmText="Να διαγραφεί αυτή η Παραγγελία;" ConfirmDialogType="RadWindow" ConfirmTitle="Διαγραφή" ButtonType="FontIconButton" HeaderTooltip="Διαγραφή" CommandName="Delete" HeaderStyle-Width="20px" ItemStyle-HorizontalAlign="Center" />
                            </Columns>
                        </telerik:GridTableView>
                    </DetailTables>
                        <Columns>
                            <telerik:GridEditCommandColumn EditText="Επεξεργασία" UniqueName="EditCommandColumn2" HeaderStyle-Width="20px" ItemStyle-HorizontalAlign="Center" />
                            <telerik:GridBoundColumn DataField="ID" HeaderText="Α/Α" ReadOnly="true" ForceExtractValue="Always" ConvertEmptyStringToNull="true" HeaderStyle-Wrap="false" HeaderStyle-Font-Bold="true" AllowFiltering="false" HeaderStyle-Width="100px" />
                            <telerik:GridBoundColumn UniqueName="RegNo" DataField="RegNo" HeaderText="Αριθμός Πρωτοκόλλου" HeaderStyle-Font-Bold="true" HeaderStyle-Wrap="false" HeaderStyle-Width="100px" >
                                <ColumnValidationSettings EnableRequiredFieldValidation="true" RequiredFieldValidator-ForeColor="Red" RequiredFieldValidator-ErrorMessage="Το πεδίο είναι υποχρεωτικό!" />
                            </telerik:GridBoundColumn>
                            <telerik:GridBoundColumn UniqueName="InvoiceProtocol" DataField="InvoiceProtocol" HeaderText="Πρωτόκολλο Τιμολόγησης" HeaderStyle-Font-Bold="true" HeaderStyle-Wrap="false" HeaderStyle-Width="100px" />
                            <telerik:GridTemplateColumn UniqueName="CountryID" DataField="CountryID" HeaderText="Χώρα" HeaderStyle-Font-Bold="true" AllowFiltering="false" SortExpression="CountryID" AllowSorting="true">
                                <ItemTemplate>
                                    <asp:Label Text='<% #Eval("Country.NameGR") %>' runat="server" /> 
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <telerik:RadDropDownList runat="server" ID="ddlCountry" RenderMode="Lightweight" DropDownHeight="200" Width="550" AutoPostBack="true" CausesValidation="false" OnSelectedIndexChanged="ddlCountry_SelectedIndexChanged" />
                                </EditItemTemplate>
                            </telerik:GridTemplateColumn>
                            <telerik:GridTemplateColumn UniqueName="ProviderID" DataField="ProviderID" HeaderText="Πάροχος" HeaderStyle-Font-Bold="true" AllowFiltering="false" SortExpression="ProviderID" AllowSorting="true">
                                <ItemTemplate>
                                    <asp:Label Text='<% #Eval("Provider.NameGR") %>' runat="server" /> 
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <telerik:RadDropDownList runat="server" ID="ddlProvider" RenderMode="Lightweight" DropDownHeight="200" Width="550" AutoPostBack="true" CausesValidation="false" OnSelectedIndexChanged="ddlProvider_SelectedIndexChanged" />
                                </EditItemTemplate>
                            </telerik:GridTemplateColumn>
                            <telerik:GridButtonColumn UniqueName="btnDelete2" ConfirmText="Να διαγραφεί αυτός ο Πάροχος;" ConfirmDialogType="RadWindow" ConfirmTitle="Διαγραφή" ButtonType="FontIconButton" HeaderTooltip="Διαγραφή" CommandName="Delete" HeaderStyle-Width="20px" ItemStyle-HorizontalAlign="Center" />
                        </Columns>
                    </telerik:GridTableView>
                </DetailTables>
                <DetailTables>
                    <telerik:GridTableView DataKeyNames="ID" Width="100%" runat="server" CommandItemDisplay="Top" AllowFilteringByColumn="False" Name="AttachedFiles" Caption="Συνοδευτικά Αρχεία" NoDetailRecordsText="Δεν υπάρχουν Συνοδευτικά Αρχεία">
                        <CommandItemSettings AddNewRecordText="Προσθήκη νέου αρχείου" RefreshText="Ανανέωση" />
                        <Columns>
                            <telerik:GridBoundColumn SortExpression="TaskID" HeaderText="TaskID" DataField="TaskID" UniqueName="TaskID" ReadOnly="true" Visible="false">
                            </telerik:GridBoundColumn>
                            <telerik:GridBoundColumn HeaderText="Όνομα" DataField="FileName" UniqueName="FileName" Visible="false">
                                <ColumnValidationSettings EnableRequiredFieldValidation="true" RequiredFieldValidator-ForeColor="Red" RequiredFieldValidator-ErrorMessage="Το πεδίο είναι υποχρεωτικό!" />
                            </telerik:GridBoundColumn>
                            <telerik:GridTemplateColumn HeaderText="Αρχείο" DataField="FilePath" UniqueName="FilePath" HeaderStyle-Font-Bold="true" >
                                <ItemTemplate>
                                    <asp:HyperLink runat="server" Text='<% #Eval("FileName") %>' NavigateUrl='<% #Eval("FilePath") %>' Target="_blank" />
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <telerik:RadAsyncUpload RenderMode="Lightweight" ID="uplFile" AllowedFileExtensions="doc,docx,xls,xlsx,zip,rar,pdf,msg" runat="server" OnFileUploaded="uplFile_FileUploaded">
                                    </telerik:RadAsyncUpload>
                                </EditItemTemplate>
                            </telerik:GridTemplateColumn>
                            <telerik:GridBoundColumn HeaderText="Ημ/νία Καταχώρησης" DataField="DateStamp" UniqueName="DateStamp" DataType="System.DateTime" ReadOnly="true" HeaderStyle-Font-Bold="true" >
                            </telerik:GridBoundColumn>
                            <telerik:GridButtonColumn UniqueName="btnDeleteFile" ConfirmText="Να διαγραφεί αυτό το Αρχείο;" ConfirmDialogType="RadWindow" ConfirmTitle="Διαγραφή" ButtonType="FontIconButton" HeaderTooltip="Διαγραφή" CommandName="Delete" HeaderStyle-Width="20px" ItemStyle-HorizontalAlign="Center" />
                        </Columns>
                    </telerik:GridTableView>
                </DetailTables>
                <Columns>
                    <telerik:GridEditCommandColumn EditText="Επεξεργασία" HeaderStyle-Width="20px" ItemStyle-HorizontalAlign="Center" />
                    <telerik:GridTemplateColumn UniqueName="btnPrintColumn" HeaderText="" AllowFiltering="false">
                        <ItemStyle HorizontalAlign="Center"></ItemStyle>
                        <ItemTemplate>
                            <asp:ImageButton ID="btnPrint" runat="server" ImageUrl="~/Images/print.png" CommandName="printCharges" ToolTip="Εκτύπωση Χρεωπιστωτικού" />
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <%--
                    <telerik:GridTemplateColumn UniqueName="btnCopyColumn" HeaderText="" AllowFiltering="false">
                        <ItemStyle HorizontalAlign="Center"></ItemStyle>
                        <ItemTemplate>
                            <telerik:RadImageButton ID="RadImageButton1" runat="server" ToolTip="Αντιγραφή" CommandName="orderCopy" Width="16px" Height="16px">
                                <ConfirmSettings ConfirmText="Να αντιγραφεί αυτή η Παραγγελία;" Title="Αντιγραφή" Width="400" Height="150" />
                                <Image Url="~/Images/copy.png" />
                            </telerik:RadImageButton>
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    --%>
                    <telerik:GridBoundColumn DataField="ID" HeaderText="Α/Α" ReadOnly="true" ForceExtractValue="Always" ConvertEmptyStringToNull="true" HeaderStyle-Wrap="false" HeaderStyle-Font-Bold="true" AllowFiltering="false" />
                    <telerik:GridTemplateColumn UniqueName="PlaceID" DataField="PlaceID" HeaderText="Χώρος Διοργάνωσης" HeaderStyle-Font-Bold="true" AllowFiltering="true" SortExpression="PlaceID" AllowSorting="true">
                        <ItemTemplate>
                            <asp:Label Text='<% #Eval("Event.Place.NameGR") %>' runat="server" /> 
                        </ItemTemplate>
                        <EditItemTemplate>
                            <telerik:RadDropDownList runat="server" ID="ddlPlace" RenderMode="Lightweight" DropDownHeight="200" Width="550" AutoPostBack="true" CausesValidation="false" OnSelectedIndexChanged="ddlPlace_SelectedIndexChanged" />
                        </EditItemTemplate>
                        <FilterTemplate>
		                    <telerik:RadDropDownList runat="server" ID="ddlPlaceFilter" RenderMode="Lightweight" AppendDataBoundItems="true" AutoPostBack="true" CausesValidation="false" Width="280px" DropDownHeight="400px" OnSelectedIndexChanged="ddlPlaceFilter_SelectedIndexChanged" OnPreRender="ddlPlaceFilter_PreRender" />
	                    </FilterTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn UniqueName="EventID" DataField="EventID" HeaderText="Διοργάνωση" HeaderStyle-Font-Bold="true" AllowFiltering="true" SortExpression="EventID" AllowSorting="true">
                        <ItemTemplate>
                            <asp:Label Text='<% #Eval("Event.NameGR") %>' runat="server" /> 
                        </ItemTemplate>
                        <EditItemTemplate>
                            <telerik:RadDropDownList runat="server" ID="ddlEvent" RenderMode="Lightweight" DropDownHeight="200" Width="550" AutoPostBack="true" CausesValidation="false" OnSelectedIndexChanged="ddlEvent_SelectedIndexChanged" />
                        </EditItemTemplate>
                        <FilterTemplate>
		                    <telerik:RadDropDownList runat="server" ID="ddlEventFilter" RenderMode="Lightweight" AppendDataBoundItems="true" AutoPostBack="true" CausesValidation="false" Width="280px" DropDownHeight="400px" OnSelectedIndexChanged="ddlEventFilter_SelectedIndexChanged" OnPreRender="ddlEventFilter_PreRender" />
	                    </FilterTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridDateTimeColumn UniqueName="DateTimeStart" DataField="DateTimeStart" HeaderText="Ημερομηνία" DataType="System.DateTime" DataFormatString="{0:dd/MM/yyyy HH:mm}" PickerType="DateTimePicker" ReadOnly="true" HeaderStyle-Font-Bold="true" EnableRangeFiltering="true" EnableTimeIndependentFiltering="true" FilterControlWidth="170px"  AllowSorting="false" />
                    <telerik:GridButtonColumn UniqueName="btnDelete" ConfirmText="Να διαγραφεί αυτό το Γεγονός;" ConfirmDialogType="RadWindow" ConfirmTitle="Διαγραφή" ConfirmDialogHeight="150" ConfirmDialogWidth="400" ButtonType="FontIconButton" HeaderTooltip="Διαγραφή" CommandName="Delete" HeaderStyle-Width="20px" ItemStyle-HorizontalAlign="Center" />
                </Columns>
                <EditFormSettings>
                    <EditColumn UpdateText="Ενημέρωση" InsertText="Εισαγωγή" CancelText="Ακύρωση" />                          
                </EditFormSettings>
            </MasterTableView>
        </telerik:RadGrid>
        <telerik:RadWindowManager RenderMode="Lightweight" ID="RadWindowManager1" runat="server" />
        <br /><br />&nbsp;Δώστε τον Α/Α της παραγγελίας για εκτύπωση:
        <asp:TextBox ID="txtOrderID" runat="server" Text="" />&nbsp;&nbsp;
        <asp:ImageButton ID="btnPrintOrder" runat="server" ImageUrl="~/Images/print.png" ToolTip="Εκτύπωση Παραγγελίας" onclick="btnPrintOrder_Click" />
    </div>
</asp:Content>