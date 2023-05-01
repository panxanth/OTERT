using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Web;
using OTERT.Model;
using OTERT_Entity;
using Telerik.Web.UI;

namespace OTERT.Controller {

    public class OrdersPTSGR2Controller {

        public int CountOrders() {
            using (var dbContext = new OTERTConnStr()) {
                try {
                    return dbContext.OrdersPTSGR2.Count();
                }
                catch (Exception) { return -1; }
            }
        }
        
        public int CountOrdersByOrdersPTSGRID(int OrdersPTSGRID) {
            using (var dbContext = new OTERTConnStr()) {
                try {
                    return dbContext.OrdersPTSGR2.Where(k => k.OrdersPTSGRID == OrdersPTSGRID).Count();
                }
                catch (Exception) { return -1; }
            }
        }

        public List<OrderPTSGR2B> GetOrdersByOrdersPTSGRID(int OrdersPTSGRID) {
            using (var dbContext = new OTERTConnStr()) {
                try {
                    dbContext.Configuration.ProxyCreationEnabled = false;
                    List<OrderPTSGR2B> data = (from us in dbContext.OrdersPTSGR2
                                               select new OrderPTSGR2B {
                                                   ID = us.ID,
                                                   RegNo = us.RegNo,
                                                   InvoiceProtocol = us.InvoiceProtocol,
                                                   OrdersPTSGRID = us.OrdersPTSGRID,
                                                   OrderPTSGR = new OrderPTSGRDTO {
                                                       ID = us.OrdersPTSGR.ID,
                                                       EventID = us.OrdersPTSGR.EventID,
                                                       Event = new EventDTO {
                                                           NameGR = us.OrdersPTSGR.Events.NameGR,
                                                           NameEN = us.OrdersPTSGR.Events.NameEN,
                                                           PlaceID = us.OrdersPTSGR.Events.PlaceID,
                                                           Place = new PlaceDTO {
                                                               NameGR = us.OrdersPTSGR.Events.Places.NameGR,
                                                               NameEN = us.OrdersPTSGR.Events.Places.NameEN
                                                           }
                                                       }
                                                   },
                                                   CountryID = us.CountryID,
                                                   Country = new CountryDTO {
                                                       NameGR = us.Countries.NameGR,
                                                       NameEN = us.Countries.NameEN
                                                   },
                                                   ProviderID = us.ProviderID,
                                                   Provider = new CustomerDTO {
                                                       ID = us.Customers.ID,
                                                       CountryID = us.Customers.CountryID,
                                                       NameGR = us.Customers.NameGR,
                                                       NameEN = us.Customers.NameEN,
                                                       NamedInvoiceGR = us.Customers.NamedInvoiceGR,
                                                       NamedInvoiceEN = us.Customers.NamedInvoiceEN,
                                                       ZIPCode = us.Customers.ZIPCode,
                                                       CityGR = us.Customers.CityGR,
                                                       CityEN = us.Customers.CityEN,
                                                       ChargeTelephone = us.Customers.ChargeTelephone,
                                                       Telephone1 = us.Customers.Telephone1,
                                                       Telephone2 = us.Customers.Telephone2,
                                                       FAX1 = us.Customers.FAX1,
                                                       FAX2 = us.Customers.FAX2,
                                                       Address1GR = us.Customers.Address1GR,
                                                       Address1EN = us.Customers.Address1EN,
                                                       Address2GR = us.Customers.Address2GR,
                                                       Address2EN = us.Customers.Address2EN,
                                                       ContactPersonGR = us.Customers.ContactPersonGR,
                                                       ContactPersonEN = us.Customers.ContactPersonEN,
                                                       CustomerTypeID = us.Customers.CustomerTypeID,
                                                       LanguageID = us.Customers.LanguageID,
                                                       Email = us.Customers.Email,
                                                       URL = us.Customers.URL,
                                                       AFM = us.Customers.AFM,
                                                       DOY = us.Customers.DOY,
                                                       SAPCode = us.Customers.SAPCode,
                                                       UserID = us.Customers.UserID,
                                                       Comments = us.Customers.Comments,
                                                       IsProvider = us.Customers.IsProvider,
                                                       IsOTE = us.Customers.IsOTE
                                                   }
                                               }).Where(o => o.OrdersPTSGRID == OrdersPTSGRID).OrderBy(o => o.ID).ToList();
                    return data;
                }
                catch (Exception) { return null; }
            }
        }

