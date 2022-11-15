using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using OTERT.Model;
using OTERT_Entity;
using Telerik.Web.UI;

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
                                                     NameEN = us.Places.Countries.NameEN
                                                 },
                                                 CountryID = us.Places.CountryID
                                             }
                                         }).OrderBy(o => o.NameGR).ToList();
                    return data;
                }
                catch (Exception) { return null; }
            }
        }

        public EventB GetEventByID(int EventID) {
            using (var dbContext = new OTERTConnStr()) {
                try {
                    dbContext.Configuration.ProxyCreationEnabled = false;
                    EventB data = (from us in dbContext.Events
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
                                            },
                                            CountryID = us.Places.CountryID
                                        }
                                    }).Where(k => k.ID == EventID).FirstOrDefault();
                    return data;
                }
                catch (Exception) { return null; }
            }
        }

        public List<EventB> GetEventsForCountry(int CountryID) {
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
                                                 },
                                                 CountryID = us.Places.CountryID
                                             }
                                         }).Where(k => k.Place.CountryID == CountryID).OrderBy(o => o.NameGR).ToList();
                    return data;
                }
                catch (Exception) { return null; }
            }
        }

        public List<EventB> GetEventsForPlace(int PlaceID) {
            using (var dbContext = new OTERTConnStr()) {
                try {
                    dbContext.Configuration.ProxyCreationEnabled = false;
                    List<EventB> data = (from us in dbContext.Events
                                         select new EventB
                                         {
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
                                                 },
                                                 CountryID = us.Places.CountryID
                                             }
                                         }).Where(k => k.PlaceID == PlaceID).OrderBy(o => o.NameGR).ToList();
                    return data;
                }
                catch (Exception) { return null; }
            }
        }

        public List<EventB> GetGreekEvents() {
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
                                                 },
                                                 CountryID = us.Places.CountryID
                                             }
                                         }).Where(k => k.Place.CountryID == 1).OrderBy(o => o.NameGR).ToList();
                    return data;
                }
                catch (Exception) { return null; }
            }
        }

        public List<EventB> GetForeignEvents() {
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
                                                 },
                                                 CountryID = us.Places.CountryID
                                             }
                                         }).Where(k => k.Place.CountryID != 1).OrderBy(o => o.NameGR).ToList();
                    return data;
                }
                catch (Exception) { return null; }
            }
        }

        public List<EventB> GetEvents(int recSkip, int recTake, string recFilter, GridSortExpressionCollection gridSortExxpressions) {
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
                                                 },
                                                 CountryID = us.Places.CountryID
                                             }
                                         }).OrderBy(o => o.ID).Skip(recSkip).Take(recTake).ToList();
                    return data;
                }
                catch (Exception) { return null; }
            }
        }

    }

}