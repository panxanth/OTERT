using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Web;
using OTERT.Model;
using OTERT_Entity;
using Telerik.Web.UI;

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

        public List<PlaceB> GetForeignPlaces() {
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
                                         }).Where(k => k.CountryID != 1).OrderBy(o => o.NameGR).ToList();
                    return data;
                }
                catch (Exception) { return null; }
            }
        }

        public List<PlaceB> GetPlacesForCountry(int CountryID) {
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
                                         }).Where(k => k.CountryID == CountryID).OrderBy(o => o.NameGR).ToList();
                    return data;
                }
                catch (Exception) { return null; }
            }
        }

        public List<PlaceB> GetPlaces(int recSkip, int recTake, string recFilter, GridSortExpressionCollection gridSortExxpressions) {
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
                    if (gridSortExxpressions.Count > 0) {
                        datatmp = datatmp.OrderBy(gridSortExxpressions[0].FieldName + " " + gridSortExxpressions[0].SortOrder);
                    } else {
                        datatmp = datatmp.OrderByDescending(o => o.ID);
                    }
                    List<PlaceB> data = datatmp.Skip(recSkip).Take(recTake).ToList();
                    return data;
                }
                catch (Exception) { return null; }
            }
        }

    }

}