        public OrderPTSGR2B GetOrder(int orderID) {
            using (var dbContext = new OTERTConnStr()) {
                try {
                    dbContext.Configuration.ProxyCreationEnabled = false;
                    OrderPTSGR2B data = (from us in dbContext.OrdersPTSGR2
                                         select new OrderPTSGR2B {
                                             ID = us.ID,
                                             RegNo = us.RegNo,
                                             InvoiceProtocol = us.InvoiceProtocol,
                                             OrdersPTSGRID = us.OrdersPTSGRID,
                                             OrderPTSGR = new OrderPTSGRDTO {
                                                 ID = us.OrdersPTSGR.ID,
                                                 EventID = us.OrdersPTSGR.EventID,
                                                 Event = new EventDTO {
                                                     NameGR = us.OrdersPTSGR.Events.NameGR,
                                                     NameEN = us.OrdersPTSGR.Events.NameEN,
                                                     PlaceID = us.OrdersPTSGR.Events.PlaceID,
                                                     Place = new PlaceDTO {
                                                         NameGR = us.OrdersPTSGR.Events.Places.NameGR,
                                                         NameEN = us.OrdersPTSGR.Events.Places.NameEN
                                                     }
                                                 }
                                             },
                                             CountryID = us.CountryID,
                                             Country = new CountryDTO {
                                                 NameGR = us.Countries.NameGR,
                                                 NameEN = us.Countries.NameEN
                                             },
                                             ProviderID = us.ProviderID,
                                             Provider = new CustomerDTO {
                                                 ID = us.Customers.ID,
                                                 CountryID = us.Customers.CountryID,
                                                 NameGR = us.Customers.NameGR,
                                                 NameEN = us.Customers.NameEN,
                                                 NamedInvoiceGR = us.Customers.NamedInvoiceGR,
                                                 NamedInvoiceEN = us.Customers.NamedInvoiceEN,
                                                 ZIPCode = us.Customers.ZIPCode,
                                                 CityGR = us.Customers.CityGR,
                                                 CityEN = us.Customers.CityEN,
                                                 ChargeTelephone = us.Customers.ChargeTelephone,
                                                 Telephone1 = us.Customers.Telephone1,
                                                 Telephone2 = us.Customers.Telephone2,
                                                 FAX1 = us.Customers.FAX1,
                                                 FAX2 = us.Customers.FAX2,
                                                 Address1GR = us.Customers.Address1GR,
                                                 Address1EN = us.Customers.Address1EN,
                                                 Address2GR = us.Customers.Address2GR,
                                                 Address2EN = us.Customers.Address2EN,
                                                 ContactPersonGR = us.Customers.ContactPersonGR,
                                                 ContactPersonEN = us.Customers.ContactPersonEN,
                                                 CustomerTypeID = us.Customers.CustomerTypeID,
                                                 LanguageID = us.Customers.LanguageID,
                                                 Email = us.Customers.Email,
                                                 URL = us.Customers.URL,
                                                 AFM = us.Customers.AFM,
                                                 DOY = us.Customers.DOY,
                                                 SAPCode = us.Customers.SAPCode,
                                                 UserID = us.Customers.UserID,
                                                 Comments = us.Customers.Comments,
                                                 IsProvider = us.Customers.IsProvider,
                                                 IsOTE = us.Customers.IsOTE
                                             }
                                         }).Where(o => o.ID == orderID).FirstOrDefault();
                    return data;
                }
                catch (Exception) { return null; }
            }
        }

