using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Web;
using OTERT.Model;
using OTERT_Entity;
using Telerik.Web.UI;

namespace OTERT.Controller {

    public class UsersController {

        public int CountUsers(string recFilter) {
            using (var dbContext = new OTERTConnStr()) {
                try {
                    int count = 0;
                    dbContext.Configuration.ProxyCreationEnabled = false;
                    if (!string.IsNullOrEmpty(recFilter)) {
                        count = dbContext.Users.Where(recFilter).Count();
                    } else {
                        count = dbContext.Users.Count();
                    }
                    return count;
                }
                catch (Exception) { return -1; }
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
                catch (Exception) { return null; }
            }
        }

        public List<UserB> GetUsers(int recSkip, int recTake, string recFilter, GridSortExpressionCollection gridSortExxpressions) {
            using (var dbContext = new OTERTConnStr()) {
                try {
                    dbContext.Configuration.ProxyCreationEnabled = false;
                    IQueryable<UserB> datatmp = (from us in dbContext.Users
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
                                                });
                    if (!string.IsNullOrEmpty(recFilter)) { datatmp = datatmp.Where(recFilter); }
                    if (gridSortExxpressions.Count > 0) {
                        datatmp = datatmp.OrderBy(gridSortExxpressions[0].FieldName + " " + gridSortExxpressions[0].SortOrder);
                    } else {
                        datatmp = datatmp.OrderByDescending(o => o.ID);
                    }
                    List<UserB> data = datatmp.Skip(recSkip).Take(recTake).ToList();
                    return data;
                }
                catch (Exception) { return null; }
            }
        }

    }

}