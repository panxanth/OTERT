using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using OTERT.Model;
using OTERT_Entity;
using System.Data.Entity;
using Telerik.Web.UI;
using OTERT.WebServices;
using static System.Net.Mime.MediaTypeNames;

namespace OTERT.Controller {

    public class TasksPTSGRController {

        public int CountTasksPTSGR() {
            using (var dbContext = new OTERTConnStr()) {
                try {
                    return dbContext.TasksPTSGR.Count();
                }
                catch (Exception) { return -1; }
            }
        }

        public int CountAllTasksPTSGR(string recFilter) {
            using (var dbContext = new OTERTConnStr()) {
                try {
                    System.Globalization.DateTimeFormatInfo greek = new System.Globalization.CultureInfo("el-GR").DateTimeFormat;
                    int count = 0;
                    dbContext.Configuration.ProxyCreationEnabled = false;
                    if (!string.IsNullOrEmpty(recFilter)) {
                        IQueryable test = dbContext.TasksPTSGR;
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
                        List<string> OrderDateExpressions = columnExpressions.Where(item => item.Contains("OrderDate")).ToList();
                        List<string> StartActualExpressions = columnExpressions.Where(item => item.Contains("DateTimeStartActual")).ToList();
                        List<string> PaymentDateActualExpressions = columnExpressions.Where(item => item.Contains("PaymentDateActual")).ToList();
                        columnExpressions.RemoveAll(item => item.Contains("OrderDate") || item.Contains("DateTimeStartActual") || item.Contains("PaymentDateActual"));
                        recFilter = string.Join("AND", columnExpressions.ToArray());
                        if (!string.IsNullOrEmpty(recFilter)) {
                            recFilter = recFilter.Replace("Order.", "OrdersPTSGR2.");
                            recFilter = recFilter.Replace("OrderPTSGR.", "OrdersPTSGR.");
                            recFilter = recFilter.Replace("Event.", "Events.");
                            recFilter = recFilter.Replace("Place.", "Places.");
                            recFilter = recFilter.Replace("Customer.", "Customers.");
                            test = test.Where(recFilter);
                        }
                        if (OrderDateExpressions.Count > 0) {
                            List<DateTime> orderDates = new List<DateTime>();
                            foreach (string dtExpression in OrderDateExpressions) {
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
                                if (OrderDateExpressions[0].Contains(">=")) {
                                    recFilter = "OrderDate >= @0 AND OrderDate <= @1";
                                } else {
                                    recFilter = "OrderDate < @0 OR OrderDate > @1";
                                }
                                test = test.Where(recFilter, new DateTime(orderDates[0].Year, orderDates[0].Month, orderDates[0].Day, orderDates[0].Hour, orderDates[0].Minute, 0), new DateTime(orderDates[1].Year, orderDates[1].Month, orderDates[1].Day, orderDates[1].Hour, orderDates[1].Minute, 0));
                            } else {
                                test = test.Where(OrderDateExpressions[0]);
                            }
                        }
                        if (StartActualExpressions.Count > 0) {
                            List<DateTime> startActualDates = new List<DateTime>();
                            foreach (string dtExpression in StartActualExpressions) {
                                string[] dateExp = dtExpression.Split(new char[] { '"' });
                                string format = "d/M/yyyy,h:mm:ss,tt";
                                DateTime newDate;
                                if (dateExp.Length > 1) {
                                    if (DateTime.TryParseExact(dateExp[1], format, greek, System.Globalization.DateTimeStyles.None, out newDate)) {
                                        startActualDates.Add(newDate);
                                    }
                                }
                            }
                            if (startActualDates.Count == 2) {
                                if (!string.IsNullOrEmpty(recFilter)) { recFilter += " AND "; }
                                if (StartActualExpressions[0].Contains(">=")) {
                                    recFilter = "DateTimeStartActual >= @0 AND DateTimeStartActual <= @1";
                                } else {
                                    recFilter = "DateTimeStartActual < @0 OR DateTimeStartActual > @1";
                                }
                                test = test.Where(recFilter, new DateTime(startActualDates[0].Year, startActualDates[0].Month, startActualDates[0].Day, startActualDates[0].Hour, startActualDates[0].Minute, 0), new DateTime(startActualDates[1].Year, startActualDates[1].Month, startActualDates[1].Day, startActualDates[1].Hour, startActualDates[1].Minute, 0));
                            } else {
                                test = test.Where(StartActualExpressions[0]);
                            }
                        }
                        if (PaymentDateActualExpressions.Count > 0) {
                            List<DateTime> startOrderDates = new List<DateTime>();
                            foreach (string dtExpression in PaymentDateActualExpressions) {
                                string[] dateExp = dtExpression.Split(new char[] { '"' });
                                string format = "d/M/yyyy,h:mm:ss,tt";
                                DateTime newDate;
                                if (dateExp.Length > 1) {
                                    if (DateTime.TryParseExact(dateExp[1], format, greek, System.Globalization.DateTimeStyles.None, out newDate)) {
                                        startOrderDates.Add(newDate);
                                    }
                                }
                            }
                            if (startOrderDates.Count == 2) {
                                if (!string.IsNullOrEmpty(recFilter)) { recFilter += " AND "; }
                                if (PaymentDateActualExpressions[0].Contains(">=")) {
                                    recFilter = "PaymentDateActual >= @0 AND PaymentDateActual <= @1";
                                } else {
                                    recFilter = "PaymentDateActual < @0 OR PaymentDateActual > @1";
                                }
                                test = test.Where(recFilter, new DateTime(startOrderDates[0].Year, startOrderDates[0].Month, startOrderDates[0].Day, startOrderDates[0].Hour, startOrderDates[0].Minute, 0), new DateTime(startOrderDates[1].Year, startOrderDates[1].Month, startOrderDates[1].Day, startOrderDates[1].Hour, startOrderDates[1].Minute, 0));
                            } else {
                                test = test.Where(PaymentDateActualExpressions[0]);
                            }
                        }
                        count = test.Count();
                    } else {
                        count = dbContext.TasksPTSGR.Count();
                    }
                    return count;
                }
                catch (Exception) { return -1; }
            }
        }

