/*
 * ER/Studio Data Architect 10.0 SQL Code Generation
 * Project :      OTE RT.DM1
 *
 * Date Created : Tuesday, October 17, 2017 11:48:14
 * Target DBMS : Microsoft SQL Server 2008
 */

IF OBJECT_ID('Countries') IS NOT NULL
BEGIN
    DROP TABLE Countries
    PRINT '<<< DROPPED TABLE Countries >>>'
END
go
IF OBJECT_ID('CountryPricelist') IS NOT NULL
BEGIN
    DROP TABLE CountryPricelist
    PRINT '<<< DROPPED TABLE CountryPricelist >>>'
END
go
IF OBJECT_ID('Customers') IS NOT NULL
BEGIN
    DROP TABLE Customers
    PRINT '<<< DROPPED TABLE Customers >>>'
END
go
IF OBJECT_ID('CustomerTypes') IS NOT NULL
BEGIN
    DROP TABLE CustomerTypes
    PRINT '<<< DROPPED TABLE CustomerTypes >>>'
END
go
IF OBJECT_ID('Distances') IS NOT NULL
BEGIN
    DROP TABLE Distances
    PRINT '<<< DROPPED TABLE Distances >>>'
END
go
IF OBJECT_ID('DistanceTypes') IS NOT NULL
BEGIN
    DROP TABLE DistanceTypes
    PRINT '<<< DROPPED TABLE DistanceTypes >>>'
END
go
IF OBJECT_ID('Events') IS NOT NULL
BEGIN
    DROP TABLE Events
    PRINT '<<< DROPPED TABLE Events >>>'
END
go
IF OBJECT_ID('FormulaAttributes') IS NOT NULL
BEGIN
    DROP TABLE FormulaAttributes
    PRINT '<<< DROPPED TABLE FormulaAttributes >>>'
END
go
IF OBJECT_ID('Formulas') IS NOT NULL
BEGIN
    DROP TABLE Formulas
    PRINT '<<< DROPPED TABLE Formulas >>>'
END
go
IF OBJECT_ID('FormulaTypes') IS NOT NULL
BEGIN
    DROP TABLE FormulaTypes
    PRINT '<<< DROPPED TABLE FormulaTypes >>>'
END
go
IF OBJECT_ID('Jobs') IS NOT NULL
BEGIN
    DROP TABLE Jobs
    PRINT '<<< DROPPED TABLE Jobs >>>'
END
go
IF OBJECT_ID('Languages') IS NOT NULL
BEGIN
    DROP TABLE Languages
    PRINT '<<< DROPPED TABLE Languages >>>'
END
go
IF OBJECT_ID('LineTypes') IS NOT NULL
BEGIN
    DROP TABLE LineTypes
    PRINT '<<< DROPPED TABLE LineTypes >>>'
END
go
IF OBJECT_ID('Orders') IS NOT NULL
BEGIN
    DROP TABLE Orders
    PRINT '<<< DROPPED TABLE Orders >>>'
END
go
IF OBJECT_ID('OrderTypes') IS NOT NULL
BEGIN
    DROP TABLE OrderTypes
    PRINT '<<< DROPPED TABLE OrderTypes >>>'
END
go
IF OBJECT_ID('Places') IS NOT NULL
BEGIN
    DROP TABLE Places
    PRINT '<<< DROPPED TABLE Places >>>'
END
go
IF OBJECT_ID('RequestedPositions') IS NOT NULL
BEGIN
    DROP TABLE RequestedPositions
    PRINT '<<< DROPPED TABLE RequestedPositions >>>'
END
go
IF OBJECT_ID('SateliteFrequencies') IS NOT NULL
BEGIN
    DROP TABLE SateliteFrequencies
    PRINT '<<< DROPPED TABLE SateliteFrequencies >>>'
END
go
IF OBJECT_ID('Satelites') IS NOT NULL
BEGIN
    DROP TABLE Satelites
    PRINT '<<< DROPPED TABLE Satelites >>>'
END
go
IF OBJECT_ID('Tasks') IS NOT NULL
BEGIN
    DROP TABLE Tasks
    PRINT '<<< DROPPED TABLE Tasks >>>'
