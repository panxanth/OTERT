using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Web;
using OTERT.Model;
using OTERT_Entity;
using Telerik.Web.Data.Extensions;
using Telerik.Web.UI;

namespace OTERT.Controller {

    public class FilesController {

        public int CountFiles(int taskID) {
            using (var dbContext = new OTERTConnStr()) {
                try {
                    return dbContext.Files.Where(k => k.TaskID == taskID).Count();
                }
                catch (Exception) { return -1; }
            }
        }

        public int CountFiles(string recFilter) {
            using (var dbContext = new OTERTConnStr()) {
                try {
                    int count = 0;
                    dbContext.Configuration.ProxyCreationEnabled = false;
                    if (!string.IsNullOrEmpty(recFilter)) {
                        IQueryable<File4ListB> test = (from us in dbContext.Files
                                                          select new File4ListB {
                                                              ID = us.ID,
                                                              OrderID = us.OrderID,
                                                              TaskID = us.TaskID,
                                                              CustomerID = us.CustomerID != null ? us.CustomerID : (us.Tasks.CustomerID > 0 ? us.Tasks.CustomerID : (us.Orders.Customer1ID > 0 ? us.Orders.Customer1ID : -1)),
                                                              CustomerName = us.CustomerID != null ? us.Customers.NameGR : (us.Tasks.CustomerID > 0 ? us.Tasks.Customers.NameGR : (us.Orders.Customer1ID > 0 ? us.Orders.Customers1.NameGR : "")),
                                                              FilePath = us.FilePath,
                                                              FileName = us.FileName,
                                                              DateStamp = us.DateStamp
                                                          });
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

                        List<string> DateStampExpressions = columnExpressions.Where(item => item.Contains("DateStamp")).ToList();
                        columnExpressions.RemoveAll(item => item.Contains("DateStamp"));
                        recFilter = string.Join("AND", columnExpressions.ToArray());
                        if (!string.IsNullOrEmpty(recFilter)) { test = test.Where(recFilter); }
                        if (DateStampExpressions.Count > 0) {
                            List<DateTime> orderDates = new List<DateTime>();
                            foreach (string dtExpression in DateStampExpressions) {
                                string[] dateExp = dtExpression.Split(new char[] { '"' });
                                string format = "M/d/yyyy,h:mm:ss,tt";
                                DateTime newDate;
                                if (dateExp.Length > 1) {
                                    if (DateTime.TryParseExact(dateExp[1], format, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out newDate)) {
                                        orderDates.Add(newDate);
                                    }
                                }
                            }
                            if (orderDates.Count == 2) {
                                if (!string.IsNullOrEmpty(recFilter)) { recFilter += " AND "; }
                                if (DateStampExpressions[0].Contains(">=")) {
                                    recFilter = "DateStamp >= @0 AND DateStamp <= @1";
                                } else {
                                    recFilter = "DateStamp < @0 OR DateStamp > @1";
                                }
                                test = test.Where(recFilter, new DateTime(orderDates[0].Year, orderDates[0].Month, orderDates[0].Day, orderDates[0].Hour, orderDates[0].Minute, 0), new DateTime(orderDates[1].Year, orderDates[1].Month, orderDates[1].Day, orderDates[1].Hour, orderDates[1].Minute, 0));
                            } else {
                                test = test.Where(DateStampExpressions[0]);
                            }
                        }
                        count = test.Count();
                    } else {
                        count = dbContext.Files.Count();
                    }
                    return count;
                }
                catch (Exception ex) { return -1; }
            }
        }

        public int CountFilesByOrderID(int orderID) {
            using (var dbContext = new OTERTConnStr()) {
                try {
                    return dbContext.Files.Where(k => k.OrderID == orderID).Count();
                }
                catch (Exception) { return -1; }
            }
        }

        public int CountFilesByCustomerID(int customerID) {
            using (var dbContext = new OTERTConnStr()) {
                try {
                    return dbContext.Files.Where(k => k.CustomerID == customerID).Count();
                }
                catch (Exception) { return -1; }
            }
        }

