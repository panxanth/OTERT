using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OTERT.Model {

    public class UserGroupDTO {
        public int ID { get; set; }
        public string Name { get; set; }
    }

    public class UserDTO {
        public int ID { get; set; }
        public int UserGroupID { get; set; }
        public UserGroupDTO UserGroup { get; set; }
        public string NameGR { get; set; }
        public string NameEN { get; set; }
        public string Telephone { get; set; }
        public string FAX { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
    }

    public class CountryDTO {
        public int ID { get; set; }
        public string NameGR { get; set; }
        public string NameEN { get; set; }
    }

    public class PlaceDTO {
        public int ID { get; set; }
        public CountryDTO Country { get; set; }
        public int CountryID { get; set; }
        public string NameGR { get; set; }
        public string NameEN { get; set; }
    }

    public class CustomerTypeDTO {
        public int ID { get; set; }
        public string NameGR { get; set; }
        public string NameEN { get; set; }
    }

    public class LanguageDTO {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
    }

    public class SaleDTO {
        public int ID { get; set; }
        public string Name { get; set; }
        public int Type { get; set; }
    }

    public class JobMainDTO {
        public int ID { get; set; }
        public int PageID { get; set; }
        public string Name { get; set; }
    }

    public class CustomerDTO {
        public int ID { get; set; }
        public CountryDTO Country { get; set; }
        public int CountryID { get; set; }
        public string NameGR { get; set; }
        public string NameEN { get; set; }
        public string NamedInvoiceGR { get; set; }
        public string NamedInvoiceEN { get; set; }
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
        public string SAPCode { get; set; }
        public UserDTO User { get; set; }
        public int? UserID { get; set; }
        public string Comments { get; set; }
        public bool? IsProvider { get; set; }
        public bool? IsOTE { get; set; }
    }

    public class RequestedPositionDTO {
        public int ID { get; set; }
        public string NameGR { get; set; }
        public string NameEN { get; set; }
    }

    public class JobDTO {
        public int ID { get; set; }
        public int JobsMainID { get; set; }
        public JobMainDTO JobsMain { get; set; }
        public string Name { get; set; }
        public int? MinimumTime { get; set; }
        public string InvoiceCode { get; set; }
        public int? SalesID { get; set; }
        public SaleDTO Sale { get; set; }
    }

    public class DistanceDTO {
        public int ID { get; set; }
        public int JobsMainID { get; set; }
        public JobMainDTO JobsMain { get; set; }
        public string Description { get; set; }
        public string Position1 { get; set; }
        public string Position2 { get; set; }
        public int KM { get; set; }
    }

    public class EventDTO {
        public int ID { get; set; }
        public PlaceDTO Place { get; set; }
        public int PlaceID { get; set; }
        public string NameGR { get; set; }
        public string NameEN { get; set; }
    }

    public class OrderTypeDTO {
        public int ID { get; set; }
        public string Name { get; set; }
    }

    public class OrderDTO {
        public int ID { get; set; }
        public int OrderTypeID { get; set; }
        public OrderTypeDTO OrderType { get; set; }
        public string RegNo { get; set; }
        public int Customer1ID { get; set; }
        public CustomerDTO Customer1 { get; set; }
        public int? Customer2ID { get; set; }
        public CustomerDTO Customer2 { get; set; }
        public int EventID { get; set; }
        public EventDTO Event { get; set; }
        public bool IsLocked { get; set; }
    }

    public class SateliteDTO {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Frequency { get; set; }
    }

    public class FileDTO {
        public int ID { get; set; }
        public int? TaskID { get; set; }
        public int? OrderID { get; set; }
        public string FilePath { get; set; }
        public string FileName { get; set; }
        public DateTime DateStamp { get; set; }
    }

    public class LineTypeDTO {
        public int ID { get; set; }
        public string Name { get; set; }
    }

}