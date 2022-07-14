using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Web;
using OTERT.Model;
using OTERT_Entity;
using Telerik.Web.UI;

namespace OTERT.Controller {

    public class OrdersController {

        public int CountOrders(int orderType) {
            using (var dbContext = new OTERTConnStr()) {
                try {
                    return dbContext.Orders.Where(o => o.OrderTypeID == orderType).Count();
                }
                catch (Exception) { return -1; }
            }
        }

        public OrderB GetOrder(int orderID) {
            using (var dbContext = new OTERTConnStr()) {
                try {
                    dbContext.Configuration.ProxyCreationEnabled = false;
                    OrderB data = (from us in dbContext.Orders
                                         select new OrderB {
                                             ID = us.ID,
                                             OrderTypeID = us.OrderTypeID,
                                             OrderType = new OrderTypeDTO { ID = us.OrderTypes.ID, Name = us.OrderTypes.Name },
                                             RegNo = us.RegNo,
                                             InoiceProtocol = us.InoiceProtocol,
                                             Customer1ID = us.Customer1ID,
                                             Customer1 = new CustomerDTO {
                                                 ID = us.Customers1.ID,
                                                 CountryID = us.Customers1.CountryID,
                                                 NameGR = us.Customers1.NameGR,
                                                 NameEN = us.Customers1.NameEN,
                                                 NamedInvoiceGR = us.Customers1.NamedInvoiceGR,
                                                 NamedInvoiceEN = us.Customers1.NamedInvoiceEN,
                                                 ZIPCode = us.Customers1.ZIPCode,
                                                 CityGR = us.Customers1.CityGR,
                                                 CityEN = us.Customers1.CityEN,
                                                 ChargeTelephone = us.Customers1.ChargeTelephone,
                                                 Telephone1 = us.Customers1.Telephone1,
                                                 Telephone2 = us.Customers1.Telephone2,
                                                 FAX1 = us.Customers1.FAX1,
                                                 FAX2 = us.Customers1.FAX2,
                                                 Address1GR = us.Customers1.Address1GR,
                                                 Address1EN = us.Customers1.Address1EN,
                                                 Address2GR = us.Customers1.Address2GR,
                                                 Address2EN = us.Customers1.Address2EN,
                                                 ContactPersonGR = us.Customers1.ContactPersonGR,
                                                 ContactPersonEN = us.Customers1.ContactPersonEN,
                                                 CustomerTypeID = us.Customers1.CustomerTypeID,
                                                 LanguageID = us.Customers1.LanguageID,
                                                 Email = us.Customers1.Email,
                                                 URL = us.Customers1.URL,
                                                 AFM = us.Customers1.AFM,
                                                 DOY = us.Customers1.DOY,
                                                 SAPCode = us.Customers1.SAPCode,
                                                 UserID = us.Customers1.UserID,
                                                 Comments = us.Customers1.Comments,
                                                 IsProvider = us.Customers1.IsProvider,
                                                 IsOTE = us.Customers1.IsOTE
                                             },
                                             Customer2ID = us.Customer2ID,
                                             Customer2 = us.Customer2ID > 0 ? new CustomerDTO {
                                                 ID = us.Customers11.ID,
                                                 CountryID = us.Customers11.CountryID,
                                                 NameGR = us.Customers11.NameGR,
                                                 NameEN = us.Customers11.NameEN,
                                                 NamedInvoiceGR = us.Customers11.NamedInvoiceGR,
                                                 NamedInvoiceEN = us.Customers11.NamedInvoiceEN,
                                                 ZIPCode = us.Customers11.ZIPCode,
                                                 CityGR = us.Customers11.CityGR,
                                                 CityEN = us.Customers11.CityEN,
                                                 ChargeTelephone = us.Customers11.ChargeTelephone,
                                                 Telephone1 = us.Customers11.Telephone1,
                                                 Telephone2 = us.Customers11.Telephone2,
                                                 FAX1 = us.Customers11.FAX1,
                                                 FAX2 = us.Customers11.FAX2,
                                                 Address1GR = us.Customers11.Address1GR,
                                                 Address1EN = us.Customers11.Address1EN,
                                                 Address2GR = us.Customers11.Address2GR,
                                                 Address2EN = us.Customers11.Address2EN,
                                                 ContactPersonGR = us.Customers11.ContactPersonGR,
                                                 ContactPersonEN = us.Customers11.ContactPersonEN,
                                                 CustomerTypeID = us.Customers11.CustomerTypeID,
                                                 LanguageID = us.Customers11.LanguageID,
                                                 Email = us.Customers11.Email,
                                                 URL = us.Customers11.URL,
                                                 AFM = us.Customers11.AFM,
                                                 DOY = us.Customers11.DOY,
                                                 SAPCode = us.Customers11.SAPCode,
                                                 UserID = us.Customers11.UserID,
                                                 Comments = us.Customers11.Comments,
                                                 IsProvider = us.Customers11.IsProvider,
                                                 IsOTE = us.Customers11.IsOTE
                                             } : null,
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
                                             IsLocked = us.IsLocked == null ? false : (bool)us.IsLocked,
                                         }).Where(o => o.ID == orderID).FirstOrDefault();
                    return data;
                }
                catch (Exception) { return null; }
            }
        }