        public TaskPTSGRB GetTaskPTSGR(int taskID) {
            using (var dbContext = new OTERTConnStr()) {
                try {
                    dbContext.Configuration.ProxyCreationEnabled = false;
                    TaskPTSGRB data = (from us in dbContext.TasksPTSGR
                                       select new TaskPTSGRB {
                                           ID = us.ID,
                                           OrderPTSGR2ID = us.OrderPTSGR2ID,
                                           Order = new OrderPTSGR2DTO {
                                               ID = us.OrdersPTSGR2.ID,
                                               RegNo = us.OrdersPTSGR2.RegNo,
                                               InvoiceProtocol = us.OrdersPTSGR2.InvoiceProtocol,
                                               OrdersPTSGRID = us.OrdersPTSGR2.OrdersPTSGRID,
                                               OrderPTSGR = new OrderPTSGRDTO {
                                                   ID = us.OrdersPTSGR2.OrdersPTSGR.ID,
                                                   EventID = us.OrdersPTSGR2.OrdersPTSGR.EventID,
                                                   Event = new EventDTO {
                                                       NameGR = us.OrdersPTSGR2.OrdersPTSGR.Events.NameGR,
                                                       NameEN = us.OrdersPTSGR2.OrdersPTSGR.Events.NameEN,
                                                       PlaceID = us.OrdersPTSGR2.OrdersPTSGR.Events.PlaceID,
                                                       Place = new PlaceDTO {
                                                           NameGR = us.OrdersPTSGR2.OrdersPTSGR.Events.Places.NameGR,
                                                           NameEN = us.OrdersPTSGR2.OrdersPTSGR.Events.Places.NameEN
                                                       }
                                                   }
                                               },
                                               CountryID = us.OrdersPTSGR2.CountryID,
                                               Country = new CountryDTO
                                               {
                                                   NameGR = us.OrdersPTSGR2.Countries.NameGR,
                                                   NameEN = us.OrdersPTSGR2.Countries.NameEN
                                               },
                                               ProviderID = us.OrdersPTSGR2.ProviderID,
                                               Provider = new CustomerDTO
                                               {
                                                   ID = us.OrdersPTSGR2.Customers.ID,
                                                   CountryID = us.OrdersPTSGR2.Customers.CountryID,
                                                   NameGR = us.OrdersPTSGR2.Customers.NameGR,
                                                   NameEN = us.OrdersPTSGR2.Customers.NameEN,
                                                   NamedInvoiceGR = us.OrdersPTSGR2.Customers.NamedInvoiceGR,
                                                   NamedInvoiceEN = us.OrdersPTSGR2.Customers.NamedInvoiceEN,
                                                   ZIPCode = us.OrdersPTSGR2.Customers.ZIPCode,
                                                   CityGR = us.OrdersPTSGR2.Customers.CityGR,
                                                   CityEN = us.OrdersPTSGR2.Customers.CityEN,
                                                   ChargeTelephone = us.OrdersPTSGR2.Customers.ChargeTelephone,
                                                   Telephone1 = us.OrdersPTSGR2.Customers.Telephone1,
                                                   Telephone2 = us.OrdersPTSGR2.Customers.Telephone2,
                                                   FAX1 = us.OrdersPTSGR2.Customers.FAX1,
                                                   FAX2 = us.OrdersPTSGR2.Customers.FAX2,
                                                   Address1GR = us.OrdersPTSGR2.Customers.Address1GR,
                                                   Address1EN = us.OrdersPTSGR2.Customers.Address1EN,
                                                   Address2GR = us.OrdersPTSGR2.Customers.Address2GR,
                                                   Address2EN = us.OrdersPTSGR2.Customers.Address2EN,
                                                   ContactPersonGR = us.OrdersPTSGR2.Customers.ContactPersonGR,
                                                   ContactPersonEN = us.OrdersPTSGR2.Customers.ContactPersonEN,
                                                   CustomerTypeID = us.OrdersPTSGR2.Customers.CustomerTypeID,
                                                   LanguageID = us.OrdersPTSGR2.Customers.LanguageID,
                                                   Email = us.OrdersPTSGR2.Customers.Email,
                                                   URL = us.OrdersPTSGR2.Customers.URL,
                                                   AFM = us.OrdersPTSGR2.Customers.AFM,
                                                   DOY = us.OrdersPTSGR2.Customers.DOY,
                                                   SAPCode = us.OrdersPTSGR2.Customers.SAPCode,
                                                   UserID = us.OrdersPTSGR2.Customers.UserID,
                                                   Comments = us.OrdersPTSGR2.Customers.Comments,
                                                   IsProvider = us.OrdersPTSGR2.Customers.IsProvider,
                                                   IsOTE = us.OrdersPTSGR2.Customers.IsOTE
                                               }
                                           },
                                           RegNo = us.RegNo,
                                           InvoiceProtocol = us.InvoiceProtocol,
                                           OrderDate = us.OrderDate,
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
                                           RequestedPositionID = us.RequestedPositionID,
                                           RequestedPosition = new RequestedPositionDTO {
                                               ID = us.RequestedPositionID == null ? -1 : us.RequestedPositions.ID,
                                               NameGR = us.RequestedPositionID == null ? "" : us.RequestedPositions.NameGR,
                                               NameEN = us.RequestedPositionID == null ? "" : us.RequestedPositions.NameEN
                                           },
                                           DateTimeStartActual = us.DateTimeStartActual,
                                           DateTimeEndActual = us.DateTimeEndActual,
                                           DateTimeDurationActual = us.DateTimeDurationActual,
                                           TelephoneNumber = us.TelephoneNumber,
                                           CallCharges = us.CallChardes,
                                           AddedCharges = us.AddedCharges,
                                           CostActual = us.CostActual,
                                           PaymentDateActual = us.PaymentDateActual,
                                           IsLocked = us.IsLocked == null ? false : (bool)us.IsLocked,
                                           IsCanceled = us.IsCanceled == null ? false : (bool)us.IsCanceled,
                                           Comments = us.Comments,
                                           CorrespondentName = us.CorrespondentName,
                                           MSNCount = us.MSNCount,
                                           MSN1 = us.MSN1,
                                           MSN2 = us.MSN2,
                                           PTSRPricelistID = us.PTSRPricelistID,
                                           PTSRPricelist = new PTSGRPricelistDTO {
                                                ID = us.PTSGRPricelist.ID,
                                                Name = us.PTSGRPricelist.Name,
                                                InstallationCost = us.PTSGRPricelist.InstallationCost,
                                                ChargesPerMonth = us.PTSGRPricelist.ChargesPerMonth,
                                                ChargesPerDay = us.PTSGRPricelist.ChargesPerDay,
                                                MSNPerMonth = us.PTSGRPricelist.MSNPerMonth == null ? 0 : us.PTSGRPricelist.MSNPerMonth,
                                                MSNPerDay = us.PTSGRPricelist.MSNPerDay == null ? 0 : us.PTSGRPricelist.MSNPerDay,
                                                IsChargePerMonth = us.PTSGRPricelist.IsChargePerMonth
                                            },
                                           InvoiceCost = us.InvoiceCost == null ? 0 : (decimal)us.InvoiceCost,
                                           DateStamp = us.DateStamp,
                                           EnteredByUser = us.EnteredByUser
                                       }).Where(k => k.ID == taskID).FirstOrDefault();
                    return data;
                }
                catch (Exception) { return null; }
            }
        }

