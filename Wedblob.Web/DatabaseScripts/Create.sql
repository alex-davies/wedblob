/****** Object:  Table [greenhippo_alexandniluwed].[Rsvp]    Script Date: 27/08/2015 07:35:06 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [Rsvp](
	[RsvpID] [int] IDENTITY(1,1) NOT NULL,
	[GroupName] [varchar](255) NULL,
	[Name] [varchar](255) NULL,
	AlternateNames [varchar](max) NULL,
	[Attending] [bit] NULL,
	[CreatedDate] [datetime] NOT NULL,
	[UpdatedDate] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[RsvpID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

ALTER TABLE [Rsvp] ADD  DEFAULT (getdate()) FOR [CreatedDate]
GO

ALTER TABLE [Rsvp] ADD  DEFAULT (getdate()) FOR [UpdatedDate]
GO


