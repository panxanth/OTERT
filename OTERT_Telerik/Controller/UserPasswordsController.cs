using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Web;
using OTERT.Model;
using OTERT_Entity;
using Telerik.Web.UI;

namespace OTERT.Controller {

    public class UserPasswordsController {

        public List<UserPasswordsB> GetUserPreviousPasswords(int userID) {
            using (var dbContext = new OTERTConnStr()) {
                try {
                    dbContext.Configuration.ProxyCreationEnabled = false;
                    List<UserPasswordsB> data = (from us in dbContext.UserPasswords
                                        select new UserPasswordsB {
                                            ID = us.ID,
                                            UserID = us.UserID,
                                            Password = us.Password,
                                            PasswordDate = us.PasswordDate,
                                        }).OrderBy(o => o.PasswordDate).Take(5).ToList();
                    return data;
                }
                catch (Exception) { return null; }
            }
        }

    }

}