END
go
IF OBJECT_ID('UIControlsVisibility') IS NOT NULL
BEGIN
    DROP TABLE UIControlsVisibility
    PRINT '<<< DROPPED TABLE UIControlsVisibility >>>'
END
go
IF OBJECT_ID('UserGroups') IS NOT NULL
BEGIN
    DROP TABLE UserGroups
    PRINT '<<< DROPPED TABLE UserGroups >>>'
END
go
IF OBJECT_ID('Users') IS NOT NULL
BEGIN
    DROP TABLE Users
    PRINT '<<< DROPPED TABLE Users >>>'
END
go
/* 
 * TABLE: Countries 
 */

CREATE TABLE Countries(
    ID        int              IDENTITY(1,1),
    NameGR    nvarchar(100)    NOT NULL,
    NameEN    nvarchar(100)    NULL,
    CONSTRAINT PK_Countries PRIMARY KEY NONCLUSTERED (ID)
)
go



IF OBJECT_ID('Countries') IS NOT NULL
    PRINT '<<< CREATED TABLE Countries >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE Countries >>>'
go


/* 
 * TABLE: CountryPricelist 
 */

CREATE TABLE CountryPricelist(
    ID                  int               IDENTITY(1,1),
    CustomerID          int               NOT NULL,
    LineTypeID          int               NOT NULL,
    InstallationCost    decimal(18, 0)    NULL,
    MonthlyCharges      decimal(18, 0)    NULL,
    Internet            decimal(18, 0)    NULL,
    MSN                 decimal(18, 0)    NULL,
    CONSTRAINT PK_CountryPricelist PRIMARY KEY NONCLUSTERED (ID)
)
go



IF OBJECT_ID('CountryPricelist') IS NOT NULL
    PRINT '<<< CREATED TABLE CountryPricelist >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE CountryPricelist >>>'
go


/* 
 * TABLE: Customers 
 */

CREATE TABLE Customers(
    ID                 int              IDENTITY(1,1),
    Name1GR            nvarchar(255)    NOT NULL,
    Name1EN            nvarchar(255)    NULL,
    Name2GR            nvarchar(255)    NULL,
    Name2EN            nvarchar(255)    NULL,
    CountryID          int              NOT NULL,
    ZIPCoode           varchar(10)      NULL,
    CityGR             nvarchar(100)    NULL,
    CityEN             nvarchar(100)    NULL,
    ChargeTelephone    varchar(50)      NULL,
    Telephone1         varchar(50)      NULL,
    Telephone2         varchar(50)      NULL,
    FAX1               varchar(50)      NULL,
    FAX2               varchar(50)      NULL,
    Address1GR         nvarchar(255)    NOT NULL,
    Address1EN         nvarchar(255)    NULL,
    Address2GR         nvarchar(255)    NULL,
    Address2EN         nvarchar(255)    NULL,
    ContactPersonGR    nvarchar(255)    NULL,
    ContactPersonEN    nvarchar(255)    NULL,
    CustomerTypeID     int              NOT NULL,
    LanguageID         int              NOT NULL,
    Email              nvarchar(255)    NULL,
    URL                nvarchar(255)    NULL,
    AFM                nvarchar(20)     NULL,
    DOY                nvarchar(255)    NULL,
    UserID             int              NULL,
    Comments           nvarchar(max)    NULL,
    IsProvider         bit              NULL,
    CONSTRAINT PK_Customers PRIMARY KEY NONCLUSTERED (ID)
)
go



IF OBJECT_ID('Customers') IS NOT NULL
    PRINT '<<< CREATED TABLE Customers >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE Customers >>>'
go


/* 
 * TABLE: CustomerTypes 
 */

CREATE TABLE CustomerTypes(
    ID        int              IDENTITY(1,1),
    NameGR    nvarchar(100)    NOT NULL,
    NameEN    nvarchar(100)    NULL,
    CONSTRAINT PK_CustomerTypes PRIMARY KEY NONCLUSTERED (ID)
)
go



