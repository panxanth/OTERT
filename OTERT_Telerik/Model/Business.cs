﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OTERT.Model {

    public class EventB {
        public int ID { get; set; }
        public PlaceDTO Place { get; set; }
        public int PlaceID { get; set; }
        public string NameGR { get; set; }
        public string NameEN { get; set; }
    }

    public class CustomerB {
        public int ID { get; set; }
        public CountryDTO Country { get; set; }
        public int CountryID { get; set; }
        public string NameGR { get; set; }
        public string NameEN { get; set; }
        public string ZIPCode { get; set; }
        public string CityGR { get; set; }
        public string CityEN { get; set; }
        public string ChargeTelephone { get; set; }
        public string Telephone1 { get; set; }
        public string Telephone2 { get; set; }
        public string FAX1 { get; set; }
        public string FAX2 { get; set; }
        public string Address1GR { get; set; }
        public string Address1EN { get; set; }
        public string Address2GR { get; set; }
        public string Address2EN { get; set; }
        public string ContactPersonGR { get; set; }
        public string ContactPersonEN { get; set; }
        public CustomerTypeDTO CustomerType { get; set; }
        public int CustomerTypeID { get; set; }
        public LanguageDTO Language { get; set; }
        public int LanguageID { get; set; }
        public string Email { get; set; }
        public string URL { get; set; }
        public string AFM { get; set; }
        public string DOY { get; set; }
        public UserDTO User { get; set; }
        public int? UserID { get; set; }
        public string Comments { get; set; }
        public bool? IsProvider { get; set; }
    }

    public class PlaceB {
        public int ID { get; set; }
        public CountryDTO Country { get; set; }
        public int CountryID { get; set; }
        public string NameGR { get; set; }
        public string NameEN { get; set; }
    }

    public class CountryB {
        public int ID { get; set; }
        public string NameGR { get; set; }
        public string NameEN { get; set; }
    }

    public class SalesFormulaB {
        public int ID { get; set; }
        public int SalesID { get; set; }
        public decimal? Distance { get; set; }
        public decimal SalePercent { get; set; }
    }

    public class SaleB {
        public int ID { get; set; }
        public string Name { get; set; }
        public int Type { get; set; }
    }

    public class SateliteB {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Frequency { get; set; }
    }

    public class LineTypeB {
        public int ID { get; set; }
        public string Name { get; set; }
    }

    public class RequestedPositionB {
        public int ID { get; set; }
        public string NameGR { get; set; }
        public string NameEN { get; set; }
    }

    public class OrderTypeB {
        public int ID { get; set; }
        public string Name { get; set; }
    }

    public class CustomerTypeB {
        public int ID { get; set; }
        public string NameGR { get; set; }
        public string NameEN { get; set; }
    }

    public class LanguageB {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
    }

    public class UserB {
        public int ID { get; set; }
        public int UserGroupID { get; set; }
        public string NameGR { get; set; }
        public string NameEN { get; set; }
        public string Telephone { get; set; }
        public string FAX { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public UserGroupDTO UserGroup { get; set; }
    }

    public class UserGroupB {
        public int ID { get; set; }
        public string Name { get; set; }
    }

    public class JobB {
        public int ID { get; set; }
        public int JobsMainID { get; set; }
        public JobMainDTO JobsMain { get; set; }
        public string Name { get; set; }
        public int? MinimumTime { get; set; }
        public string InvoiceCode { get; set; }
        public int? SalesID { get; set; }
        public SaleDTO Sale { get; set; }
    }

    public class JobMainB {
        public int ID { get; set; }
        public string Name { get; set; }
    }

    public class JobCancelPriceB {
        public int ID { get; set; }
        public int JobsID { get; set; }
        public decimal Price { get; set; }
    }

    public class JobFormulaB {
        public int ID { get; set; }
        public int JobsID { get; set; }
        public string Condition { get; set; }
        public string Formula { get; set; }
    }

}