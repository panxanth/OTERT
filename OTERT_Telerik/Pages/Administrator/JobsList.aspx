<%@ Page Title="" Language="C#" MasterPageFile="~/Masters/Inside.Master" Culture="el-GR" AutoEventWireup="true" CodeBehind="JobsList.aspx.cs" Inherits="OTERT.Pages.Administrator.JobsList" %>

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
        <telerik:RadGrid ID="gridMain" runat="server" AutoGenerateColumns="false" AllowPaging="true" AllowCustomPaging="true" PageSize="10" Skin="Metro" AllowFilteringByColumn="True"  EnableViewState="true"
            OnNeedDataSource="gridMain_NeedDataSource" 
            OnUpdateCommand="gridMain_UpdateCommand"
            OnItemCreated="gridMain_ItemCreated" 
            OnDeleteCommand="gridMain_DeleteCommand"
            OnInsertCommand="gridMain_InsertCommand" 
            OnItemDataBound="gridMain_ItemDataBound"
            OnDetailTableDataBind="gridMain_DetailTableDataBind" >
            <MasterTableView DataKeyNames="ID" CommandItemDisplay="Top" InsertItemPageIndexAction="ShowItemOnCurrentPage" AllowFilteringByColumn="True" NoMasterRecordsText="Δεν υπάρχουν ακόμη εγγραφές" Name="Master">
                <CommandItemSettings AddNewRecordText="Προσθήκη νέας εγγραφής" RefreshText="Ανανέωση" />
                <PagerStyle PageSizeLabelText=" Εγγραφές ανά σελίδα:" PagerTextFormat=" {4} <strong>{5}</strong> εγγραφές σε <strong>{1}</strong> σελίδες " AlwaysVisible="true" />
                <DetailTables>
                    <telerik:GridTableView DataKeyNames="ID" Width="100%" runat="server" CommandItemDisplay="Top" Name="FormulaDetails" Caption="Τύποι Υπολγισμού Κόστους">
                        <CommandItemSettings AddNewRecordText="Προσθήκη νέας εγγραφής" RefreshText="Ανανέωση" />
                        <Columns>
                            <telerik:GridEditCommandColumn EditText="Επεξεργασία" UniqueName="EditCommandColumn1" HeaderStyle-Width="20px" ItemStyle-HorizontalAlign="Center" />
                            <telerik:GridBoundColumn SortExpression="JobsID" HeaderText="JobsID" DataField="JobsID" UniqueName="JobsID" ReadOnly="true" Visible="false" />
                            <telerik:GridBoundColumn HeaderText="Α/Α" DataField="ID" UniqueName="ID" ReadOnly="true" />
                            <telerik:GridBoundColumn HeaderText="Προϋπόθεση" DataField="Condition" UniqueName="Condition" />
                            <telerik:GridBoundColumn HeaderText="Τύπος" DataField="Formula" UniqueName="Formula">
                                <ColumnValidationSettings EnableRequiredFieldValidation="true">
                                    <RequiredFieldValidator ForeColor="Red" ErrorMessage="Το πεδίο είναι υποχρεωτικό!" />
                                </ColumnValidationSettings>
                            </telerik:GridBoundColumn>
                            <telerik:GridButtonColumn UniqueName="btnDelete2" ConfirmText="Να διαγραφεί αυτός ο Τύπος;" ConfirmDialogType="RadWindow" ConfirmTitle="Διαγραφή" ButtonType="FontIconButton" HeaderTooltip="Διαγραφή" CommandName="Delete" HeaderStyle-Width="20px" ItemStyle-HorizontalAlign="Center" />
                        </Columns>
                        <EditFormSettings EditFormType="Template" >
	                        <FormTemplate>
		                        <table id="Table2" cellspacing="2" cellpadding="1" width="100%" border="0" rules="none" style="border-collapse: collapse;">
			                        <tr>
				                        <td>
					                        <table id="Table3" width="450px" border="0" class="module">
						                        <tr>
							                        <td>Προϋπόθεση:</td>
							                        <td><asp:TextBox ID="txtCondition" ClientIDMode="Static" runat="server" Text='<%# Bind("Condition") %>' /></td>
						                        </tr>
						                        <tr>
							                        <td>Τύπος:</td>
							                        <td><asp:TextBox ID="txtFormula" ClientIDMode="Static" runat="server" Text='<%# Bind("Formula") %>' TabIndex="1" /></td>
						                        </tr>
					                        </table>
				                        </td>
				                        <td style="vertical-align: top">
					                        <table id="Table1" cellspacing="1" cellpadding="1" border="0" class="module">
						                        <tr>
							                        <td></td>
						                        </tr>
						                        <tr>
							                        <td>
                                                        <button type="button" title="Click Me!2" onclick="addValue('txtCondition', '#BANDWIDTH#');">BANDWIDTH</button>&nbsp;&nbsp;
                                                        <button type="button" title="Click Me!2" onclick="addValue('txtCondition', '#DISTANCE#');">ΑΠΟΣΤΑΣΗ (ΧΛΜ)</button>&nbsp;&nbsp;
                                                        <button type="button" title="Click Me!2" onclick="addValue('txtCondition', '#TIME#');">ΧΡΟΝΟΣ</button>
							                        </td>
						                        </tr>
						                        <tr>
							                        <td></td>
						                        </tr>
						                        <tr>
							                        <td>
                                                        <button type="button" title="Click Me!2" onclick="addValue('txtFormula', '#BANDWIDTH#');">BANDWIDTH</button>&nbsp;&nbsp;
                                                        <button type="button" title="Click Me!2" onclick="addValue('txtFormula', '#DISTANCE#');">ΑΠΟΣΤΑΣΗ (ΧΛΜ)</button>&nbsp;&nbsp;
                                                        <button type="button" title="Click Me!2" onclick="addValue('txtFormula', '#TIME#');">ΧΡΟΝΟΣ</button>
							                        </td>
						                        </tr>
					                        </table>
				                        </td>
			                        </tr>
			                        <tr>
				                        <td colspan="2"></td>
			                        </tr>
			                        <tr>
				                        <td></td>
				                        <td></td>
			                        </tr>
			                        <tr>
				                        <td align="right" colspan="2">
					                        <asp:Button ID="btnUpdate" Text='<%# (Container is GridEditFormInsertItem) ? "Εισαγωγή" : "Ενημέρωση" %>' runat="server" CommandName='<%# (Container is GridEditFormInsertItem) ? "PerformInsert" : "Update" %>' />&nbsp;
					                        <asp:Button ID="btnCancel" Text="Ακύρωση" runat="server" CausesValidation="False" CommandName="Cancel" />
				                        </td>
			                        </tr>
		                        </table>
	                        </FormTemplate>
                        </EditFormSettings>
                    </telerik:GridTableView>
                </DetailTables>
                <DetailTables>
                    <telerik:GridTableView DataKeyNames="ID" Width="100%" runat="server" CommandItemDisplay="Top" Name="CancelDetails" Caption="Κόστη Ακυρώσεων">
                        <CommandItemSettings AddNewRecordText="Προσθήκη νέας εγγραφής" RefreshText="Ανανέωση" />
                        <Columns>
                            <telerik:GridEditCommandColumn EditText="Επεξεργασία" UniqueName="EditCommandColumn1" HeaderStyle-Width="20px" ItemStyle-HorizontalAlign="Center" />
                            <telerik:GridBoundColumn SortExpression="JobsID" HeaderText="JobsID" DataField="JobsID" UniqueName="JobsID" ReadOnly="true" Visible="false" />
                            <telerik:GridBoundColumn HeaderText="Α/Α" DataField="ID" UniqueName="ID" ReadOnly="true" />
                            <telerik:GridBoundColumn HeaderText="Ονομασία" DataField="Name" UniqueName="Name">
                                <ColumnValidationSettings EnableRequiredFieldValidation="true">
                                    <RequiredFieldValidator ForeColor="Red" ErrorMessage="Το πεδίο είναι υποχρεωτικό!" />
                                </ColumnValidationSettings>
                            </telerik:GridBoundColumn>
                            <telerik:GridBoundColumn HeaderText="Τύπος" DataField="Price" UniqueName="Price">
                                <ColumnValidationSettings EnableRequiredFieldValidation="true">
                                    <RequiredFieldValidator ForeColor="Red" ErrorMessage="Το πεδίο είναι υποχρεωτικό!" />
                                </ColumnValidationSettings>
                            </telerik:GridBoundColumn>
                            <telerik:GridButtonColumn UniqueName="btnDelete3" ConfirmText="Να διαγραφεί αυτό το Κόστος Ακύρωσης;" ConfirmDialogType="RadWindow" ConfirmTitle="Διαγραφή" ButtonType="FontIconButton" HeaderTooltip="Διαγραφή" CommandName="Delete" HeaderStyle-Width="20px" ItemStyle-HorizontalAlign="Center" />
                        </Columns>
                        <EditFormSettings EditFormType="Template" >
	                        <FormTemplate>
		                        <table id="Table2" cellspacing="2" cellpadding="1" width="100%" border="0" rules="none" style="border-collapse: collapse;">
			                        <tr>
				                        <td>
					                        <table id="Table3" width="450px" border="0" class="module">
						                        <tr>
							                        <td>Ονομασία:</td>
							                        <td><asp:TextBox ID="txtName" ClientIDMode="Static" runat="server" Text='<%# Bind("Name") %>' /></td>
						                        </tr>
						                        <tr>
							                        <td>Τύπος:</td>
							                        <td><asp:TextBox ID="txtPrice" ClientIDMode="Static" runat="server" Text='<%# Bind("Price") %>' TabIndex="1" /></td>
						                        </tr>
					                        </table>
				                        </td>
				                        <td style="vertical-align: top">
					                        <table id="Table1" cellspacing="1" cellpadding="1" border="0" class="module">
						                        <tr>
							                        <td></td>
						                        </tr>
						                        <tr>
							                        <td style="height: 30px;"></td>
						                        </tr>
						                        <tr>
							                        <td></td>
						                        </tr>
						                        <tr>
							                        <td>
                                                        <button type="button" title="Click Me!2" onclick="addValue('txtPrice', '#CALCPRICE#');">ΠΡΟΥΠ. ΚΟΣΤΟΣ (€)</button>&nbsp;&nbsp;
                                                        <button type="button" title="Click Me!2" onclick="addValue('txtPrice', '#EUTELSAT#');">EUTELSAT</button>
							                        </td>
						                        </tr>
					                        </table>
				                        </td>
			                        </tr>
			                        <tr>
				                        <td colspan="2"></td>
			                        </tr>
			                        <tr>
				                        <td></td>
				                        <td></td>
			                        </tr>
			                        <tr>
				                        <td align="right" colspan="2">
					                        <asp:Button ID="btnUpdate" Text='<%# (Container is GridEditFormInsertItem) ? "Εισαγωγή" : "Ενημέρωση" %>' runat="server" CommandName='<%# (Container is GridEditFormInsertItem) ? "PerformInsert" : "Update" %>' />&nbsp;
					                        <asp:Button ID="btnCancel" Text="Ακύρωση" runat="server" CausesValidation="False" CommandName="Cancel" />
				                        </td>
			                        </tr>
		                        </table>
	                        </FormTemplate>
                        </EditFormSettings>
                    </telerik:GridTableView>
                </DetailTables>
                <Columns>
                    <telerik:GridEditCommandColumn EditText="Επεξεργασία" HeaderStyle-Width="20px" ItemStyle-HorizontalAlign="Center" />
                    <telerik:GridBoundColumn DataField="ID" HeaderText="Α/Α" ReadOnly="true" ForceExtractValue="Always" ConvertEmptyStringToNull="true" AllowFiltering="false" />
                    <telerik:GridBoundColumn DataField="Name" HeaderText="Όνομα" AllowFiltering="false" >
                        <ColumnValidationSettings EnableRequiredFieldValidation="true">
                            <RequiredFieldValidator ForeColor="Red" ErrorMessage="Το πεδίο είναι υποχρεωτικό!" />
                        </ColumnValidationSettings>
                    </telerik:GridBoundColumn>
                    <telerik:GridTemplateColumn HeaderText="Κύρια Κατηγορία Έργου" UniqueName="JobsMainID" DataField="JobsMainID">
                        <ItemTemplate>
                            <asp:Label Text='<% #Eval("JobsMain.Name") %>' runat="server" /> 
                        </ItemTemplate>
                        <EditItemTemplate>
                            <telerik:RadDropDownList runat="server" ID="ddlJobsMain" RenderMode="Lightweight" DropDownHeight="200" Width="550" AutoPostBack="true" CausesValidation="false" OnSelectedIndexChanged="ddlJobsMain_SelectedIndexChanged" />
                        </EditItemTemplate>
                        <FilterTemplate>
		                    <telerik:RadDropDownList runat="server" ID="ddlJobsMainFilter" RenderMode="Lightweight" AppendDataBoundItems="true" AutoPostBack="true" CausesValidation="false" Width="470px" DropDownHeight="400px" OnSelectedIndexChanged="ddlJobsMainFilter_SelectedIndexChanged" OnPreRender="ddlJobsMainFilter_PreRender" />
	                    </FilterTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn HeaderText="Τύπος Έργου" UniqueName="JobTypesID" DataField="JobTypesID" AllowFiltering="false">
                        <ItemTemplate>
                            <asp:Label Text='<% #Eval("JobType.Name") %>' runat="server" /> 
                        </ItemTemplate>
                        <EditItemTemplate>
                            <telerik:RadDropDownList runat="server" ID="ddlJobTypes" RenderMode="Lightweight" Width="550" AutoPostBack="true" CausesValidation="false" OnSelectedIndexChanged="ddlJobTypes_SelectedIndexChanged" />
                        </EditItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn UniqueName="Sale" DataField="SaleID" HeaderText="Κατηγορία Έκπτωσης" AllowFiltering="false" >
                        <ItemTemplate>
                            <asp:Label ID="lblSale" runat="server" /> 
                        </ItemTemplate>
                        <EditItemTemplate>
                            <telerik:RadDropDownList runat="server" ID="ddlSale" RenderMode="Lightweight" DropDownHeight="200" Width="550" AutoPostBack="true" CausesValidation="false" OnSelectedIndexChanged="ddlSale_SelectedIndexChanged" />
                        </EditItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridBoundColumn DataField="MinimumTime" HeaderText="Ελάχιστος χρόνος χρέωσης" AllowFiltering="false" />
                    <telerik:GridBoundColumn DataField="InvoiceCode" HeaderText="Κωδικός Έργου (για τιμολόγιο)" AllowFiltering="false" />
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