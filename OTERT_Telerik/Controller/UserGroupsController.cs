using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using OTERT_Telerik.Model;
using OTERT_Entity;

namespace OTERT_Telerik.Controller {

    public class UserGroupsController {

        public int CountUserGroups() {
            using (var dbContext = new OTERTConnStr()) {
                try {
                    return dbContext.UserGroups.Count();
                }
                catch (Exception ex) { return -1; }
            }
        }

        public List<UserGroupB> GetUserGroups() {
            using (var dbContext = new OTERTConnStr()) {
                try {
                    dbContext.Configuration.ProxyCreationEnabled = false;
                    List<UserGroupB> data = (from us in dbContext.UserGroups
                                             select new UserGroupB {
                                                 ID = us.ID,
                                                 Name = us.Name
                                             }).OrderBy(o => o.Name).ToList();
                    return data;
                }
                catch (Exception ex) { return null; }
            }
        }

        public List<UserGroupB> GetUserGroups(int recSkip, int recTake) {
            using (var dbContext = new OTERTConnStr()) {
                try {
                    dbContext.Configuration.ProxyCreationEnabled = false;
                    List<UserGroupB> data = (from us in dbContext.UserGroups
                                             select new UserGroupB {
                                                 ID = us.ID,
                                                 Name = us.Name
                                             }).OrderBy(o => o.ID).Skip(recSkip).Take(recTake).ToList();
                    return data;
                }
                catch (Exception ex) { return null; }
            }
        }

    }

}