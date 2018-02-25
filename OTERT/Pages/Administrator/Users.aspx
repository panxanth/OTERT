<%@ Page Title="" Language="C#" MasterPageFile="~/Masters/Inside.Master" AutoEventWireup="true" CodeBehind="Users.aspx.cs" Inherits="OTERT.Pages.Administrator.Users" %>
<%@ Import Namespace="System.Web.Optimization"  %>

<asp:Content ID="Content1" ContentPlaceHolderID="PHHead" runat="server">
    <script type="text/javascript">

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
                url: "http://otert/WebServices/OTERTWS.asmx/GetUsers",
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

        function complete(args) {
            $("#ShipCity").ejDropDownList();
            if (args.requestType == "beginedit") {
                $('#' + this._id + '_dialogEdit').ejDialog({ tooltip: { close: "Άκυρο" } });
                $('#' + this._id + '_dialogEdit').ejDialog("option", "title", "Επεξεργασία χρήστη: " + args.model.dataSource[args.rowIndex].NameGR);
                $('input[type=button]').each(function () {
                    if (endsWith(this.id, 'Save')) {
                        $(this).off('click');
                        $(this).click(function () {
                            saveData(args.model.dataSource[args.rowIndex]);
                        });
                    }
                });
            } else if (args.requestType == "add") {
                $('#' + this._id + '_dialogEdit').ejDialog({ tooltip: { close: "Άκυρο" } });
                $('#' + this._id + '_dialogEdit').ejDialog("option", "title", "Δημιουργία νέου χρήστη");
                $('input[type=text]').each(function () {
                    if (this.id == 'ID') { $(this).closest("tr").remove(); }
                });
                $('input[type=button]').each(function () {
                    if (endsWith(this.id, 'Save')) {
                        $(this).off('click');
                        $(this).click(function () {
                            saveData(args.model.dataSource[args.rowIndex]);
                        });
                    }
                });
            }
        }

        function saveData(data) {
            console.log(data);
        }

        function endsWith(str, suffix) {
            return str.indexOf(suffix, str.length - suffix.length) !== -1;
        }

    </script>
    <%: Styles.Render("~/Content/Accordion.css") %>
    <script id="templateAdd" type="text/template">                             
        <table cellspacing="10">
	        <tr>
		        <td>ID</td>
		        <td><input id="ID" name="ID" type="text" disabled="disabled" value="{{:ID}}" class="e-field e-ejinputtext" style="width: 116px; height: 28px;" /></td>
	        </tr>
            <tr>
		        <td>Ομάδα Χρηστών</td>
		        <td>
                    
                    <select id="ShipCity.Name" name="ShipCity">
					    <option value="Argentina">Argentina</option>
					    <option value="Austria">Austria</option>
					    <option value="Belgium">Belgium</option>
					    <option value="Brazil">Brazil</option>
					    <option value="Canada">Canada</option>
					    <option value="Denmark">Denmark</option>
				    </select>
                    <%--<input id="UserGroup.Name" name="UserGroup.Name" type="text" value="{{:UserGroup.Name}}" class="e-field e-ejinputtext" style="width: 116px; height: 28px;" />--%>

		        </td>
	        </tr>
            <tr>
		        <td>Ονοματεπώνυμο</td>
		        <td><input id="NameGR" name="NameGR" type="text" value="{{:NameGR}}" class="e-field e-ejinputtext" style="width: 300px; height: 28px;" /></td>
	        </tr>
            <tr>
		        <td>Ονοματεπώνυμο (English)</td>
		        <td><input id="NameEN" name="NameEN" type="text" value="{{:NameEN}}" class="e-field e-ejinputtext" style="width: 300px; height: 28px;" /></td>
	        </tr>
            <tr>
		        <td>Τηλέφωνο</td>
		        <td><input id="Telephone" name="Telephone" type="text" value="{{:Telephone}}" class="e-field e-ejinputtext" style="width: 116px; height: 28px;" /></td>
	        </tr>
            <tr>
		        <td>FAX</td>
		        <td><input id="FAX" name="FAX" type="text" value="{{:FAX}}" class="e-field e-ejinputtext" style="width: 116px; height: 28px;" /></td>
	        </tr>
            <tr>
		        <td>Email</td>
		        <td><input id="Email" name="Email" type="text" value="{{:Email}}" class="e-field e-ejinputtext" style="width: 300px; height: 28px;" /></td>
	        </tr>
	        <tr>
		        <td>Όνομα Χρήστη</td>
		        <td><input id="UserName" name="UserName" type="text" value="{{:UserName}}" class="e-field e-ejinputtext" style="width: 116px; height: 28px;" /></td>
	        </tr>
            <tr>
		        <td>Κωδικός Χρήστη</td>
		        <td><input id="Password" name="Password" type="text" value="{{:Password}}" class="e-field e-ejinputtext" style="width: 116px; height: 28px;" /></td>
	        </tr>
        </table>
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="PHContent" runat="server">
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
                    <ej:Grid ID="UGGrid" runat="server" AllowSorting="true" Width="900" AllowPaging="true" >
                        <PageSettings PageSize="10" />
                        <EditSettings AllowEditing="True" AllowAdding="True" AllowDeleting="True" EditMode="DialogTemplate" DialogEditorTemplateID="#templateAdd" ShowDeleteConfirmDialog="true"></EditSettings>
                        <ClientSideEvents ActionComplete="complete" />
                        <ToolbarSettings ShowToolbar="True" ToolbarItems="add,edit,delete,update,cancel"></ToolbarSettings>
                        <ClientSideEvents ActionComplete="complete" EndAdd="endAdd" EndDelete="endDelete" EndEdit="endEdit" ActionBegin="begin" />
                        <Columns>                
                            <ej:Column Field="ID" HeaderText="Α/Α" TextAlign="Right" Width="50" IsPrimaryKey="true" AllowEditing="false" />
                            <ej:Column Field="NameGR" HeaderText="Ονοματεπώνυμο" Width="200">
                                <ValidationRule>
                                    <ej:KeyValue Key="required" Value="true" />
                                </ValidationRule>
                            </ej:Column>
                            <ej:Column Field="NameEN" HeaderText="Ονοματεπώνυμο (English)" Width="200" Visible="false" />
                            <ej:Column Field="UserName" HeaderText="Όνομα Χρήστη" Width="150">
                                <ValidationRule>
                                    <ej:KeyValue Key="required" Value="true" />
                                </ValidationRule>
                            </ej:Column>
                            <ej:Column Field="Password" HeaderText="Κωδικός Χρήστη" Width="100" Visible="false">
                                <ValidationRule>
                                    <ej:KeyValue Key="required" Value="true" />
                                </ValidationRule>
                            </ej:Column>
                            <ej:Column Field="Email" HeaderText="Email" Width="200" />
                            <ej:Column Field="Telephone" HeaderText="Τηλέφωνο" Width="100" />
                            <ej:Column Field="FAX" HeaderText="FAX" Width="100" Visible="false" />
                            <ej:Column Field="UserGroup.Name" HeaderText="Ομάδα Χρηστών" Width="150">
                                <ValidationRule>
                                    <ej:KeyValue Key="required" Value="true" />
                                </ValidationRule>
                            </ej:Column>
                        </Columns>
                    </ej:Grid>
                </ContentSection>
            </ej:AccordionItem>
        </Items>
    </ej:Accordion>
</asp:Content>