IF OBJECT_ID('CustomerTypes') IS NOT NULL
    PRINT '<<< CREATED TABLE CustomerTypes >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE CustomerTypes >>>'
go


/* 
 * TABLE: Distances 
 */

CREATE TABLE Distances(
    ID                int               IDENTITY(1,1),
    DistanceTypeID    int               NOT NULL,
    Position1         nvarchar(255)     NOT NULL,
    Position2         nvarchar(255)     NOT NULL,
    KM                decimal(18, 0)    NOT NULL,
    CONSTRAINT PK_Distances PRIMARY KEY NONCLUSTERED (ID)
)
go



IF OBJECT_ID('Distances') IS NOT NULL
    PRINT '<<< CREATED TABLE Distances >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE Distances >>>'
go


/* 
 * TABLE: DistanceTypes 
 */

CREATE TABLE DistanceTypes(
    ID      int              IDENTITY(1,1),
    Name    nvarchar(100)    NOT NULL,
    CONSTRAINT PK_DistanceTypes PRIMARY KEY NONCLUSTERED (ID)
)
go



IF OBJECT_ID('DistanceTypes') IS NOT NULL
    PRINT '<<< CREATED TABLE DistanceTypes >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE DistanceTypes >>>'
go


/* 
 * TABLE: Events 
 */

CREATE TABLE Events(
    ID         int              IDENTITY(1,1),
    PlaceID    int              NOT NULL,
    NameGR     nvarchar(max)    NOT NULL,
    NameEN     nvarchar(max)    NULL,
    CONSTRAINT PK_Events PRIMARY KEY NONCLUSTERED (ID)
)
go



IF OBJECT_ID('Events') IS NOT NULL
    PRINT '<<< CREATED TABLE Events >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE Events >>>'
go


/* 
 * TABLE: FormulaAttributes 
 */

CREATE TABLE FormulaAttributes(
    ID          int              IDENTITY(1,1),
    Name        nvarchar(100)    NOT NULL,
    Template    varchar(50)      NOT NULL,
    UIField     varchar(50)      NOT NULL,
    CONSTRAINT PK_FormulaAttributes PRIMARY KEY NONCLUSTERED (ID)
)
go



IF OBJECT_ID('FormulaAttributes') IS NOT NULL
    PRINT '<<< CREATED TABLE FormulaAttributes >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE FormulaAttributes >>>'
go


/* 
 * TABLE: Formulas 
 */

CREATE TABLE Formulas(
    ID             int             IDENTITY(1,1),
    JobID          int             NOT NULL,
    FormulaType    int             NOT NULL,
    Condition      varchar(255)    NOT NULL,
    Formula        varchar(255)    NOT NULL,
    CONSTRAINT PK_Formulas PRIMARY KEY NONCLUSTERED (ID)
)
go



IF OBJECT_ID('Formulas') IS NOT NULL
    PRINT '<<< CREATED TABLE Formulas >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE Formulas >>>'
go


/* 
 * TABLE: FormulaTypes 
 */

CREATE TABLE FormulaTypes(
    ID      int              IDENTITY(1,1),
    Name    nvarchar(100)    NOT NULL,
    CONSTRAINT PK_FormulaTypes PRIMARY KEY NONCLUSTERED (ID)
)
go



IF OBJECT_ID('FormulaTypes') IS NOT NULL
    PRINT '<<< CREATED TABLE FormulaTypes >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE FormulaTypes >>>'
go


/* 
 * TABLE: Jobs 
 */

CREATE TABLE Jobs(
    ID             int              IDENTITY(1,1),
    Name           nvarchar(100)    NOT NULL,
    DiscountID     int              NULL,
    MinimumTime    int              NULL,
    InvoiceCode    nvarchar(50)     NULL,
    CONSTRAINT PK_Jobs PRIMARY KEY NONCLUSTERED (ID)
)
go



IF OBJECT_ID('Jobs') IS NOT NULL
    PRINT '<<< CREATED TABLE Jobs >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE Jobs >>>'
go


/* 
 * TABLE: Languages 
 */

