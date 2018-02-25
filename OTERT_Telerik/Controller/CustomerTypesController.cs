using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using OTERT_Telerik.Model;
using OTERT_Entity;

namespace OTERT_Telerik.Controller {

    public class CustomerTypesController {

        public int CountCustomerTypes() {
            using (var dbContext = new OTERTConnStr()) {
                try {
                    return dbContext.CustomerTypes.Count();
                }
                catch (Exception ex) { return -1; }
            }
        }

        public List<CustomerTypeB> GetCustomerTypes() {
            using (var dbContext = new OTERTConnStr()) {
                try {
                    dbContext.Configuration.ProxyCreationEnabled = false;
                    List<CustomerTypeB> data = (from us in dbContext.CustomerTypes
                                           select new CustomerTypeB {
                                               ID = us.ID,
                                               NameGR = us.NameGR,
                                               NameEN = us.NameEN
                                             }).OrderBy(o => o.NameGR).ToList();
                    return data;
                }
                catch (Exception ex) { return null; }
            }
        }

        public List<CustomerTypeB> GetCustomerTypes(int recSkip, int recTake) {
            using (var dbContext = new OTERTConnStr()) {
                try {
                    dbContext.Configuration.ProxyCreationEnabled = false;
                    List<CustomerTypeB> data = (from us in dbContext.CustomerTypes
                                           select new CustomerTypeB {
                                                ID = us.ID,
                                                NameGR = us.NameGR,
                                                NameEN = us.NameEN
                                           }).OrderBy(o => o.ID).Skip(recSkip).Take(recTake).ToList();
                    return data;
                }
                catch (Exception ex) { return null; }
            }
        }

    }

}