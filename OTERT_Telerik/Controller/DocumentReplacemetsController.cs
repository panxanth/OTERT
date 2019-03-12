using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using OTERT.Model;
using OTERT_Entity;

namespace OTERT.Controller {

    public class DocumentReplacemetsController {

        public int CountDocumentReplacemets(string usw) {
            using (var dbContext = new OTERTConnStr()) {
                try {
                    return dbContext.DocumentReplacemets.Where(o => o.UniqueName.StartsWith(usw)).Count();
                }
                catch (Exception) { return -1; }
            }
        }

        public List<DocumentReplacemetB> GetDocumentReplacemets(string usw) {
            using (var dbContext = new OTERTConnStr()) {
                try {
                    dbContext.Configuration.ProxyCreationEnabled = false;
                    List<DocumentReplacemetB> data = (from us in dbContext.DocumentReplacemets.Where(o => o.UniqueName.StartsWith(usw))
                                                      select new DocumentReplacemetB {
                                                          ID = us.ID,
                                                          UniqueName = us.UniqueName,
                                                          Title = us.Title,
                                                          Text = us.Text
                                                      }).OrderBy(o => o.Title).ToList();
                    return data;
                }
                catch (Exception) { return null; }
            }
        }

        public List<DocumentReplacemetB> GetDocumentReplacemets(string usw, int recSkip, int recTake) {
            using (var dbContext = new OTERTConnStr()) {
                try {
                    dbContext.Configuration.ProxyCreationEnabled = false;
                    List<DocumentReplacemetB> data = (from us in dbContext.DocumentReplacemets.Where(o => o.UniqueName.StartsWith(usw))
                                                      select new DocumentReplacemetB {
                                                          ID = us.ID,
                                                          UniqueName = us.UniqueName,
                                                          Title = us.Title,
                                                          Text = us.Text
                                                      }).OrderBy(o => o.ID).Skip(recSkip).Take(recTake).ToList();
                    return data;
                }
                catch (Exception) { return null; }
            }
        }

    }

}