        public List<OrderPTSGR2B> GetOrders() {
            using (var dbContext = new OTERTConnStr()) {
                try {
                    dbContext.Configuration.ProxyCreationEnabled = false;
                    List<OrderPTSGR2B> data = (from us in dbContext.OrdersPTSGR2
                                              select new OrderPTSGR2B {
                                                  ID = us.ID,
                                                  RegNo = us.RegNo,
                                                  InvoiceProtocol = us.InvoiceProtocol,
                                                  OrdersPTSGRID = us.OrdersPTSGRID,
                                                  OrderPTSGR = new OrderPTSGRDTO {
                                                      ID = us.OrdersPTSGR.ID,
                                                      EventID = us.OrdersPTSGR.EventID,
                                                      Event = new EventDTO {
                                                          NameGR = us.OrdersPTSGR.Events.NameGR,
                                                          NameEN = us.OrdersPTSGR.Events.NameEN,
                                                          PlaceID = us.OrdersPTSGR.Events.PlaceID,
                                                          Place = new PlaceDTO {
                                                              NameGR = us.OrdersPTSGR.Events.Places.NameGR,
                                                              NameEN = us.OrdersPTSGR.Events.Places.NameEN
                                                          }
                                                      }
                                                  },
                                                  CountryID = us.CountryID,
                                                  Country = new CountryDTO {
                                                      NameGR = us.Countries.NameGR,
                                                      NameEN = us.Countries.NameEN
                                                  },
                                                  ProviderID = us.ProviderID,
                                                  Provider = new CustomerDTO {
                                                      ID = us.Customers.ID,
                                                      CountryID = us.Customers.CountryID,
                                                      NameGR = us.Customers.NameGR,
                                                      NameEN = us.Customers.NameEN,
                                                      NamedInvoiceGR = us.Customers.NamedInvoiceGR,
                                                      NamedInvoiceEN = us.Customers.NamedInvoiceEN,
                                                      ZIPCode = us.Customers.ZIPCode,
                                                      CityGR = us.Customers.CityGR,
                                                      CityEN = us.Customers.CityEN,
                                                      ChargeTelephone = us.Customers.ChargeTelephone,
                                                      Telephone1 = us.Customers.Telephone1,
                                                      Telephone2 = us.Customers.Telephone2,
                                                      FAX1 = us.Customers.FAX1,
                                                      FAX2 = us.Customers.FAX2,
                                                      Address1GR = us.Customers.Address1GR,
                                                      Address1EN = us.Customers.Address1EN,
                                                      Address2GR = us.Customers.Address2GR,
                                                      Address2EN = us.Customers.Address2EN,
                                                      ContactPersonGR = us.Customers.ContactPersonGR,
                                                      ContactPersonEN = us.Customers.ContactPersonEN,
                                                      CustomerTypeID = us.Customers.CustomerTypeID,
                                                      LanguageID = us.Customers.LanguageID,
                                                      Email = us.Customers.Email,
                                                      URL = us.Customers.URL,
                                                      AFM = us.Customers.AFM,
                                                      DOY = us.Customers.DOY,
                                                      SAPCode = us.Customers.SAPCode,
                                                      UserID = us.Customers.UserID,
                                                      Comments = us.Customers.Comments,
                                                      IsProvider = us.Customers.IsProvider,
                                                      IsOTE = us.Customers.IsOTE
                                                  }
                                              }).OrderBy(o => o.ID).ToList();
                    return data;
                }
                catch (Exception) { return null; }
            }
        }

        public List<OrderPTSGR2B> GetOrders(int recSkip, int recTake) {
            using (var dbContext = new OTERTConnStr()) {
                try {
                    dbContext.Configuration.ProxyCreationEnabled = false;
                    List<OrderPTSGR2B> data = (from us in dbContext.OrdersPTSGR2
                                               select new OrderPTSGR2B {
                                                   ID = us.ID,
                                                   RegNo = us.RegNo,
                                                   InvoiceProtocol = us.InvoiceProtocol,
                                                   OrdersPTSGRID = us.OrdersPTSGRID,
                                                   OrderPTSGR = new OrderPTSGRDTO {
                                                       ID = us.OrdersPTSGR.ID,
                                                       EventID = us.OrdersPTSGR.EventID,
                                                       Event = new EventDTO {
                                                           NameGR = us.OrdersPTSGR.Events.NameGR,
                                                           NameEN = us.OrdersPTSGR.Events.NameEN,
                                                           PlaceID = us.OrdersPTSGR.Events.PlaceID,
                                                           Place = new PlaceDTO {
                                                               NameGR = us.OrdersPTSGR.Events.Places.NameGR,
                                                               NameEN = us.OrdersPTSGR.Events.Places.NameEN
                                                           }
                                                       }
                                                   },
                                                   CountryID = us.CountryID,
                                                   Country = new CountryDTO {
                                                       NameGR = us.Countries.NameGR,
                                                       NameEN = us.Countries.NameEN
                                                   },
                                                   ProviderID = us.ProviderID,
                                                   Provider = new CustomerDTO {
                                                       ID = us.Customers.ID,
                                                       CountryID = us.Customers.CountryID,
                                                       NameGR = us.Customers.NameGR,
                                                       NameEN = us.Customers.NameEN,
                                                       NamedInvoiceGR = us.Customers.NamedInvoiceGR,
                                                       NamedInvoiceEN = us.Customers.NamedInvoiceEN,
                                                       ZIPCode = us.Customers.ZIPCode,
                                                       CityGR = us.Customers.CityGR,
                                                       CityEN = us.Customers.CityEN,
                                                       ChargeTelephone = us.Customers.ChargeTelephone,
                                                       Telephone1 = us.Customers.Telephone1,
                                                       Telephone2 = us.Customers.Telephone2,
                                                       FAX1 = us.Customers.FAX1,
                                                       FAX2 = us.Customers.FAX2,
                                                       Address1GR = us.Customers.Address1GR,
                                                       Address1EN = us.Customers.Address1EN,
                                                       Address2GR = us.Customers.Address2GR,
                                                       Address2EN = us.Customers.Address2EN,
                                                       ContactPersonGR = us.Customers.ContactPersonGR,
                                                       ContactPersonEN = us.Customers.ContactPersonEN,
                                                       CustomerTypeID = us.Customers.CustomerTypeID,
                                                       LanguageID = us.Customers.LanguageID,
                                                       Email = us.Customers.Email,
                                                       URL = us.Customers.URL,
                                                       AFM = us.Customers.AFM,
                                                       DOY = us.Customers.DOY,
                                                       SAPCode = us.Customers.SAPCode,
                                                       UserID = us.Customers.UserID,
                                                       Comments = us.Customers.Comments,
                                                       IsProvider = us.Customers.IsProvider,
                                                       IsOTE = us.Customers.IsOTE
                                                   }
                                               }).OrderByDescending(o => o.ID).Skip(recSkip).Take(recTake).ToList();
                    return data;
                }
                catch (Exception) { return null; }
            }
        }

