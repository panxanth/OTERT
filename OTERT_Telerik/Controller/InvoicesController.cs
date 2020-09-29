using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Web;
using Telerik.Web.UI;
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

        public int CountInvoices(string recFilter) {
            using (var dbContext = new OTERTConnStr()) {
                try {
                    System.Globalization.DateTimeFormatInfo greek = new System.Globalization.CultureInfo("el-GR").DateTimeFormat;
                    int count = 0;
                    dbContext.Configuration.ProxyCreationEnabled = false;
                    if (!string.IsNullOrEmpty(recFilter)) {
                        IQueryable test = dbContext.Invoices;
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
                        List<string> DateFromExpressions = columnExpressions.Where(item => item.Contains("DateFrom")).ToList();
                        List<string> DateToExpressions = columnExpressions.Where(item => item.Contains("DateTo")).ToList();
                        columnExpressions.RemoveAll(item => item.Contains("DateFrom") || item.Contains("DateTo"));
                        recFilter = string.Join("AND", columnExpressions.ToArray());
                        if (DateFromExpressions.Count > 0) {
                            List<DateTime> dateFromDates = new List<DateTime>();
                            foreach (string dtExpression in DateFromExpressions) {
                                char testChar = '"';
                                if (dtExpression.Contains("'")) { testChar = '\''; }
                                string[] dateExp = dtExpression.Split(new char[] { testChar });
                                string format = "d/M/yyyy,h:mm:ss,tt";
                                DateTime newDate;
                                if (dateExp.Length > 1) {
                                    if (DateTime.TryParseExact(dateExp[1], format, greek, System.Globalization.DateTimeStyles.None, out newDate)) {
                                        dateFromDates.Add(newDate);
                                    }
                                }
                            }
                            if (dateFromDates.Count == 2) {
                                if (!string.IsNullOrEmpty(recFilter)) { recFilter += " AND "; }
                                if (DateFromExpressions[0].Contains(">=")) {
                                    recFilter = "DateFrom >= @0 AND DateFrom <= @1";
                                } else {
                                    recFilter = "DateFrom < @0 OR DateFrom > @1";
                                }
                                test = test.Where(recFilter, new DateTime(dateFromDates[0].Year, dateFromDates[0].Month, dateFromDates[0].Day, dateFromDates[0].Hour, dateFromDates[0].Minute, 0), new DateTime(dateFromDates[1].Year, dateFromDates[1].Month, dateFromDates[1].Day, dateFromDates[1].Hour, dateFromDates[1].Minute, 0));
                            } else {
                                test = test.Where(DateFromExpressions[0]);
                            }
                        }
                        if (DateToExpressions.Count > 0) {
                            List<DateTime> dateToDates = new List<DateTime>();
                            foreach (string dtExpression in DateToExpressions) {
                                char testChar = '"';
                                if (dtExpression.Contains("'")) { testChar = '\''; }
                                string[] dateExp = dtExpression.Split(new char[] { testChar });
                                string format = "d/M/yyyy,h:mm:ss,tt";
                                DateTime newDate;
                                if (dateExp.Length > 1) {
                                    if (DateTime.TryParseExact(dateExp[1], format, greek, System.Globalization.DateTimeStyles.None, out newDate)) {
                                        dateToDates.Add(newDate);
                                    }
                                }
                            }
                            if (dateToDates.Count == 2) {
                                if (!string.IsNullOrEmpty(recFilter)) { recFilter += " AND "; }
                                if (DateToExpressions[0].Contains(">=")) {
                                    recFilter = "DateTo >= @0 AND DateTo <= @1";
                                } else {
                                    recFilter = "DateTo < @0 OR DateTo > @1";
                                }
                                test = test.Where(recFilter, new DateTime(dateToDates[0].Year, dateToDates[0].Month, dateToDates[0].Day, dateToDates[0].Hour, dateToDates[0].Minute, 0), new DateTime(dateToDates[1].Year, dateToDates[1].Month, dateToDates[1].Day, dateToDates[1].Hour, dateToDates[1].Minute, 0));
                            } else {
                                test = test.Where(DateToExpressions[0]);
                            }
                        }
                        count = test.Count();
                    } else {
                        count = dbContext.Invoices.Count();
                    }
                    return count;
                }
                catch (Exception ex) { return -1; }
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

        public List<InvoiceB> GetInvoices(int recSkip, int recTake, string recFilter, GridSortExpressionCollection gridSortExxpressions) {
            using (var dbContext = new OTERTConnStr()) {
                try {
                    dbContext.Configuration.ProxyCreationEnabled = false;
                    IQueryable<InvoiceB> datatmp = (from us in dbContext.Invoices
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
                    if (!string.IsNullOrEmpty(recFilter)) {
                        IQueryable test = dbContext.Invoices;
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
                        List<string> DateFromExpressions = columnExpressions.Where(item => item.Contains("DateFrom")).ToList();
                        List<string> DateToExpressions = columnExpressions.Where(item => item.Contains("DateTo")).ToList();
                        columnExpressions.RemoveAll(item => item.Contains("DateFrom") || item.Contains("DateTo"));
                        recFilter = string.Join("AND", columnExpressions.ToArray());
                        if (!string.IsNullOrEmpty(recFilter)) { 
                            datatmp = datatmp.Where(recFilter.Replace("[","").Replace("]", "")); 
                        }
                        System.Globalization.DateTimeFormatInfo greek = new System.Globalization.CultureInfo("el-GR").DateTimeFormat;
                        if (DateFromExpressions.Count > 0) {
                            List<DateTime> dateFromDates = new List<DateTime>();
                            foreach (string dtExpression in DateFromExpressions) {
                                char testChar = '"';
                                if (dtExpression.Contains("'")) { testChar = '\''; }
                                string[] dateExp = dtExpression.Split(new char[] { testChar }); ;
                                string format = "d/M/yyyy,h:mm:ss,tt";
                                DateTime newDate;
                                if (dateExp.Length > 1) {
                                    if (DateTime.TryParseExact(dateExp[1], format, greek, System.Globalization.DateTimeStyles.None, out newDate)) {
                                        dateFromDates.Add(newDate);
                                    }
                                }
                            }
                            if (dateFromDates.Count == 2) {
                                if (!string.IsNullOrEmpty(recFilter)) { recFilter += " AND "; }
                                if (DateFromExpressions[0].Contains(">=")) {
                                    recFilter = "DateFrom >= @0 AND DateFrom <= @1";
                                } else {
                                    recFilter = "DateFrom < @0 OR DateFrom > @1";
                                }
                                test = test.Where(recFilter, new DateTime(dateFromDates[0].Year, dateFromDates[0].Month, dateFromDates[0].Day, dateFromDates[0].Hour, dateFromDates[0].Minute, 0), new DateTime(dateFromDates[1].Year, dateFromDates[1].Month, dateFromDates[1].Day, dateFromDates[1].Hour, dateFromDates[1].Minute, 0));
                            } else {
                                test = test.Where(DateFromExpressions[0]);
                            }
                        }
                        if (DateToExpressions.Count > 0) {
                            List<DateTime> dateToDates = new List<DateTime>();
                            foreach (string dtExpression in DateToExpressions) {
                                char testChar = '"';
                                if (dtExpression.Contains("'")) { testChar = '\''; }
                                string[] dateExp = dtExpression.Split(new char[] { testChar });
                                string format = "d/M/yyyy,h:mm:ss,tt";
                                DateTime newDate;
                                if (dateExp.Length > 1) {
                                    if (DateTime.TryParseExact(dateExp[1], format, greek, System.Globalization.DateTimeStyles.None, out newDate)) {
                                        dateToDates.Add(newDate);
                                    }
                                }
                            }
                            if (dateToDates.Count == 2) {
                                if (!string.IsNullOrEmpty(recFilter)) { recFilter += " AND "; }
                                if (DateToExpressions[0].Contains(">=")) {
                                    recFilter = "DateTo >= @0 AND DateTo <= @1";
                                } else {
                                    recFilter = "DateTo < @0 OR DateTo > @1";
                                }
                                test = test.Where(recFilter, new DateTime(dateToDates[0].Year, dateToDates[0].Month, dateToDates[0].Day, dateToDates[0].Hour, dateToDates[0].Minute, 0), new DateTime(dateToDates[1].Year, dateToDates[1].Month, dateToDates[1].Day, dateToDates[1].Hour, dateToDates[1].Minute, 0));
                            } else {
                                test = test.Where(DateToExpressions[0]);
                            }
                        }
                    }
                    if (gridSortExxpressions.Count > 0) {
                        string sortFieldName = "";
                        if (gridSortExxpressions[0].FieldName == "CustomerID") { sortFieldName = "Customer.NameGR"; } else { sortFieldName = gridSortExxpressions[0].FieldName; }
                        datatmp = datatmp.OrderBy(sortFieldName + " " + gridSortExxpressions[0].SortOrder);
                    } else {
                        datatmp = datatmp.OrderByDescending(o => o.ID);
                    }
                    List<InvoiceB> data = datatmp.Skip(recSkip).Take(recTake).ToList();
                    return data;
                }
                catch (Exception ex) { return null; }
            }
        }

    }

}