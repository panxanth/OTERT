using OTERT.Controller;
using OTERT.Model;
using OTERT_Entity;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace OTERT.Pages.Password {

    public partial class ForgotPassword : Page {

        protected TextBox txtUserName;
        protected CustomValidator valUserName;
        protected string pageTitle;
        protected int newID;
        

        protected void Page_Load(object sender, EventArgs e) {
            if (!Page.IsPostBack) {
                pageTitle = ConfigurationManager.AppSettings["AppTitle"].ToString() + "Αλλαγή Κωδικού Χρήστη";
                newID = -1;
                Session.Remove("UserGroupID");
            }
        }

        protected void btnUpdate_Click(object sender, EventArgs e) {
            try {
                if (Page.IsValid) {
                    UserB loggedUser;
                    string userName = txtUserName.Text.Trim();
                    UsersController uc = new UsersController();
                    List<UserB> users = uc.GetUsers(userName);
                    loggedUser = users[0];
                    Guid newGUID = Guid.NewGuid();
                    using (var dbContext = new OTERTConnStr()) {
                        UserPasswordReset upr = new UserPasswordReset();
                        upr.UserID = loggedUser.ID;
                        upr.GUID = newGUID.ToString();
                        upr.RequestDate = DateTime.Now;
                        dbContext.UserPasswordReset.Add(upr);
                        dbContext.SaveChanges(); 
                    }
                    string siteURL = ConfigurationManager.AppSettings["siteURL"];
                    string emailTo = loggedUser.Email.Trim();
                    string emailSubject = "OTE-RT - Αίτημα για νέο Κωδικό Χρήστη";
                    string emailURL = siteURL + "Pages/Password/EmailChangePassword.aspx?GUID=" + newGUID.ToString();
                    string emailLink = "<a href=\"" + emailURL + "\">εδώ</a>";
                    string emailBody = "Λάβατε αυτό το μύνημα γιατί κάποιος (πιθανότατα εσείς) ζητήσατε την αλλαγή του Κωδικού Χρήστη για την είσοδό σας στην εφαρμογή OTE-RT.<br/><br/>";
                    emailBody += "Για να ολοκληρωθεί η διαδικασία θα πρέπει να πατήσετε " + emailLink + ". (Ο σύνδεσμος θα παραμείνει ενεργός για τά επόμενα 30 λεπτά.)<br/><br/>";
                    emailBody += "Αν δεν ζητήσατε εσείς την αλλαγή του κωδικού σας, τότε αγνοήστε αυτό το μήνυμα και ο κωδικός σας δεν θα αλλάξει.<br/><br/>";
                    emailBody += "<strong>OTE-RT Administrator.</strong>";
                    bool success = Utilities.sendEmail(emailTo, emailSubject, emailBody);
                    string redirectURL = "~/Pages/Password/ForgotPasswordOK.aspx?result=" + success.ToString();
                    Response.Redirect(redirectURL);
                }
            }
            catch (Exception) { }
        }

        protected void valUserName_Validate(object sender, ServerValidateEventArgs e) {
            string userName = txtUserName.Text.Trim();
            UsersController uc = new UsersController();
            List<UserB> users = uc.GetUsers(userName);
            if (users.Count != 1) {
                e.IsValid = false;
                valUserName.ErrorMessage = "Ο Χρήστης δεν υπάρχει!";
            } else if (string.IsNullOrEmpty(users[0].Email.Trim())) {
                e.IsValid = false;
                valUserName.ErrorMessage = "Ο Χρήστης δεν έχει δηλωμένο Email!";
            } else if (!users[0].Email.Trim().EndsWith("@ote.gr") && !users[0].Email.Trim().EndsWith("@cosmote.gr") && !users[0].Email.Trim().EndsWith("@cosmote.gr")) {
                e.IsValid = false;
                valUserName.ErrorMessage = "Ο Χρήστης δεν έχει δηλωμένο ετειρικό Email! (@ote, @cosmote ή @evalue)";
            }
        }

    }

}