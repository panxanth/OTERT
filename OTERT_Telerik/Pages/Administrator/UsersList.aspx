<%@ Page Title="" Language="C#" MasterPageFile="~/Masters/Inside.Master" Culture="el-GR" AutoEventWireup="true" CodeBehind="UsersList.aspx.cs" Inherits="OTERT.Pages.Administrator.UsersList" %>

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
        <telerik:RadGrid ID="gridMain" runat="server" AutoGenerateColumns="false" MasterTableView-AllowPaging="true" MasterTableView-AllowCustomPaging="true" MasterTableView-PageSize="10" EnableViewState="true" Skin="Metro"
            AllowFilteringByColumn="true" EnableLinqExpressions="true" PagerStyle-AlwaysVisible="true" MasterTableView-AllowSorting="true" MasterTableView-AllowCustomSorting="true"
            OnNeedDataSource="gridMain_NeedDataSource" 
            OnUpdateCommand="gridMain_UpdateCommand"
            OnItemCreated="gridMain_ItemCreated" 
            OnDeleteCommand="gridMain_DeleteCommand"
            OnInsertCommand="gridMain_InsertCommand"
            OnItemDataBound="gridMain_ItemDataBound" >
            <GroupingSettings CaseSensitive="false" />
            <ExportSettings> 
                <Pdf FontType="Subset" PaperSize="Letter" /> 
                <Excel Format="Html" /> 
                <Csv ColumnDelimiter="Comma" RowDelimiter="NewLine" /> 
            </ExportSettings> 
            <MasterTableView DataKeyNames="ID" CommandItemDisplay="Top" InsertItemPageIndexAction="ShowItemOnCurrentPage" NoMasterRecordsText="Δεν υπάρχουν ακόμη εγγραφές">
                <CommandItemSettings AddNewRecordText="Προσθήκη νέας εγγραφής" RefreshText="Ανανέωση" />
                <PagerStyle PageSizeLabelText=" Εγγραφές ανά σελίδα:" PagerTextFormat=" {4} <strong>{5}</strong> εγγραφές σε <strong>{1}</strong> σελίδες " AlwaysVisible="true" />
                <Columns>
                    <telerik:GridEditCommandColumn EditText="Επεξεργασία" HeaderStyle-Width="50" />
                    <telerik:GridBoundColumn DataField="ID" HeaderText="Α/Α" ReadOnly="true" ForceExtractValue="Always" ConvertEmptyStringToNull="true" AllowFiltering="false" HeaderStyle-Width="80" HeaderStyle-Font-Bold="true"  />
                    <telerik:GridTemplateColumn HeaderText="Κατηγορία Χρήστη" HeaderStyle-Width="180px" UniqueName="UserGroupID" DataField="UserGroupID" AutoPostBackOnFilter="true" CurrentFilterFunction="Contains" ShowFilterIcon="true" HeaderStyle-Font-Bold="true" >
                        <FilterTemplate>
		                    <telerik:RadDropDownList runat="server" ID="ddlUserGroupsFilter" RenderMode="Lightweight" AppendDataBoundItems="true" AutoPostBack="true" CausesValidation="false" OnSelectedIndexChanged="ddlUserGroupsFilter_SelectedIndexChanged" OnPreRender="ddlUserGroupsFilter_PreRender" />
	                    </FilterTemplate>
                        <ItemTemplate>
                            <asp:Label Text='<% #Eval("UserGroup.Name") %>' runat="server" /> 
                        </ItemTemplate>
                        <EditItemTemplate>
                            <telerik:RadDropDownList runat="server" ID="ddlUserGroups" RenderMode="Lightweight" DropDownHeight="200" AutoPostBack="true" CausesValidation="false" OnSelectedIndexChanged="ddlUserGroups_SelectedIndexChanged" />
                        </EditItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridBoundColumn DataField="NameGR" HeaderText="Όνομα (GR)" HeaderStyle-Font-Bold="true" >
                        <ColumnValidationSettings EnableRequiredFieldValidation="true">
                            <RequiredFieldValidator ForeColor="Red" ErrorMessage="Το πεδίο είναι υποχρεωτικό!" />
                        </ColumnValidationSettings>
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn DataField="NameEN" HeaderText="Όνομα (EN)" Visible="true" HeaderStyle-Font-Bold="true"  />
                    <telerik:GridBoundColumn DataField="Telephone" HeaderText="Τηλέφωνο" Visible="false" HeaderStyle-Font-Bold="true"  />
                    <telerik:GridBoundColumn DataField="FAX" HeaderText="FAX" Visible="false" HeaderStyle-Font-Bold="true"  />
                    <telerik:GridBoundColumn DataField="Email" HeaderText="Email" Visible="false" HeaderStyle-Font-Bold="true"  />
                    <telerik:GridBoundColumn DataField="UserName" HeaderText="UserName" Visible="false" HeaderStyle-Font-Bold="true" >
                        <ColumnValidationSettings EnableRequiredFieldValidation="true">
                            <RequiredFieldValidator ForeColor="Red" ErrorMessage="Το πεδίο είναι υποχρεωτικό!" />
                        </ColumnValidationSettings>
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn DataField="Password" HeaderText="Password" Visible="false" HeaderStyle-Font-Bold="true" >
                        <ColumnValidationSettings EnableRequiredFieldValidation="true">
                            <RequiredFieldValidator ForeColor="Red" ErrorMessage="Το πεδίο είναι υποχρεωτικό!" />
                        </ColumnValidationSettings>
                    </telerik:GridBoundColumn>
                    <telerik:GridButtonColumn ConfirmText="Να διαγραφεί αυτή η εγγραφή;" ConfirmDialogType="RadWindow" ConfirmTitle="Διαγραφή" ButtonType="FontIconButton" HeaderTooltip="Διαγραφή" CommandName="Delete" HeaderStyle-Width="50" />
                </Columns>
                <NestedViewTemplate>
	                <div class="contactWrap">
		                <fieldset style="padding: 10px;">
			                <legend style="padding: 5px;"><b>Συνολική εικόνα χρήστη: <%#Eval("NameGR") %></b></legend>
			                <table>
				                <tr>
					                <td>Email: </td>
					                <td><asp:Label ID="addressLabel" Text='<%#Bind("Email") %>' runat="server" /></td>
				                </tr>
			                </table>
		                </fieldset>
	                </div>
                </NestedViewTemplate>
                <EditFormSettings>
                    <EditColumn UpdateText="Ενημέρωση" InsertText="Εισαγωγή" CancelText="Ακύρωση" />                          
                </EditFormSettings>
            </MasterTableView>
        </telerik:RadGrid>
        <telerik:RadWindowManager RenderMode="Lightweight" ID="RadWindowManager1" runat="server" />
    </div>
</asp:Content>