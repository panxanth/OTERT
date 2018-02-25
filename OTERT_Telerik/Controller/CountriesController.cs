using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using OTERT_Telerik.Model;
using OTERT_Entity;

namespace OTERT_Telerik.Controller {

    public class CountriesController {

        public int CountCountries() {
            using (var dbContext = new OTERTConnStr()) {
                try {
                    return dbContext.Countries.Count();
                }
                catch (Exception ex) { return -1; }
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
                catch (Exception ex) { return null; }
            }
        }

        public List<CountryB> GetCountries(int recSkip, int recTake) {
            using (var dbContext = new OTERTConnStr()) {
                try {
                    dbContext.Configuration.ProxyCreationEnabled = false;
                    List<CountryB> data = (from us in dbContext.Countries
                                           select new CountryB {
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