CREATE TABLE Languages(
    ID      int             IDENTITY(1,1),
    Name    nvarchar(50)    NOT NULL,
    Code    varchar(10)     NOT NULL,
    CONSTRAINT PK_Languages PRIMARY KEY NONCLUSTERED (ID)
)
go



IF OBJECT_ID('Languages') IS NOT NULL
    PRINT '<<< CREATED TABLE Languages >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE Languages >>>'
go


/* 
 * TABLE: LineTypes 
 */

CREATE TABLE LineTypes(
    ID      int              IDENTITY(1,1),
    Name    nvarchar(100)    NOT NULL,
    CONSTRAINT PK_LineTypes PRIMARY KEY NONCLUSTERED (ID)
)
go



IF OBJECT_ID('LineTypes') IS NOT NULL
    PRINT '<<< CREATED TABLE LineTypes >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE LineTypes >>>'
go


/* 
 * TABLE: Orders 
 */

CREATE TABLE Orders(
    ID             int              IDENTITY(1,1),
    OrderTypeID    int              NOT NULL,
    CustomerID     int              NOT NULL,
    RegNo          nvarchar(100)    NOT NULL,
    PlaceID        int              NOT NULL,
    CONSTRAINT PK_Orders PRIMARY KEY NONCLUSTERED (ID)
)
go



IF OBJECT_ID('Orders') IS NOT NULL
    PRINT '<<< CREATED TABLE Orders >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE Orders >>>'
go


/* 
 * TABLE: OrderTypes 
 */

CREATE TABLE OrderTypes(
    ID      int              IDENTITY(1,1),
    Name    nvarchar(100)    NOT NULL,
    CONSTRAINT PK_OrderTypes PRIMARY KEY NONCLUSTERED (ID)
)
go



IF OBJECT_ID('OrderTypes') IS NOT NULL
    PRINT '<<< CREATED TABLE OrderTypes >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE OrderTypes >>>'
go


/* 
 * TABLE: Places 
 */

CREATE TABLE Places(
    ID           int              IDENTITY(1,1),
    CountryID    int              NOT NULL,
    NameGR       nvarchar(255)    NOT NULL,
    NameEN       nvarchar(255)    NULL,
    CONSTRAINT PK_Places PRIMARY KEY NONCLUSTERED (ID)
)
go



IF OBJECT_ID('Places') IS NOT NULL
    PRINT '<<< CREATED TABLE Places >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE Places >>>'
go


/* 
 * TABLE: RequestedPositions 
 */

CREATE TABLE RequestedPositions(
    ID        int              IDENTITY(1,1),
    NameGR    nvarchar(255)    NOT NULL,
    NameEN    nvarchar(255)    NULL,
    CONSTRAINT PK_RequestedPositions PRIMARY KEY NONCLUSTERED (ID)
)
go



IF OBJECT_ID('RequestedPositions') IS NOT NULL
    PRINT '<<< CREATED TABLE RequestedPositions >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE RequestedPositions >>>'
go


/* 
 * TABLE: SateliteFrequencies 
 */

CREATE TABLE SateliteFrequencies(
    ID           int            IDENTITY(1,1),
    Frequency    varchar(50)    NOT NULL,
    CONSTRAINT PK_SateliteFrequencies PRIMARY KEY NONCLUSTERED (ID)
)
go



IF OBJECT_ID('SateliteFrequencies') IS NOT NULL
    PRINT '<<< CREATED TABLE SateliteFrequencies >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE SateliteFrequencies >>>'
go


/* 
 * TABLE: Satelites 
 */

CREATE TABLE Satelites(
    ID      int              IDENTITY(1,1),
    Name    nvarchar(255)    NOT NULL,
    CONSTRAINT PK_Satelites PRIMARY KEY NONCLUSTERED (ID)
)
go



IF OBJECT_ID('Satelites') IS NOT NULL
    PRINT '<<< CREATED TABLE Satelites >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE Satelites >>>'
go


/* 
 * TABLE: Tasks 
 */

