﻿CREATE TABLE tbl_db_reply (    ReplyId uniqueidentifier NOT NULL PRIMARY KEY,    PostId uniqueidentifier NOT NULL FOREIGN KEY(PostId) REFERENCES tbl_db_post(PostId),    Description nvarchar(255),    Role varchar(255),    IsEdited bit,    CreatedOn datetime,    CreatedBy uniqueidentifier);