        public List<TaskPTSGRB> GetTasksPTSGRForOrderPTSGR2ID(int OrderID) {
            using (var dbContext = new OTERTConnStr()) {
                try {
                    dbContext.Configuration.ProxyCreationEnabled = false;
                    List<TaskPTSGRB> data = (from us in dbContext.TasksPTSGR
                                             select new TaskPTSGRB {
                                                 ID = us.ID,
                                                 RegNo = us.OrdersPTSGR2.RegNo,
                                                 InvoiceProtocol = us.OrdersPTSGR2.InvoiceProtocol,
                                                 OrderPTSGR2ID = us.OrderPTSGR2ID,
                                                 Order = new OrderPTSGR2DTO {
                                                     ID = us.OrdersPTSGR2.ID,
                                                     OrdersPTSGRID = us.OrdersPTSGR2.OrdersPTSGRID,
                                                     OrderPTSGR = new OrderPTSGRDTO {
                                                         ID = us.OrdersPTSGR2.OrdersPTSGR.ID,
                                                         EventID = us.OrdersPTSGR2.OrdersPTSGR.EventID,
                                                         Event = new EventDTO {
                                                             NameGR = us.OrdersPTSGR2.OrdersPTSGR.Events.NameGR,
                                                             NameEN = us.OrdersPTSGR2.OrdersPTSGR.Events.NameEN,
                                                             PlaceID = us.OrdersPTSGR2.OrdersPTSGR.Events.PlaceID,
                                                             Place = new PlaceDTO {
                                                                 NameGR = us.OrdersPTSGR2.OrdersPTSGR.Events.Places.NameGR,
                                                                 NameEN = us.OrdersPTSGR2.OrdersPTSGR.Events.Places.NameEN
                                                             }
                                                         }
                                                     },
                                                     CountryID = us.OrdersPTSGR2.CountryID,
                                                     Country = new CountryDTO {
                                                         NameGR = us.OrdersPTSGR2.Countries.NameGR,
                                                         NameEN = us.OrdersPTSGR2.Countries.NameEN
                                                     },
                                                     ProviderID = us.OrdersPTSGR2.ProviderID,
                                                     Provider = new CustomerDTO {
                                                         ID = us.OrdersPTSGR2.Customers.ID,
                                                         CountryID = us.OrdersPTSGR2.Customers.CountryID,
                                                         NameGR = us.OrdersPTSGR2.Customers.NameGR,
                                                         NameEN = us.OrdersPTSGR2.Customers.NameEN,
                                                         NamedInvoiceGR = us.OrdersPTSGR2.Customers.NamedInvoiceGR,
                                                         NamedInvoiceEN = us.OrdersPTSGR2.Customers.NamedInvoiceEN,
                                                         ZIPCode = us.OrdersPTSGR2.Customers.ZIPCode,
                                                         CityGR = us.OrdersPTSGR2.Customers.CityGR,
                                                         CityEN = us.OrdersPTSGR2.Customers.CityEN,
                                                         ChargeTelephone = us.OrdersPTSGR2.Customers.ChargeTelephone,
                                                         Telephone1 = us.OrdersPTSGR2.Customers.Telephone1,
                                                         Telephone2 = us.OrdersPTSGR2.Customers.Telephone2,
                                                         FAX1 = us.OrdersPTSGR2.Customers.FAX1,
                                                         FAX2 = us.OrdersPTSGR2.Customers.FAX2,
                                                         Address1GR = us.OrdersPTSGR2.Customers.Address1GR,
                                                         Address1EN = us.OrdersPTSGR2.Customers.Address1EN,
                                                         Address2GR = us.OrdersPTSGR2.Customers.Address2GR,
                                                         Address2EN = us.OrdersPTSGR2.Customers.Address2EN,
                                                         ContactPersonGR = us.OrdersPTSGR2.Customers.ContactPersonGR,
                                                         ContactPersonEN = us.OrdersPTSGR2.Customers.ContactPersonEN,
                                                         CustomerTypeID = us.OrdersPTSGR2.Customers.CustomerTypeID,
                                                         LanguageID = us.OrdersPTSGR2.Customers.LanguageID,
                                                         Email = us.OrdersPTSGR2.Customers.Email,
                                                         URL = us.OrdersPTSGR2.Customers.URL,
                                                         AFM = us.OrdersPTSGR2.Customers.AFM,
                                                         DOY = us.OrdersPTSGR2.Customers.DOY,
                                                         SAPCode = us.OrdersPTSGR2.Customers.SAPCode,
                                                         UserID = us.OrdersPTSGR2.Customers.UserID,
                                                         Comments = us.OrdersPTSGR2.Customers.Comments,
                                                         IsProvider = us.OrdersPTSGR2.Customers.IsProvider,
                                                         IsOTE = us.OrdersPTSGR2.Customers.IsOTE
                                                     }
                                                 },
                                                 OrderDate = us.OrderDate,
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
                                                 RequestedPositionID = us.RequestedPositionID,
                                                 RequestedPosition = new RequestedPositionDTO {
                                                     ID = us.RequestedPositionID == null ? -1 : us.RequestedPositions.ID,
                                                     NameGR = us.RequestedPositionID == null ? "" : us.RequestedPositions.NameGR,
                                                     NameEN = us.RequestedPositionID == null ? "" : us.RequestedPositions.NameEN
                                                 },
                                                 DateTimeStartActual = us.DateTimeStartActual,
                                                 DateTimeEndActual = us.DateTimeEndActual,
                                                 DateTimeDurationActual = us.DateTimeDurationActual,
                                                 TelephoneNumber = us.TelephoneNumber,
                                                 CallCharges = us.CallChardes,
                                                 AddedCharges = us.AddedCharges,
                                                 CostActual = us.CostActual,
                                                 PaymentDateActual = us.PaymentDateActual,
                                                 IsLocked = us.IsLocked == null ? false : (bool)us.IsLocked,
                                                 IsCanceled = us.IsCanceled == null ? false : (bool)us.IsCanceled,
                                                 Comments = us.Comments,
                                                 CorrespondentName = us.CorrespondentName,
                                                 MSNCount = us.MSNCount,
                                                 MSN1 = us.MSN1,
                                                 MSN2 = us.MSN2,
                                                 PTSRPricelistID = us.PTSRPricelistID,
                                                 PTSRPricelist = new PTSGRPricelistDTO {
                                                     ID = us.PTSGRPricelist.ID,
                                                     Name = us.PTSGRPricelist.Name,
                                                     InstallationCost = us.PTSGRPricelist.InstallationCost,
                                                     ChargesPerMonth = us.PTSGRPricelist.ChargesPerMonth,
                                                     ChargesPerDay = us.PTSGRPricelist.ChargesPerDay,
                                                     MSNPerMonth = us.PTSGRPricelist.MSNPerMonth == null ? 0 : us.PTSGRPricelist.MSNPerMonth,
                                                     MSNPerDay = us.PTSGRPricelist.MSNPerDay == null ? 0 : us.PTSGRPricelist.MSNPerDay,
                                                     IsChargePerMonth = us.PTSGRPricelist.IsChargePerMonth
                                                 },
                                                 InvoiceCost = us.InvoiceCost == null ? 0 : (decimal)us.InvoiceCost,
                                                 DateStamp = us.DateStamp,
                                                 EnteredByUser = us.EnteredByUser
                                             }).Where(k => k.OrderPTSGR2ID == OrderID).OrderByDescending(o => o.OrderDate).ToList();
                    return data;
                }
                catch (Exception) { return null; }
            }
        }

