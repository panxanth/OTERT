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

        public List<TaskB> GetTasks() {
            using (var dbContext = new OTERTConnStr()) {
                try {
                    dbContext.Configuration.ProxyCreationEnabled = false;
                    List<TaskB> data = (from us in dbContext.Tasks
                                        select new TaskB {
                                            ID = us.ID,
                                            OrderID = us.OrderID,
                                            Order = us.OrderID == null ? null : new OrderDTO {
                                                ID = us.Orders.ID,
                                                OrderTypeID = us.Orders.OrderTypeID,
                                                OrderType = null,
                                                RegNo = us.Orders.RegNo,
                                                Customer1ID = us.Orders.Customer1ID,
                                                Customer1 = null,
                                                Customer2ID = us.Orders.Customer2ID,
                                                Customer2 = null,
                                                EventID = us.Orders.EventID,
                                                Event = null
                                            },
                                            RegNo = us.RegNo,
                                            OrderDate = us.OrderDate,
                                            CustomerID = us.CustomerID,
                                            Customer = new CustomerDTO {
                                                ID = us.Customers.ID,
                                                CountryID = us.Customers.CountryID,
                                                Country = null,
                                                NameGR = us.Customers.NameGR,
                                                NameEN = us.Customers.NameEN,
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
                                                CustomerType = null,
                                                LanguageID = us.Customers.LanguageID,
                                                Language = null,
                                                Email = us.Customers.Email,
                                                URL = us.Customers.URL,
                                                AFM = us.Customers.AFM,
                                                DOY = us.Customers.DOY,
                                                UserID = us.Customers.UserID,
                                                User = null,
                                                Comments = us.Customers.Comments,
                                                IsProvider = us.Customers.IsProvider
                                            },
                                            RequestedPositionID = us.RequestedPositionID,
                                            RequestedPosition = us.RequestedPositionID == null ? null : new RequestedPositionDTO {
                                                ID = us.RequestedPositions.ID,
                                                NameGR = us.RequestedPositions.NameGR,
                                                NameEN = us.RequestedPositions.NameEN
                                            },
                                            JobID = us.JobID,
                                            Job = us.JobID == null ? null : new JobDTO {
                                                ID = us.Jobs.ID,
                                                JobsMainID = us.Jobs.JobsMainID,
                                                JobsMain = null,
                                                Name = us.Jobs.Name,
                                                MinimumTime = us.Jobs.MinimumTime,
                                                InvoiceCode = us.Jobs.InvoiceCode,
                                                SalesID = us.Jobs.SalesID,
                                                Sale = null
                                            },
                                            PlaceFrom = us.PlaceFrom,
                                            PlaceTo = us.PlaceTo,
                                            PlaceDistanceKm = us.PlaceDistanceKm,
                                            DateTimeStartOrder = us.DateTimeStartOrder,
                                            DateTimeEndOrder = us.DateTimeEndOrder,
                                            DateTimeDurationOrder = us.DateTimeDurationOrder,
                                            IsDurationInDays = us.IsDurationInDays == null ? false : (bool)us.IsDurationInDays,
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
                                            SateliteID = us.SateliteID
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
                                            Order = us.OrderID == null ? null : new OrderDTO {
                                                ID = us.Orders.ID,
                                                OrderTypeID = us.Orders.OrderTypeID,
                                                OrderType = null,
                                                RegNo = us.Orders.RegNo,
                                                Customer1ID = us.Orders.Customer1ID,
                                                Customer1 = null,
                                                Customer2ID = us.Orders.Customer2ID,
                                                Customer2 = null,
                                                EventID = us.Orders.EventID,
                                                Event = null
                                            },
                                            RegNo = us.RegNo,
                                            OrderDate = us.OrderDate,
                                            CustomerID = us.CustomerID,
                                            Customer = new CustomerDTO {
                                                ID = us.Customers.ID,
                                                CountryID = us.Customers.CountryID,
                                                Country = null,
                                                NameGR = us.Customers.NameGR,
                                                NameEN = us.Customers.NameEN,
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
                                                CustomerType = null,
                                                LanguageID = us.Customers.LanguageID,
                                                Language = null,
                                                Email = us.Customers.Email,
                                                URL = us.Customers.URL,
                                                AFM = us.Customers.AFM,
                                                DOY = us.Customers.DOY,
                                                UserID = us.Customers.UserID,
                                                User = null,
                                                Comments = us.Customers.Comments,
                                                IsProvider = us.Customers.IsProvider
                                            },
                                            RequestedPositionID = us.RequestedPositionID,
                                            RequestedPosition = us.RequestedPositionID == null ? null : new RequestedPositionDTO {
                                                ID = us.RequestedPositions.ID,
                                                NameGR = us.RequestedPositions.NameGR,
                                                NameEN = us.RequestedPositions.NameEN
                                            },
                                            JobID = us.JobID,
                                            Job = us.JobID == null ? null : new JobDTO {
                                                ID = us.Jobs.ID,
                                                JobsMainID = us.Jobs.JobsMainID,
                                                JobsMain = null,
                                                Name = us.Jobs.Name,
                                                MinimumTime = us.Jobs.MinimumTime,
                                                InvoiceCode = us.Jobs.InvoiceCode,
                                                SalesID = us.Jobs.SalesID,
                                                Sale = null
                                            },
                                            PlaceFrom = us.PlaceFrom,
                                            PlaceTo = us.PlaceTo,
                                            PlaceDistanceKm = us.PlaceDistanceKm,
                                            DateTimeStartOrder = us.DateTimeStartOrder,
                                            DateTimeEndOrder = us.DateTimeEndOrder,
                                            DateTimeDurationOrder = us.DateTimeDurationOrder,
                                            IsDurationInDays = us.IsDurationInDays == null ? false : (bool)us.IsDurationInDays,
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
                                            SateliteID = us.SateliteID
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
                                            Order = us.OrderID == null ? null : new OrderDTO {
                                                ID = us.Orders.ID,
                                                OrderTypeID = us.Orders.OrderTypeID,
                                                OrderType = null,
                                                RegNo = us.Orders.RegNo,
                                                Customer1ID = us.Orders.Customer1ID,
                                                Customer1 = null,
                                                Customer2ID = us.Orders.Customer2ID,
                                                Customer2 = null,
                                                EventID = us.Orders.EventID,
                                                Event = null
                                            },
                                            RegNo = us.RegNo,
                                            OrderDate = us.OrderDate,
                                            CustomerID = us.CustomerID,
                                            Customer = new CustomerDTO {
                                                ID = us.Customers.ID,
                                                CountryID = us.Customers.CountryID,
                                                Country = null,
                                                NameGR = us.Customers.NameGR,
                                                NameEN = us.Customers.NameEN,
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
                                                CustomerType = null,
                                                LanguageID = us.Customers.LanguageID,
                                                Language = null,
                                                Email = us.Customers.Email,
                                                URL = us.Customers.URL,
                                                AFM = us.Customers.AFM,
                                                DOY = us.Customers.DOY,
                                                UserID = us.Customers.UserID,
                                                User = null,
                                                Comments = us.Customers.Comments,
                                                IsProvider = us.Customers.IsProvider
                                            },
                                            RequestedPositionID = us.RequestedPositionID,
                                            RequestedPosition = us.RequestedPositionID == null ? null : new RequestedPositionDTO {
                                                ID = us.RequestedPositions.ID,
                                                NameGR = us.RequestedPositions.NameGR,
                                                NameEN = us.RequestedPositions.NameEN
                                            },
                                            JobID = us.JobID,
                                            Job = us.JobID == null ? null : new JobDTO {
                                                ID = us.Jobs.ID,
                                                JobsMainID = us.Jobs.JobsMainID,
                                                JobsMain = null,
                                                Name = us.Jobs.Name,
                                                MinimumTime = us.Jobs.MinimumTime,
                                                InvoiceCode = us.Jobs.InvoiceCode,
                                                SalesID = us.Jobs.SalesID,
                                                Sale = null
                                            },
                                            PlaceFrom = us.PlaceFrom,
                                            PlaceTo = us.PlaceTo,
                                            PlaceDistanceKm = us.PlaceDistanceKm,
                                            DateTimeStartOrder = us.DateTimeStartOrder,
                                            DateTimeEndOrder = us.DateTimeEndOrder,
                                            DateTimeDurationOrder = us.DateTimeDurationOrder,
                                            IsDurationInDays = us.IsDurationInDays == null ? false : (bool)us.IsDurationInDays,
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
                                            SateliteID = us.SateliteID
                                        }).Where(k => k.OrderID == OrderID).OrderBy(o => o.OrderDate).ToList();
                    return data;
                }
                catch (Exception) { return null; }
            }
        }

    }

}