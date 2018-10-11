using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using OTERT.Model;
using OTERT_Entity;

namespace OTERT.Controller {

    public class OrdersController {

        public int CountOrders() {
            using (var dbContext = new OTERTConnStr()) {
                try {
                    return dbContext.Orders.Count();
                }
                catch (Exception) { return -1; }
            }
        }

        public List<OrderB> GetOrders() {
            using (var dbContext = new OTERTConnStr()) {
                try {
                    dbContext.Configuration.ProxyCreationEnabled = false;
                    List<OrderB> data = (from us in dbContext.Orders
                                         select new OrderB {
                                             ID = us.ID,
                                             OrderTypeID = us.OrderTypeID,
                                             OrderType = new OrderTypeDTO { ID = us.OrderTypes.ID, Name = us.OrderTypes.Name },
                                             RegNo = us.RegNo,
                                             Customer1ID = us.Customer1ID,
                                             Customer1 = new CustomerDTO { ID = us.Customers1.ID,
                                                                        CountryID = us.Customers1.CountryID,
                                                                        NameGR = us.Customers1.NameGR,
                                                                        NameEN = us.Customers1.NameEN,
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
                                                                        UserID = us.Customers1.UserID,
                                                                        Comments = us.Customers1.Comments,
                                                                        IsProvider = us.Customers1.IsProvider
                                             },
                                             Customer2ID = us.Customer2ID,
                                             Customer2 = new CustomerDTO {
                                                                         ID = us.Customers11.ID,
                                                                         CountryID = us.Customers11.CountryID,
                                                                         NameGR = us.Customers11.NameGR,
                                                                         NameEN = us.Customers11.NameEN,
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
                                                                         UserID = us.Customers11.UserID,
                                                                         Comments = us.Customers11.Comments,
                                                                         IsProvider = us.Customers11.IsProvider
                                             },
                                             EventID = us.EventID,
                                             Event = new EventDTO { ID = us.Events.ID, PlaceID = us.Events.PlaceID, NameGR = us.Events.NameGR, NameEN = us.Events.NameEN }
                                         }).OrderBy(o => o.ID).ToList();
                    return data;
                }
                catch (Exception) { return null; }
            }
        }

        public List<OrderB> GetOrders(int recSkip, int recTake) {
            using (var dbContext = new OTERTConnStr()) {
                try {
                    dbContext.Configuration.ProxyCreationEnabled = false;
                    List<OrderB> data = (from us in dbContext.Orders
                                         select new OrderB {
                                             ID = us.ID,
                                             OrderTypeID = us.OrderTypeID,
                                             OrderType = new OrderTypeDTO { ID = us.OrderTypes.ID, Name = us.OrderTypes.Name },
                                             RegNo = us.RegNo,
                                             Customer1ID = us.Customer1ID,
                                             Customer1 = new CustomerDTO {
                                                                        ID = us.Customers1.ID,
                                                                        CountryID = us.Customers1.CountryID,
                                                                        NameGR = us.Customers1.NameGR,
                                                                        NameEN = us.Customers1.NameEN,
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
                                                                        UserID = us.Customers1.UserID,
                                                                        Comments = us.Customers1.Comments,
                                                                        IsProvider = us.Customers1.IsProvider
                                             },
                                             Customer2ID = us.Customer2ID,
                                             Customer2 = new CustomerDTO {
                                                                         ID = us.Customers11.ID,
                                                                         CountryID = us.Customers11.CountryID,
                                                                         NameGR = us.Customers11.NameGR,
                                                                         NameEN = us.Customers11.NameEN,
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
                                                                         UserID = us.Customers11.UserID,
                                                                         Comments = us.Customers11.Comments,
                                                                         IsProvider = us.Customers11.IsProvider
                                             },
                                             EventID = us.EventID,
                                             Event = new EventDTO { ID = us.Events.ID, PlaceID = us.Events.PlaceID, NameGR = us.Events.NameGR, NameEN = us.Events.NameEN }
                                         }).OrderBy(o => o.ID).Skip(recSkip).Take(recTake).ToList();
                    return data;
                }
                catch (Exception) { return null; }
            }
        }

    }

}