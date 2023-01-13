using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using OTERT.Model;
using OTERT_Entity;
using System.Data.Entity;
using Telerik.Web.UI;
using OTERT.WebServices;

namespace OTERT.Controller {

    public class TasksController {

        public int CountTasks() {
            using (var dbContext = new OTERTConnStr()) {
                try {
                    return dbContext.Tasks.Count();
                }
                catch (Exception) { return -1; }
            }
        }

        public int CountTasksForPageID(int PageID, string recFilter) {
            using (var dbContext = new OTERTConnStr()) {
                try {
                    System.Globalization.DateTimeFormatInfo greek = new System.Globalization.CultureInfo("el-GR").DateTimeFormat;
                    int count = 0;
                    dbContext.Configuration.ProxyCreationEnabled = false;
                    if (!string.IsNullOrEmpty(recFilter)) {
                        IQueryable test = dbContext.Tasks.Where(o => o.Jobs.JobsMain.PageID == PageID && o.OrderID == null);
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
                        List<string> StartOrderExpression = columnExpressions.Where(item => item.Contains("DateTimeStartOrder")).ToList();
                        columnExpressions.RemoveAll(item => item.Contains("OrderDate") || item.Contains("DateTimeStartActual") || item.Contains("DateTimeStartOrder"));
                        recFilter = string.Join("AND", columnExpressions.ToArray());
                        if (!string.IsNullOrEmpty(recFilter)) {
                            recFilter = recFilter.Replace("DistanceID", "DistancesID");
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
                        if (StartOrderExpression.Count > 0) {
                            List<DateTime> startOrderDates = new List<DateTime>();
                            foreach (string dtExpression in StartOrderExpression) {
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
                                if (StartOrderExpression[0].Contains(">=")) {
                                    recFilter = "DateTimeStartOrder >= @0 AND DateTimeStartOrder <= @1";
                                } else {
                                    recFilter = "DateTimeStartOrder < @0 OR DateTimeStartOrder > @1";
                                }
                                test = test.Where(recFilter, new DateTime(startOrderDates[0].Year, startOrderDates[0].Month, startOrderDates[0].Day, startOrderDates[0].Hour, startOrderDates[0].Minute, 0), new DateTime(startOrderDates[1].Year, startOrderDates[1].Month, startOrderDates[1].Day, startOrderDates[1].Hour, startOrderDates[1].Minute, 0));
                            } else {
                                test = test.Where(StartOrderExpression[0]);
                            }
                        }
                        count = test.Count();
                    } else {
                        count = dbContext.Tasks.Where(o => o.Jobs.JobsMain.PageID == PageID && o.OrderID == null).Count();
                    }
                    return count;
                }
                catch (Exception) { return -1; }
            }
        }

        public int CountAllTasks(string recFilter) {
            using (var dbContext = new OTERTConnStr()) {
                try {
                    System.Globalization.DateTimeFormatInfo greek = new System.Globalization.CultureInfo("el-GR").DateTimeFormat;
                    int count = 0;
                    dbContext.Configuration.ProxyCreationEnabled = false;
                    if (!string.IsNullOrEmpty(recFilter)) {
                        IQueryable test = dbContext.Tasks.Where(o => o.OrderID == null);
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
                        List<string> StartOrderExpressions = columnExpressions.Where(item => item.Contains("DateTimeStartOrder")).ToList();
                        List<string> StartActualExpressions = columnExpressions.Where(item => item.Contains("DateTimeStartActual")).ToList();
                        columnExpressions.RemoveAll(item => item.Contains("OrderDate") || item.Contains("DateTimeStartActual") || item.Contains("DateTimeStartOrder"));
                        recFilter = string.Join("AND", columnExpressions.ToArray());
                        if (!string.IsNullOrEmpty(recFilter)) {
                            recFilter = recFilter.Replace("DistanceID", "DistancesID");
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
                        if (StartOrderExpressions.Count > 0) {
                            List<DateTime> startOrderDates = new List<DateTime>();
                            foreach (string dtExpression in StartOrderExpressions) {
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
                                if (StartOrderExpressions[0].Contains(">=")) {
                                    recFilter = "DateTimeStartOrder >= @0 AND DateTimeStartOrder <= @1";
                                } else {
                                    recFilter = "DateTimeStartOrder < @0 OR DateTimeStartOrder > @1";
                                }
                                test = test.Where(recFilter, new DateTime(startOrderDates[0].Year, startOrderDates[0].Month, startOrderDates[0].Day, startOrderDates[0].Hour, startOrderDates[0].Minute, 0), new DateTime(startOrderDates[1].Year, startOrderDates[1].Month, startOrderDates[1].Day, startOrderDates[1].Hour, startOrderDates[1].Minute, 0));
                            } else {
                                test = test.Where(StartOrderExpressions[0]);
                            }
                        }
                        count = test.Count();
                    } else {
                        count = dbContext.Tasks.Where(o => o.OrderID == null).Count();
                    }
                    return count;
                }
                catch (Exception) { return -1; }
            }
        }

        public int CountAllTasksforPTStoAbroad(string recFilter) {
            using (var dbContext = new OTERTConnStr()) {
                try {
                    System.Globalization.DateTimeFormatInfo greek = new System.Globalization.CultureInfo("el-GR").DateTimeFormat;
                    int count = 0;
                    dbContext.Configuration.ProxyCreationEnabled = false;
                    if (!string.IsNullOrEmpty(recFilter)) {
                        IQueryable test = dbContext.Tasks.Where(o => o.OrderID != null);
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
                        List<string> PaymentDateOrderExpressions = columnExpressions.Where(item => item.Contains("PaymentDateOrder")).ToList();
                        List<string> PaymentDateCalculatedExpressions = columnExpressions.Where(item => item.Contains("PaymentDateCalculated")).ToList();
                        columnExpressions.RemoveAll(item => item.Contains("OrderDate") || item.Contains("DateTimeStartActual") || item.Contains("PaymentDateOrder") || item.Contains("PaymentDateCalculated"));
                        recFilter = string.Join("AND", columnExpressions.ToArray());
                        if (!string.IsNullOrEmpty(recFilter)) {
                            recFilter = recFilter.Replace("Order.", "Orders.");
                            recFilter = recFilter.Replace("Event.Place.", "Events.Places.");
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
                        if (PaymentDateOrderExpressions.Count > 0) {
                            List<DateTime> startOrderDates = new List<DateTime>();
                            foreach (string dtExpression in PaymentDateOrderExpressions) {
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
                                if (PaymentDateOrderExpressions[0].Contains(">=")) {
                                    recFilter = "PaymentDateOrder >= @0 AND PaymentDateOrder <= @1";
                                } else {
                                    recFilter = "PaymentDateOrder < @0 OR PaymentDateOrder > @1";
                                }
                                test = test.Where(recFilter, new DateTime(startOrderDates[0].Year, startOrderDates[0].Month, startOrderDates[0].Day, startOrderDates[0].Hour, startOrderDates[0].Minute, 0), new DateTime(startOrderDates[1].Year, startOrderDates[1].Month, startOrderDates[1].Day, startOrderDates[1].Hour, startOrderDates[1].Minute, 0));
                            } else {
                                test = test.Where(PaymentDateOrderExpressions[0]);
                            }
                        }
                        if (PaymentDateCalculatedExpressions.Count > 0) {
                            List<DateTime> startOrderDates = new List<DateTime>();
                            foreach (string dtExpression in PaymentDateCalculatedExpressions) {
                                string[] dateExp = dtExpression.Split(new char[] { '"' });
                                string format = "d/M/yyyy,h:mm:ss,tt";
                                DateTime newDate;
                                if (dateExp.Length > 1) {
                                    if (DateTime.TryParseExact(dateExp[1], format, greek, System.Globalization.DateTimeStyles.None, out newDate)) {
                                        startOrderDates.Add(newDate);
                                    }
                                }
                            }
                            if (PaymentDateCalculatedExpressions.Count == 2) {
                                if (!string.IsNullOrEmpty(recFilter)) { recFilter += " AND "; }
                                if (PaymentDateOrderExpressions[0].Contains(">=")) {
                                    recFilter = "PaymentDateOrder >= @0 AND PaymentDateOrder <= @1";
                                } else {
                                    recFilter = "PaymentDateOrder < @0 OR PaymentDateOrder > @1";
                                }
                                test = test.Where(recFilter, new DateTime(startOrderDates[0].Year, startOrderDates[0].Month, startOrderDates[0].Day, startOrderDates[0].Hour, startOrderDates[0].Minute, 0), new DateTime(startOrderDates[1].Year, startOrderDates[1].Month, startOrderDates[1].Day, startOrderDates[1].Hour, startOrderDates[1].Minute, 0));
                            } else {
                                test = test.Where(PaymentDateCalculatedExpressions[0]);
                            }
                        }
                        count = test.Count();
                    } else {
                        count = dbContext.Tasks.Where(o => o.OrderID != null).Count();
                    }
                    return count;
                }
                catch (Exception) { return -1; }
            }
        }

        public TaskB GetTask(int taskID) {
            using (var dbContext = new OTERTConnStr()) {
                try {
                    dbContext.Configuration.ProxyCreationEnabled = false;
                    TaskB data = (from us in dbContext.Tasks
                                  select new TaskB {
                                      ID = us.ID,
                                      OrderID = us.OrderID,
                                      Order = new OrderDTO {
                                          ID = us.OrderID == null ? -1 : us.Orders.ID,
                                          OrderTypeID = us.OrderID == null ? -1 : us.Orders.OrderTypeID,
                                          RegNo = us.OrderID == null ? "" : us.Orders.RegNo,
                                          Customer1ID = us.OrderID == null ? -1 : us.Orders.Customer1ID,
                                          Customer2ID = us.OrderID == null ? -1 : us.Orders.Customer2ID,
                                          EventID = us.OrderID == null ? -1 : us.Orders.EventID
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
                                  }).Where(k => k.ID == taskID).FirstOrDefault();
                    return data;
                }
                catch (Exception) { return null; }
            }
        }

        public List<TaskB> GetTasks() {
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
                                                EventID = us.OrderID == null ? -1 : us.Orders.EventID
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
                                            EnteredByUser = us.EnteredByUser
                                        }).OrderBy(o => o.OrderDate).ToList();
                    return data;
                }
                catch (Exception) { return null; }
            }
        }

