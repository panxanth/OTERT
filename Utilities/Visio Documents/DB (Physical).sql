CREATE TABLE [CountryPricelist] (
[ID] int NOT NULL IDENTITY(1,1),
[CustomerID] int NOT NULL,
[LineTypeID] int NOT NULL,
[InstallationCost] decimal NULL,
[MonthlyCharges] decimal NULL,
[Internet] decimal NULL,
[MSN] decimal NULL,
PRIMARY KEY ([ID]) 
)
GO

CREATE TABLE [LineTypes] (
[ID] int NOT NULL IDENTITY(1,1),
[Name] nvarchar(100) NOT NULL,
PRIMARY KEY ([ID]) 
)
GO

CREATE TABLE [Customers] (
[ID] int NOT NULL IDENTITY(1,1),
[Name1GR] nvarchar(255) NOT NULL,
[Name1EN] nvarchar(255) NULL,
[Name2GR] nvarchar(255) NULL,
[Name2EN] nvarchar(255) NULL,
[CountryID] int NOT NULL,
[ZIPCoode] varchar(10) NULL,
[CityGR] nvarchar(100) NULL,
[CityEN] nvarchar(100) NULL,
[ChargeTelephone] varchar(50) NULL,
[Telephone1] varchar(50) NULL,
[Telephone2] varchar(50) NULL,
[FAX1] varchar(50) NULL,
[FAX2] varchar(50) NULL,
[Address1GR] nvarchar(255) NOT NULL,
[Address1EN] nvarchar(255) NULL,
[Address2GR] nvarchar(255) NULL,
[Address2EN] nvarchar(255) NULL,
[ContactPersonGR] nvarchar(255) NULL,
[ContactPersonEN] nvarchar(255) NULL,
[CustomerTypeID] int NOT NULL,
[LanguageID] int NOT NULL,
[Email] nvarchar(255) NULL,
[URL] nvarchar(255) NULL,
[AFM] nvarchar(20) NULL,
[DOY] nvarchar(255) NULL,
[UserID] int NULL,
[Comments] nvarchar(MAX) NULL,
[IsProvider] bit NULL,
PRIMARY KEY ([ID]) 
)
GO

CREATE TABLE [CustomerTypes] (
[ID] int NOT NULL IDENTITY(1,1),
[NameGR] nvarchar(100) NOT NULL,
[NameEN] nvarchar(100) NULL,
PRIMARY KEY ([ID]) 
)
GO

CREATE TABLE [RequestedPositions] (
[ID] int NOT NULL IDENTITY(1,1),
[NameGR] nvarchar(255) NOT NULL,
[NameEN] nvarchar(255) NULL,
PRIMARY KEY ([ID]) 
)
GO

CREATE TABLE [Languages] (
[ID] int NOT NULL IDENTITY(1,1),
[Name] nvarchar(50) NOT NULL,
[Code] varchar(10) NOT NULL,
PRIMARY KEY ([ID]) 
)
GO

CREATE TABLE [Countries] (
[ID] int NOT NULL IDENTITY(1,1),
[NameGR] nvarchar(100) NOT NULL,
[NameEN] nvarchar(100) NULL,
PRIMARY KEY ([ID]) 
)
GO

CREATE TABLE [Places] (
[ID] int NOT NULL IDENTITY(1,1),
[CountryID] int NOT NULL,
[NameGR] nvarchar(255) NOT NULL,
[NameEN] nvarchar(255) NULL,
PRIMARY KEY ([ID]) 
)
GO

CREATE TABLE [Events] (
[ID] int NOT NULL IDENTITY(1,1),
[PlaceID] int NOT NULL,
[NameGR] nvarchar(MAX) NOT NULL,
[NameEN] nvarchar(MAX) NULL,
PRIMARY KEY ([ID]) 
)
GO

CREATE TABLE [OrderTypes] (
[ID] int NOT NULL IDENTITY(1,1),
[Name] nvarchar(100) NOT NULL,
PRIMARY KEY ([ID]) 
)
GO

CREATE TABLE [UserGroups] (
[ID] int NOT NULL IDENTITY(1,1),
[Name] nvarchar(100) NOT NULL,
PRIMARY KEY ([ID]) 
)
GO

