CREATE TABLE [dbo].[RefreshToken](
	[Id] [nvarchar](256) NOT NULL,
	[UserId] [uniqueidentifier] NOT NULL,
	[Issued] [datetimeoffset](7) NOT NULL,
	[Expired] [datetimeoffset](7) NOT NULL,
	[ProtectedTicket] [nvarchar](512) NOT NULL,
 CONSTRAINT [PK_RefreshToken] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[RefreshToken]  WITH CHECK ADD  CONSTRAINT [fk_RefreshToken_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[User] ([Id])
GO

ALTER TABLE [dbo].[RefreshToken] CHECK CONSTRAINT [fk_RefreshToken_UserId]
GO