        public List<TaskB> GetTasksForHelpers(DateTime forDate) {
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
                                                EventID = us.OrderID == null ? -1 : us.Orders.EventID
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
                                            EnteredByUser = us.EnteredByUser
                                        }).Where(o => (o.DateTimeStartOrder.HasValue ? DbFunctions.TruncateTime(o.DateTimeStartOrder.Value) == DbFunctions.TruncateTime(forDate) : false) && o.IsForHelpers == true).OrderBy(o => o.OrderDate).ToList();
                    return data;
                }
                catch (Exception) { return null; }
            }
        }

        public List<TaskB> GetTasksForInvoice(int customerID, DateTime fromDate, DateTime toDate, List<string> selectedJobs) {
            using (var dbContext = new OTERTConnStr()) {
                try {
                    dbContext.Configuration.ProxyCreationEnabled = false;
                    List<int> invoicedTasks = (from us in dbContext.TasksLine select us.TaskID).ToList();
                    List<TaskB> data1 = (from us in dbContext.Tasks
                                         select new TaskB {
                                             ID = us.ID,
                                             OrderID = us.OrderID,
                                             Order = new OrderDTO {
                                                 ID = us.OrderID == null ? -1 : us.Orders.ID,
                                                 OrderTypeID = us.OrderID == null ? -1 : us.Orders.OrderTypeID,
                                                 RegNo = us.OrderID == null ? "" : us.Orders.RegNo,
                                                 Customer1ID = us.OrderID == null ? -1 : us.Orders.Customer1ID,
                                                 Customer2ID = us.OrderID == null ? -1 : us.Orders.Customer2ID,
                                                 EventID = us.OrderID == null ? -1 : us.Orders.EventID
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
                                             EnteredByUser = us.EnteredByUser
                                             // }).Where(o => (o.DateTimeStartOrder.HasValue ? DbFunctions.TruncateTime(o.DateTimeStartOrder.Value) >= DbFunctions.TruncateTime(fromDate) : false) && (o.DateTimeStartOrder.HasValue ? DbFunctions.TruncateTime(o.DateTimeStartOrder.Value) <= DbFunctions.TruncateTime(toDate) : false) && o.CustomerID == customerID && selectedJobs.Contains(o.JobID.ToString()) && (o.CostActual != null && o.CostActual >= 0) && !invoicedTasks.Contains(o.ID)).OrderBy(o => o.OrderDate).ToList();
                                             // }).Where(o => (o.DateTimeStartActual.HasValue ? DbFunctions.TruncateTime(o.DateTimeStartActual.Value) >= DbFunctions.TruncateTime(fromDate) : false) && (o.DateTimeStartActual.HasValue ? DbFunctions.TruncateTime(o.DateTimeStartActual.Value) <= DbFunctions.TruncateTime(toDate) : false) && o.CustomerID == customerID && selectedJobs.Contains(o.JobID.ToString()) && !invoicedTasks.Contains(o.ID) && o.OrderID == null).OrderBy(o => o.OrderDate).ToList();
                                         }).Where(o => (o.DateTimeStartActual.HasValue ? DbFunctions.TruncateTime(o.DateTimeStartActual.Value) >= DbFunctions.TruncateTime(fromDate) : false) && (o.DateTimeStartActual.HasValue ? DbFunctions.TruncateTime(o.DateTimeStartActual.Value) <= DbFunctions.TruncateTime(toDate) : false) && o.CustomerID == customerID && o.OrderID == null).OrderBy(o => o.OrderDate).ToList();
                    List<TaskB> data = data1.Where(o => selectedJobs.Contains(o.JobID.ToString()) && !invoicedTasks.Contains(o.ID)).ToList();
                    return data;
                }
                catch (Exception ex) { appendError(ex, "GetTasksForInvoice Error"); return null; }
            }
        }

        public List<TaskB> GetTasksForInvoice(int customerID, DateTime fromDate, DateTime toDate, List<string> selectedJobs, List<string> selectedTasks) {
            using (var dbContext = new OTERTConnStr()) {
                try {
                    dbContext.Configuration.ProxyCreationEnabled = false;
                    List<int> invoicedTasks = (from us in dbContext.TasksLine select us.TaskID).ToList();
                    List<TaskB> data1 = (from us in dbContext.Tasks
                                         select new TaskB {
                                             ID = us.ID,
                                             OrderID = us.OrderID,
                                             Order = new OrderDTO {
                                                 ID = us.OrderID == null ? -1 : us.Orders.ID,
                                                 OrderTypeID = us.OrderID == null ? -1 : us.Orders.OrderTypeID,
                                                 RegNo = us.OrderID == null ? "" : us.Orders.RegNo,
                                                 Customer1ID = us.OrderID == null ? -1 : us.Orders.Customer1ID,
                                                 Customer2ID = us.OrderID == null ? -1 : us.Orders.Customer2ID,
                                                 EventID = us.OrderID == null ? -1 : us.Orders.EventID
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
                                             EnteredByUser = us.EnteredByUser
                                             // }).Where(o => (o.DateTimeStartOrder.HasValue ? DbFunctions.TruncateTime(o.DateTimeStartOrder.Value) >= DbFunctions.TruncateTime(fromDate) : false) && (o.DateTimeStartOrder.HasValue ? DbFunctions.TruncateTime(o.DateTimeStartOrder.Value) <= DbFunctions.TruncateTime(toDate) : false) && o.CustomerID == customerID && selectedJobs.Contains(o.JobID.ToString()) && selectedTasks.Contains(o.ID.ToString()) && (o.CostActual != null && o.CostActual >= 0) && !invoicedTasks.Contains(o.ID)).OrderBy(o => o.OrderDate).ToList();
                                             // }).Where(o => (o.DateTimeStartOrder.HasValue ? DbFunctions.TruncateTime(o.DateTimeStartOrder.Value) >= DbFunctions.TruncateTime(fromDate) : false) && (o.DateTimeStartOrder.HasValue ? DbFunctions.TruncateTime(o.DateTimeStartOrder.Value) <= DbFunctions.TruncateTime(toDate) : false) && o.CustomerID == customerID && selectedJobs.Contains(o.JobID.ToString()) && selectedTasks.Contains(o.ID.ToString()) && !invoicedTasks.Contains(o.ID) && o.OrderID == null).OrderBy(o => o.OrderDate).ToList();
                                         }).Where(o => (o.DateTimeStartOrder.HasValue ? DbFunctions.TruncateTime(o.DateTimeStartOrder.Value) >= DbFunctions.TruncateTime(fromDate) : false) && (o.DateTimeStartOrder.HasValue ? DbFunctions.TruncateTime(o.DateTimeStartOrder.Value) <= DbFunctions.TruncateTime(toDate) : false) && o.CustomerID == customerID && o.OrderID == null).OrderBy(o => o.OrderDate).ToList();
                    List<TaskB> data = data1.Where(o => selectedJobs.Contains(o.JobID.ToString()) && selectedTasks.Contains(o.ID.ToString()) && !invoicedTasks.Contains(o.ID)).ToList();
                    return data;
                }
                catch (Exception) { return null; }
            }
        }

        public List<TaskB> GetTasks(int recSkip, int recTake) {
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
                                                EventID = us.OrderID == null ? -1 : us.Orders.EventID
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
                                            EnteredByUser = us.EnteredByUser
                                        }).OrderBy(o => o.OrderDate).Skip(recSkip).Take(recTake).ToList();
                    return data;
                }
                catch (Exception) { return null; }
            }
        }

        public List<TaskB> GetTasksForOrder(int OrderID) {
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
                                                EventID = us.OrderID == null ? -1 : us.Orders.EventID
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
                                        }).Where(k => k.OrderID == OrderID).OrderByDescending(o => o.OrderDate).ToList();
                    return data;
                }
                catch (Exception) { return null; }
            }
        }

        public List<TaskB> GetTasksForPage(int PageID, int recSkip, int recTake, string recFilter, GridSortExpressionCollection gridSortExxpressions) {
            using (var dbContext = new OTERTConnStr()) {
                try {
                    dbContext.Configuration.ProxyCreationEnabled = false;
                    IQueryable<TaskB> datatmp = (from us in dbContext.Tasks
                                                 select new TaskB {
                                                     ID = us.ID,
                                                     OrderID = us.OrderID,
                                                     Order = new OrderDTO {
                                                         ID = us.OrderID == null ? -1 : us.Orders.ID,
                                                         OrderTypeID = us.OrderID == null ? -1 : us.Orders.OrderTypeID,
                                                         RegNo = us.OrderID == null ? "" : us.Orders.RegNo,
                                                         Customer1ID = us.OrderID == null ? -1 : us.Orders.Customer1ID,
                                                         Customer2ID = us.OrderID == null ? -1 : us.Orders.Customer2ID,
                                                         EventID = us.OrderID == null ? -1 : us.Orders.EventID
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
                                                     EnteredByUser = us.EnteredByUser
                                                 });
                    if (!string.IsNullOrEmpty(recFilter)) {
                        IQueryable test = dbContext.Tasks.Where(o => o.Jobs.JobsMain.PageID == PageID && o.OrderID == null);
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
                        List<string> StartOrderExpressions = columnExpressions.Where(item => item.Contains("DateTimeStartOrder")).ToList();
                        List<string> StartActualExpressions = columnExpressions.Where(item => item.Contains("DateTimeStartActual")).ToList();
                        columnExpressions.RemoveAll(item => item.Contains("OrderDate") || item.Contains("DateTimeStartActual") || item.Contains("DateTimeStartOrder"));
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
                        if (StartOrderExpressions.Count > 0) {
                            List<DateTime> startOrderDates = new List<DateTime>();
                            foreach (string dtExpression in StartOrderExpressions) {
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
                                if (StartOrderExpressions[0].Contains(">=")) {
                                    recFilter = "DateTimeStartOrder >= @0 AND DateTimeStartOrder <= @1";
                                } else {
                                    recFilter = "DateTimeStartOrder < @0 OR DateTimeStartOrder > @1";
                                }
                                datatmp = datatmp.Where(recFilter, new DateTime(startOrderDates[0].Year, startOrderDates[0].Month, startOrderDates[0].Day, startOrderDates[0].Hour, startOrderDates[0].Minute, 0), new DateTime(startOrderDates[1].Year, startOrderDates[1].Month, startOrderDates[1].Day, startOrderDates[1].Hour, startOrderDates[1].Minute, 0));
                            } else {
                                datatmp = datatmp.Where(StartOrderExpressions[0]);
                            }
                        }
                    }
                    datatmp = datatmp.Where(k => k.Job.JobsMain.PageID == PageID && k.OrderID == null);
                    if (gridSortExxpressions.Count > 0) {
                        string sortFieldName = "";
                        if (gridSortExxpressions[0].FieldName == "CustomerID") { sortFieldName = "Customer.NameGR"; }
                        else if (gridSortExxpressions[0].FieldName == "JobID") { sortFieldName = "Job.Name"; }
                        else if (gridSortExxpressions[0].FieldName == "DistanceID") { sortFieldName = "Distance.Description"; } else { sortFieldName = gridSortExxpressions[0].FieldName; }
                        datatmp = datatmp.OrderBy(sortFieldName + " " + gridSortExxpressions[0].SortOrder);
                    } else {
                        datatmp = datatmp.OrderByDescending(o => o.ID);
                    }
                    List<TaskB> data = datatmp.Skip(recSkip).Take(recTake).ToList();
                    foreach (TaskB curTask in data) {
                        curTask.Files = (from us in dbContext.Files
                                         select new FileB {
                                             ID = us.ID,
                                             TaskID = us.TaskID,
                                             OrderID = us.OrderID,
                                             FileName = us.FileName,
                                             FilePath = us.FilePath,
                                             DateStamp = us.DateStamp
                                         }).Where(k => k.TaskID == curTask.ID && k.OrderID == null).OrderBy(o => o.DateStamp).ToList();
                    }
                    return data;
                }
                catch (Exception) { return null; }
            }
        }

        public List<TaskB> GetAllTasks(int recSkip, int recTake, string recFilter, GridSortExpressionCollection gridSortExxpressions) {
            using (var dbContext = new OTERTConnStr()) {
                try {
                    dbContext.Configuration.ProxyCreationEnabled = false;
                    IQueryable<TaskB> datatmp = (from us in dbContext.Tasks
                                                 select new TaskB {
                                                     ID = us.ID,
                                                     OrderID = us.OrderID,
                                                     Order = new OrderDTO {
                                                         ID = us.OrderID == null ? -1 : us.Orders.ID,
                                                         OrderTypeID = us.OrderID == null ? -1 : us.Orders.OrderTypeID,
                                                         RegNo = us.OrderID == null ? "" : us.Orders.RegNo,
                                                         Customer1ID = us.OrderID == null ? -1 : us.Orders.Customer1ID,
                                                         Customer2ID = us.OrderID == null ? -1 : us.Orders.Customer2ID,
                                                         EventID = us.OrderID == null ? -1 : us.Orders.EventID
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
                                                     EnteredByUser = us.EnteredByUser
                                                 }).Where(o => o.OrderID == null);
                    if (!string.IsNullOrEmpty(recFilter)) {
                        //IQueryable test = dbContext.Tasks.Where(o => o.OrderID == null);
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
                        List<string> StartOrderExpressions = columnExpressions.Where(item => item.Contains("DateTimeStartOrder")).ToList();
                        List<string> StartActualExpressions = columnExpressions.Where(item => item.Contains("DateTimeStartActual")).ToList();
                        columnExpressions.RemoveAll(item => item.Contains("OrderDate") || item.Contains("DateTimeStartActual") || item.Contains("DateTimeStartOrder"));
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
                        if (StartOrderExpressions.Count > 0) {
                            List<DateTime> startOrderDates = new List<DateTime>();
                            foreach (string dtExpression in StartOrderExpressions) {
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
                                if (StartOrderExpressions[0].Contains(">=")) {
                                    recFilter = "DateTimeStartOrder >= @0 AND DateTimeStartOrder <= @1";
                                } else {
                                    recFilter = "DateTimeStartOrder < @0 OR DateTimeStartOrder > @1";
                                }
                                datatmp = datatmp.Where(recFilter, new DateTime(startOrderDates[0].Year, startOrderDates[0].Month, startOrderDates[0].Day, startOrderDates[0].Hour, startOrderDates[0].Minute, 0), new DateTime(startOrderDates[1].Year, startOrderDates[1].Month, startOrderDates[1].Day, startOrderDates[1].Hour, startOrderDates[1].Minute, 0));
                            } else {
                                datatmp = datatmp.Where(StartOrderExpressions[0]);
                            }
                        }
                    }
                    //datatmp = datatmp.Where(k => k.OrderID == null);
                    if (gridSortExxpressions.Count > 0) {
                        string sortFieldName = "";
                        if (gridSortExxpressions[0].FieldName == "CustomerID") { sortFieldName = "Customer.NameGR"; }
                        else if (gridSortExxpressions[0].FieldName == "JobID") { sortFieldName = "Job.Name"; }
                        else if (gridSortExxpressions[0].FieldName == "DistanceID") { sortFieldName = "Distance.Description"; }
                        else { sortFieldName = gridSortExxpressions[0].FieldName; }
                        datatmp = datatmp.OrderBy(sortFieldName + " " + gridSortExxpressions[0].SortOrder);
                    } else {
                        datatmp = datatmp.OrderByDescending(o => o.ID);
                    }
                    List<TaskB> data = datatmp.Skip(recSkip).Take(recTake).ToList();
                    return data;
                }
                catch (Exception) { return null; }
            }
        }

        public List<TaskB> GetAllTasksforPTStoAbroad(int recSkip, int recTake, string recFilter, GridSortExpressionCollection gridSortExxpressions) {
            using (var dbContext = new OTERTConnStr()) {
                try {
                    dbContext.Configuration.ProxyCreationEnabled = false;
                    IQueryable<TaskB> datatmp = (from us in dbContext.Tasks
                                                 select new TaskB {
                                                     ID = us.ID,
                                                     OrderID = us.OrderID,
                                                     Order = new OrderDTO {
                                                         ID = us.OrderID == null ? -1 : us.Orders.ID,
                                                         OrderTypeID = us.OrderID == null ? -1 : us.Orders.OrderTypeID,
                                                         RegNo = us.OrderID == null ? "" : us.Orders.RegNo,
                                                         Customer1ID = us.OrderID == null ? -1 : us.Orders.Customer1ID,
                                                         Customer1 = us.OrderID == null ? null : new CustomerDTO {
                                                             ID = us.Orders.Customers1.ID,
                                                             CountryID = us.Orders.Customers1.CountryID,
                                                             NameGR = us.Orders.Customers1.NameGR,
                                                             NameEN = us.Orders.Customers1.NameEN,
                                                             NamedInvoiceGR = us.Orders.Customers1.NamedInvoiceGR,
                                                             NamedInvoiceEN = us.Orders.Customers1.NamedInvoiceEN,
                                                             ZIPCode = us.Orders.Customers1.ZIPCode,
                                                             CityGR = us.Orders.Customers1.CityGR,
                                                             CityEN = us.Orders.Customers1.CityEN,
                                                             ChargeTelephone = us.Orders.Customers1.ChargeTelephone,
                                                             Telephone1 = us.Orders.Customers1.Telephone1,
                                                             Telephone2 = us.Orders.Customers1.Telephone2,
                                                             FAX1 = us.Orders.Customers1.FAX1,
                                                             FAX2 = us.Orders.Customers1.FAX2,
                                                             Address1GR = us.Orders.Customers1.Address1GR,
                                                             Address1EN = us.Orders.Customers1.Address1EN,
                                                             Address2GR = us.Orders.Customers1.Address2GR,
                                                             Address2EN = us.Orders.Customers1.Address2EN,
                                                             ContactPersonGR = us.Orders.Customers1.ContactPersonGR,
                                                             ContactPersonEN = us.Orders.Customers1.ContactPersonEN,
                                                             CustomerTypeID = us.Orders.Customers1.CustomerTypeID,
                                                             LanguageID = us.Orders.Customers1.LanguageID,
                                                             Email = us.Orders.Customers1.Email,
                                                             URL = us.Orders.Customers1.URL,
                                                             AFM = us.Orders.Customers1.AFM,
                                                             DOY = us.Orders.Customers1.DOY,
                                                             SAPCode = us.Orders.Customers1.SAPCode,
                                                             UserID = us.Orders.Customers1.UserID,
                                                             Comments = us.Orders.Customers1.Comments,
                                                             IsProvider = us.Orders.Customers1.IsProvider,
                                                             IsOTE = us.Orders.Customers1.IsOTE
                                                         },
                                                         EventID = us.Orders.EventID,
                                                         Event = new EventDTO {
                                                             ID = us.OrderID == null ? -1 : us.Orders.Events.ID,
                                                             PlaceID = us.OrderID == null ? -1 : us.Orders.Events.PlaceID,
                                                             Place = new PlaceDTO {
                                                                 ID = us.OrderID == null ? -1 : us.Orders.Events.Places.ID,
                                                                 CountryID = us.OrderID == null ? -1 : us.Orders.Events.Places.CountryID,
                                                                 Country = new CountryDTO {
                                                                     ID = us.OrderID == null ? -1 : us.Orders.Events.Places.Countries.ID,
                                                                     NameGR = us.OrderID == null ? "" : us.Orders.Events.Places.Countries.NameGR,
                                                                     NameEN = us.OrderID == null ? "" : us.Orders.Events.Places.Countries.NameEN
                                                                 },
                                                                 NameGR = us.OrderID == null ? "" : us.Orders.Events.Places.NameGR,
                                                                 NameEN = us.OrderID == null ? "" : us.Orders.Events.Places.NameEN
                                                             },
                                                             NameGR = us.OrderID == null ? "" : us.Orders.Events.NameGR,
                                                             NameEN = us.OrderID == null ? "" : us.Orders.Events.NameEN
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
                                                     EnteredByUser = us.EnteredByUser
                                                 }).Where(o => o.OrderID != null);
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
                        List<string> PaymentDateOrderExpressions = columnExpressions.Where(item => item.Contains("PaymentDateOrder")).ToList();
                        List<string> PaymentDateCalculatedExpressions = columnExpressions.Where(item => item.Contains("PaymentDateCalculated")).ToList();
                        columnExpressions.RemoveAll(item => item.Contains("OrderDate") || item.Contains("DateTimeStartActual") || item.Contains("PaymentDateOrder") || item.Contains("PaymentDateCalculated"));
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
                        if (PaymentDateOrderExpressions.Count > 0) {
                            List<DateTime> startOrderDates = new List<DateTime>();
                            foreach (string dtExpression in PaymentDateOrderExpressions) {
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
                                if (PaymentDateOrderExpressions[0].Contains(">=")) {
                                    recFilter = "PaymentDateOrder >= @0 AND PaymentDateOrder <= @1";
                                } else {
                                    recFilter = "PaymentDateOrder < @0 OR PaymentDateOrder > @1";
                                }
                                datatmp = datatmp.Where(recFilter, new DateTime(startOrderDates[0].Year, startOrderDates[0].Month, startOrderDates[0].Day, startOrderDates[0].Hour, startOrderDates[0].Minute, 0), new DateTime(startOrderDates[1].Year, startOrderDates[1].Month, startOrderDates[1].Day, startOrderDates[1].Hour, startOrderDates[1].Minute, 0));
                            } else {
                                datatmp = datatmp.Where(PaymentDateOrderExpressions[0]);
                            }
                        }
                        if (PaymentDateCalculatedExpressions.Count > 0) {
                            List<DateTime> startOrderDates = new List<DateTime>();
                            foreach (string dtExpression in PaymentDateCalculatedExpressions) {
                                string[] dateExp = dtExpression.Split(new char[] { '"' });
                                string format = "d/M/yyyy,h:mm:ss,tt";
                                DateTime newDate;
                                if (dateExp.Length > 1) {
                                    if (DateTime.TryParseExact(dateExp[1], format, greek, System.Globalization.DateTimeStyles.None, out newDate)) {
                                        startOrderDates.Add(newDate);
                                    }
                                }
                            }
                            if (PaymentDateCalculatedExpressions.Count == 2) {
                                if (!string.IsNullOrEmpty(recFilter)) { recFilter += " AND "; }
                                if (PaymentDateOrderExpressions[0].Contains(">=")) {
                                    recFilter = "PaymentDateOrder >= @0 AND PaymentDateOrder <= @1";
                                } else {
                                    recFilter = "PaymentDateOrder < @0 OR PaymentDateOrder > @1";
                                }
                                datatmp = datatmp.Where(recFilter, new DateTime(startOrderDates[0].Year, startOrderDates[0].Month, startOrderDates[0].Day, startOrderDates[0].Hour, startOrderDates[0].Minute, 0), new DateTime(startOrderDates[1].Year, startOrderDates[1].Month, startOrderDates[1].Day, startOrderDates[1].Hour, startOrderDates[1].Minute, 0));
                            } else {
                                datatmp = datatmp.Where(PaymentDateCalculatedExpressions[0]);
                            }
                        }
                    }
                    if (gridSortExxpressions.Count > 0) {
                        string sortFieldName = "";
                        if (gridSortExxpressions[0].FieldName == "Order.RegNo") { sortFieldName = "Order.RegNo"; }
                        else if (gridSortExxpressions[0].FieldName == "Order.Event.Place.CountryID") { sortFieldName = "Order.Event.Place.Country.NameGR"; }
                        else if (gridSortExxpressions[0].FieldName == "Order.Customer1ID") { sortFieldName = "Order.Customer1.NameGR"; }
                        else if (gridSortExxpressions[0].FieldName == "Order.EventID") { sortFieldName = "Order.Event.NameGR"; }
                        else if (gridSortExxpressions[0].FieldName == "CustomerID") { sortFieldName = "Customer.NameGR"; }
                        else { sortFieldName = gridSortExxpressions[0].FieldName; }
                        datatmp = datatmp.OrderBy(sortFieldName + " " + gridSortExxpressions[0].SortOrder);
                    } else {
                        datatmp = datatmp.OrderByDescending(o => o.OrderID).ThenByDescending(o => o.ID);
                    }
                    List<TaskB> data = datatmp.Skip(recSkip).Take(recTake).ToList();
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