using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using OTERT.Model;
using OTERT_Entity;

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

        public int CountTasksForPageID(int PageID) {
            using (var dbContext = new OTERTConnStr()) {
                try {
                    return dbContext.Tasks.Where(o => o.Jobs.JobsMain.PageID == PageID).Count();
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
                                            LineType = new LineTypeDTO { ID = us.LineTypes.ID, Name = us.LineTypes.Name }
                                        }).OrderBy(o => o.OrderDate).ToList();
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
                                            LineType = new LineTypeDTO { ID = us.LineTypes.ID, Name = us.LineTypes.Name }
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
                                            LineType = new LineTypeDTO { ID = us.LineTypes.ID, Name = us.LineTypes.Name }
                                        }).Where(k => k.OrderID == OrderID).OrderBy(o => o.OrderDate).ToList();
                    return data;
                }
                catch (Exception) { return null; }
            }
        }

        public List<TaskB> GetTasksForPage(int PageID, int recSkip, int recTake) {
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
                                            LineType = new LineTypeDTO { ID = us.LineTypes.ID, Name = us.LineTypes.Name }
                                        }).Where(k => k.Job.JobsMain.PageID == PageID).OrderBy(o => o.OrderDate).Skip(recSkip).Take(recTake).ToList();
                    foreach(TaskB curTask in data) {
                        curTask.Files = (from us in dbContext.Files
                                        select new FileB {
                                            ID = us.ID,
                                            TaskID = us.TaskID,
                                            OrderID = us.OrderID,
                                            FileName = us.FileName,
                                            FilePath = us.FilePath,
                                            DateStamp = us.DateStamp
                                        }).Where(k => k.TaskID == curTask.ID).OrderBy(o => o.DateStamp).ToList();
                    }
                    return data;
                }
                catch (Exception) { return null; }
            }
        }

    }

}