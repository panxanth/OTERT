using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using OTERT.Model;
using OTERT_Entity;

namespace OTERT.Controller {

    public class LanguagesController {

        public int CountLanguages() {
            using (var dbContext = new OTERTConnStr()) {
                try {
                    return dbContext.Languages.Count();
                }
                catch (Exception ex) { return -1; }
            }
        }

        public List<LanguageB> GetLanguages() {
            using (var dbContext = new OTERTConnStr()) {
                try {
                    dbContext.Configuration.ProxyCreationEnabled = false;
                    List<LanguageB> data = (from us in dbContext.Languages
                                            select new LanguageB {
                                               ID = us.ID,
                                               Name = us.Name,
                                               Code = us.Code
                                             }).OrderBy(o => o.Name).ToList();
                    return data;
                }
                catch (Exception ex) { return null; }
            }
        }

        public List<LanguageB> GetLanguages(int recSkip, int recTake) {
            using (var dbContext = new OTERTConnStr()) {
                try {
                    dbContext.Configuration.ProxyCreationEnabled = false;
                    List<LanguageB> data = (from us in dbContext.Languages
                                            select new LanguageB {
                                                ID = us.ID,
                                                Name = us.Name,
                                                Code = us.Code
                                            }).OrderBy(o => o.ID).Skip(recSkip).Take(recTake).ToList();
                    return data;
                }
                catch (Exception ex) { return null; }
            }
        }

    }

}