using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Web;
using OTERT_Telerik.Model;
using OTERT_Entity;

namespace OTERT_Telerik.Controller {

    public class UsersController {

        public int CountUsers() {
            using (var dbContext = new OTERTConnStr()) {
                try {
                    dbContext.Configuration.ProxyCreationEnabled = false;
                    return dbContext.Users.Count();
                }
                catch (Exception ex) { return -1; }
            }
        }

        public List<UserB> GetUsers() {
            using (var dbContext = new OTERTConnStr()) {
                try {
                    dbContext.Configuration.ProxyCreationEnabled = false;
                    List<UserB> data = (from us in dbContext.Users
                                        select new UserB {
                                            ID = us.ID,
                                            UserGroupID = us.UserGroupID,
                                            NameGR = us.NameGR,
                                            NameEN = us.NameEN,
                                            Telephone = us.Telephone,
                                            FAX = us.FAX,
                                            Email = us.Email,
                                            UserName = us.UserName,
                                            Password = us.Password,
                                            UserGroup = new UserGroupDTO { ID = us.UserGroups.ID, Name = us.UserGroups.Name }
                                        }).OrderBy(o => o.NameGR).ToList();
                    return data;
                }
                catch (Exception ex) { return null; }
            }
        }

        public List<UserB> GetUsers(int recSkip, int recTake) {
            using (var dbContext = new OTERTConnStr()) {
                try {
                    dbContext.Configuration.ProxyCreationEnabled = false;
                    List<UserB> data = (from us in dbContext.Users
                                        select new UserB {
                                            ID = us.ID,
                                            UserGroupID = us.UserGroupID,
                                            NameGR = us.NameGR,
                                            NameEN = us.NameEN,
                                            Telephone = us.Telephone,
                                            FAX = us.FAX,
                                            Email = us.Email,
                                            UserName = us.UserName,
                                            Password = us.Password,
                                            UserGroup = new UserGroupDTO { ID = us.UserGroups.ID, Name = us.UserGroups.Name }
                                        }).OrderBy(o => o.ID).Skip(recSkip).Take(recTake).ToList();
                    return data;
                }
                catch (Exception ex) { return null; }
            }
        }

    }

}