        public List<TaskPTSGRB> GetTasksPTSGRForOrderPTSGRID(int OrderID) {
            using (var dbContext = new OTERTConnStr()) {
                try {
                    dbContext.Configuration.ProxyCreationEnabled = false;
                    List<TaskPTSGRB> data = (from us in dbContext.TasksPTSGR
                                             select new TaskPTSGRB {
                                                 ID = us.ID,
                                                 OrderPTSGR2ID = us.OrderPTSGR2ID,
                                                 Order = new OrderPTSGR2DTO {
                                                     ID = us.OrdersPTSGR2.ID,
                                                     RegNo = us.OrdersPTSGR2.RegNo,
                                                     InvoiceProtocol = us.OrdersPTSGR2.InvoiceProtocol,
                                                     OrdersPTSGRID = us.OrdersPTSGR2.OrdersPTSGRID,
                                                     OrderPTSGR = new OrderPTSGRDTO {
                                                         ID = us.OrdersPTSGR2.OrdersPTSGR.ID,
                                                         EventID = us.OrdersPTSGR2.OrdersPTSGR.EventID,
                                                         Event = new EventDTO {
                                                             NameGR = us.OrdersPTSGR2.OrdersPTSGR.Events.NameGR,
                                                             NameEN = us.OrdersPTSGR2.OrdersPTSGR.Events.NameEN,
                                                             PlaceID = us.OrdersPTSGR2.OrdersPTSGR.Events.PlaceID,
                                                             Place = new PlaceDTO {
                                                                 NameGR = us.OrdersPTSGR2.OrdersPTSGR.Events.Places.NameGR,
                                                                 NameEN = us.OrdersPTSGR2.OrdersPTSGR.Events.Places.NameEN
                                                             }
                                                         }
                                                     },
                                                     CountryID = us.OrdersPTSGR2.CountryID,
                                                     Country = new CountryDTO {
                                                         NameGR = us.OrdersPTSGR2.Countries.NameGR,
                                                         NameEN = us.OrdersPTSGR2.Countries.NameEN
                                                     },
                                                     ProviderID = us.OrdersPTSGR2.ProviderID,
                                                     Provider = new CustomerDTO {
                                                         ID = us.OrdersPTSGR2.Customers.ID,
                                                         CountryID = us.OrdersPTSGR2.Customers.CountryID,
                                                         NameGR = us.OrdersPTSGR2.Customers.NameGR,
                                                         NameEN = us.OrdersPTSGR2.Customers.NameEN,
                                                         NamedInvoiceGR = us.OrdersPTSGR2.Customers.NamedInvoiceGR,
                                                         NamedInvoiceEN = us.OrdersPTSGR2.Customers.NamedInvoiceEN,
                                                         ZIPCode = us.OrdersPTSGR2.Customers.ZIPCode,
                                                         CityGR = us.OrdersPTSGR2.Customers.CityGR,
                                                         CityEN = us.OrdersPTSGR2.Customers.CityEN,
                                                         ChargeTelephone = us.OrdersPTSGR2.Customers.ChargeTelephone,
                                                         Telephone1 = us.OrdersPTSGR2.Customers.Telephone1,
                                                         Telephone2 = us.OrdersPTSGR2.Customers.Telephone2,
                                                         FAX1 = us.OrdersPTSGR2.Customers.FAX1,
                                                         FAX2 = us.OrdersPTSGR2.Customers.FAX2,
                                                         Address1GR = us.OrdersPTSGR2.Customers.Address1GR,
                                                         Address1EN = us.OrdersPTSGR2.Customers.Address1EN,
                                                         Address2GR = us.OrdersPTSGR2.Customers.Address2GR,
                                                         Address2EN = us.OrdersPTSGR2.Customers.Address2EN,
                                                         ContactPersonGR = us.OrdersPTSGR2.Customers.ContactPersonGR,
                                                         ContactPersonEN = us.OrdersPTSGR2.Customers.ContactPersonEN,
                                                         CustomerTypeID = us.OrdersPTSGR2.Customers.CustomerTypeID,
                                                         LanguageID = us.OrdersPTSGR2.Customers.LanguageID,
                                                         Email = us.OrdersPTSGR2.Customers.Email,
                                                         URL = us.OrdersPTSGR2.Customers.URL,
                                                         AFM = us.OrdersPTSGR2.Customers.AFM,
                                                         DOY = us.OrdersPTSGR2.Customers.DOY,
                                                         SAPCode = us.OrdersPTSGR2.Customers.SAPCode,
                                                         UserID = us.OrdersPTSGR2.Customers.UserID,
                                                         Comments = us.OrdersPTSGR2.Customers.Comments,
                                                         IsProvider = us.OrdersPTSGR2.Customers.IsProvider,
                                                         IsOTE = us.OrdersPTSGR2.Customers.IsOTE
                                                     }
                                                 },
                                                 RegNo = us.RegNo,
                                                 OrderDate = us.OrderDate,
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
                                                 RequestedPositionID = us.RequestedPositionID,
                                                 RequestedPosition = new RequestedPositionDTO {
                                                     ID = us.RequestedPositionID == null ? -1 : us.RequestedPositions.ID,
                                                     NameGR = us.RequestedPositionID == null ? "" : us.RequestedPositions.NameGR,
                                                     NameEN = us.RequestedPositionID == null ? "" : us.RequestedPositions.NameEN
                                                 },
                                                 DateTimeStartActual = us.DateTimeStartActual,
                                                 DateTimeEndActual = us.DateTimeEndActual,
                                                 DateTimeDurationActual = us.DateTimeDurationActual,
                                                 TelephoneNumber = us.TelephoneNumber,
                                                 CallCharges = us.CallChardes,
                                                 AddedCharges = us.AddedCharges,
                                                 CostActual = us.CostActual,
                                                 PaymentDateActual = us.PaymentDateActual,
                                                 IsLocked = us.IsLocked == null ? false : (bool)us.IsLocked,
                                                 IsCanceled = us.IsCanceled == null ? false : (bool)us.IsCanceled,
                                                 Comments = us.Comments,
                                                 CorrespondentName = us.CorrespondentName,
                                                 MSNCount = us.MSNCount,
                                                 MSN1 = us.MSN1,
                                                 MSN2 = us.MSN2,
                                                 PTSRPricelistID = us.PTSRPricelistID,
                                                 PTSRPricelist = new PTSGRPricelistDTO {
                                                     ID = us.PTSGRPricelist.ID,
                                                     Name = us.PTSGRPricelist.Name,
                                                     InstallationCost = us.PTSGRPricelist.InstallationCost,
                                                     ChargesPerMonth = us.PTSGRPricelist.ChargesPerMonth,
                                                     ChargesPerDay = us.PTSGRPricelist.ChargesPerDay,
                                                     MSNPerMonth = us.PTSGRPricelist.MSNPerMonth == null ? 0 : us.PTSGRPricelist.MSNPerMonth,
                                                     MSNPerDay = us.PTSGRPricelist.MSNPerDay == null ? 0 : us.PTSGRPricelist.MSNPerDay,
                                                     IsChargePerMonth = us.PTSGRPricelist.IsChargePerMonth
                                                 },
                                                 InvoiceCost = us.InvoiceCost == null ? 0 : (decimal)us.InvoiceCost,
                                                 DateStamp = us.DateStamp,
                                                 EnteredByUser = us.EnteredByUser
                                             }).Where(k => k.Order.OrderPTSGR.ID == OrderID && (k.DateTimeStartActual != null && k.DateTimeEndActual != null)).OrderByDescending(o => o.OrderDate).ToList();
                    return data;
                }
                catch (Exception) { return null; }
            }
        }

