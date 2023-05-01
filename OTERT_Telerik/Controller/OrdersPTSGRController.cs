using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Web;
using OTERT.Model;
using OTERT_Entity;
using Telerik.Web.UI;

namespace OTERT.Controller {

    public class OrdersPTSGRController {

        public int CountOrders() {
            using (var dbContext = new OTERTConnStr()) {
                try {
                    return dbContext.OrdersPTSGR.Count();
                }
                catch (Exception) { return -1; }
            }
        }

        public OrderPTSGRB GetOrder(int orderID) {
            using (var dbContext = new OTERTConnStr()) {
                try {
                    dbContext.Configuration.ProxyCreationEnabled = false;
                    OrderPTSGRB data = (from us in dbContext.OrdersPTSGR
                                         select new OrderPTSGRB {
                                             ID = us.ID,
                                             EventID = us.EventID,
                                             Event = new EventDTO {
                                                 ID = us.Events.ID,
                                                 PlaceID = us.Events.PlaceID,
                                                 Place = new PlaceDTO {
                                                     ID = us.Events.PlaceID,
                                                     Country = new CountryDTO {
                                                         ID = us.Events.Places.CountryID,
                                                         NameGR = us.Events.Places.Countries.NameGR,
                                                         NameEN = us.Events.Places.Countries.NameEN
                                                     },
                                                     CountryID = us.Events.Places.CountryID,
                                                     NameGR = us.Events.Places.NameGR,
                                                     NameEN = us.Events.Places.NameEN
                                                 },
                                                 NameGR = us.Events.NameGR,
                                                 NameEN = us.Events.NameEN
                                             },
                                             DateTimeStart = us.DateTimeStart,
                                             DateTimeEnd = us.DateTimeEnd,
                                         }).Where(o => o.ID == orderID).FirstOrDefault();
                    return data;
                }
                catch (Exception) { return null; }
            }
        }

        public List<OrderPTSGRB> GetOrders(int orderType) {
            using (var dbContext = new OTERTConnStr()) {
                try {
                    dbContext.Configuration.ProxyCreationEnabled = false;
                    List<OrderPTSGRB> data = (from us in dbContext.OrdersPTSGR
                                              select new OrderPTSGRB {
                                              ID = us.ID,
                                              EventID = us.EventID,
                                              Event = new EventDTO {
                                                                 ID = us.Events.ID,
                                                                 PlaceID = us.Events.PlaceID,
                                                                 Place = new PlaceDTO {
                                                                                     ID = us.Events.PlaceID,
                                                                                     Country = new CountryDTO {
                                                                                                             ID = us.Events.Places.CountryID,
                                                                                                             NameGR = us.Events.Places.Countries.NameGR,
                                                                                                             NameEN = us.Events.Places.Countries.NameEN
                                                                                     },
                                                                                     CountryID = us.Events.Places.CountryID,
                                                                                     NameGR = us.Events.Places.NameGR,
                                                                                     NameEN = us.Events.Places.NameEN
                                                                 },
                                                                 NameGR = us.Events.NameGR,
                                                                 NameEN = us.Events.NameEN
                                              },
                                              DateTimeStart = us.DateTimeStart,
                                              DateTimeEnd = us.DateTimeEnd,
                                          }).OrderBy(o => o.ID).ToList();
                    return data;
                }
                catch (Exception) { return null; }
            }
        }

        public List<OrderPTSGRB> GetOrders(int orderType, int recSkip, int recTake) {
            using (var dbContext = new OTERTConnStr()) {
                try {
                    dbContext.Configuration.ProxyCreationEnabled = false;
                    List<OrderPTSGRB> data = (from us in dbContext.OrdersPTSGR
                                              select new OrderPTSGRB{
                                              ID = us.ID,
                                              EventID = us.EventID,
                                              Event = new EventDTO {
                                                                   ID = us.Events.ID,
                                                                   PlaceID = us.Events.PlaceID,
                                                                   Place = new PlaceDTO {
                                                                                         ID = us.Events.PlaceID,
                                                                                         Country = new CountryDTO {
                                                                                                                   ID = us.Events.Places.CountryID,
                                                                                                                   NameGR = us.Events.Places.Countries.NameGR,
                                                                                                                   NameEN = us.Events.Places.Countries.NameEN
                                                                                         },
                                                                                         CountryID = us.Events.Places.CountryID,
                                                                                         NameGR = us.Events.Places.NameGR,
                                                                                         NameEN = us.Events.Places.NameEN
                                                                   },
                                                                   NameGR = us.Events.NameGR,
                                                                   NameEN = us.Events.NameEN
                                              },
                                              DateTimeStart = us.DateTimeStart,
                                              DateTimeEnd = us.DateTimeEnd,
                                          }).OrderByDescending(o => o.ID).Skip(recSkip).Take(recTake).ToList();
                    return data;
                }
                catch (Exception) { return null; }
            }
        }

