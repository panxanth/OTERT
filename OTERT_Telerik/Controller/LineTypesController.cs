using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using OTERT.Model;
using OTERT_Entity;

namespace OTERT.Controller {

    public class LineTypesController {

        public int CountLineTypes() {
            using (var dbContext = new OTERTConnStr()) {
                try {
                    return dbContext.LineTypes.Count();
                }
                catch (Exception) { return -1; }
            }
        }

        public List<LineTypeB> GetLineTypes() {
            using (var dbContext = new OTERTConnStr()) {
                try {
                    dbContext.Configuration.ProxyCreationEnabled = false;
                    List<LineTypeB> data = (from us in dbContext.LineTypes
                                            select new LineTypeB {
                                               ID = us.ID,
                                               Name = us.Name
                                             }).OrderBy(o => o.Name).ToList();
                    return data;
                }
                catch (Exception) { return null; }
            }
        }

        public List<LineTypeB> GetLineTypes(int recSkip, int recTake) {
            using (var dbContext = new OTERTConnStr()) {
                try {
                    dbContext.Configuration.ProxyCreationEnabled = false;
                    List<LineTypeB> data = (from us in dbContext.LineTypes
                                            select new LineTypeB {
                                                ID = us.ID,
                                                Name = us.Name
                                           }).OrderBy(o => o.ID).Skip(recSkip).Take(recTake).ToList();
                    return data;
                }
                catch (Exception) { return null; }
            }
        }

    }

}