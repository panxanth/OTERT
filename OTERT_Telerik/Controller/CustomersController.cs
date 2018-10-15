using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using OTERT.Model;
using OTERT_Entity;

namespace OTERT.Controller {

    public class CustomersController {

        public int CountCustomers() {
            using (var dbContext = new OTERTConnStr()) {
                try {
                    return dbContext.Customers.Count();
                }
                catch (Exception) { return -1; }
            }
        }

        public List<CustomerB> GetCustomers() {
            using (var dbContext = new OTERTConnStr()) {
                try {
                    dbContext.Configuration.ProxyCreationEnabled = false;
                    List<CustomerB> data = (from us in dbContext.Customers
                                            select new CustomerB {
                                                ID = us.ID,
                                                CountryID = us.CountryID,
                                                Country = new CountryDTO {
                                                    ID = us.Countries.ID,
                                                    NameGR = us.Countries.NameGR,
                                                    NameEN = us.Countries.NameEN
                                                },
                                                NameGR = us.NameGR,
                                                NameEN = us.NameEN,
                                                NamedInvoiceGR = us.NamedInvoiceGR,
                                                NamedInvoiceEN = us.NamedInvoiceEN,
                                                ZIPCode = us.ZIPCode,
                                                CityGR = us.CityGR,
                                                CityEN = us.CityEN,
                                                ChargeTelephone = us.ChargeTelephone,
                                                Telephone1 = us.Telephone1,
                                                Telephone2 = us.Telephone2,
                                                FAX1 = us.FAX1,
                                                FAX2 = us.FAX2,
                                                Address1GR = us.Address1GR,
                                                Address1EN = us.Address1EN,
                                                Address2GR = us.Address2GR,
                                                Address2EN = us.Address2EN,
                                                ContactPersonGR = us.ContactPersonGR,
                                                ContactPersonEN = us.ContactPersonEN,
                                                CustomerTypeID = us.CustomerTypeID,
                                                CustomerType = new CustomerTypeDTO {
                                                    ID = us.CustomerTypes.ID,
                                                    NameGR = us.CustomerTypes.NameGR,
                                                    NameEN = us.CustomerTypes.NameEN
                                                },
                                                LanguageID = us.LanguageID,
                                                Language = new LanguageDTO {
                                                    ID = us.Languages.ID,
                                                    Name = us.Languages.Name,
                                                    Code = us.Languages.Code
                                                },
                                                Email = us.Email,
                                                URL = us.URL,
                                                AFM = us.AFM,
                                                DOY = us.DOY,
                                                SAPCode = us.SAPCode,
                                                UserID = us.UserID,
                                                User = us.UserID == null ? null : new UserDTO {
                                                    ID = us.Users.ID,
                                                    UserGroupID = us.Users.UserGroupID,
                                                    UserGroup = new UserGroupDTO {
                                                        ID = us.Users.UserGroups.ID,
                                                        Name = us.Users.UserGroups.Name
                                                    },
                                                    NameGR = us.Users.NameGR,
                                                    NameEN = us.Users.NameEN,

                                                    Telephone = us.Users.Telephone,
                                                    FAX = us.Users.FAX,
                                                    Email = us.Users.Email,
                                                    UserName = us.Users.UserName,
                                                    Password = us.Users.Password
                                                },
                                                Comments = us.Comments,
                                                IsProvider = us.IsProvider
                                            }).OrderBy(o => o.NameGR).ToList();
                    return data;
                }
                catch (Exception) { return null; }
            }
        }