CREATE TABLE [Users] (
[ID] int NOT NULL IDENTITY(1,1),
[UserGroupID] int NOT NULL,
[NameGR] nvarchar(255) NOT NULL,
[NameEN] nvarchar(255) NULL,
[Telephone] nvarchar(50) NULL,
[FAX] nvarchar(50) NULL,
[Email] nvarchar(100) NULL,
[UserName] nvarchar(50) NOT NULL,
[Password] nvarchar(50) NOT NULL,
PRIMARY KEY ([ID]) 
)
GO

CREATE TABLE [Orders] (
[ID] int NOT NULL IDENTITY(1,1),
[OrderTypeID] int NOT NULL,
[CustomerID] int NOT NULL,
[RegNo] nvarchar(100) NOT NULL,
[PlaceID] int NOT NULL,
PRIMARY KEY ([ID]) 
)
GO

CREATE TABLE [DistanceTypes] (
[ID] int NOT NULL IDENTITY(1,1),
[Name] nvarchar(100) NOT NULL,
PRIMARY KEY ([ID]) 
)
GO

CREATE TABLE [Distances] (
[ID] int NOT NULL IDENTITY(1,1),
[DistanceTypeID] int NOT NULL,
[Position1] nvarchar(255) NOT NULL,
[Position2] nvarchar(255) NOT NULL,
[KM] decimal NOT NULL,
PRIMARY KEY ([ID]) 
)
GO

CREATE TABLE [Formulas] (
[ID] int NOT NULL IDENTITY(1,1),
[JobID] int NOT NULL,
[FormulaType] int NOT NULL,
[Condition] varchar(255) NOT NULL,
[Formula] varchar(255) NOT NULL,
PRIMARY KEY ([ID]) 
)
GO

CREATE TABLE [FormulaTypes] (
[ID] int NOT NULL IDENTITY(1,1),
[Name] nvarchar(100) NOT NULL,
PRIMARY KEY ([ID]) 
)
GO

CREATE TABLE [FormulaAttributes] (
[ID] int NOT NULL IDENTITY(1,1),
[Name] nvarchar(100) NOT NULL,
[Template] varchar(50) NOT NULL,
[UIField] varchar(50) NOT NULL,
PRIMARY KEY ([ID]) 
)
GO

CREATE TABLE [Satelites] (
[ID] int NOT NULL IDENTITY(1,1),
[Name] nvarchar(255) NOT NULL,
PRIMARY KEY ([ID]) 
)
GO

CREATE TABLE [SateliteFrequencies] (
[ID] int NOT NULL IDENTITY(1,1),
[Frequency] varchar(50) NOT NULL,
PRIMARY KEY ([ID]) 
)
GO

CREATE TABLE [UIControlsVisibility] (
[ID] int NOT NULL IDENTITY(1,1),
[UIControlID] varchar(100) NOT NULL,
[OrderTypeID] int NOT NULL,
PRIMARY KEY ([ID]) 
)
GO

CREATE TABLE [Jobs] (
[ID] int NOT NULL IDENTITY(1,1),
[Name] nvarchar(100) NOT NULL,
[DiscountID] int NULL,
[MinimumTime] int NULL,
[InvoiceCode] nvarchar(50) NULL,
PRIMARY KEY ([ID]) 
)
GO

CREATE TABLE [Tasks] (
[ID] int NOT NULL IDENTITY(1,1),
[OrderID] int NULL,
[RegNo] nvarchar(100) NOT NULL,
[OrderDate] datetime NOT NULL,
[CustomerID] int NOT NULL,
[RequestedPositionID] int NULL,
[JobID] int NULL,
[PlaceFrom] nvarchar(150) NULL,
[PlaceTo] nvarchar(150) NULL,
[PlaceDistanceKm] int NULL,
[SateliteFrequencyID] int NULL,
[DateTimeStartOrder] datetime NULL,
[DateTimeEndOrder] datetime NULL,
[DateTimeDurationOrder] int NOT NULL,
[IsDurationInDays] bit NULL,
[DateTimeStartActual] datetime NULL,
[DateTimeEndActual] datetime NULL,
[DateTimeDurationActual] int NULL,
[CostCalculated] decimal NULL,
[InstallationCharges] bit NULL,
[MonthlyCharges] bit NULL,
[CallCharges] decimal NULL,
[TelephoneNumber] varchar(100) NULL,
[TechnicalSupport] decimal NULL,
[AddedCharges] decimal NULL,
[CostActual] decimal NULL,
[PaymentDateOrder] datetime NULL,
[PaymentDateCalculated] datetime NULL,
[PaymentDateActual] datetime NULL,
[IsForHelpers] bit NULL,
[IsLocked] bit NULL,
[IsCanceled] bit NULL,
[CancelPrice] decimal NULL,
[Comments] nvarchar(MAX) NULL,
[InvoceComments] nvarchar(MAX) NULL
)
GO

