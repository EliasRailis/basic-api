USE
[haefele_db]
GO

INSERT INTO [dbo].[roles]
           ([created]
           ,[last_modified]
           ,[created_by]
           ,[last_modified_by]
           ,[name]
           ,[is_deleted])
     VALUES
           (GETDATE(), GETDATE(),'from-seed', 'from-seed', 'Admin', 0),
		   (GETDATE(), GETDATE(),'from-seed', 'from-seed', 'Customer', 0);
GO