        public List<CustomerB> GetCustomers(int recSkip, int recTake) {
            using (var dbContext = new OTERTConnStr()) {
                try {
                    dbContext.Configuration.ProxyCreationEnabled = false;
                    List<CustomerB> data = (from us in dbContext.Customers
                                            select new CustomerB {
                                                ID = us.ID,
                                                CountryID = us.CountryID,
                                                Country = new CountryDTO {
                                                    ID = us.Countries.ID,
                                                    NameGR = us.Countries.NameGR,
                                                    NameEN = us.Countries.NameEN
                                                },
                                                NameGR = us.NameGR,
                                                NameEN = us.NameEN,
                                                NamedInvoiceGR = us.NamedInvoiceGR,
                                                NamedInvoiceEN = us.NamedInvoiceEN,
                                                ZIPCode = us.ZIPCode,
                                                CityGR = us.CityGR,
                                                CityEN = us.CityEN,
                                                ChargeTelephone = us.ChargeTelephone,
                                                Telephone1 = us.Telephone1,
                                                Telephone2 = us.Telephone2,
                                                FAX1 = us.FAX1,
                                                FAX2 = us.FAX2,
                                                Address1GR = us.Address1GR,
                                                Address1EN = us.Address1EN,
                                                Address2GR = us.Address2GR,
                                                Address2EN = us.Address2EN,
                                                ContactPersonGR = us.ContactPersonGR,
                                                ContactPersonEN = us.ContactPersonEN,
                                                CustomerTypeID = us.CustomerTypeID,
                                                CustomerType = new CustomerTypeDTO {
                                                    ID = us.CustomerTypes.ID,
                                                    NameGR = us.CustomerTypes.NameGR,
                                                    NameEN = us.CustomerTypes.NameEN
                                                },
                                                LanguageID = us.LanguageID,
                                                Language = new LanguageDTO {
                                                    ID = us.Languages.ID,
                                                    Name = us.Languages.Name,
                                                    Code = us.Languages.Code
                                                },
                                                Email = us.Email,
                                                URL = us.URL,
                                                AFM = us.AFM,
                                                DOY = us.DOY,
                                                SAPCode = us.SAPCode,
                                                UserID = us.UserID,
                                                User = us.UserID == null ? null : new UserDTO {
                                                    ID = us.Users.ID,
                                                    UserGroupID = us.Users.UserGroupID,
                                                    UserGroup = new UserGroupDTO {
                                                        ID = us.Users.UserGroups.ID,
                                                        Name = us.Users.UserGroups.Name
                                                    },
                                                    NameGR = us.Users.NameGR,
                                                    NameEN = us.Users.NameEN,

                                                    Telephone = us.Users.Telephone,
                                                    FAX = us.Users.FAX,
                                                    Email = us.Users.Email,
                                                    UserName = us.Users.UserName,
                                                    Password = us.Users.Password
                                                },
                                                Comments = us.Comments,
                                                IsProvider = us.IsProvider
                                            }).OrderBy(o => o.ID).Skip(recSkip).Take(recTake).ToList();
                    return data;
                }
                catch (Exception) { return null; }
            }
        }

        public List<CustomerB> GetProviders() {
            using (var dbContext = new OTERTConnStr()) {
                try {
                    dbContext.Configuration.ProxyCreationEnabled = false;
                    List<CustomerB> data = (from us in dbContext.Customers
                                            select new CustomerB {
                                                ID = us.ID,
                                                CountryID = us.CountryID,
                                                Country = new CountryDTO {
                                                    ID = us.Countries.ID,
                                                    NameGR = us.Countries.NameGR,
                                                    NameEN = us.Countries.NameEN
                                                },
                                                NameGR = us.NameGR,
                                                NameEN = us.NameEN,
                                                NamedInvoiceGR = us.NamedInvoiceGR,
                                                NamedInvoiceEN = us.NamedInvoiceEN,
                                                ZIPCode = us.ZIPCode,
                                                CityGR = us.CityGR,
                                                CityEN = us.CityEN,
                                                ChargeTelephone = us.ChargeTelephone,
                                                Telephone1 = us.Telephone1,
                                                Telephone2 = us.Telephone2,
                                                FAX1 = us.FAX1,
                                                FAX2 = us.FAX2,
                                                Address1GR = us.Address1GR,
                                                Address1EN = us.Address1EN,
                                                Address2GR = us.Address2GR,
                                                Address2EN = us.Address2EN,
                                                ContactPersonGR = us.ContactPersonGR,
                                                ContactPersonEN = us.ContactPersonEN,
                                                CustomerTypeID = us.CustomerTypeID,
                                                CustomerType = new CustomerTypeDTO {
                                                    ID = us.CustomerTypes.ID,
                                                    NameGR = us.CustomerTypes.NameGR,
                                                    NameEN = us.CustomerTypes.NameEN
                                                },
                                                LanguageID = us.LanguageID,
                                                Language = new LanguageDTO {
                                                    ID = us.Languages.ID,
                                                    Name = us.Languages.Name,
                                                    Code = us.Languages.Code
                                                },
                                                Email = us.Email,
                                                URL = us.URL,
                                                AFM = us.AFM,
                                                DOY = us.DOY,
                                                SAPCode = us.SAPCode,
                                                UserID = us.UserID,
                                                User = us.UserID == null ? null : new UserDTO {
                                                    ID = us.Users.ID,
                                                    UserGroupID = us.Users.UserGroupID,
                                                    UserGroup = new UserGroupDTO {
                                                        ID = us.Users.UserGroups.ID,
                                                        Name = us.Users.UserGroups.Name
                                                    },
                                                    NameGR = us.Users.NameGR,
                                                    NameEN = us.Users.NameEN,

                                                    Telephone = us.Users.Telephone,
                                                    FAX = us.Users.FAX,
                                                    Email = us.Users.Email,
                                                    UserName = us.Users.UserName,
                                                    Password = us.Users.Password
                                                },
                                                Comments = us.Comments,
                                                IsProvider = us.IsProvider
                                            }).Where(k => k.IsProvider == true).OrderBy(o => o.NameGR).ToList();
                    return data;
                }
                catch (Exception) { return null; }
            }
        }

    }

}