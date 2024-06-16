USE [haefele_db]
GO

INSERT INTO [dbo].[artists]
( [created]
, [last_modified]
, [created_by]
, [last_modified_by]
, [name]
, [is_deleted])
VALUES (GETDATE(), NULL, 'from-seed', NULL, 'Twenty One Pilots', 0),
       (GETDATE(), NULL, 'from-seed', NULL, 'Imagine Dragons', 0),
       (GETDATE(), NULL, 'from-seed', NULL, 'Coldplay', 0),
       (GETDATE(), NULL, 'from-seed', NULL, 'The Chainsmokers', 0),
       (GETDATE(), NULL, 'from-seed', NULL, 'Maroon 5', 0),
       (GETDATE(), NULL, 'from-seed', NULL, 'The Weeknd', 0),
       (GETDATE(), NULL, 'from-seed', NULL, 'Ed Sheeran', 0),
       (GETDATE(), NULL, 'from-seed', NULL, 'Adele', 0),
       (GETDATE(), NULL, 'from-seed', NULL, 'Drake', 0),
       (GETDATE(), NULL, 'from-seed', NULL, 'Beyoncé', 0);
GO
