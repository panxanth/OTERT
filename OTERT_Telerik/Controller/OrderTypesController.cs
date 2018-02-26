using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using OTERT.Model;
using OTERT_Entity;

namespace OTERT.Controller {

    public class OrderTypesController {

        public int CountOrderTypes() {
            using (var dbContext = new OTERTConnStr()) {
                try {
                    return dbContext.OrderTypes.Count();
                }
                catch (Exception ex) { return -1; }
            }
        }

        public List<OrderTypeB> GetOrderTypes() {
            using (var dbContext = new OTERTConnStr()) {
                try {
                    dbContext.Configuration.ProxyCreationEnabled = false;
                    List<OrderTypeB> data = (from us in dbContext.OrderTypes
                                            select new OrderTypeB {
                                               ID = us.ID,
                                               Name = us.Name
                                             }).OrderBy(o => o.Name).ToList();
                    return data;
                }
                catch (Exception ex) { return null; }
            }
        }

        public List<OrderTypeB> GetOrderTypes(int recSkip, int recTake) {
            using (var dbContext = new OTERTConnStr()) {
                try {
                    dbContext.Configuration.ProxyCreationEnabled = false;
                    List<OrderTypeB> data = (from us in dbContext.OrderTypes
                                            select new OrderTypeB {
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