        public List<TaskPTSGRB> GetAllTasksPTSGR(int recSkip, int recTake, string recFilter, GridSortExpressionCollection gridSortExxpressions) {
            using (var dbContext = new OTERTConnStr()) {
                try {
                    dbContext.Configuration.ProxyCreationEnabled = false;
                    IQueryable<TaskPTSGRB> datatmp = (from us in dbContext.TasksPTSGR
                                                      select new TaskPTSGRB {
                                                          ID = us.ID,
                                                          OrderPTSGR2ID = us.OrderPTSGR2ID,
                                                          Order = new OrderPTSGR2DTO {
                                                              ID = us.OrderPTSGR2ID,
                                                              OrdersPTSGRID = us.OrdersPTSGR2.OrdersPTSGRID,
                                                              RegNo = us.OrdersPTSGR2.RegNo,
                                                              InvoiceProtocol = us.OrdersPTSGR2.InvoiceProtocol,
                                                              OrderPTSGR = new OrderPTSGRDTO {
                                                                  ID = us.OrdersPTSGR2.OrdersPTSGR.ID,
                                                                  EventID = us.OrdersPTSGR2.OrdersPTSGR.EventID,
                                                                  Event = new EventDTO {
                                                                      ID = us.OrdersPTSGR2.OrdersPTSGR.Events.ID,
                                                                      PlaceID = us.OrdersPTSGR2.OrdersPTSGR.Events.PlaceID,
                                                                      Place = new PlaceDTO {
                                                                          ID = us.OrdersPTSGR2.OrdersPTSGR.Events.Places.ID,
                                                                          NameGR = us.OrdersPTSGR2.OrdersPTSGR.Events.Places.NameGR,
                                                                          NameEN = us.OrdersPTSGR2.OrdersPTSGR.Events.Places.NameEN,
                                                                      },
                                                                      NameGR = us.OrdersPTSGR2.OrdersPTSGR.Events.NameGR,
                                                                      NameEN = us.OrdersPTSGR2.OrdersPTSGR.Events.NameEN
                                                                  }
                                                              },
                                                              CountryID = us.OrdersPTSGR2.CountryID,
                                                              Country = new CountryDTO { 
                                                                    ID = us.OrdersPTSGR2.Countries.ID,
                                                                    NameGR = us.OrdersPTSGR2.Countries.NameGR,
                                                                    NameEN = us.OrdersPTSGR2.Countries.NameEN
                                                              },
                                                              ProviderID = us.OrdersPTSGR2.ProviderID,
                                                              Provider = new CustomerDTO {
                                                                  ID = us.OrdersPTSGR2.Customers.ID,
                                                                  CountryID = us.OrdersPTSGR2.Customers.CountryID,
                                                                  NameGR = us.OrdersPTSGR2.Customers.NameGR,
                                                                  NameEN = us.OrdersPTSGR2.Customers.NameEN,
                                                                  NamedInvoiceGR = us.OrdersPTSGR2.Customers.NamedInvoiceGR,
                                                                  NamedInvoiceEN = us.OrdersPTSGR2.Customers.NamedInvoiceEN,
                                                                  ZIPCode = us.OrdersPTSGR2.Customers.ZIPCode,
                                                                  CityGR = us.OrdersPTSGR2.Customers.CityGR,
                                                                  CityEN = us.OrdersPTSGR2.Customers.CityEN,
                                                                  ChargeTelephone = us.OrdersPTSGR2.Customers.ChargeTelephone,
                                                                  Telephone1 = us.OrdersPTSGR2.Customers.Telephone1,
                                                                  Telephone2 = us.OrdersPTSGR2.Customers.Telephone2,
                                                                  FAX1 = us.OrdersPTSGR2.Customers.FAX1,
                                                                  FAX2 = us.OrdersPTSGR2.Customers.FAX2,
                                                                  Address1GR = us.OrdersPTSGR2.Customers.Address1GR,
                                                                  Address1EN = us.OrdersPTSGR2.Customers.Address1EN,
                                                                  Address2GR = us.OrdersPTSGR2.Customers.Address2GR,
                                                                  Address2EN = us.OrdersPTSGR2.Customers.Address2EN,
                                                                  ContactPersonGR = us.OrdersPTSGR2.Customers.ContactPersonGR,
                                                                  ContactPersonEN = us.OrdersPTSGR2.Customers.ContactPersonEN,
                                                                  CustomerTypeID = us.OrdersPTSGR2.Customers.CustomerTypeID,
                                                                  LanguageID = us.OrdersPTSGR2.Customers.LanguageID,
                                                                  Email = us.OrdersPTSGR2.Customers.Email,
                                                                  URL = us.OrdersPTSGR2.Customers.URL,
                                                                  AFM = us.OrdersPTSGR2.Customers.AFM,
                                                                  DOY = us.OrdersPTSGR2.Customers.DOY,
                                                                  SAPCode = us.OrdersPTSGR2.Customers.SAPCode,
                                                                  UserID = us.OrdersPTSGR2.Customers.UserID,
                                                                  Comments = us.OrdersPTSGR2.Customers.Comments,
                                                                  IsProvider = us.OrdersPTSGR2.Customers.IsProvider,
                                                                  IsOTE = us.OrdersPTSGR2.Customers.IsOTE
                                                              }
                                                         },
                                                         RegNo = us.RegNo,
                                                         InvoiceProtocol = us.InvoiceProtocol,
                                                         OrderDate = us.OrderDate,
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
                                                         RequestedPositionID = us.RequestedPositionID,
                                                         RequestedPosition = new RequestedPositionDTO {
                                                             ID = us.RequestedPositionID == null ? -1 : us.RequestedPositions.ID,
                                                             NameGR = us.RequestedPositionID == null ? "" : us.RequestedPositions.NameGR,
                                                             NameEN = us.RequestedPositionID == null ? "" : us.RequestedPositions.NameEN
                                                         },
                                                         DateTimeStartActual = us.DateTimeStartActual,
                                                         DateTimeEndActual = us.DateTimeEndActual,
                                                         DateTimeDurationActual = us.DateTimeDurationActual,
                                                         TelephoneNumber = us.TelephoneNumber,
                                                         CallCharges = us.CallChardes,
                                                         AddedCharges = us.AddedCharges,
                                                         CostActual = us.CostActual,
                                                         PaymentDateActual = us.PaymentDateActual,
                                                         IsLocked = us.IsLocked == null ? false : (bool)us.IsLocked,
                                                         IsCanceled = us.IsCanceled == null ? false : (bool)us.IsCanceled,
                                                         Comments = us.Comments,
                                                          CorrespondentName = us.CorrespondentName,
                                                          MSNCount = us.MSNCount,
                                                          MSN1 = us.MSN1,
                                                          MSN2 = us.MSN2,
                                                          PTSRPricelistID = us.PTSRPricelistID,
                                                          PTSRPricelist = new PTSGRPricelistDTO {
                                                              ID = us.PTSGRPricelist.ID,
                                                              Name = us.PTSGRPricelist.Name,
                                                              InstallationCost = us.PTSGRPricelist.InstallationCost,
                                                              ChargesPerMonth = us.PTSGRPricelist.ChargesPerMonth,
                                                              ChargesPerDay = us.PTSGRPricelist.ChargesPerDay,
                                                              MSNPerMonth = us.PTSGRPricelist.MSNPerMonth == null ? 0 : us.PTSGRPricelist.MSNPerMonth,
                                                              MSNPerDay = us.PTSGRPricelist.MSNPerDay == null ? 0 : us.PTSGRPricelist.MSNPerDay,
                                                              IsChargePerMonth = us.PTSGRPricelist.IsChargePerMonth
                                                          },
                                                          InvoiceCost = us.InvoiceCost == null ? 0 : (decimal)us.InvoiceCost,
                                                          DateStamp = us.DateStamp,
                                                         EnteredByUser = us.EnteredByUser
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
                        List<string> OrderDateExpressions = columnExpressions.Where(item => item.Contains("OrderDate")).ToList();
                        List<string> StartActualExpressions = columnExpressions.Where(item => item.Contains("DateTimeStartActual")).ToList();
                        List<string> PaymentDateActualExpressions = columnExpressions.Where(item => item.Contains("PaymentDateActual")).ToList();
                        columnExpressions.RemoveAll(item => item.Contains("OrderDate") || item.Contains("DateTimeStartActual") || item.Contains("PaymentDateActual"));
                        recFilter = string.Join("AND", columnExpressions.ToArray());
                        if (!string.IsNullOrEmpty(recFilter)) { datatmp = datatmp.Where(recFilter); }
                        System.Globalization.DateTimeFormatInfo greek = new System.Globalization.CultureInfo("el-GR").DateTimeFormat;
                        if (OrderDateExpressions.Count > 0) {
                            List<DateTime> orderDates = new List<DateTime>();
                            foreach (string dtExpression in OrderDateExpressions) {
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
                                if (OrderDateExpressions[0].Contains(">=")) {
                                    recFilter = "OrderDate >= @0 AND OrderDate <= @1";
                                } else {
                                    recFilter = "OrderDate < @0 OR OrderDate > @1";
                                }
                                datatmp = datatmp.Where(recFilter, new DateTime(orderDates[0].Year, orderDates[0].Month, orderDates[0].Day, orderDates[0].Hour, orderDates[0].Minute, 0), new DateTime(orderDates[1].Year, orderDates[1].Month, orderDates[1].Day, orderDates[1].Hour, orderDates[1].Minute, 0));
                            } else {
                                datatmp = datatmp.Where(OrderDateExpressions[0]);
                            }
                        }
                        if (StartActualExpressions.Count > 0) {
                            List<DateTime> startActualDates = new List<DateTime>();
                            foreach (string dtExpression in StartActualExpressions) {
                                string[] dateExp = dtExpression.Split(new char[] { '"' });
                                string format = "d/M/yyyy,h:mm:ss,tt";
                                DateTime newDate;
                                if (dateExp.Length > 1) {
                                    if (DateTime.TryParseExact(dateExp[1], format, greek, System.Globalization.DateTimeStyles.None, out newDate)) {
                                        startActualDates.Add(newDate);
                                    }
                                }
                            }
                            if (startActualDates.Count == 2) {
                                if (!string.IsNullOrEmpty(recFilter)) { recFilter += " AND "; }
                                if (StartActualExpressions[0].Contains(">=")) {
                                    recFilter = "DateTimeStartActual >= @0 AND DateTimeStartActual <= @1";
                                } else {
                                    recFilter = "DateTimeStartActual < @0 OR DateTimeStartActual > @1";
                                }
                                datatmp = datatmp.Where(recFilter, new DateTime(startActualDates[0].Year, startActualDates[0].Month, startActualDates[0].Day, startActualDates[0].Hour, startActualDates[0].Minute, 0), new DateTime(startActualDates[1].Year, startActualDates[1].Month, startActualDates[1].Day, startActualDates[1].Hour, startActualDates[1].Minute, 0));
                            } else {
                                datatmp = datatmp.Where(StartActualExpressions[0]);
                            }
                        }
                        if (PaymentDateActualExpressions.Count > 0) {
                            List<DateTime> PaymentDateActual = new List<DateTime>();
                            foreach (string dtExpression in PaymentDateActualExpressions) {
                                string[] dateExp = dtExpression.Split(new char[] { '"' });
                                string format = "d/M/yyyy,h:mm:ss,tt";
                                DateTime newDate;
                                if (dateExp.Length > 1) {
                                    if (DateTime.TryParseExact(dateExp[1], format, greek, System.Globalization.DateTimeStyles.None, out newDate)) {
                                        PaymentDateActual.Add(newDate);
                                    }
                                }
                            }
                            if (PaymentDateActual.Count == 2) {
                                if (!string.IsNullOrEmpty(recFilter)) { recFilter += " AND "; }
                                if (PaymentDateActualExpressions[0].Contains(">=")) {
                                    recFilter = "PaymentDateActual >= @0 AND PaymentDateActual <= @1";
                                } else {
                                    recFilter = "PaymentDateActual < @0 OR PaymentDateActual > @1";
                                }
                                datatmp = datatmp.Where(recFilter, new DateTime(PaymentDateActual[0].Year, PaymentDateActual[0].Month, PaymentDateActual[0].Day, PaymentDateActual[0].Hour, PaymentDateActual[0].Minute, 0), new DateTime(PaymentDateActual[1].Year, PaymentDateActual[1].Month, PaymentDateActual[1].Day, PaymentDateActual[1].Hour, PaymentDateActual[1].Minute, 0));
                            } else {
                                datatmp = datatmp.Where(PaymentDateActualExpressions[0]);
                            }
                        }
                    }
                    if (gridSortExxpressions.Count > 0) {
                        string sortFieldName = "";
                        if (gridSortExxpressions[0].FieldName == "Order.EventID") { sortFieldName = "Order.OrderPTSGR.Event.NameGR"; }
                        else if (gridSortExxpressions[0].FieldName == "Order.CountryID") { sortFieldName = "Order.OrderPTSGR.Country.NameGR"; }
                        else if (gridSortExxpressions[0].FieldName == "Order.ProviderID") { sortFieldName = "Order.OrderPTSGR.Provider.NameGR"; }
                        else if (gridSortExxpressions[0].FieldName == "CustomerID") { sortFieldName = "Customer.NameGR"; }
                        else { sortFieldName = gridSortExxpressions[0].FieldName; }
                        datatmp = datatmp.OrderBy(sortFieldName + " " + gridSortExxpressions[0].SortOrder);
                    } else {
                        datatmp = datatmp.OrderByDescending(o => o.OrderPTSGR2ID).ThenByDescending(o => o.Order.OrdersPTSGRID).ThenByDescending(o => o.ID);
                    }
                    List<TaskPTSGRB> data = datatmp.Skip(recSkip).Take(recTake).ToList();
                    return data;
                }
                catch (Exception) { return null; }
            }
        }

        protected void appendError(Exception ex, string EventID) {
            EventLogs newEvent = new EventLogs();
            newEvent.EventDate = DateTime.Now;
            newEvent.EventID = EventID;
            newEvent.EventDescription = "Message: " + ex.Message + " ------------ Inner Exeption: " + ex.InnerException + " ------------ Stack Trace: " + ex.StackTrace;
            using (var dbContext = new OTERTConnStr()) {
                dbContext.EventLogs.Add(newEvent);
                dbContext.SaveChanges();
            }
        }

        public List<TaskB> GetAllTasksForOrdersBetweenDates(DateTime fromDate, DateTime toDate) {
            using (var dbContext = new OTERTConnStr()) {
                try {
                    dbContext.Configuration.ProxyCreationEnabled = false;
                    List<TaskB> data = (from us in dbContext.Tasks
                                        select new TaskB {
                                            ID = us.ID,
                                            OrderID = us.OrderID,
                                            Order = new OrderDTO {
                                                ID = us.OrderID == null ? -1 : us.Orders.ID,
                                                OrderTypeID = us.OrderID == null ? -1 : us.Orders.OrderTypeID,
                                                RegNo = us.OrderID == null ? "" : us.Orders.RegNo,
                                                Customer1ID = us.OrderID == null ? -1 : us.Orders.Customer1ID,
                                                Customer2ID = us.OrderID == null ? -1 : us.Orders.Customer2ID,
                                                EventID = us.OrderID == null ? -1 : us.Orders.EventID,
                                            },
                                            RegNo = us.RegNo,
                                            OrderDate = us.OrderDate,
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
                                            RequestedPositionID = us.RequestedPositionID,
                                            RequestedPosition = new RequestedPositionDTO {
                                                ID = us.RequestedPositionID == null ? -1 : us.RequestedPositions.ID,
                                                NameGR = us.RequestedPositionID == null ? "" : us.RequestedPositions.NameGR,
                                                NameEN = us.RequestedPositionID == null ? "" : us.RequestedPositions.NameEN
                                            },
                                            JobID = us.JobID,
                                            Job = new JobDTO {
                                                ID = us.Jobs.ID,
                                                JobsMainID = us.Jobs.JobsMainID,
                                                JobsMain = new JobMainDTO { ID = us.Jobs.JobsMain.ID, Name = us.Jobs.JobsMain.Name, PageID = us.Jobs.JobsMain.PageID },
                                                Name = us.Jobs.Name,
                                                MinimumTime = us.Jobs.MinimumTime,
                                                InvoiceCode = us.Jobs.InvoiceCode,
                                                SalesID = us.Jobs.SalesID
                                            },
                                            DistanceID = us.DistancesID,
                                            Distance = new DistanceDTO {
                                                ID = us.Distances.ID,
                                                JobsMainID = us.Distances.JobsMainID,
                                                JobsMain = new JobMainDTO { ID = us.Distances.JobsMain.ID, Name = us.Distances.JobsMain.Name, PageID = us.Distances.JobsMain.PageID },
                                                Description = us.Distances.Position1 + " - " + us.Distances.Position2 + " (" + us.Distances.KM.ToString() + " km)",
                                                Position1 = us.Distances.Position1,
                                                Position2 = us.Distances.Position2,
                                                KM = us.Distances.KM
                                            },
                                            DateTimeStartOrder = us.DateTimeStartOrder,
                                            DateTimeEndOrder = us.DateTimeEndOrder,
                                            DateTimeDurationOrder = us.DateTimeDurationOrder,
                                            DateTimeStartActual = us.DateTimeStartActual,
                                            DateTimeEndActual = us.DateTimeEndActual,
                                            DateTimeDurationActual = us.DateTimeDurationActual,
                                            CostCalculated = us.CostCalculated,
                                            InstallationCharges = us.InstallationCharges == null ? false : (bool)us.InstallationCharges,
                                            MonthlyCharges = us.MonthlyCharges == null ? false : (bool)us.MonthlyCharges,
                                            CallCharges = us.CallCharges,
                                            TelephoneNumber = us.TelephoneNumber,
                                            TechnicalSupport = us.TechnicalSupport,
                                            AddedCharges = us.AddedCharges,
                                            CostActual = us.CostActual,
                                            PaymentDateOrder = us.PaymentDateOrder,
                                            PaymentDateCalculated = us.PaymentDateCalculated,
                                            PaymentDateActual = us.PaymentDateActual,
                                            IsForHelpers = us.IsForHelpers == null ? false : (bool)us.IsForHelpers,
                                            IsLocked = us.IsLocked == null ? false : (bool)us.IsLocked,
                                            IsCanceled = us.IsCanceled == null ? false : (bool)us.IsCanceled,
                                            CancelPrice = us.CancelPrice,
                                            Comments = us.Comments,
                                            InvoceComments = us.InvoceComments,
                                            SateliteID = us.SateliteID,
                                            Satelite = new SateliteDTO {
                                                ID = us.SateliteID == null ? -1 : us.Satelites.ID,
                                                Name = us.SateliteID == null ? "" : us.Satelites.Name,
                                                Frequency = us.SateliteID == null ? "" : us.Satelites.Frequency
                                            },
                                            MSN = us.MSN == null ? false : (bool)us.MSN,
                                            Internet = us.Internet == null ? false : (bool)us.Internet,
                                            LineTypeID = us.LineTypeID,
                                            LineType = us.LineTypeID == null ? null : new LineTypeDTO { ID = us.LineTypes.ID, Name = us.LineTypes.Name },
                                            DateStamp = us.DateStamp,
                                            EnteredByUser = us.EnteredByUser,
                                            InvoiceCost = us.InvoiceCost,
                                            GivenPhoneNumber = us.GivenPhoneNumber,
                                            CorrespondentPhone = us.CorrespondentPhone
                                        }).Where(k => k.OrderID > 0 && k.PaymentDateOrder >= fromDate && k.PaymentDateOrder <= toDate).ToList();
                    return data;
                }
                catch (Exception) { return null; }
            }

        }

    }
}