        public List<FileB> GetFilesByTaskID(int taskID) {
            using (var dbContext = new OTERTConnStr()) {
                try {
                    dbContext.Configuration.ProxyCreationEnabled = false;
                    List<FileB> data = (from us in dbContext.Files
                                              select new FileB {
                                                  ID = us.ID,
                                                  OrderID = us.OrderID,
                                                  TaskID = us.TaskID,
                                                  CustomerID = us.CustomerID,
                                                  FilePath = us.FilePath,
                                                  FileName = us.FileName,
                                                  DateStamp = us.DateStamp
                                              }).Where(k => k.TaskID == taskID).OrderBy(o => o.DateStamp).ToList();
                    return data;
                }
                catch (Exception) { return null; }
            }
        }

        public List<FileB> GetFilesByTaskID(int taskID, int recSkip, int recTake) {
            using (var dbContext = new OTERTConnStr()) {
                try {
                    dbContext.Configuration.ProxyCreationEnabled = false;
                    List<FileB> data = (from us in dbContext.Files
                                        select new FileB {
                                            ID = us.ID,
                                            OrderID = us.OrderID,
                                            TaskID = us.TaskID,
                                            CustomerID = us.CustomerID,
                                            FilePath = us.FilePath,
                                            FileName = us.FileName,
                                            DateStamp = us.DateStamp
                                        }).Where(k => k.TaskID == taskID).OrderBy(o => o.DateStamp).Skip(recSkip).Take(recTake).ToList();
                    return data;
                }
                catch (Exception) { return null; }
            }
        }

        public List<FileB> GetFilesByOrderID(int orderID) {
            using (var dbContext = new OTERTConnStr()) {
                try {
                    dbContext.Configuration.ProxyCreationEnabled = false;
                    List<FileB> data = (from us in dbContext.Files
                                        select new FileB {
                                            ID = us.ID,
                                            OrderID = us.OrderID,
                                            TaskID = us.TaskID,
                                            CustomerID = us.CustomerID,
                                            FilePath = us.FilePath,
                                            FileName = us.FileName,
                                            DateStamp = us.DateStamp
                                        }).Where(k => k.OrderID == orderID).OrderBy(o => o.DateStamp).ToList();
                    return data;
                }
                catch (Exception) { return null; }
            }
        }

        public List<FileB> GetFilesByOrderID(int orderID, int recSkip, int recTake) {
            using (var dbContext = new OTERTConnStr()) {
                try {
                    dbContext.Configuration.ProxyCreationEnabled = false;
                    List<FileB> data = (from us in dbContext.Files
                                        select new FileB {
                                            ID = us.ID,
                                            OrderID = us.OrderID,
                                            TaskID = us.TaskID,
                                            CustomerID = us.CustomerID,
                                            FilePath = us.FilePath,
                                            FileName = us.FileName,
                                            DateStamp = us.DateStamp
                                        }).Where(k => k.OrderID == orderID).OrderBy(o => o.DateStamp).Skip(recSkip).Take(recTake).ToList();
                    return data;
                }
                catch (Exception) { return null; }
            }
        }

        public List<FileB> GetFilesByCustomerID(int customerID) {
            using (var dbContext = new OTERTConnStr()) {
                try {
                    dbContext.Configuration.ProxyCreationEnabled = false;
                    List<FileB> data = (from us in dbContext.Files
                                        select new FileB {
                                            ID = us.ID,
                                            OrderID = us.OrderID,
                                            TaskID = us.TaskID,
                                            CustomerID = us.CustomerID,
                                            FilePath = us.FilePath,
                                            FileName = us.FileName,
                                            DateStamp = us.DateStamp
                                        }).Where(k => k.CustomerID == customerID).OrderBy(o => o.DateStamp).ToList();
                    return data;
                }
                catch (Exception) { return null; }
            }
        }

