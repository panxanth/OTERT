using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using OTERT.Model;
using OTERT_Entity;

namespace OTERT.Controller {

    public class EventsController {

        public int CountEvents() {
            using (var dbContext = new OTERTConnStr()) {
                try {
                    return dbContext.Events.Count();
                }
                catch (Exception) { return -1; }
            }
        }

        public List<EventB> GetEvents() {
            using (var dbContext = new OTERTConnStr()) {
                try {
                    dbContext.Configuration.ProxyCreationEnabled = false;
                    List<EventB> data = (from us in dbContext.Events
                                         select new EventB {
                                             ID = us.ID,
                                             PlaceID = us.PlaceID,
                                             NameGR = us.NameGR,
                                             NameEN = us.NameEN,
                                             Place = new PlaceDTO {
                                                 ID = us.Places.ID,
                                                 NameGR = us.Places.NameGR,
                                                 NameEN = us.Places.NameEN,
                                                 Country = new CountryDTO {
                                                     ID = us.Places.Countries.ID,
                                                     NameGR = us.Places.Countries.NameGR,
                                                     NameEN = us.Places.Countries.NameEN }
                                             }
                                         }).OrderBy(o => o.NameGR).ToList();
                    return data;
                }
                catch (Exception) { return null; }
            }
        }

        public List<EventB> GetEvents(int recSkip, int recTake) {
            using (var dbContext = new OTERTConnStr()) {
                try {
                    dbContext.Configuration.ProxyCreationEnabled = false;
                    List<EventB> data = (from us in dbContext.Events
                                         select new EventB {
                                             ID = us.ID,
                                             PlaceID = us.PlaceID,
                                             NameGR = us.NameGR,
                                             NameEN = us.NameEN,
                                             Place = new PlaceDTO {
                                                 ID = us.Places.ID,
                                                 NameGR = us.Places.NameGR,
                                                 NameEN = us.Places.NameEN,
                                                 Country = new CountryDTO {
                                                     ID = us.Places.Countries.ID,
                                                     NameGR = us.Places.Countries.NameGR,
                                                     NameEN = us.Places.Countries.NameEN
                                                 }
                                             }
                                         }).OrderBy(o => o.ID).Skip(recSkip).Take(recTake).ToList();
                    return data;
                }
                catch (Exception) { return null; }
            }
        }

    }

}