        public List<OrderPTSGRB> GetOrders(int orderType, int recSkip, int recTake, string recFilter, GridSortExpressionCollection gridSortExxpressions) {
            using (var dbContext = new OTERTConnStr()) {
                try {
                    dbContext.Configuration.ProxyCreationEnabled = false;
                    IQueryable<OrderPTSGRB> datatmp = (from us in dbContext.OrdersPTSGR
                                                       select new OrderPTSGRB {
                                                       ID = us.ID,
                                                       EventID = us.EventID,
                                                       Event = new EventDTO {
                                                          ID = us.Events.ID,
                                                          PlaceID = us.Events.PlaceID,
                                                          Place = new PlaceDTO {
                                                              ID = us.Events.PlaceID,
                                                              Country = new CountryDTO {
                                                                  ID = us.Events.Places.CountryID,
                                                                  NameGR = us.Events.Places.Countries.NameGR,
                                                                  NameEN = us.Events.Places.Countries.NameEN
                                                              },
                                                              CountryID = us.Events.Places.CountryID,
                                                              NameGR = us.Events.Places.NameGR,
                                                              NameEN = us.Events.Places.NameEN
                                                          },
                                                          NameGR = us.Events.NameGR,
                                                          NameEN = us.Events.NameEN
                                                       },
                                                       DateTimeStart = us.DateTimeStart,
                                                       DateTimeEnd = us.DateTimeEnd,
                                                   });
                    if (!string.IsNullOrEmpty(recFilter)) {
                        string[] expressionsAND = recFilter.Split(new string[] { "AND" }, StringSplitOptions.None);
                        List<string> columnExpressions = new List<string>();
                        for (int k = 0; k < expressionsAND.Length; k++) {
                            if (!expressionsAND[k].Contains("OR")) {
                                columnExpressions.Add(expressionsAND[k]);
                            } else {
                                string[] expressionsOR = expressionsAND[k].Split(new string[] { "OR" }, StringSplitOptions.None);
                                for (int i = 0; i < expressionsOR.Length; i++) { columnExpressions.Add(expressionsOR[i]); }
                            }
                        }
                        List<string> DateTimeStartExpressions = columnExpressions.Where(item => item.Contains("DateTimeStart")).ToList();
                        columnExpressions.RemoveAll(item => item.Contains("DateTimeStart"));
                        recFilter = string.Join("AND", columnExpressions.ToArray());
                        //if (!string.IsNullOrEmpty(recFilter)) { datatmp = datatmp.Where(recFilter); }
                        System.Globalization.DateTimeFormatInfo greek = new System.Globalization.CultureInfo("el-GR").DateTimeFormat;
                        if (DateTimeStartExpressions.Count > 0) {
                            List<DateTime> orderDates = new List<DateTime>();
                            foreach (string dtExpression in DateTimeStartExpressions) {
                                string[] dateExp = dtExpression.Split(new char[] { '"' });
                                string format = "d/M/yyyy,h:mm:ss,tt";
                                DateTime newDate;
                                if (dateExp.Length > 1) {
                                    if (DateTime.TryParseExact(dateExp[1], format, greek, System.Globalization.DateTimeStyles.None, out newDate)) {
                                        orderDates.Add(newDate);
                                    }
                                }
                            }
                            if (orderDates.Count == 2) {
                                if (!string.IsNullOrEmpty(recFilter)) { recFilter += " AND "; }
                                if (DateTimeStartExpressions[0].Contains(">=")) {
                                    recFilter = "DateTimeStart >= @0 AND DateTimeStart <= @1";
                                } else {
                                    recFilter = "DateTimeStart < @0 OR DateTimeStart > @1";
                                }
                                datatmp = datatmp.Where(recFilter, new DateTime(orderDates[0].Year, orderDates[0].Month, orderDates[0].Day, orderDates[0].Hour, orderDates[0].Minute, 0), new DateTime(orderDates[1].Year, orderDates[1].Month, orderDates[1].Day, orderDates[1].Hour, orderDates[1].Minute, 0));
                            } else {
                                datatmp = datatmp.Where(DateTimeStartExpressions[0]);
                            }
                        }
                        
                        
                    }
                    if (gridSortExxpressions.Count > 0) {
                        string sortFieldName = "";
                        if (gridSortExxpressions[0].FieldName == "PlaceID") { sortFieldName = "Event.Place.NameGR"; }
                        else if (gridSortExxpressions[0].FieldName == "EventID") { sortFieldName = "Event.NameGR"; } 
                        else { sortFieldName = gridSortExxpressions[0].FieldName; }
                        datatmp = datatmp.OrderBy(sortFieldName + " " + gridSortExxpressions[0].SortOrder);
                    } else {
                        datatmp = datatmp.OrderByDescending(o => o.ID);
                    }
                    List<OrderPTSGRB> data = datatmp.Skip(recSkip).Take(recTake).ToList();
                    return data;
                }
                catch (Exception) { return null; }
            }
        }

    }

}