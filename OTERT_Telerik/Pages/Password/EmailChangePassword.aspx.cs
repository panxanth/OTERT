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

    public partial class EmailChangePassword : Page {

        protected TextBox txtNewPasswd, txtRetypePasswd;
        protected CustomValidator valOldPasswd, valNewPasswd, valRetypePasswd;
        protected string pageTitle;
        protected int newID;
        protected UserB loggedUser;
        protected Label lblError;
        protected PlaceHolder phSuccess, phError;

        protected void Page_Load(object sender, EventArgs e) {
            if (!Page.IsPostBack) {
                pageTitle = ConfigurationManager.AppSettings["AppTitle"].ToString() + "Αλλαγή Κωδικού Χρήστη";
                newID = -1;
                Session.Remove("UserGroupID");
                phError.Visible = false;
                phSuccess.Visible = false;
            }
            string GUID = Request.QueryString["GUID"];
            if (string.IsNullOrEmpty(GUID)) {
                phError.Visible = true;
                phSuccess.Visible = false;
                lblError.Text = "Το GUID είναι κενό!";
            }
            UserPasswordResetController uprc = new UserPasswordResetController();
            UserPasswordResetB upr = uprc.GetPasswordReset(GUID);
            if (upr == null ) {
                phError.Visible = true;
                phSuccess.Visible = false;
                lblError.Text = "Το GUID είναι λάθος!";
            }
            if (DateTime.Now > upr.RequestDate.AddMinutes(30)) {
                phError.Visible = true;
                phSuccess.Visible = false;
                lblError.Text = "Το χρονικό διάστημα των 30 λεπτών έχει παρέλθει!";
            }
            UsersController uc = new UsersController();
            loggedUser = uc.GetUser(upr.UserID);
            phError.Visible = false;
            phSuccess.Visible = true;
            string newPassword = txtNewPasswd.Text;
            txtNewPasswd.Attributes.Add("value", newPassword);
            string retypePassword = txtRetypePasswd.Text;
            txtRetypePasswd.Attributes.Add("value", retypePassword);
        }

        protected void btnUpdate_Click(object sender, EventArgs e) {
            bool success = false;
            try {
                if (Page.IsValid) {
                    using (var dbContext = new OTERTConnStr()) {
                        Users user = dbContext.Users.Where(n => n.ID == loggedUser.ID).FirstOrDefault();
                        if (user != null) {
                            string plainPassword = txtNewPasswd.Text.Trim();
                            string salt = "";
                            if (user.PasswordIsHashed == true) { salt = user.PasswordSalt; } else { salt = Utilities.GetRandomSalt(10); }
                            string hashedPassword = Utilities.ComputeHash(plainPassword, salt);
                            user.Password = hashedPassword;
                            user.PasswordSalt = salt;
                            user.PasswordReset = false;
                            user.PasswordWrongTimes = 0;
                            user.PasswordLockedDatetime = new DateTime(1900, 1, 1);
                            user.PasswordIsHashed = true;
                            UserPasswords up = new UserPasswords();
                            up.UserID = loggedUser.ID;
                            up.Password = hashedPassword;  
                            up.PasswordDate = DateTime.Now;
                            dbContext.UserPasswords.Add(up);
                            dbContext.SaveChanges();
                            success = true;
                            Utilities.logSomething(loggedUser.UserName, Utilities.GetIPAddress(), Utilities.LogEventTypes.UserChangedPassword);
                            string redirectURL = "~/Pages/Password/EmailChangePasswordOK?result=" + success.ToString();
                            Response.Redirect(redirectURL);
                        }
                    }
                }
            }
            catch (Exception) {
                success = false;
                string redirectURL = "~/Pages/Password/EmailChangePasswordOK?result=" + success.ToString();
                Response.Redirect(redirectURL);
            }
        }

        protected void btnBack_Click(object sender, EventArgs e) {
            Response.Redirect("~/Default.aspx");
        }

        protected void valNewPasswd_Validate(object sender, ServerValidateEventArgs e) {
            string newPassword = txtNewPasswd.Text.Trim();
            string newHashedPassword = Utilities.ComputeHash(newPassword, loggedUser.PasswordSalt);
            Regex RegExNewPasswd = new Regex("^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{12,}$");
            UserPasswordsController upc = new UserPasswordsController();
            List<UserPasswordsB> previousPasswords = upc.GetUserPreviousPasswords(loggedUser.ID);
            string email = " ";
            if (!string.IsNullOrEmpty(loggedUser.Email)) { email = loggedUser.Email.Split('@')[0]; }
            if (string.IsNullOrEmpty(newPassword)) {
                e.IsValid = false;
                valNewPasswd.ErrorMessage = "Το πεδίο είναι κενό!";
            } else if (!RegExNewPasswd.IsMatch(newPassword)) {
                e.IsValid = false;
                valNewPasswd.ErrorMessage = "Ο κωδικός δεν πληρoί τους Κανόνες δημιουργίας Κωδικού Χρήστη!";
            } else if (previousPasswords.Any(cus => cus.Password == newHashedPassword)) {
                e.IsValid = false;
                valNewPasswd.ErrorMessage = "Ο κωδικός έχει χρησιμοποιηθεί στο παρελθόν!";
            } else if (newPassword.ToUpper().Contains(loggedUser.UserName.ToUpper()) || newPassword.ToUpper().Contains(email.ToUpper())) {
                e.IsValid = false;
                valNewPasswd.ErrorMessage = "Ο κωδικός δεν πρέπει να περιέχει το Όνομα Χρήστη ή/και το email του!";
            } else if (loggedUser.PasswordIsHashed == false) {
                if (newPassword == loggedUser.Password) {
                    e.IsValid = false;
                    valNewPasswd.ErrorMessage = "Ο νέος Κωδικός δεν μπορεί να είναι ίδιος με τον παλιό!";
                }
            } else if (loggedUser.PasswordIsHashed == true) {
                if (newHashedPassword == loggedUser.Password) {
                    e.IsValid = false;
                    valNewPasswd.ErrorMessage = "Ο νέος Κωδικός δεν μπορεί να είναι ίδιος με τον παλιό!";
                }
            }
        }

        protected void valRetypePasswd_Validate(object sender, ServerValidateEventArgs e) {
            string newPassword = txtNewPasswd.Text.Trim();
            string retypePassword = txtRetypePasswd.Text.Trim();
            if (string.IsNullOrEmpty(retypePassword)) {
                e.IsValid = false;
                valRetypePasswd.ErrorMessage = "Το πεδίο είναι κενό!";
            } else if (retypePassword != newPassword) {
                e.IsValid = false;
                valRetypePasswd.ErrorMessage = "Οι δύο κωδικοί δεν είναι ίδιοι!";
            }
        }

    }

}