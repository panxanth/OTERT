﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Web.Services;
using System.Web.Script.Serialization;
using System.Web.Script.Services;
using OTERT.Model;
using OTERT_Entity;

namespace OTERT.WebServices {

    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    [ScriptService]

    public class OTERTWS : WebService {

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public object GetUserGroups(string strValue) {
            JSON2WS value = (JSON2WS)Newtonsoft.Json.JsonConvert.DeserializeObject(strValue, typeof(JSON2WS));
            using (var dbContext = new OTERTConnStr()) {
                try {
                    dbContext.Configuration.ProxyCreationEnabled = false;
                    bool alreadyOrdered = false;
                    var query = from u in dbContext.UserGroups select u;
                    if (value.SearchFilters.Count > 0) {
                        foreach (string filter in value.SearchFilters) {
                            query = query.Where(Uri.UnescapeDataString(filter));
                        }
                    }
                    if (!string.IsNullOrEmpty(value.Order)) {
                        query = query.OrderBy(Uri.UnescapeDataString(value.Order));
                        alreadyOrdered = true;
                    }
                    if (value.Skip > 0) {
                        if (!alreadyOrdered) {
                            query = query.OrderBy("ID");
                            alreadyOrdered = true;
                        }
                        query = query.Skip(value.Skip);
                    }
                    if (value.Take > 0) {
                        if (!alreadyOrdered) {
                            query = query.OrderBy("ID");
                            alreadyOrdered = true;
                        }
                        query = query.Take(value.Take);
                    }
                    var data = query.Select(u => new UserGroupDTO {
                        ID = u.ID,
                        Name = u.Name
                    }).ToList();

                    var count = data.Count;
                    object responseObj = new { result = data, count = count };
                    return new JavaScriptSerializer().Serialize(responseObj);
                }
                catch (Exception) { return null; }
            }
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public object ValidateLogin(string strLogin) {
            Login login = (Login)Newtonsoft.Json.JsonConvert.DeserializeObject(strLogin, typeof(Login));
            using (var dbContext = new OTERTConnStr()) {
                try {
                    var query = dbContext.Users
                                .Where("UserName == @0 and Password == @1", login.Username, login.Password);
                    UserB loggedUser = query.Select(us => new UserB {
                        ID = us.ID,
                        NameGR = us.NameGR,
                        NameEN = us.NameEN,
                        Telephone = us.Telephone,
                        FAX = us.FAX,
                        Email = us.Email,
                        UserName = us.UserName,
                        Password = us.Password,
                        UserGroup = new UserGroupDTO { ID = us.UserGroups.ID, Name = us.UserGroups.Name },
                        UserGroupID = us.UserGroupID
                    }).FirstOrDefault();
                    object responseObj;
                    if (loggedUser != null) {
                        Session["LoggedUser"] = loggedUser;
                        if (loggedUser.UserGroup.Name == "Helper") {
                            responseObj = new { result = "OK_Helper" };
                        } else {
                            responseObj = new { result = "OK" };
                        }
                    } else {
                        responseObj = new { result = "Unknown" };
                    }
                    return new JavaScriptSerializer().Serialize(responseObj);
                }
                catch (Exception) { return null; }
            }
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public object GetPlaces(string strValue) {
            strValue = strValue.Substring(1, strValue.Length - 1);
            strValue = strValue.Remove(strValue.Length - 1);
            //strValue = strValue.Replace(@"\"", "\"");
            //DataManager value = WSUtils.Deserialize<DataManager>(strValue);
            using (var dbContext = new OTERTConnStr()) {
                try {
                    dbContext.Configuration.ProxyCreationEnabled = false;
                    var data = (from pl in dbContext.Places.Take(200)
                                select new PlaceB {
                                    ID = pl.ID,
                                    Country = new CountryDTO { ID = pl.Countries.ID, NameGR = pl.Countries.NameGR, NameEN = pl.Countries.NameEN, },
                                    NameGR = pl.NameGR,
                                    NameEN = pl.NameEN
                                }).ToList();
                    var count = data.Count;
                    //data = data.Skip(value.Skip).Take(value.Take).ToList();
                    data = data.ToList();
                    object responseObj = new { result = data, count = count };
                    return new JavaScriptSerializer().Serialize(responseObj);
                }
                catch (Exception) { return null; }
            }
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public object GetUsers(string strValue) {
            strValue = strValue.Substring(1, strValue.Length - 1);
            strValue = strValue.Remove(strValue.Length - 1);
            //strValue = strValue.Replace(@"\"", "\"");
            //DataManager value = WSUtils.Deserialize<DataManager>(strValue);
            using (var dbContext = new OTERTConnStr()) {
                try {
                    dbContext.Configuration.ProxyCreationEnabled = false;
                    var data = (from us in dbContext.Users
                                select new UserB {
                                    ID = us.ID,
                                    UserGroup = new UserGroupDTO { ID = us.UserGroups.ID, Name = us.UserGroups.Name },
                                    NameGR = us.NameGR,
                                    NameEN = us.NameEN,
                                    Telephone = us.Telephone,
                                    FAX = us.FAX,
                                    Email = us.Email,
                                    UserName = us.UserName,
                                    Password = us.Password
                                }).ToList();
                    var count = data.Count;
                    //data = data.Skip(value.Skip).Take(value.Take).ToList();
                    data = data.ToList();
                    object responseObj = new { result = data, count = count };
                    return new JavaScriptSerializer().Serialize(responseObj);
                }
                catch (Exception) { return null; }
            }
        }

    }

}