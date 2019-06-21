using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using OTERT.Model;
using OTERT_Entity;

namespace OTERT.Controller {

    public class SalesController {

        public int CountSales() {
            using (var dbContext = new OTERTConnStr()) {
                try {
                    return dbContext.Sales.Count();
                }
                catch (Exception) { return -1; }
            }
        }

        public List<SaleB> GetSales() {
            using (var dbContext = new OTERTConnStr()) {
                try {
                    dbContext.Configuration.ProxyCreationEnabled = false;
                    List<SaleB> data = (from us in dbContext.Sales
                                           select new SaleB {
                                               ID = us.ID,
                                               Name = us.Name,
                                               Type = us.Type
                                           }).OrderBy(o => o.Name).ToList();
                    return data;
                }
                catch (Exception) { return null; }
            }
        }

        public List<SaleB> GetSales(int recSkip, int recTake) {
            using (var dbContext = new OTERTConnStr()) {
                try {
                    dbContext.Configuration.ProxyCreationEnabled = false;
                    List<SaleB> data = (from us in dbContext.Sales
                                        select new SaleB {
                                                ID = us.ID,
                                                Name = us.Name,
                                                Type = us.Type
                                           }).OrderBy(o => o.ID).Skip(recSkip).Take(recTake).ToList();
                    return data;
                }
                catch (Exception) { return null; }
            }
        }

    }

}