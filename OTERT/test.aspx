<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="test.aspx.cs" Inherits="OTERT.test" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">



    <link href="/Content/bootstrap.min.css" rel="stylesheet"/>
    <link href="/Content/Site.css" rel="stylesheet"/>
    <link href="/Content/ej/web/Office-365/ej.web.all.min.css" rel="stylesheet"/>
    <!--[if lt IE 9]>
        <script src="/Scripts/jquery-1.11.3.min.js"></script>
    <![endif]-->
    <!--[if gte IE 9]><!-->
        <script src="/Scripts/jquery-3.1.1.min.js"></script>
    <!--<![endif]-->
    <script src="/Scripts/pivotgrid.datasource.js"></script>
    <script src="/Scripts/jsrender.min.js"></script>
    <script src="/Scripts/ej/ej.web.all.min.js"></script>
    <script src="/Scripts/ej/ej.webform.min.js"></script>
    <script src="/Scripts/jsondatachart.js"></script>
    <script src="/Scripts/sampleslist.js"></script>
    <script src="/Scripts/properties.js"></script>
    <script src="/Scripts/ZeroClipboard.js"></script>
    <link href="/Content/mbcsmbmcp.css" rel="stylesheet"/>
    <script src="/Scripts/mbjsmbmcp.js"></script>


    <title></title>
    <script>
        $(function () {
            var dataManger = ej.DataManager({
                url: "WebServices/OTERTWS.asmx/GetUserGroups",
                offline: true,
            });
            $("#Grid").ejGrid({
                dataSource: dataManger,
                allowPaging: true,
                columns: [
                    { field: "id", headerText: "Order ID", width: 75 },
                    { field: "Name", headerText: "Customer ID", width: 80 },
                    { field: "Name", headerText: "Employee ID",  width: 75 }
                ]
            });

        });
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <div id="Grid" style="width: 700px"></div>
    </div>
    </form>
</body>
</html>
