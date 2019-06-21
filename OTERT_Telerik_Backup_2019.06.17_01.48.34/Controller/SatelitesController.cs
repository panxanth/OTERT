using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using OTERT.Model;
using OTERT_Entity;

namespace OTERT.Controller {

    public class SatelitesController {

        public int CountSatelites() {
            using (var dbContext = new OTERTConnStr()) {
                try {
                    return dbContext.Satelites.Count();
                }
                catch (Exception) { return -1; }
            }
        }

        public SateliteB GetSatelite(int SateliteID) {
            using (var dbContext = new OTERTConnStr()) {
                try {
                    dbContext.Configuration.ProxyCreationEnabled = false;
                    SateliteB data = (from us in dbContext.Satelites
                                      select new SateliteB {
                                            ID = us.ID,
                                            Name = us.Name,
                                            Frequency = us.Frequency
                                      }).Where(o => o.ID == SateliteID).FirstOrDefault();
                    return data;
                }
                catch (Exception) { return null; }
            }
        }

        public List<SateliteB> GetSatelites() {
            using (var dbContext = new OTERTConnStr()) {
                try {
                    dbContext.Configuration.ProxyCreationEnabled = false;
                    List<SateliteB> data = (from us in dbContext.Satelites
                                           select new SateliteB {
                                               ID = us.ID,
                                               Name = us.Name,
                                               Frequency = us.Frequency
                                           }).OrderBy(o => o.Name).ToList();
                    return data;
                }
                catch (Exception) { return null; }
            }
        }

        public List<SateliteB> GetSatelites(int recSkip, int recTake) {
            using (var dbContext = new OTERTConnStr()) {
                try {
                    dbContext.Configuration.ProxyCreationEnabled = false;
                    List<SateliteB> data = (from us in dbContext.Satelites
                                            select new SateliteB {
                                                ID = us.ID,
                                                Name = us.Name,
                                                Frequency = us.Frequency
                                            }).OrderBy(o => o.ID).Skip(recSkip).Take(recTake).ToList();
                    return data;
                }
                catch (Exception) { return null; }
            }
        }

    }

}