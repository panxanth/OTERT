<%@ Page Title="" Language="C#" MasterPageFile="~/Masters/Inside.Master" AutoEventWireup="true" CodeBehind="DistancesList.aspx.cs" Inherits="OTERT.Pages.Administrator.DistancesList" %>

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
        <telerik:RadGrid ID="gridMain" runat="server" AutoGenerateColumns="false" AllowPaging="true" AllowCustomPaging="true" PageSize="10" Skin="Metro" MasterTableView-AllowFilteringByColumn="True"
            OnNeedDataSource="gridMain_NeedDataSource" 
            OnUpdateCommand="gridMain_UpdateCommand"
            OnItemDataBound="gridMain_ItemDataBound"
            OnDeleteCommand="gridMain_DeleteCommand"
            OnInsertCommand="gridMain_InsertCommand">
            <GroupingSettings CaseSensitive="false" />
            <MasterTableView DataKeyNames="ID" CommandItemDisplay="Top" InsertItemPageIndexAction="ShowItemOnCurrentPage" NoMasterRecordsText="Δεν υπάρχουν ακόμη εγγραφές">
                <CommandItemSettings AddNewRecordText="Προσθήκη νέας εγγραφής" RefreshText="Ανανέωση" />
                <PagerStyle PageSizeLabelText=" Εγγραφές ανά σελίδα:" PagerTextFormat=" {4} <strong>{5}</strong> εγγραφές σε <strong>{1}</strong> σελίδες " AlwaysVisible="true" />
                <Columns>
                    <telerik:GridEditCommandColumn EditText="Επεξεργασία" HeaderStyle-Width="50" HeaderStyle-Font-Bold="true" />
                    <telerik:GridBoundColumn DataField="ID" HeaderText="Α/Α" ReadOnly="true" ForceExtractValue="Always" ConvertEmptyStringToNull="true" AllowFiltering="false" HeaderStyle-Width="50" HeaderStyle-Font-Bold="true" />
                    <telerik:GridBoundColumn DataField="Description" HeaderText="Απόσταση Ζεύγους" ReadOnly="true" HeaderStyle-Font-Bold="true" />
                    <telerik:GridTemplateColumn HeaderText="Κύρια Κατηγορία Έργου" UniqueName="JobsMainID" DataField="JobsMainID" AutoPostBackOnFilter="true" CurrentFilterFunction="Contains" ShowFilterIcon="true" HeaderStyle-Font-Bold="true" >
                        <FilterTemplate>
		                    <telerik:RadDropDownList runat="server" ID="ddlJobsMainFilter" RenderMode="Lightweight" Width="440px" AppendDataBoundItems="true" AutoPostBack="true" CausesValidation="false" OnSelectedIndexChanged="ddlJobsMainFilter_SelectedIndexChanged" OnPreRender="ddlJobsMainFilter_PreRender" />
	                    </FilterTemplate>
                        <ItemTemplate>
                            <asp:Label Text='<% #Eval("JobsMain.Name") %>' runat="server" /> 
                        </ItemTemplate>
                        <EditItemTemplate>
                            <telerik:RadDropDownList runat="server" ID="ddlJobsMain" RenderMode="Lightweight" DropDownHeight="200" Width="550" AutoPostBack="true" CausesValidation="false" OnSelectedIndexChanged="ddlJobsMain_SelectedIndexChanged" />
                        </EditItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn DataField="Position1" HeaderText="Σημείο 1" Visible="false">
                        <ItemTemplate>
                            <asp:Label Text='<% #Eval("Position1") %>' runat="server" /> 
                        </ItemTemplate>
                        <EditItemTemplate>
                            <telerik:RadAutoCompleteBox ID="txtPosition1"  runat="server"
                                DataSourceID="SqlDataSource1" 
                                DataTextField="Positions" 
                                DataValueField="Positions" 
                                InputType="Text" AllowCustomEntry="True" Filter="StartsWith" TextSettings-SelectionMode="Single"
                                OnTextChanged="txtPosition1_TextChanged" />
                            <asp:RequiredFieldValidator runat="server" ControlToValidate="txtPosition1" ForeColor="Red" ErrorMessage="Το πεδίο είναι υποχρεωτικό!" />
                        </EditItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn DataField="Position2" HeaderText="Σημείο 2" Visible="false">
                        <ItemTemplate>
                            <asp:Label Text='<% #Eval("Position2") %>' runat="server" /> 
                        </ItemTemplate>
                        <EditItemTemplate>
                            <telerik:RadAutoCompleteBox ID="txtPosition2" runat="server" 
                                DataSourceID="SqlDataSource1" 
                                DataTextField="Positions" 
                                DataValueField="Positions" 
                                InputType="Text" AllowCustomEntry="True" Filter="StartsWith" TextSettings-SelectionMode="Single" 
                                OnTextChanged="txtPosition2_TextChanged" />
                            <asp:RequiredFieldValidator runat="server" ControlToValidate="txtPosition2" ForeColor="Red" ErrorMessage="Το πεδίο είναι υποχρεωτικό!" />
                        </EditItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridNumericColumn DataField="KM" HeaderText="Απόσταση (ΧΜ)" Visible="false" DataType="System.Int32" DecimalDigits="0">
                        <ColumnValidationSettings EnableRequiredFieldValidation="true">
                            <RequiredFieldValidator ForeColor="Red" ErrorMessage=" Το πεδίο είναι υποχρεωτικό!" />
                        </ColumnValidationSettings>
                    </telerik:GridNumericColumn>
                    <telerik:GridButtonColumn ConfirmText="Να διαγραφεί αυτό το Ζεύγος;" ConfirmDialogType="RadWindow" ConfirmTitle="Διαγραφή" ButtonType="FontIconButton" HeaderTooltip="Διαγραφή" CommandName="Delete" HeaderStyle-Width="50" HeaderStyle-Font-Bold="true" />
                </Columns>
                <EditFormSettings>
                    <EditColumn UpdateText="Ενημέρωση" InsertText="Εισαγωγή" CancelText="Ακύρωση" />                          
                </EditFormSettings>
            </MasterTableView>
        </telerik:RadGrid>
        <telerik:RadWindowManager RenderMode="Lightweight" ID="RadWindowManager1" runat="server" />
    </div>
    <asp:SqlDataSource
          id="SqlDataSource1"
          runat="server"
          DataSourceMode="DataReader"
          ConnectionString="<%$ ConnectionStrings:OTERTConnectionString %>"
          SelectCommand="SELECT DISTINCT Positions FROM Distances UNPIVOT (Positions FOR col IN (Position1, Position2)) un ORDER BY Positions">
      </asp:SqlDataSource>
</asp:Content>