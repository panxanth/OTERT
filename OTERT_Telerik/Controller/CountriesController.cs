using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Web;
using OTERT.Model;
using OTERT_Entity;

namespace OTERT.Controller {

    public class CountriesController {

        public int CountCountries(string recFilter) {
            using (var dbContext = new OTERTConnStr()) {
                try {
                    int count = 0;
                    dbContext.Configuration.ProxyCreationEnabled = false;
                    if (!string.IsNullOrEmpty(recFilter)) {
                        count = dbContext.Countries.Where(recFilter).Count();
                    } else {
                        count = dbContext.Countries.Count();
                    }
                    return count;
                }
                catch (Exception) { return -1; }
            }
        }

        public List<CountryB> GetCountries() {
            using (var dbContext = new OTERTConnStr()) {
                try {
                    dbContext.Configuration.ProxyCreationEnabled = false;
                    List<CountryB> data = (from us in dbContext.Countries
                                           select new CountryB {
                                               ID = us.ID,
                                               NameGR = us.NameGR,
                                               NameEN = us.NameEN
                                             }).OrderBy(o => o.NameGR).ToList();
                    return data;
                }
                catch (Exception) { return null; }
            }
        }

        public List<CountryB> GetCountries(int recSkip, int recTake, string recFilter) {
            using (var dbContext = new OTERTConnStr()) {
                try {
                    dbContext.Configuration.ProxyCreationEnabled = false;
                    IQueryable<CountryB> datatmp = (from us in dbContext.Countries
                                                   select new CountryB {
                                                        ID = us.ID,
                                                        NameGR = us.NameGR,
                                                        NameEN = us.NameEN
                                                   });
                    if (!string.IsNullOrEmpty(recFilter)) { datatmp = datatmp.Where(recFilter); }
                    List<CountryB> data = datatmp.OrderBy(o => o.ID).Skip(recSkip).Take(recTake).ToList();
                    return data;
                }
                catch (Exception) { return null; }
            }
        }

    }

}