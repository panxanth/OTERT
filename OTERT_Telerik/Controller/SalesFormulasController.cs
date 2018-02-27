using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using OTERT.Model;
using OTERT_Entity;

namespace OTERT.Controller {

    public class SalesFormulasController {

        public int CountSalesFormulas(int salesID) {
            using (var dbContext = new OTERTConnStr()) {
                try {
                    return dbContext.SalesFormulas.Where(k => k.SalesID == salesID).Count();
                }
                catch (Exception) { return -1; }
            }
        }

        public List<SalesFormulaB> GetSalesFormulas(int salesID) {
            using (var dbContext = new OTERTConnStr()) {
                try {
                    dbContext.Configuration.ProxyCreationEnabled = false;
                    List<SalesFormulaB> data = (from us in dbContext.SalesFormulas
                                        select new SalesFormulaB {
                                            ID = us.ID,
                                            SalesID = us.SalesID,
                                            Distance = us.Distance,
                                            SalePercent = us.SalePercent
                                        }).Where(k => k.SalesID == salesID).OrderBy(o => o.Distance).ToList();
                    return data;
                }
                catch (Exception) { return null; }
            }
        }

        public List<SalesFormulaB> GetSalesFormulas(int salesID, int recSkip, int recTake) {
            using (var dbContext = new OTERTConnStr()) {
                try {
                    dbContext.Configuration.ProxyCreationEnabled = false;
                    List<SalesFormulaB> data = (from us in dbContext.SalesFormulas
                                        select new SalesFormulaB {
                                                ID = us.ID,
                                                SalesID = us.SalesID,
                                                Distance = us.Distance,
                                                SalePercent = us.SalePercent
                                           }).Where(k => k.SalesID == salesID).OrderBy(o => o.ID).Skip(recSkip).Take(recTake).ToList();
                    return data;
                }
                catch (Exception) { return null; }
            }
        }

    }

}