        public List<FileB> GetFilesByCustomerID(int customerID, int recSkip, int recTake) {
            using (var dbContext = new OTERTConnStr()) {
                try {
                    dbContext.Configuration.ProxyCreationEnabled = false;
                    List<FileB> data = (from us in dbContext.Files
                                        select new FileB {
                                            ID = us.ID,
                                            OrderID = us.OrderID,
                                            TaskID = us.TaskID,
                                            CustomerID = us.CustomerID,
                                            FilePath = us.FilePath,
                                            FileName = us.FileName,
                                            DateStamp = us.DateStamp
                                        }).Where(k => k.CustomerID == customerID).OrderBy(o => o.DateStamp).Skip(recSkip).Take(recTake).ToList();
                    return data;
                }
                catch (Exception) { return null; }
            }
        }

        public List<File4ListB> GetFilesForList(int recSkip, int recTake, string recFilter, GridSortExpressionCollection gridSortExxpressions) {
            using (var dbContext = new OTERTConnStr()) {
                try {
                    dbContext.Configuration.ProxyCreationEnabled = false;
                    IQueryable<File4ListB> datatmp = (from us in dbContext.Files
                                                    select new File4ListB {
                                                        ID = us.ID,
                                                        OrderID = us.OrderID,
                                                        TaskID = us.TaskID,
                                                        CustomerID = us.CustomerID != null ? us.CustomerID : (us.Tasks.CustomerID > 0 ? us.Tasks.CustomerID : (us.Orders.Customer1ID > 0 ? us.Orders.Customer1ID : -1)),
                                                        CustomerName = us.CustomerID != null ? us.Customers.NameGR : (us.Tasks.CustomerID > 0 ? us.Tasks.Customers.NameGR : (us.Orders.Customer1ID > 0 ? us.Orders.Customers1.NameGR : "")),
                                                        FilePath = us.FilePath,
                                                        FileName = us.FileName,
                                                        DateStamp = us.DateStamp
                                                    });
                    if (!string.IsNullOrEmpty(recFilter)) {
                        IQueryable test = dbContext.Files;
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
                        List<string> OrderDateExpressions = columnExpressions.Where(item => item.Contains("DateStamp")).ToList();
                        columnExpressions.RemoveAll(item => item.Contains("DateStamp"));
                        recFilter = string.Join("AND", columnExpressions.ToArray());
                        if (!string.IsNullOrEmpty(recFilter)) { datatmp = datatmp.Where(recFilter); }
                        if (OrderDateExpressions.Count > 0) {
                            List<DateTime> orderDates = new List<DateTime>();
                            foreach (string dtExpression in OrderDateExpressions) {
                                string[] dateExp = dtExpression.Split(new char[] { '"' });
                                string format = "M/d/yyyy,h:mm:ss,tt";
                                DateTime newDate;
                                if (dateExp.Length > 1) {
                                    if (DateTime.TryParseExact(dateExp[1], format, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out newDate)) {
                                        orderDates.Add(newDate);
                                    }
                                }
                            }
                            if (orderDates.Count == 2) {
                                if (!string.IsNullOrEmpty(recFilter)) { recFilter += " AND "; }
                                if (OrderDateExpressions[0].Contains(">=")) {
                                    recFilter = "DateStamp >= @0 AND DateStamp <= @1";
                                } else {
                                    recFilter = "DateStamp < @0 OR DateStamp > @1";
                                }
                                datatmp = datatmp.Where(recFilter, new DateTime(orderDates[0].Year, orderDates[0].Month, orderDates[0].Day, orderDates[0].Hour, orderDates[0].Minute, 0), new DateTime(orderDates[1].Year, orderDates[1].Month, orderDates[1].Day, orderDates[1].Hour, orderDates[1].Minute, 0));
                            } else {
                                datatmp = datatmp.Where(OrderDateExpressions[0]);
                            }
                        }
                    }
                    if (gridSortExxpressions.Count > 0) {
                        datatmp = datatmp.OrderBy(gridSortExxpressions[0].FieldName + " " + gridSortExxpressions[0].SortOrder);
                    } else {
                        datatmp = datatmp.OrderByDescending(o => o.DateStamp);
                    }
                    List<File4ListB> data = datatmp.Skip(recSkip).Take(recTake).ToList();
                    return data;
                }
                catch (Exception) { return null; }
            }
        }

    }

}