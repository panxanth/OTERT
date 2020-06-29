using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using OTERT.Model;
using OTERT_Entity;

namespace OTERT.Controller {

    public class InvoicesController {

        public int CountInvoices() {
            using (var dbContext = new OTERTConnStr()) {
                try {
                    return dbContext.Invoices.Count();
                }
                catch (Exception) { return -1; }
            }
        }

        public List<InvoiceB> GetInvoices() {
            using (var dbContext = new OTERTConnStr()) {
                try {
                    dbContext.Configuration.ProxyCreationEnabled = false;
                    List<InvoiceB> data = (from us in dbContext.Invoices
                                           select new InvoiceB{
                                                ID = us.ID,
                                                CustomerID = us.CustomerID,
                                                Customer = new CustomerDTO{
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
                                                DateFrom = us.DateFrom,
                                                DateTo = us.DateTo,
                                                DateCreated = us.DateCreated,
                                                RegNo = us.RegNo,
                                                DatePaid = us.DatePaid,
                                                TasksLineAmount = us.TasksLineAmount,
                                                DiscountLineAmount = us.DiscountLineAmount,
                                                IsLocked = us.IsLocked
                                           }).OrderByDescending(o => o.ID).ToList();
                    return data;
                }
                catch (Exception) { return null; }
            }
        }

        public List<InvoiceB> GetInvoices(int customerID, DateTime dateFrom, DateTime dateTo, string regNo) {
            using (var dbContext = new OTERTConnStr()) {
                try {
                    dbContext.Configuration.ProxyCreationEnabled = false;
                    IQueryable<InvoiceB> data = (from us in dbContext.Invoices
                                                   select new InvoiceB {
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
                                                       DateFrom = us.DateFrom,
                                                       DateTo = us.DateTo,
                                                       DateCreated = us.DateCreated,
                                                       RegNo = us.RegNo,
                                                       DatePaid = us.DatePaid,
                                                       TasksLineAmount = us.TasksLineAmount,
                                                       DiscountLineAmount = us.DiscountLineAmount,
                                                       IsLocked = us.IsLocked
                                                   });
                    DateTime nullDate = new DateTime(1900, 1, 1);
                    if (customerID != -1) { data = data.Where(o => o.CustomerID == customerID); }
                    if (dateFrom > nullDate) { data = data.Where(o => o.DateFrom >= dateFrom); }
                    if (dateTo > nullDate) { data = data.Where(o => o.DateTo <= dateTo); }
                    if (!string.IsNullOrEmpty(regNo.Trim())) { data = data.Where(o => o.RegNo == regNo); }
                    return data.OrderByDescending(o => o.ID).ToList();
                }
                catch (Exception) { return null; }
            }
        }

        public InvoiceB GetInvoice(int InvoiceID) {
            using (var dbContext = new OTERTConnStr()) {
                try {
                    dbContext.Configuration.ProxyCreationEnabled = false;
                    InvoiceB data = (from us in dbContext.Invoices
                                           select new InvoiceB {
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
                                               DateFrom = us.DateFrom,
                                               DateTo = us.DateTo,
                                               DateCreated = us.DateCreated,
                                               RegNo = us.RegNo,
                                               DatePaid = us.DatePaid,
                                               TasksLineAmount = us.TasksLineAmount,
                                               DiscountLineAmount = us.DiscountLineAmount,
                                               IsLocked = us.IsLocked
                                           }).Where(o => o.ID == InvoiceID).FirstOrDefault();
                    return data;
                }
                catch (Exception) { return null; }
            }
        }

    }

}