        public List<OrderB> GetOrders(int orderType) {
            using (var dbContext = new OTERTConnStr()) {
                try {
                    dbContext.Configuration.ProxyCreationEnabled = false;
                    List<OrderB> data = (from us in dbContext.Orders
                                         select new OrderB {
                                             ID = us.ID,
                                             OrderTypeID = us.OrderTypeID,
                                             OrderType = new OrderTypeDTO { ID = us.OrderTypes.ID, Name = us.OrderTypes.Name },
                                             RegNo = us.RegNo,
                                             InoiceProtocol = us.InoiceProtocol,
                                             Customer1ID = us.Customer1ID,
                                             Customer1 = new CustomerDTO { ID = us.Customers1.ID,
                                                                        CountryID = us.Customers1.CountryID,
                                                                        NameGR = us.Customers1.NameGR,
                                                                        NameEN = us.Customers1.NameEN,
                                                                        NamedInvoiceGR = us.Customers1.NamedInvoiceGR,
                                                                        NamedInvoiceEN = us.Customers1.NamedInvoiceEN,
                                                                        ZIPCode = us.Customers1.ZIPCode,
                                                                        CityGR = us.Customers1.CityGR,
                                                                        CityEN = us.Customers1.CityEN,
                                                                        ChargeTelephone = us.Customers1.ChargeTelephone,
                                                                        Telephone1 = us.Customers1.Telephone1,
                                                                        Telephone2 = us.Customers1.Telephone2,
                                                                        FAX1 = us.Customers1.FAX1,
                                                                        FAX2 = us.Customers1.FAX2,
                                                                        Address1GR = us.Customers1.Address1GR,
                                                                        Address1EN = us.Customers1.Address1EN,
                                                                        Address2GR = us.Customers1.Address2GR,
                                                                        Address2EN = us.Customers1.Address2EN,
                                                                        ContactPersonGR = us.Customers1.ContactPersonGR,
                                                                        ContactPersonEN = us.Customers1.ContactPersonEN,
                                                                        CustomerTypeID = us.Customers1.CustomerTypeID,
                                                                        LanguageID = us.Customers1.LanguageID,
                                                                        Email = us.Customers1.Email,
                                                                        URL = us.Customers1.URL,
                                                                        AFM = us.Customers1.AFM,
                                                                        DOY = us.Customers1.DOY,
                                                                        SAPCode = us.Customers1.SAPCode,
                                                                        UserID = us.Customers1.UserID,
                                                                        Comments = us.Customers1.Comments,
                                                                        IsProvider = us.Customers1.IsProvider,
                                                                        IsOTE = us.Customers1.IsOTE
                                             },
                                             Customer2ID = us.Customer2ID,
                                             Customer2 = us.Customer2ID > 0 ? new CustomerDTO {
                                                                                                ID = us.Customers11.ID,
                                                                                                CountryID = us.Customers11.CountryID,
                                                                                                NameGR = us.Customers11.NameGR,
                                                                                                NameEN = us.Customers11.NameEN,
                                                                                                NamedInvoiceGR = us.Customers11.NamedInvoiceGR,
                                                                                                NamedInvoiceEN = us.Customers11.NamedInvoiceEN,
                                                                                                ZIPCode = us.Customers11.ZIPCode,
                                                                                                CityGR = us.Customers11.CityGR,
                                                                                                CityEN = us.Customers11.CityEN,
                                                                                                ChargeTelephone = us.Customers11.ChargeTelephone,
                                                                                                Telephone1 = us.Customers11.Telephone1,
                                                                                                Telephone2 = us.Customers11.Telephone2,
                                                                                                FAX1 = us.Customers11.FAX1,
                                                                                                FAX2 = us.Customers11.FAX2,
                                                                                                Address1GR = us.Customers11.Address1GR,
                                                                                                Address1EN = us.Customers11.Address1EN,
                                                                                                Address2GR = us.Customers11.Address2GR,
                                                                                                Address2EN = us.Customers11.Address2EN,
                                                                                                ContactPersonGR = us.Customers11.ContactPersonGR,
                                                                                                ContactPersonEN = us.Customers11.ContactPersonEN,
                                                                                                CustomerTypeID = us.Customers11.CustomerTypeID,
                                                                                                LanguageID = us.Customers11.LanguageID,
                                                                                                Email = us.Customers11.Email,
                                                                                                URL = us.Customers11.URL,
                                                                                                AFM = us.Customers11.AFM,
                                                                                                DOY = us.Customers11.DOY,
                                                                                                SAPCode = us.Customers11.SAPCode,
                                                                                                UserID = us.Customers11.UserID,
                                                                                                Comments = us.Customers11.Comments,
                                                                                                IsProvider = us.Customers11.IsProvider,
                                                                                                IsOTE = us.Customers11.IsOTE
                                             } : null,
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
                                             IsLocked = us.IsLocked == null ? false : (bool)us.IsLocked,
                                         }).Where(o => o.OrderTypeID == orderType).OrderBy(o => o.ID).ToList();
                    return data;
                }
                catch (Exception) { return null; }
            }
        }

