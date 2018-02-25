using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Script.Services;
using Syncfusion.JavaScript;
using OTERT_Entity;
using OTERT.DTO;
using System.Web.Script.Serialization;

namespace OTERT.WebServices {

    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    [ScriptService]

    public class OTERTWS : WebService {

        [WebMethod]
        [ScriptMethod(UseHttpGet = true, ResponseFormat = ResponseFormat.Json)]
        public void GetUserGroups() {
            using (var dbContext = new OTERTConnStr()) {
                try {
                    HttpContext.Current.Response.Clear();
                    dbContext.Configuration.ProxyCreationEnabled = false;
                    var data = (from ord in dbContext.UserGroups.Take(200)
                                select new UserGroupDTO {
                                    ID = ord.ID,
                                    Name = ord.Name
                                }).ToList();
                    //data = data.Skip(value.Skip).Take(value.Take).ToList();
                    //return new { result = data, count = data.Count };
                    HttpContext.Current.Response.ContentType = "application/json;charset=utf-8";
                    JavaScriptSerializer serialize = new JavaScriptSerializer();
                    HttpContext.Current.Response.Write(String.Format("{{\"d\":{{\"results\":{0},\"__count\":{1}}}}}", serialize.Serialize(data), data.Count));
                    HttpContext.Current.ApplicationInstance.CompleteRequest();
                }
                catch (Exception) { bool test = false; }
            }
        }

    }

}