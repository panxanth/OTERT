using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Web;
using OTERT.Model;
using OTERT_Entity;

namespace OTERT.Controller {

    public class UserGroupsController {

        public int CountUserGroups(string recFilter) {
            using (var dbContext = new OTERTConnStr()) {
                try {
                    int count = 0;
                    dbContext.Configuration.ProxyCreationEnabled = false;
                    if (!string.IsNullOrEmpty(recFilter)) {
                        count = dbContext.UserGroups.Where(recFilter).Count();
                    } else {
                        count = dbContext.UserGroups.Count();
                    }
                    return count;
                }
                catch (Exception) { return -1; }
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
                catch (Exception) { return null; }
            }
        }

        public List<UserGroupB> GetUserGroups(int recSkip, int recTake, string recFilter) {
            using (var dbContext = new OTERTConnStr()) {
                try {
                    dbContext.Configuration.ProxyCreationEnabled = false;
                    IQueryable<UserGroupB> datatmp =  (from us in dbContext.UserGroups
                                                       select new UserGroupB {
                                                           ID = us.ID,
                                                           Name = us.Name
                                                       });
                    if (!string.IsNullOrEmpty(recFilter)) { datatmp = datatmp.Where(recFilter); }
                    List<UserGroupB> data = datatmp.OrderBy(o => o.ID).Skip(recSkip).Take(recTake).ToList();
                    return data;
                }
                catch (Exception) { return null; }
            }
        }

    }

}