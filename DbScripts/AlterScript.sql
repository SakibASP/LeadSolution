DROP TABLE IF EXISTS AspNetServiceTypes ;

GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'AspNetServiceTypes' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    CREATE TABLE dbo.AspNetServiceTypes
    (
        [Id] INT NOT NULL IDENTITY(1,1) PRIMARY KEY CLUSTERED,
        [Name] NVARCHAR(256) NOT NULL,
        [IsActive] BIT DEFAULT(1) 
    );
END

GO


INSERT INTO AspNetServiceTypes(Name)
Values('Car Rent'),('Flooring'),('Roofing')

GO

SELECT * FROM AspNetServiceTypes

GO

ALTER TABLE  [LeadSolution].[dbo].[AspNetUsers] 
DROP COLUMN RefreshToken,RefreshTokenExpiryTime;

GO

ALTER TABLE  [LeadSolution].[dbo].[AspNetUsers] 
ADD Company nvarchar(256),ServiceTypeId INT REFERENCES AspNetServiceTypes(Id);

GO

CREATE TABLE [dbo].[Audit](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[TableName] [nvarchar](128) NULL,
	[UserId] [nvarchar](256) NULL,
	[Actions] [nvarchar](128) NULL,
	[KeyValue] [bigint] NULL,
	[OldData] [nvarchar](max) NULL,
	[NewData] [nvarchar](max) NULL,
	[OperatingSystem] [nvarchar](128) NULL,
	[IPAddress] [nvarchar](256) NULL,
	[AreaAccessed] [nvarchar](512) NULL,
	[UpdateDate] [datetime] NULL,
 CONSTRAINT [PK_Audit] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO


