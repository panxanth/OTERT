using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OTERT.Model {

    public class CountryB {
        public int ID { get; set; }
        public string NameGR { get; set; }
        public string NameEN { get; set; }
    }

    public class CountryPricelistB {
        public int ID { get; set; }
        public CustomerDTO Customer { get; set; }
        public int CustomerID { get; set; }
        public LineTypeDTO LineType { get; set; }
        public int LineTypeID { get; set; }
        public decimal? InstallationCost { get; set; }
        public decimal? MonthlyCharges { get; set; }
        public decimal? Internet { get; set; }
        public decimal? MSN { get; set; }
        public bool? PaymentIsForWholeMonth { get; set; }
    }

    public class CustomerB {
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

    public class CustomerTypeB {
        public int ID { get; set; }
        public string NameGR { get; set; }
        public string NameEN { get; set; }
    }

    public class DistanceB {
        public int ID { get; set; }
        public int JobsMainID { get; set; }
        public JobMainDTO JobsMain { get; set; }
        public string Description { get; set; }
        public string Position1 { get; set; }
        public string Position2 { get; set; }
        public int KM { get; set; }
    }

    public class DocumentReplacemetB {
        public int ID { get; set; }
        public string UniqueName { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public string Type { get; set; }
        public int? ImageWidth { get; set; }
        public int? ImageHeight { get; set; }
        public string BookmarkTitle { get; set; }
        public string DocumentPart { get; set; }
    }

    public class EventB {
        public int ID { get; set; }
        public PlaceDTO Place { get; set; }
        public int PlaceID { get; set; }
        public string NameGR { get; set; }
        public string NameEN { get; set; }
    }

    public class FileB {
        public int ID { get; set; }
        public int? TaskID { get; set; }
        public int? OrderID { get; set; }
        public int? CustomerID { get; set; }
        public string FilePath { get; set; }
        public string FileName { get; set; }
        public DateTime DateStamp { get; set; }
    }

    public class JobB {
        public int ID { get; set; }
        public int JobsMainID { get; set; }
        public JobMainDTO JobsMain { get; set; }
        public int JobTypesID { get; set; }
        public JobTypeDTO JobType { get; set; }
        public string Name { get; set; }
        public int? MinimumTime { get; set; }
        public string InvoiceCode { get; set; }
        public int? SalesID { get; set; }
        public SaleDTO Sale { get; set; }
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

    public class JobMainB {
        public int ID { get; set; }
        public int PageID { get; set; }
        public string Name { get; set; }
    }

    public class JobTypeB {
        public int ID { get; set; }
        public string Name { get; set; }
    }

    public class LanguageB {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
    }

    public class LineTypeB {
        public int ID { get; set; }
        public string Name { get; set; }
    }

    public class OrderB {
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

    public class OrderTypeB {
        public int ID { get; set; }
        public string Name { get; set; }
    }

    public class PlaceB {
        public int ID { get; set; }
        public CountryDTO Country { get; set; }
        public int CountryID { get; set; }
        public string NameGR { get; set; }
        public string NameEN { get; set; }
    }

    public class RequestedPositionB {
        public int ID { get; set; }
        public string NameGR { get; set; }
        public string NameEN { get; set; }
    }

    public class SaleB {
        public int ID { get; set; }
        public string Name { get; set; }
        public int Type { get; set; }
    }

    public class SalesFormulaB {
        public int ID { get; set; }
        public int SalesID { get; set; }
        public SaleDTO Sale { get; set; }
        public decimal? Distance { get; set; }
        public decimal SalePercent { get; set; }
    }

    public class SateliteB {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Frequency { get; set; }
    }

    public class TaskB {
        public int ID { get; set; }
        public int? OrderID { get; set; }
        public OrderDTO Order { get; set; }
        public string RegNo { get; set; }
        public DateTime OrderDate { get; set; }
        public int CustomerID { get; set; }
        public CustomerDTO Customer { get; set; }
        public int? RequestedPositionID { get; set; }
        public RequestedPositionDTO RequestedPosition { get; set; }
        public int? JobID { get; set; }
        public JobDTO Job { get; set; }
        public int? DistanceID { get; set; }
        public DistanceDTO Distance { get; set; }
        public DateTime? DateTimeStartOrder { get; set; }
        public DateTime? DateTimeEndOrder { get; set; }
        public int DateTimeDurationOrder { get; set; }
        public DateTime? DateTimeStartActual { get; set; }
        public DateTime? DateTimeEndActual { get; set; }
        public int? DateTimeDurationActual { get; set; }
        public decimal? CostCalculated { get; set; }
        public bool InstallationCharges { get; set; }
        public bool MonthlyCharges { get; set; }
        public decimal? CallCharges { get; set; }
        public string TelephoneNumber { get; set; }
        public decimal? TechnicalSupport { get; set; }
        public decimal? AddedCharges { get; set; }
        public decimal? CostActual { get; set; }
        public DateTime? PaymentDateOrder { get; set; }
        public DateTime? PaymentDateCalculated { get; set; }
        public DateTime? PaymentDateActual { get; set; }
        public bool IsForHelpers { get; set; }
        public bool IsLocked { get; set; }
        public bool IsCanceled { get; set; }
        public decimal? CancelPrice { get; set; }
        public string Comments { get; set; }
        public string InvoceComments { get; set; }
        public int? SateliteID { get; set; }
        public SateliteDTO Satelite { get; set; }
        public List<FileB> Files { get; set; }
        public bool MSN { get; set; }
        public bool Internet { get; set; }
        public int? LineTypeID { get; set; }
        public LineTypeDTO LineType { get; set; }
        public DateTime DateStamp { get; set; }
        public string EnteredByUser { get; set; }
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

    public class TaskForH {
        public int Count { get; set; }
        public string Customer { get; set; }
        public string FromPlace { get; set; }
        public string ToPlace { get; set; }
        public string FromTime { get; set; }
        public string ToTime { get; set; }
        public string Comments { get; set; }
    }

    public class InvoiceB {
        public int ID { get; set; }
        public int CustomerID { get; set; }
        public CustomerDTO Customer { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public DateTime? DateCreated { get; set; }
        public string RegNo { get; set; }
        public DateTime? DatePaid { get; set; }
        public decimal? TasksLineAmount { get; set; }
        public decimal? DiscountLineAmount { get; set; }
        public bool? IsLocked { get; set; }
    }

    public class DiscountLineB {
        public int ID { get; set; }
        public int InvoiceID { get; set; }
        public InvoiceDTO Invoice { get; set; }
        public int JobID { get; set; }
        public JobDTO Job { get; set; }
        public decimal? TasksAmount { get; set; }
        public decimal? DiscountAmount { get; set; }
        public bool Applied { get; set; }
    }

    public class TasksLineB {
        public int ID { get; set; }
        public int InvoiceID { get; set; }
        public InvoiceDTO Invoice { get; set; }
        public int TaskID { get; set; }
        public TaskDTO Task { get; set; }
        public int JobID { get; set; }
        public JobDTO Job { get; set; }
    }

}