        public List<OrderB> GetOrders(int orderType, int recSkip, int recTake) {
            using (var dbContext = new OTERTConnStr()) {
                try {
                    dbContext.Configuration.ProxyCreationEnabled = false;
                    List<OrderB> data = (from us in dbContext.Orders
                                         select new OrderB {
                                             ID = us.ID,
                                             OrderTypeID = us.OrderTypeID,
                                             OrderType = new OrderTypeDTO { ID = us.OrderTypes.ID, Name = us.OrderTypes.Name },
                                             RegNo = us.RegNo,
                                             InoiceProtocol = us.InoiceProtocol,
                                             Customer1ID = us.Customer1ID,
                                             Customer1 = new CustomerDTO {
                                                                        ID = us.Customers1.ID,
                                                                        CountryID = us.Customers1.CountryID,
                                                                        NameGR = us.Customers1.NameGR,
                                                                        NameEN = us.Customers1.NameEN,
                                                                        NamedInvoiceGR = us.Customers1.NamedInvoiceGR,
                                                                        NamedInvoiceEN = us.Customers1.NamedInvoiceEN,
                                                                        ZIPCode = us.Customers1.ZIPCode,
                                                                        CityGR = us.Customers1.CityGR,
                                                                        CityEN = us.Customers1.CityEN,
                                                                        ChargeTelephone = us.Customers1.ChargeTelephone,
                                                                        Telephone1 = us.Customers1.Telephone1,
                                                                        Telephone2 = us.Customers1.Telephone2,
                                                                        FAX1 = us.Customers1.FAX1,
                                                                        FAX2 = us.Customers1.FAX2,
                                                                        Address1GR = us.Customers1.Address1GR,
                                                                        Address1EN = us.Customers1.Address1EN,
                                                                        Address2GR = us.Customers1.Address2GR,
                                                                        Address2EN = us.Customers1.Address2EN,
                                                                        ContactPersonGR = us.Customers1.ContactPersonGR,
                                                                        ContactPersonEN = us.Customers1.ContactPersonEN,
                                                                        CustomerTypeID = us.Customers1.CustomerTypeID,
                                                                        LanguageID = us.Customers1.LanguageID,
                                                                        Email = us.Customers1.Email,
                                                                        URL = us.Customers1.URL,
                                                                        AFM = us.Customers1.AFM,
                                                                        DOY = us.Customers1.DOY,
                                                                        SAPCode = us.Customers1.SAPCode,
                                                                        UserID = us.Customers1.UserID,
                                                                        Comments = us.Customers1.Comments,
                                                                        IsProvider = us.Customers1.IsProvider,
                                                                        IsOTE = us.Customers1.IsOTE
                                             },
                                             Customer2ID = us.Customer2ID,
                                             Customer2 = us.Customer2ID > 0 ? new CustomerDTO {
                                                                                                 ID = us.Customers11.ID,
                                                                                                 CountryID = us.Customers11.CountryID,
                                                                                                 NameGR = us.Customers11.NameGR,
                                                                                                 NameEN = us.Customers11.NameEN,
                                                                                                 NamedInvoiceGR = us.Customers11.NamedInvoiceGR,
                                                                                                 NamedInvoiceEN = us.Customers11.NamedInvoiceEN,
                                                                                                 ZIPCode = us.Customers11.ZIPCode,
                                                                                                 CityGR = us.Customers11.CityGR,
                                                                                                 CityEN = us.Customers11.CityEN,
                                                                                                 ChargeTelephone = us.Customers11.ChargeTelephone,
                                                                                                 Telephone1 = us.Customers11.Telephone1,
                                                                                                 Telephone2 = us.Customers11.Telephone2,
                                                                                                 FAX1 = us.Customers11.FAX1,
                                                                                                 FAX2 = us.Customers11.FAX2,
                                                                                                 Address1GR = us.Customers11.Address1GR,
                                                                                                 Address1EN = us.Customers11.Address1EN,
                                                                                                 Address2GR = us.Customers11.Address2GR,
                                                                                                 Address2EN = us.Customers11.Address2EN,
                                                                                                 ContactPersonGR = us.Customers11.ContactPersonGR,
                                                                                                 ContactPersonEN = us.Customers11.ContactPersonEN,
                                                                                                 CustomerTypeID = us.Customers11.CustomerTypeID,
                                                                                                 LanguageID = us.Customers11.LanguageID,
                                                                                                 Email = us.Customers11.Email,
                                                                                                 URL = us.Customers11.URL,
                                                                                                 AFM = us.Customers11.AFM,
                                                                                                 DOY = us.Customers11.DOY,
                                                                                                 SAPCode = us.Customers11.SAPCode,
                                                                                                 UserID = us.Customers11.UserID,
                                                                                                 Comments = us.Customers11.Comments,
                                                                                                 IsProvider = us.Customers11.IsProvider,
                                                                                                 IsOTE = us.Customers11.IsOTE
                                             } : null,
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
                                             IsLocked = us.IsLocked == null ? false : (bool)us.IsLocked,
                                         }).Where(o => o.OrderTypeID == orderType).OrderByDescending(o => o.ID).Skip(recSkip).Take(recTake).ToList();
                    return data;
                }
                catch (Exception) { return null; }
            }
        }

