USE [erp]
GO

/****** Object:  Table [dbo].[LocationData]    Script Date: 05/05/2023 22:51:20 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[LocationData](
	[factoryId] [nvarchar](50) NULL,
	[DutyManager] [nvarchar](50) NULL,
	[DutyStartTime] [datetime] NULL,
	[DutyEndTime] [datetime] NULL
) ON [PRIMARY]
GO