CREATE TABLE [table_1] (
)
GO


ALTER TABLE [CountryPricelist] ADD CONSTRAINT [fk_CountryPricelist_Customers] FOREIGN KEY ([CustomerID]) REFERENCES [Customers] ([ID])
GO
ALTER TABLE [CountryPricelist] ADD CONSTRAINT [fk_CountryPricelist_LineTypes] FOREIGN KEY ([LineTypeID]) REFERENCES [LineTypes] ([ID])
GO
ALTER TABLE [Customers] ADD CONSTRAINT [fk_Customers_Countries] FOREIGN KEY ([CountryID]) REFERENCES [Countries] ([ID])
GO
ALTER TABLE [Places] ADD CONSTRAINT [fk_Places_Countries] FOREIGN KEY ([CountryID]) REFERENCES [Countries] ([ID])
GO
ALTER TABLE [Events] ADD CONSTRAINT [fk_Events_Places] FOREIGN KEY ([PlaceID]) REFERENCES [Places] ([ID])
GO
ALTER TABLE [Customers] ADD CONSTRAINT [fk_Customers_CustomerTypes] FOREIGN KEY ([CustomerTypeID]) REFERENCES [CustomerTypes] ([ID])
GO
ALTER TABLE [Customers] ADD CONSTRAINT [fk_Customers_Languages] FOREIGN KEY ([LanguageID]) REFERENCES [Languages] ([ID])
GO
ALTER TABLE [Customers] ADD CONSTRAINT [fk_Customers_Users] FOREIGN KEY ([UserID]) REFERENCES [Users] ([ID])
GO
ALTER TABLE [Orders] ADD CONSTRAINT [fk_Orders_OrderTypes] FOREIGN KEY ([OrderTypeID]) REFERENCES [OrderTypes] ([ID])
GO
ALTER TABLE [Orders] ADD CONSTRAINT [fk_Orders_Customers] FOREIGN KEY ([CustomerID]) REFERENCES [Customers] ([ID])
GO
ALTER TABLE [Users] ADD CONSTRAINT [fk_Users_UserGroups] FOREIGN KEY ([UserGroupID]) REFERENCES [UserGroups] ([ID])
GO
ALTER TABLE [Distances] ADD CONSTRAINT [fk_Distances_DistanceTypes] FOREIGN KEY ([DistanceTypeID]) REFERENCES [DistanceTypes] ([ID])
GO
ALTER TABLE [Formulas] ADD CONSTRAINT [fk_Formulas_FormulaTypes] FOREIGN KEY ([FormulaType]) REFERENCES [FormulaTypes] ([ID])
GO
ALTER TABLE [UIControlsVisibility] ADD CONSTRAINT [fk_UIControlsVisibility_OrderTypes] FOREIGN KEY ([OrderTypeID]) REFERENCES [OrderTypes] ([ID])
GO
ALTER TABLE [Tasks] ADD CONSTRAINT [fk_Tasks_Orders] FOREIGN KEY ([OrderID]) REFERENCES [Orders] ([ID])
GO
ALTER TABLE [Tasks] ADD CONSTRAINT [fk_Tasks_Customers] FOREIGN KEY ([CustomerID]) REFERENCES [Customers] ([ID])
GO
ALTER TABLE [Tasks] ADD CONSTRAINT [fk_Tasks_SateliteFrequencies] FOREIGN KEY ([SateliteFrequencyID]) REFERENCES [SateliteFrequencies] ([ID])
GO
ALTER TABLE [Orders] ADD CONSTRAINT [fk_Orders_Events] FOREIGN KEY ([PlaceID]) REFERENCES [Events] ([ID])
GO
ALTER TABLE [Tasks] ADD CONSTRAINT [fk_Tasks_RequestedPositions] FOREIGN KEY ([RequestedPositionID]) REFERENCES [RequestedPositions] ([ID])
GO
ALTER TABLE [Tasks] ADD CONSTRAINT [fk_Tasks_Jobs] FOREIGN KEY ([JobID]) REFERENCES [Jobs] ([ID])
GO
ALTER TABLE [Formulas] ADD CONSTRAINT [fk_Formulas_Jobs] FOREIGN KEY ([JobID]) REFERENCES [Jobs] ([ID])
GO