        public List<OrderB> GetOrders(int orderType, int recSkip, int recTake, string recFilter, GridSortExpressionCollection gridSortExxpressions) {
            using (var dbContext = new OTERTConnStr()) {
                try {
                    dbContext.Configuration.ProxyCreationEnabled = false;
                    IQueryable<OrderB> datatmp = (from us in dbContext.Orders
                                                  select new OrderB {
                                                      ID = us.ID,
                                                      OrderTypeID = us.OrderTypeID,
                                                      OrderType = new OrderTypeDTO { ID = us.OrderTypes.ID, Name = us.OrderTypes.Name },
                                                      RegNo = us.RegNo,
                                                      InoiceProtocol = us.InoiceProtocol,
                                                      Customer1ID = us.Customer1ID,
                                                      Customer1 = new CustomerDTO {
                                                          ID = us.Customers1.ID,
                                                          CountryID = us.Customers1.CountryID,
                                                          NameGR = us.Customers1.NameGR,
                                                          NameEN = us.Customers1.NameEN,
                                                          NamedInvoiceGR = us.Customers1.NamedInvoiceGR,
                                                          NamedInvoiceEN = us.Customers1.NamedInvoiceEN,
                                                          ZIPCode = us.Customers1.ZIPCode,
                                                          CityGR = us.Customers1.CityGR,
                                                          CityEN = us.Customers1.CityEN,
                                                          ChargeTelephone = us.Customers1.ChargeTelephone,
                                                          Telephone1 = us.Customers1.Telephone1,
                                                          Telephone2 = us.Customers1.Telephone2,
                                                          FAX1 = us.Customers1.FAX1,
                                                          FAX2 = us.Customers1.FAX2,
                                                          Address1GR = us.Customers1.Address1GR,
                                                          Address1EN = us.Customers1.Address1EN,
                                                          Address2GR = us.Customers1.Address2GR,
                                                          Address2EN = us.Customers1.Address2EN,
                                                          ContactPersonGR = us.Customers1.ContactPersonGR,
                                                          ContactPersonEN = us.Customers1.ContactPersonEN,
                                                          CustomerTypeID = us.Customers1.CustomerTypeID,
                                                          LanguageID = us.Customers1.LanguageID,
                                                          Email = us.Customers1.Email,
                                                          URL = us.Customers1.URL,
                                                          AFM = us.Customers1.AFM,
                                                          DOY = us.Customers1.DOY,
                                                          SAPCode = us.Customers1.SAPCode,
                                                          UserID = us.Customers1.UserID,
                                                          Comments = us.Customers1.Comments,
                                                          IsProvider = us.Customers1.IsProvider,
                                                          IsOTE = us.Customers1.IsOTE
                                                      },
                                                      Customer2ID = us.Customer2ID,
                                                      Customer2 = us.Customer2ID > 0 ? new CustomerDTO {
                                                          ID = us.Customers11.ID,
                                                          CountryID = us.Customers11.CountryID,
                                                          NameGR = us.Customers11.NameGR,
                                                          NameEN = us.Customers11.NameEN,
                                                          NamedInvoiceGR = us.Customers11.NamedInvoiceGR,
                                                          NamedInvoiceEN = us.Customers11.NamedInvoiceEN,
                                                          ZIPCode = us.Customers11.ZIPCode,
                                                          CityGR = us.Customers11.CityGR,
                                                          CityEN = us.Customers11.CityEN,
                                                          ChargeTelephone = us.Customers11.ChargeTelephone,
                                                          Telephone1 = us.Customers11.Telephone1,
                                                          Telephone2 = us.Customers11.Telephone2,
                                                          FAX1 = us.Customers11.FAX1,
                                                          FAX2 = us.Customers11.FAX2,
                                                          Address1GR = us.Customers11.Address1GR,
                                                          Address1EN = us.Customers11.Address1EN,
                                                          Address2GR = us.Customers11.Address2GR,
                                                          Address2EN = us.Customers11.Address2EN,
                                                          ContactPersonGR = us.Customers11.ContactPersonGR,
                                                          ContactPersonEN = us.Customers11.ContactPersonEN,
                                                          CustomerTypeID = us.Customers11.CustomerTypeID,
                                                          LanguageID = us.Customers11.LanguageID,
                                                          Email = us.Customers11.Email,
                                                          URL = us.Customers11.URL,
                                                          AFM = us.Customers11.AFM,
                                                          DOY = us.Customers11.DOY,
                                                          SAPCode = us.Customers11.SAPCode,
                                                          UserID = us.Customers11.UserID,
                                                          Comments = us.Customers11.Comments,
                                                          IsProvider = us.Customers11.IsProvider,
                                                          IsOTE = us.Customers11.IsOTE
                                                      } : null,
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
                                                      IsLocked = us.IsLocked == null ? false : (bool)us.IsLocked,
                                                  });
                    if (!string.IsNullOrEmpty(recFilter)) { datatmp = datatmp.Where(recFilter); }
                    if (gridSortExxpressions.Count > 0) {
                        string sortFieldName = "";
                        if (gridSortExxpressions[0].FieldName == "Customer1ID") { sortFieldName = "Customer1.NameGR"; }
                        else if (gridSortExxpressions[0].FieldName == "CountryID") { sortFieldName = "Event.Place.Country.NameGR"; }
                        else if (gridSortExxpressions[0].FieldName == "PlaceID") { sortFieldName = "Event.Place.NameGR"; }
                        else if (gridSortExxpressions[0].FieldName == "EventID") { sortFieldName = "Event.NameGR"; } 
                        else { sortFieldName = gridSortExxpressions[0].FieldName; }
                        datatmp = datatmp.OrderBy(sortFieldName + " " + gridSortExxpressions[0].SortOrder);
                    } else {
                        datatmp = datatmp.OrderByDescending(o => o.ID);
                    }
                    List<OrderB> data = datatmp.Skip(recSkip).Take(recTake).ToList();
                    return data;
                }
                catch (Exception) { return null; }
            }
        }

    }

}