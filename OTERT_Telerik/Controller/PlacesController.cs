using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Web;
using OTERT.Model;
using OTERT_Entity;

namespace OTERT.Controller {

    public class PlacesController {

        public int CountPlaces(string recFilter) {
            using (var dbContext = new OTERTConnStr()) {
                try {
                    int count = 0;
                    dbContext.Configuration.ProxyCreationEnabled = false;
                    if (!string.IsNullOrEmpty(recFilter)) {
                        count = dbContext.Places.Where(recFilter).Count();
                    } else {
                        count = dbContext.Places.Count();
                    }
                    return count;
                }
                catch (Exception) { return -1; }
            }
        }

        public List<PlaceB> GetPlaces() {
            using (var dbContext = new OTERTConnStr()) {
                try {
                    dbContext.Configuration.ProxyCreationEnabled = false;
                    List<PlaceB> data = (from us in dbContext.Places
                                         select new PlaceB {
                                             ID = us.ID,
                                             CountryID = us.CountryID,
                                             NameGR = us.NameGR,
                                             NameEN = us.NameEN,
                                             Country = new CountryDTO { ID = us.Countries.ID, NameGR = us.Countries.NameGR, NameEN = us.Countries.NameEN }
                                         }).OrderBy(o => o.NameGR).ToList();
                    return data;
                }
                catch (Exception) { return null; }
            }
        }

        public List<PlaceB> GetPlaces(int recSkip, int recTake, string recFilter) {
            using (var dbContext = new OTERTConnStr()) {
                try {
                    dbContext.Configuration.ProxyCreationEnabled = false;
                    IQueryable<PlaceB> datatmp = (from us in dbContext.Places
                                                 select new PlaceB {
                                                     ID = us.ID,
                                                     CountryID = us.CountryID,
                                                     NameGR = us.NameGR,
                                                     NameEN = us.NameEN,
                                                     Country = new CountryDTO { ID = us.Countries.ID, NameGR = us.Countries.NameGR, NameEN = us.Countries.NameEN }
                                                 });
                    if (!string.IsNullOrEmpty(recFilter)) { datatmp = datatmp.Where(recFilter); }
                    List<PlaceB> data = datatmp.OrderBy(o => o.ID).Skip(recSkip).Take(recTake).ToList();
                    return data;
                }
                catch (Exception) { return null; }
            }
        }

    }

}