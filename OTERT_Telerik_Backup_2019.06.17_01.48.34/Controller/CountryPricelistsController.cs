using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using OTERT.Model;
using OTERT_Entity;

namespace OTERT.Controller {

    public class CountryPricelistsController {

        public int CountCountryPricelists() {
            using (var dbContext = new OTERTConnStr()) {
                try {
                    return dbContext.CountryPricelist.Count();
                }
                catch (Exception) { return -1; }
            }
        }

        public int CountCountryPricelists(int countryID) {
            using (var dbContext = new OTERTConnStr()) {
                try {
                    return dbContext.CountryPricelist.Where(o => o.Customers.CountryID == countryID).Count();
                }
                catch (Exception) { return -1; }
            }
        }

        public CountryPricelistB GetCountryPricelist(int CountryPricelistID) {
            using (var dbContext = new OTERTConnStr()) {
                try {
                    dbContext.Configuration.ProxyCreationEnabled = false;
                    CountryPricelistB data = (from us in dbContext.CountryPricelist
                                             select new CountryPricelistB {
                                                 ID = us.ID,
                                                 CustomerID = us.CustomerID,
                                                 Customer = new CustomerDTO {
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
                                                 },
                                                 LineTypeID = us.LineTypeID,
                                                 LineType = new LineTypeDTO { ID = us.LineTypes.ID, Name = us.LineTypes.Name },
                                                 InstallationCost = us.InstallationCost,
                                                 MonthlyCharges = us.MonthlyCharges,
                                                 Internet = us.Internet,
                                                 MSN = us.MSN,
                                                 PaymentIsForWholeMonth = us.PaymentIsForWholeMonth
                                             }).Where(o => o.ID == CountryPricelistID).FirstOrDefault();
                    return data;
                }
                catch (Exception) { return null; }
            }
        }

        public CountryPricelistB GetCountryPricelist(int CustomerID, int LineTypeID) {
            using (var dbContext = new OTERTConnStr()) {
                try {
                    dbContext.Configuration.ProxyCreationEnabled = false;
                    CountryPricelistB data = (from us in dbContext.CountryPricelist
                                              select new CountryPricelistB {
                                                  ID = us.ID,
                                                  CustomerID = us.CustomerID,
                                                  Customer = new CustomerDTO {
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
                                                  },
                                                  LineTypeID = us.LineTypeID,
                                                  LineType = new LineTypeDTO { ID = us.LineTypes.ID, Name = us.LineTypes.Name },
                                                  InstallationCost = us.InstallationCost,
                                                  MonthlyCharges = us.MonthlyCharges,
                                                  Internet = us.Internet,
                                                  MSN = us.MSN,
                                                  PaymentIsForWholeMonth = us.PaymentIsForWholeMonth
                                              }).Where(o => o.CustomerID == CustomerID && o.LineTypeID == LineTypeID).FirstOrDefault();
                    return data;
                }
                catch (Exception) { return null; }
            }
        }

        public List<CountryPricelistB> GetCountryPricelists(int recSkip, int recTake) {
            using (var dbContext = new OTERTConnStr()) {
                try {
                    dbContext.Configuration.ProxyCreationEnabled = false;
                    List<CountryPricelistB> data = (from us in dbContext.CountryPricelist
                                                    select new CountryPricelistB {
                                                        ID = us.ID,
                                                        CustomerID = us.CustomerID,
                                                        Customer = new CustomerDTO {
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
                                                        },
                                                        LineTypeID = us.LineTypeID,
                                                        LineType = new LineTypeDTO { ID = us.LineTypes.ID, Name = us.LineTypes.Name },
                                                        InstallationCost = us.InstallationCost,
                                                        MonthlyCharges = us.MonthlyCharges,
                                                        Internet = us.Internet,
                                                        MSN = us.MSN,
                                                        PaymentIsForWholeMonth = us.PaymentIsForWholeMonth
                                                    }).OrderBy(o => o.Customer.NameGR).Skip(recSkip).Take(recTake).ToList();
                    return data;
                }
                catch (Exception) { return null; }
            }
        }

    }

}