CREATE TABLE Tasks(
    ID                        int               IDENTITY(1,1),
    OrderID                   int               NULL,
    RegNo                     nvarchar(100)     NOT NULL,
    OrderDate                 datetime          NOT NULL,
    CustomerID                int               NOT NULL,
    RequestedPositionID       int               NULL,
    JobID                     int               NULL,
    PlaceFrom                 nvarchar(150)     NULL,
    PlaceTo                   nvarchar(150)     NULL,
    PlaceDistanceKm           int               NULL,
    SateliteFrequencyID       int               NULL,
    DateTimeStartOrder        datetime          NULL,
    DateTimeEndOrder          datetime          NULL,
    DateTimeDurationOrder     int               NOT NULL,
    IsDurationInDays          bit               NULL,
    DateTimeStartActual       datetime          NULL,
    DateTimeEndActual         datetime          NULL,
    DateTimeDurationActual    int               NULL,
    CostCalculated            decimal(18, 0)    NULL,
    InstallationCharges       bit               NULL,
    MonthlyCharges            bit               NULL,
    CallCharges               decimal(18, 0)    NULL,
    TelephoneNumber           varchar(100)      NULL,
    TechnicalSupport          decimal(18, 0)    NULL,
    AddedCharges              decimal(18, 0)    NULL,
    CostActual                decimal(18, 0)    NULL,
    PaymentDateOrder          datetime          NULL,
    PaymentDateCalculated     datetime          NULL,
    PaymentDateActual         datetime          NULL,
    IsForHelpers              bit               NULL,
    IsLocked                  bit               NULL,
    IsCanceled                bit               NULL,
    CancelPrice               decimal(18, 0)    NULL,
    Comments                  nvarchar(max)     NULL,
    InvoceComments            nvarchar(max)     NULL,
    CONSTRAINT PK_Tasks PRIMARY KEY NONCLUSTERED (ID)
)
go



IF OBJECT_ID('Tasks') IS NOT NULL
    PRINT '<<< CREATED TABLE Tasks >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE Tasks >>>'
go


/* 
 * TABLE: UIControlsVisibility 
 */

CREATE TABLE UIControlsVisibility(
    ID             int             IDENTITY(1,1),
    UIControlID    varchar(100)    NOT NULL,
    OrderTypeID    int             NOT NULL,
    CONSTRAINT PK_UIControlsVisibility PRIMARY KEY NONCLUSTERED (ID)
)
go



IF OBJECT_ID('UIControlsVisibility') IS NOT NULL
    PRINT '<<< CREATED TABLE UIControlsVisibility >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE UIControlsVisibility >>>'
go


/* 
 * TABLE: UserGroups 
 */

CREATE TABLE UserGroups(
    ID      int              IDENTITY(1,1),
    Name    nvarchar(100)    NOT NULL,
    CONSTRAINT PK_UserGroups PRIMARY KEY NONCLUSTERED (ID)
)
go



IF OBJECT_ID('UserGroups') IS NOT NULL
    PRINT '<<< CREATED TABLE UserGroups >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE UserGroups >>>'
go


/* 
 * TABLE: Users 
 */

CREATE TABLE Users(
    ID             int              IDENTITY(1,1),
    UserGroupID    int              NOT NULL,
    NameGR         nvarchar(255)    NOT NULL,
    NameEN         nvarchar(255)    NULL,
    Telephone      nvarchar(50)     NULL,
    FAX            nvarchar(50)     NULL,
    Email          nvarchar(100)    NULL,
    UserName       nvarchar(50)     NOT NULL,
    Password       nvarchar(50)     NOT NULL,
    CONSTRAINT PK_Users PRIMARY KEY NONCLUSTERED (ID)
)
go



IF OBJECT_ID('Users') IS NOT NULL
    PRINT '<<< CREATED TABLE Users >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE Users >>>'
go


/* 
 * TABLE: CountryPricelist 
 */

ALTER TABLE CountryPricelist ADD CONSTRAINT fk_CountryPricelist_Customers 
    FOREIGN KEY (CustomerID)
    REFERENCES Customers(ID)
go

