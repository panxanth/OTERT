<%@ Page Title="" Language="C#" MasterPageFile="~/Masters/Inside.Master" AutoEventWireup="true" CodeBehind="InvoiceMail.aspx.cs" Inherits="OTERT.Pages.PrintTemplates.InvoiceMail" %>

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
        <telerik:RadGrid ID="gridMain" runat="server" AutoGenerateColumns="false" Skin="Metro" 
            OnNeedDataSource="gridMain_NeedDataSource" 
            OnUpdateCommand="gridMain_UpdateCommand"
            OnItemCreated="gridMain_ItemCreated"
            OnPreRender="gridMain_PreRender">
            <MasterTableView DataKeyNames="ID" CommandItemDisplay="Top" InsertItemPageIndexAction="ShowItemOnCurrentPage" NoMasterRecordsText="Δεν υπάρχουν ακόμη εγγραφές">
                <CommandItemSettings RefreshText="Ανανέωση" ShowAddNewRecordButton="false" />
                <Columns>
                    <telerik:GridEditCommandColumn EditText="Επεξεργασία" HeaderStyle-Width="50" HeaderStyle-Font-Bold="true" />
                    <telerik:GridBoundColumn DataField="ID" HeaderText="Α/Α" ReadOnly="true" ForceExtractValue="Always" ConvertEmptyStringToNull="true" HeaderStyle-Width="50" HeaderStyle-Font-Bold="true" />
                    <telerik:GridBoundColumn DataField="UniqueName" HeaderText="UniqueName" Visible="false" ReadOnly="true" HeaderStyle-Font-Bold="true" />
                    <telerik:GridBoundColumn DataField="Title" HeaderText="Χώρος" ReadOnly="true" HeaderStyle-Font-Bold="true" />
                    <telerik:GridTemplateColumn DataField="Text" HeaderText="Περιεχόμενο" UniqueName="Text" HeaderStyle-Font-Bold="true">
                        <ItemTemplate>
                            <asp:Panel ID="pnlText" runat="server">
                                <asp:Literal Text='<% #Eval("Text").ToString().Replace("\r\n", "<br />").Replace("\n", "<br />") %>' runat="server" /> 
                            </asp:Panel>
                            <asp:Panel ID="pnlImage" runat="server">
                                <asp:Image runat="server" ImageUrl='<% #string.Concat("~/UploadedFiles/",Eval("Text")) %>' AlternateText='<% #Eval("Text") %>' />
                            </asp:Panel>
                            <asp:Panel ID="pnlDate" runat="server">
                                <asp:Literal Text='<% #DateTime.Now.ToString(Eval("Text").ToString()) %>' runat="server" /> 
                            </asp:Panel>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:Panel ID="pnlText" runat="server">
                                <asp:TextBox ID="txtText" Text='<% #Bind("Text") %>' runat="server" TextMode="MultiLine" style="width:500px" />
                            </asp:Panel>
                            <asp:Panel ID="pnlImage" runat="server">
                                <telerik:RadAsyncUpload RenderMode="Lightweight" ID="uplFile" AllowedFileExtensions="jpg,gif,png" runat="server" OnFileUploaded="uplFile_FileUploaded" />
                            </asp:Panel>
                            <asp:Panel ID="pnlPageNo" runat="server">
                                 <telerik:RadDropDownList runat="server" ID="ddlText" RenderMode="Lightweight" Width="190px" AutoPostBack="true" CausesValidation="false" OnSelectedIndexChanged="ddlText_SelectedIndexChanged">
                                     <Items>
                                         <telerik:DropDownListItem runat="server" Text="Σελίδα Χ" Value="Σελίδα Χ" />
                                         <telerik:DropDownListItem runat="server" Text="Σελίδα Χ από Υ" Value="Σελίδα Χ από Υ" />
                                     </Items>
                                 </telerik:RadDropDownList>
                            </asp:Panel>
                            <asp:Panel ID="pnlDate" runat="server">
                                 <telerik:RadDropDownList runat="server" ID="ddlDate" RenderMode="Lightweight" Width="250px" AutoPostBack="true" CausesValidation="false" OnSelectedIndexChanged="ddlText_SelectedIndexChanged">
                                     <Items>
                                         <telerik:DropDownListItem runat="server" Text='<% #DateTime.Now.ToString("dddd, dd MMMM yyyy") %>' Value="dddd, dd MMMM yyyy" />
                                         <telerik:DropDownListItem runat="server" Text='<% #DateTime.Now.ToString("dd/MM/yyyy") %>' Value="dd/MM/yyyy" />
                                     </Items>
                                 </telerik:RadDropDownList>
                            </asp:Panel>
                        </EditItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn DataField="ImageWidth" HeaderText="Πλάτος" UniqueName="ImageWidth" HeaderStyle-Font-Bold="true">
                        <ItemTemplate>
                            <asp:Panel ID="pnlTextWidth" runat="server" />
                            <asp:Panel ID="pnlImageWidth" runat="server">
                                <asp:Literal Text='<% #string.Concat(Eval("ImageWidth"), " px") %>' runat="server" /> 
                            </asp:Panel>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:Panel ID="pnlTextWidth" runat="server" />
                            <asp:Panel ID="pnlImageWidth" runat="server">
                                <telerik:RadNumericTextBox RenderMode="Lightweight" runat="server" ID="ntextImageWidth" Width="190px" DbValue='<% #Bind("ImageWidth") %>' Type="Number" MinValue="0" >
                                    <NumberFormat GroupSeparator="" DecimalDigits="0" /> 
                                </telerik:RadNumericTextBox>
                            </asp:Panel>
                        </EditItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn DataField="ImageHeight" HeaderText="Ύψος" UniqueName="ImageHeight" HeaderStyle-Font-Bold="true">
                        <ItemTemplate>
                            <asp:Panel ID="pnlTextHeight" runat="server" />
                            <asp:Panel ID="pnlImageHeight" runat="server">
                                <asp:Literal Text='<% #string.Concat(Eval("ImageHeight"), " px") %>' runat="server" /> 
                            </asp:Panel>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:Panel ID="pnlTextHeight" runat="server" />
                            <asp:Panel ID="pnlImageHeight" runat="server">
                                <telerik:RadNumericTextBox RenderMode="Lightweight" runat="server" ID="ntextImageHeight" Width="190px" DbValue='<% #Bind("ImageHeight") %>' Type="Number" MinValue="0" >
                                    <NumberFormat GroupSeparator="" DecimalDigits="0" /> 
                                </telerik:RadNumericTextBox>
                            </asp:Panel>
                        </EditItemTemplate>
                    </telerik:GridTemplateColumn>
                </Columns>
                <EditFormSettings>
                    <EditColumn UpdateText="Ενημέρωση" CancelText="Ακύρωση" />                          
                </EditFormSettings>
            </MasterTableView>
        </telerik:RadGrid>
        <telerik:RadWindowManager RenderMode="Lightweight" ID="RadWindowManager1" runat="server" />
    </div>
</asp:Content>