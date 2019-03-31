<%@ Page Title="" Language="C#" MasterPageFile="~/Masters/Inside.Master" AutoEventWireup="true" CodeBehind="KETDailyList.aspx.cs" Inherits="OTERT.Pages.PrintTemplates.KETDailyList" %>

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
                    <telerik:GridEditCommandColumn EditText="Επεξεργασία" />
                    <telerik:GridBoundColumn DataField="ID" HeaderText="Α/Α" ReadOnly="true" ForceExtractValue="Always" ConvertEmptyStringToNull="true" />
                    <telerik:GridBoundColumn DataField="UniqueName" HeaderText="UniqueName" Visible="false" ReadOnly="true" />
                    <telerik:GridBoundColumn DataField="Title" HeaderText="Χώρος" ReadOnly="true" />
                    <telerik:GridTemplateColumn DataField="Text" HeaderText="Περιεχόμενο" UniqueName="Text">
                        <ItemTemplate>
                            <asp:Panel ID="pnlText" runat="server">
                                <asp:Literal Text='<% #Eval("Text").ToString().Replace("\n", "<br />") %>' runat="server" /> 
                            </asp:Panel>
                            <asp:Panel ID="pnlImage" runat="server">
                                <asp:Image runat="server" ImageUrl='<% #string.Concat("~/UploadedFiles/",Eval("Text")) %>' AlternateText='<% #Eval("Text") %>' />
                            </asp:Panel>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:Panel ID="pnlText" runat="server">
                                <asp:TextBox ID="txtText" Text='<% #Bind("Text") %>' runat="server" TextMode="MultiLine" />
                            </asp:Panel>
                            <asp:Panel ID="pnlImage" runat="server">
                                <telerik:RadAsyncUpload RenderMode="Lightweight" ID="uplFile" AllowedFileExtensions="jpg,jpeg,gif,png" runat="server" OnFileUploaded="uplFile_FileUploaded">
                                </telerik:RadAsyncUpload>
                            </asp:Panel>
                        </EditItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn DataField="ImageWidth" HeaderText="Πλάτος" UniqueName="ImageWidth">
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
                    <telerik:GridTemplateColumn DataField="ImageHeight" HeaderText="Ύψος" UniqueName="ImageHeight">
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
        <br /><br />
        <asp:Button ID="btnPrint" runat="server" Text="Print Test" OnClick="btnPrint_Click" />
        <telerik:RadWindowManager RenderMode="Lightweight" ID="RadWindowManager1" runat="server" />
    </div>
</asp:Content>