ALTER TABLE CountryPricelist ADD CONSTRAINT fk_CountryPricelist_LineTypes 
    FOREIGN KEY (LineTypeID)
    REFERENCES LineTypes(ID)
go


/* 
 * TABLE: Customers 
 */

ALTER TABLE Customers ADD CONSTRAINT fk_Customers_Countries 
    FOREIGN KEY (CountryID)
    REFERENCES Countries(ID)
go

ALTER TABLE Customers ADD CONSTRAINT fk_Customers_CustomerTypes 
    FOREIGN KEY (CustomerTypeID)
    REFERENCES CustomerTypes(ID)
go

ALTER TABLE Customers ADD CONSTRAINT fk_Customers_Languages 
    FOREIGN KEY (LanguageID)
    REFERENCES Languages(ID)
go

ALTER TABLE Customers ADD CONSTRAINT fk_Customers_Users 
    FOREIGN KEY (UserID)
    REFERENCES Users(ID)
go


/* 
 * TABLE: Distances 
 */

ALTER TABLE Distances ADD CONSTRAINT fk_Distances_DistanceTypes 
    FOREIGN KEY (DistanceTypeID)
    REFERENCES DistanceTypes(ID)
go


/* 
 * TABLE: Events 
 */

ALTER TABLE Events ADD CONSTRAINT fk_Events_Places 
    FOREIGN KEY (PlaceID)
    REFERENCES Places(ID)
go


/* 
 * TABLE: Formulas 
 */

ALTER TABLE Formulas ADD CONSTRAINT fk_Formulas_FormulaTypes 
    FOREIGN KEY (FormulaType)
    REFERENCES FormulaTypes(ID)
go

ALTER TABLE Formulas ADD CONSTRAINT fk_Formulas_Jobs 
    FOREIGN KEY (JobID)
    REFERENCES Jobs(ID)
go


/* 
 * TABLE: Orders 
 */

ALTER TABLE Orders ADD CONSTRAINT fk_Orders_Customers 
    FOREIGN KEY (CustomerID)
    REFERENCES Customers(ID)
go

ALTER TABLE Orders ADD CONSTRAINT fk_Orders_Events 
    FOREIGN KEY (PlaceID)
    REFERENCES Events(ID)
go

ALTER TABLE Orders ADD CONSTRAINT fk_Orders_OrderTypes 
    FOREIGN KEY (OrderTypeID)
    REFERENCES OrderTypes(ID)
go


/* 
 * TABLE: Places 
 */

ALTER TABLE Places ADD CONSTRAINT fk_Places_Countries 
    FOREIGN KEY (CountryID)
    REFERENCES Countries(ID)
go


/* 
 * TABLE: Tasks 
 */

ALTER TABLE Tasks ADD CONSTRAINT fk_Tasks_Customers 
    FOREIGN KEY (CustomerID)
    REFERENCES Customers(ID)
go

ALTER TABLE Tasks ADD CONSTRAINT fk_Tasks_Jobs 
    FOREIGN KEY (JobID)
    REFERENCES Jobs(ID)
go

ALTER TABLE Tasks ADD CONSTRAINT fk_Tasks_Orders 
    FOREIGN KEY (OrderID)
    REFERENCES Orders(ID)
go

ALTER TABLE Tasks ADD CONSTRAINT fk_Tasks_RequestedPositions 
    FOREIGN KEY (RequestedPositionID)
    REFERENCES RequestedPositions(ID)
go

ALTER TABLE Tasks ADD CONSTRAINT fk_Tasks_SateliteFrequencies 
    FOREIGN KEY (SateliteFrequencyID)
    REFERENCES SateliteFrequencies(ID)
go


/* 
 * TABLE: UIControlsVisibility 
 */

ALTER TABLE UIControlsVisibility ADD CONSTRAINT fk_UIControlsVisibility_OrderTypes 
    FOREIGN KEY (OrderTypeID)
    REFERENCES OrderTypes(ID)
go


/* 
 * TABLE: Users 
 */

ALTER TABLE Users ADD CONSTRAINT fk_Users_UserGroups 
    FOREIGN KEY (UserGroupID)
    REFERENCES UserGroups(ID)
go