        public List<OrderPTSGR2B> GetOrders(int recSkip, int recTake, string recFilter, GridSortExpressionCollection gridSortExxpressions) {
            using (var dbContext = new OTERTConnStr()) {
                try {
                    dbContext.Configuration.ProxyCreationEnabled = false;
                    IQueryable<OrderPTSGR2B> datatmp = (from us in dbContext.OrdersPTSGR2
                                                        select new OrderPTSGR2B {
                                                            ID = us.ID,
                                                            RegNo = us.RegNo,
                                                            InvoiceProtocol = us.InvoiceProtocol,
                                                            OrdersPTSGRID = us.OrdersPTSGRID,
                                                            OrderPTSGR = new OrderPTSGRDTO {
                                                                ID = us.OrdersPTSGR.ID,
                                                                EventID = us.OrdersPTSGR.EventID,
                                                                Event = new EventDTO {
                                                                    NameGR = us.OrdersPTSGR.Events.NameGR,
                                                                    NameEN = us.OrdersPTSGR.Events.NameEN,
                                                                    PlaceID = us.OrdersPTSGR.Events.PlaceID,
                                                                    Place = new PlaceDTO {
                                                                        NameGR = us.OrdersPTSGR.Events.Places.NameGR,
                                                                        NameEN = us.OrdersPTSGR.Events.Places.NameEN
                                                                    }
                                                                }
                                                            },
                                                            CountryID = us.CountryID,
                                                            Country = new CountryDTO {
                                                                NameGR = us.Countries.NameGR,
                                                                NameEN = us.Countries.NameEN
                                                            },
                                                            ProviderID = us.ProviderID,
                                                            Provider = new CustomerDTO {
                                                                ID = us.Customers.ID,
                                                                CountryID = us.Customers.CountryID,
                                                                NameGR = us.Customers.NameGR,
                                                                NameEN = us.Customers.NameEN,
                                                                NamedInvoiceGR = us.Customers.NamedInvoiceGR,
                                                                NamedInvoiceEN = us.Customers.NamedInvoiceEN,
                                                                ZIPCode = us.Customers.ZIPCode,
                                                                CityGR = us.Customers.CityGR,
                                                                CityEN = us.Customers.CityEN,
                                                                ChargeTelephone = us.Customers.ChargeTelephone,
                                                                Telephone1 = us.Customers.Telephone1,
                                                                Telephone2 = us.Customers.Telephone2,
                                                                FAX1 = us.Customers.FAX1,
                                                                FAX2 = us.Customers.FAX2,
                                                                Address1GR = us.Customers.Address1GR,
                                                                Address1EN = us.Customers.Address1EN,
                                                                Address2GR = us.Customers.Address2GR,
                                                                Address2EN = us.Customers.Address2EN,
                                                                ContactPersonGR = us.Customers.ContactPersonGR,
                                                                ContactPersonEN = us.Customers.ContactPersonEN,
                                                                CustomerTypeID = us.Customers.CustomerTypeID,
                                                                LanguageID = us.Customers.LanguageID,
                                                                Email = us.Customers.Email,
                                                                URL = us.Customers.URL,
                                                                AFM = us.Customers.AFM,
                                                                DOY = us.Customers.DOY,
                                                                SAPCode = us.Customers.SAPCode,
                                                                UserID = us.Customers.UserID,
                                                                Comments = us.Customers.Comments,
                                                                IsProvider = us.Customers.IsProvider,
                                                                IsOTE = us.Customers.IsOTE
                                                            }
                                                        });
                    /*
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
                                    recFilter = "DateTimeStart >= @0 AND DateTimeEnd <= @1";
                                } else {
                                    recFilter = "DateTimeStart < @0 OR DateTimeEnd > @1";
                                }
                                datatmp = datatmp.Where(recFilter, new DateTime(orderDates[0].Year, orderDates[0].Month, orderDates[0].Day, orderDates[0].Hour, orderDates[0].Minute, 0), new DateTime(orderDates[1].Year, orderDates[1].Month, orderDates[1].Day, orderDates[1].Hour, orderDates[1].Minute, 0));
                            } else {
                                datatmp = datatmp.Where(DateTimeStartExpressions[0]);
                            }
                        }
                        
                        
                    }
                    */
                    //if (gridSortExxpressions.Count > 0) {
                    //    string sortFieldName = "";
                    //    if (gridSortExxpressions[0].FieldName == "PlaceID") { sortFieldName = "Event.Place.NameGR"; }
                    //    else if (gridSortExxpressions[0].FieldName == "EventID") { sortFieldName = "Event.NameGR"; } 
                    //    else { sortFieldName = gridSortExxpressions[0].FieldName; }
                    //    datatmp = datatmp.OrderBy(sortFieldName + " " + gridSortExxpressions[0].SortOrder);
                    //} else {
                        datatmp = datatmp.OrderByDescending(o => o.ID);
                    //}
                    List<OrderPTSGR2B> data = datatmp.Skip(recSkip).Take(recTake).ToList();
                    return data;
                }
                catch (Exception) { return null; }
            }
        }

    }

}