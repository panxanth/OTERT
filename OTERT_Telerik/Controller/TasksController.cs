using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Web;
using OTERT.Model;
using OTERT_Entity;
using System.Data.Entity;

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
                    int count = 0;
                    dbContext.Configuration.ProxyCreationEnabled = false;
                    if (!string.IsNullOrEmpty(recFilter)) {
                        count = dbContext.Tasks.Where(o => o.Jobs.JobsMain.PageID == PageID && o.OrderID == null).Where(recFilter).Count();
                    } else {
                        count = dbContext.Tasks.Count();
                    }
                    return count;
                }
                catch (Exception) { return -1; }
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
                                        }).Where(o => (o.DateTimeStartOrder.HasValue ? DbFunctions.TruncateTime(o.DateTimeStartOrder.Value) >= DbFunctions.TruncateTime(fromDate) : false) && (o.DateTimeStartOrder.HasValue ? DbFunctions.TruncateTime(o.DateTimeStartOrder.Value) <= DbFunctions.TruncateTime(toDate) : false) && o.CustomerID == customerID && selectedJobs.Contains(o.JobID.ToString()) && o.CostActual.GetValueOrDefault() > 0).OrderBy(o => o.OrderDate).ToList();
                    return data;
                }
                catch (Exception) { return null; }
            }
        }

        public List<TaskB> GetTasksForInvoice(int customerID, DateTime fromDate, DateTime toDate, List<string> selectedJobs, List<string> selectedTasks) {
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
                                        }).Where(o => (o.DateTimeStartOrder.HasValue ? DbFunctions.TruncateTime(o.DateTimeStartOrder.Value) >= DbFunctions.TruncateTime(fromDate) : false) && (o.DateTimeStartOrder.HasValue ? DbFunctions.TruncateTime(o.DateTimeStartOrder.Value) <= DbFunctions.TruncateTime(toDate) : false) && o.CustomerID == customerID && selectedJobs.Contains(o.JobID.ToString()) && selectedTasks.Contains(o.ID.ToString()) && o.CostActual.GetValueOrDefault() > 0).OrderBy(o => o.OrderDate).ToList();
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
                                            EnteredByUser = us.EnteredByUser
                                        }).Where(k => k.OrderID == OrderID).OrderBy(o => o.OrderDate).ToList();
                    return data;
                }
                catch (Exception) { return null; }
            }
        }

        public List<TaskB> GetTasksForPage(int PageID, int recSkip, int recTake, string recFilter) {
            /*
            string finalFilter = "";
            if (recFilter.Contains("DateTime.Parse(")) {
                var filters = recFilter.Split(new string[] { "DateTime.Parse(" }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < filters.Count(); i++) {
                    if (i != 0) {
                        var oldDate = filters[i].Substring(1, 22);
                        var newDate = DateTime.Parse(oldDate);
                        filters[i] = filters[i].Replace('\"' + oldDate + "\")", "DateTime(" + newDate.Year + ',' + newDate.Month + ',' + newDate.Day + ',' + newDate.Hour + ',' + newDate.Minute + ',' + newDate.Second + ")");
                    }
                    finalFilter += filters[i];
                }
            } else {
                finalFilter = recFilter;
            }
            */
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
                    if (!string.IsNullOrEmpty(recFilter)) { datatmp = datatmp.Where(recFilter); }
                    List<TaskB> data = datatmp.Where(k => k.Job.JobsMain.PageID == PageID && k.OrderID == null).OrderByDescending(o => o.ID).Skip(recSkip).Take(recTake).ToList();
                    foreach(TaskB curTask in data) {
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

    }

}