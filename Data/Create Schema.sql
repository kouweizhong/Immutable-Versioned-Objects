USE [IVO]
GO
/****** Object:  Table [dbo].[Tree]    Script Date: 09/01/2011 17:10:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Tree](
	[treeid] [binary](20) NOT NULL,
 CONSTRAINT [PK_Tree] PRIMARY KEY CLUSTERED 
(
	[treeid] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Blob]    Script Date: 09/01/2011 17:10:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Blob](
	[blobid] [binary](20) NOT NULL,
	[contents] [varbinary](max) NOT NULL,
 CONSTRAINT [PK_Blob] PRIMARY KEY CLUSTERED 
(
	[blobid] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[TreeTree]    Script Date: 09/01/2011 17:10:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[TreeTree](
	[treeid] [binary](20) NOT NULL,
	[linked_treeid] [binary](20) NOT NULL,
	[name] [nvarchar](128) NOT NULL,
 CONSTRAINT [IX_TreeTree] UNIQUE NONCLUSTERED 
(
	[treeid] ASC,
	[name] ASC,
	[linked_treeid] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[TreeBlob]    Script Date: 09/01/2011 17:10:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[TreeBlob](
	[treeid] [binary](20) NOT NULL,
	[linked_blobid] [binary](20) NOT NULL,
	[name] [nvarchar](128) NOT NULL,
 CONSTRAINT [IX_TreeBlob] UNIQUE NONCLUSTERED 
(
	[treeid] ASC,
	[name] ASC,
	[linked_blobid] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Commit]    Script Date: 09/01/2011 17:10:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Commit](
	[commitid] [binary](20) NOT NULL,
	[treeid] [binary](20) NOT NULL,
	[committer] [nvarchar](512) NOT NULL,
	[date_committed] [datetimeoffset](7) NOT NULL,
	[message] [nvarchar](max) NOT NULL,
 CONSTRAINT [PK_Commit] PRIMARY KEY CLUSTERED 
(
	[commitid] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Tag]    Script Date: 09/01/2011 17:10:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Tag](
	[tagid] [binary](20) NOT NULL,
	[name] [nvarchar](128) NOT NULL,
	[commitid] [binary](20) NOT NULL,
	[tagger] [nvarchar](128) NOT NULL,
	[date_tagged] [datetimeoffset](7) NOT NULL,
	[message] [nvarchar](max) NULL,
 CONSTRAINT [PK_Tag] PRIMARY KEY CLUSTERED 
(
	[tagid] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY],
 CONSTRAINT [IX_Tag] UNIQUE NONCLUSTERED 
(
	[name] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Ref]    Script Date: 09/01/2011 17:10:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Ref](
	[name] [nvarchar](128) NOT NULL,
	[commitid] [binary](20) NOT NULL,
 CONSTRAINT [PK_Ref] PRIMARY KEY CLUSTERED 
(
	[name] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[CommitParent]    Script Date: 09/01/2011 17:10:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[CommitParent](
	[commitid] [binary](20) NOT NULL,
	[parent_commitid] [binary](20) NOT NULL,
 CONSTRAINT [IX_CommitParent] UNIQUE CLUSTERED 
(
	[commitid] ASC,
	[parent_commitid] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  ForeignKey [FK_TreeTree_LinkedTree]    Script Date: 09/01/2011 17:10:19 ******/
ALTER TABLE [dbo].[TreeTree]  WITH CHECK ADD  CONSTRAINT [FK_TreeTree_LinkedTree] FOREIGN KEY([linked_treeid])
REFERENCES [dbo].[Tree] ([treeid])
GO
ALTER TABLE [dbo].[TreeTree] CHECK CONSTRAINT [FK_TreeTree_LinkedTree]
GO
/****** Object:  ForeignKey [FK_TreeTree_Tree]    Script Date: 09/01/2011 17:10:19 ******/
ALTER TABLE [dbo].[TreeTree]  WITH CHECK ADD  CONSTRAINT [FK_TreeTree_Tree] FOREIGN KEY([treeid])
REFERENCES [dbo].[Tree] ([treeid])
GO
ALTER TABLE [dbo].[TreeTree] CHECK CONSTRAINT [FK_TreeTree_Tree]
GO
/****** Object:  ForeignKey [FK_TreeBlob_LinkedBlob]    Script Date: 09/01/2011 17:10:19 ******/
ALTER TABLE [dbo].[TreeBlob]  WITH CHECK ADD  CONSTRAINT [FK_TreeBlob_LinkedBlob] FOREIGN KEY([linked_blobid])
REFERENCES [dbo].[Blob] ([blobid])
GO
ALTER TABLE [dbo].[TreeBlob] CHECK CONSTRAINT [FK_TreeBlob_LinkedBlob]
GO
/****** Object:  ForeignKey [FK_TreeBlob_Tree]    Script Date: 09/01/2011 17:10:19 ******/
ALTER TABLE [dbo].[TreeBlob]  WITH CHECK ADD  CONSTRAINT [FK_TreeBlob_Tree] FOREIGN KEY([treeid])
REFERENCES [dbo].[Tree] ([treeid])
GO
ALTER TABLE [dbo].[TreeBlob] CHECK CONSTRAINT [FK_TreeBlob_Tree]
GO
/****** Object:  ForeignKey [FK_Commit_Tree]    Script Date: 09/01/2011 17:10:19 ******/
ALTER TABLE [dbo].[Commit]  WITH CHECK ADD  CONSTRAINT [FK_Commit_Tree] FOREIGN KEY([treeid])
REFERENCES [dbo].[Tree] ([treeid])
GO
ALTER TABLE [dbo].[Commit] CHECK CONSTRAINT [FK_Commit_Tree]
GO
/****** Object:  ForeignKey [FK_Tag_Commit]    Script Date: 09/01/2011 17:10:19 ******/
ALTER TABLE [dbo].[Tag]  WITH CHECK ADD  CONSTRAINT [FK_Tag_Commit] FOREIGN KEY([commitid])
REFERENCES [dbo].[Commit] ([commitid])
GO
ALTER TABLE [dbo].[Tag] CHECK CONSTRAINT [FK_Tag_Commit]
GO
/****** Object:  ForeignKey [FK_Ref_Commit]    Script Date: 09/01/2011 17:10:19 ******/
ALTER TABLE [dbo].[Ref]  WITH CHECK ADD  CONSTRAINT [FK_Ref_Commit] FOREIGN KEY([commitid])
REFERENCES [dbo].[Commit] ([commitid])
GO
ALTER TABLE [dbo].[Ref] CHECK CONSTRAINT [FK_Ref_Commit]
GO
/****** Object:  ForeignKey [FK_CommitParent_Commit]    Script Date: 09/01/2011 17:10:19 ******/
ALTER TABLE [dbo].[CommitParent]  WITH CHECK ADD  CONSTRAINT [FK_CommitParent_Commit] FOREIGN KEY([commitid])
REFERENCES [dbo].[Commit] ([commitid])
GO
ALTER TABLE [dbo].[CommitParent] CHECK CONSTRAINT [FK_CommitParent_Commit]
GO
/****** Object:  ForeignKey [FK_CommitParent_ParentCommit]    Script Date: 09/01/2011 17:10:19 ******/
ALTER TABLE [dbo].[CommitParent]  WITH CHECK ADD  CONSTRAINT [FK_CommitParent_ParentCommit] FOREIGN KEY([parent_commitid])
REFERENCES [dbo].[Commit] ([commitid])
GO
ALTER TABLE [dbo].[CommitParent] CHECK CONSTRAINT [FK_CommitParent_ParentCommit]
GO
