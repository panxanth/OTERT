<%@ Page Title="" Language="C#" MasterPageFile="~/Masters/Inside.Master" AutoEventWireup="true" CodeBehind="insidetest.aspx.cs" Inherits="OTERT.insidetest" %>
<%@ Import Namespace="System.Web.Optimization"  %>

<asp:Content ID="Content1" ContentPlaceHolderID="PHHead" runat="server">
    <script type="text/javascript" class="jsScript">

        function json2ws(skip, take) {
            this.skip = skip;
            this.take = take;
            this.search = new Array();
            this.order = "";
        }

        $(function () {
            populateGrid();
        });

        function btnRefresh_onClick(e) {
            $('#<%= UGGrid.ClientID %>').data('ejGrid').refreshContent();
        }

        function btnChangeLocalData_onClick(e) {
            var newData = [
                { ID: 1, Name: "Mistos" },
                { ID: 2, Name: "Kitsos" }
            ];
            $('#<%= UGGrid.ClientID %>').ejGrid("dataSource", newData);
        }

        function btnFilter_onClick(e) {
            populateGrid();
        }

        function populateGrid() {
            $('#<%= UGGrid.ClientID %>').ejWaitingPopup("show");
            var params = new json2ws(10, 20);
            //params.order = encodeURIComponent('ID asc');
            //params.search.push(encodeURIComponent('Name.Contains("Panos 100")'));
            //params.search.push(encodeURIComponent('ID > 1000 and ID < 1005'));
            $.ajax({
                type: "POST",
                url: "http://otert/WebServices/OTERTWS.asmx/GetUserGroups",
                data: "{ strValue: '" + JSON.stringify(params) + "' }",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    var gridContent = JSON.parse(data.d);
                    $('#<%= UGGrid.ClientID %>').ejGrid("dataSource", gridContent.result);
                    $('#<%= UGGrid.ClientID %>').ejWaitingPopup("hide");
                }
            });
        }

    </script>
    <%: Styles.Render("~/Content/Accordion.css") %>

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="PHContent" runat="server">
    Cut <span class = "fa fa-cut"></span><br /><br />
    <ej:Accordion ID="mainAccordion" runat="server" EnableMultipleOpen="true" SelectedItemIndex="1">
        <CustomIcon Header="e-arrowheaddown" SelectedHeader="e-arrowheadup"/>
        <Items>
            <ej:AccordionItem  ImageCssClass="logos volkswagan"  Text="Filter Controls">
                <ContentSection>
                    <ej:Button ID="btnRefresh" runat="server" Type="Button" Text="Refresh Data" ClientSideOnClick="btnRefresh_onClick"></ej:Button>
                    <ej:Button ID="btnChangeLocalData" runat="server" Type="Button" Text="Change Local Data" ClientSideOnClick="btnChangeLocalData_onClick"></ej:Button>
                    <br /><br />
                    Filter by: <input id="txtFilter" type="text" placeholder="Type something..." />
                    <ej:Button ID="btnFilter" runat="server" Type="Button" Text="Filter Data" ClientSideOnClick="btnFilter_onClick"></ej:Button>
                </ContentSection>
            </ej:AccordionItem>
            <ej:AccordionItem ImageCssClass="logos benz" Text="Grid Data">
                <ContentSection>
                    <ej:Grid ID="UGGrid" runat="server" AllowSorting="true" Width="800" AllowPaging="true" >
                        <PageSettings PageSize="10" />
                        <EditSettings AllowEditing="True" AllowAdding="True" AllowDeleting="True"></EditSettings>
                        <ToolbarSettings ShowToolbar="True" ToolbarItems="add,edit,delete,update,cancel"></ToolbarSettings>
                        <Columns>                
                            <ej:Column Field="ID" HeaderText="ID" TextAlign="Right" Width="100" IsPrimaryKey="true" />
                            <ej:Column Field="Name" HeaderText="Group Name" Width="100" />
                        </Columns>
                    </ej:Grid>
                </ContentSection>
            </ej:AccordionItem>
        </Items>
    </ej:Accordion>
</asp:Content>