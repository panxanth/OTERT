using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Web;
using OTERT.Model;
using OTERT_Entity;
using Telerik.Web.UI;

namespace OTERT.Controller {

    public class UserPasswordResetController {

        public UserPasswordResetB GetPasswordReset(string GUID) {
            using (var dbContext = new OTERTConnStr()) {
                try {
                    dbContext.Configuration.ProxyCreationEnabled = false;
                    UserPasswordResetB data = (from us in dbContext.UserPasswordReset 
                                                select new UserPasswordResetB {
                                                        ID = us.ID,
                                                        UserID = us.UserID,
                                                        GUID = us.GUID,
                                                        RequestDate = us.RequestDate,
                                                    }).Where(o => o.GUID == GUID).FirstOrDefault();
                    return data;
                }
                catch (Exception) { return null; }
            }
        }

    }

}