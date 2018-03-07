<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <link rel="shortcut icon" href="../favicon.ico" /> 
    <link rel="stylesheet" type="text/css" href="../Scripts/login_style.css" />
    <script type="text/javascript" src="../Scripts/jquery-1.7.1.min.js"></script>
    <script type="text/javascript" src="../Scripts/modernizr.custom.63321.js"></script>
    <!--[if lte IE 7]><style>.main{display:none;} .support-note .note-ie{display:block;}</style><![endif]-->
    <title>OTE RT - Είσοδος</title>
    <script type="text/javascript">
        function jsonData(name, password) {
            this.name = name;
            this.password = password;
        }
        $(document).ready(function () {
            hideErrors();
            $("#submit").click(function (e) {
                var name = $("#login").val();
                var password = $("#password").val();
                console.log("username: " + name + ", password: " + password);
                if (name == '' || password == '') {
                    if (name == '' && password != '') {
                        hideErrors();
                        $("#div_name_empty").fadeIn("slow");
                    } else if (name != '' && password == '') {
                        hideErrors();
                        $("#div_password_empty").fadeIn( "slow" );
                    } else {
                        hideErrors();
                        $("#div_empty").fadeIn( "slow" );
                    }
                } else {
                    hideErrors();
                    var data = new jsonData(name, password);
                    $.ajax({
                        type: "POST",
                        url: "http://otert/WebServices/OTERTWS.asmx/ValidateLogin",
                        data: "{ strLogin: '" + JSON.stringify(data) + "' }",
                        contentType: "application/json; charset=utf-8",
                        dataType: "json",
                        error: function (jqXHR, exception) {
                            if (jqXHR.status === 0) {
                                alert('Not connect.\nVerify Network.');
                            } else if (jqXHR.status == 404) {
                                alert('Requested page not found. [404]');
                            } else if (jqXHR.status == 500) {
                                alert('Internal Server Error [500].');
                            } else if (exception === 'parsererror') {
                                alert('Requested JSON parse failed.');
                            } else if (exception === 'timeout') {
                                alert('Time out error.');
                            } else if (exception === 'abort') {
                                alert('Ajax request aborted.');
                            } else {
                                alert('Uncaught Error.\n' + jqXHR.responseText);
                            }
                        },
                        success: function (response) {
                            var returnedLogin = JSON.parse(response.d);
                            if (returnedLogin.result == "OK") {
                                $(location).attr("href", "/Pages/Users/SateliteUplink.aspx");
                            } else {
                                hideErrors();
                                $("#div_error").fadeIn("slow");
                            }
                        }
                    });
                }
                e.preventDefault();
            });
        });
        function hideErrors() {
            $("#div_error").hide();
            $("#div_name_empty").hide();
            $("#div_password_empty").hide();
            $("#div_empty").hide();
        }
    </script>
</head>
<body>
    <div class="container">
		<section class="main">
            <form id="form1" runat="server" class="form-1">
			    <p class="field">
				    <input type="text" name="login" id="login" placeholder="όνομα χρήστη" />
				    <i class="icon-user icon-large"></i>
			    </p>
			    <p class="field">
				    <input type="password" id="password" name="password" placeholder="κωδικός χρήστη" />
				    <i class="icon-lock icon-large"></i>
			    </p>
			    <p class="submit">
				    <button type="submit" id="submit" name="submit"><i class="icon-arrow-right icon-large"></i></button>
			    </p>
            </form>
            <div id="div_error" class="div-error">
                <p class="error">Ο συνδιασμός Όνομα &amp; Κωδικός χρήστη είναι λάθος!<br />Παρακαλούμε προσπαθήστε ξανά.</p>
            </div>
            <div id="div_name_empty" class="div-error">
                <p class="error">To πεδίο &quot;Όνομα Χρήστη&quot; είναι κενό!</p>
            </div>
            <div id="div_password_empty" class="div-error">
                <p class="error">To πεδίο &quot;Κωδικός Χρήστη&quot; είναι κενό!</p>
            </div>
            <div id="div_empty" class="div-error">
                <p class="error">Η φόρμα εισόδου είναι κενη!</p>
            </div>
		</section>
    </div>
</body>
</html>
