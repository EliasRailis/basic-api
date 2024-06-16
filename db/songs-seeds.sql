USE [haefele_db]
GO

INSERT INTO [dbo].[songs]
( [created]
, [last_modified]
, [created_by]
, [last_modified_by]
, [name]
, [duration]
, [is_deleted]
, [fk_album_id])
VALUES (GETDATE(), NULL, 'from-seed', NULL, 'Ode to Sleep', '5m 8s', 0, 1),
       (GETDATE(), NULL, 'from-seed', NULL, 'Holding on to You', '4m 23s', 0, 1),
       (GETDATE(), NULL, 'from-seed', NULL, 'Stressed Out', '3m 22s', 0, 2),
       (GETDATE(), NULL, 'from-seed', NULL, 'Ride', '3m 34s', 0, 2),
       (GETDATE(), NULL, 'from-seed', NULL, 'Radioactive', '3m 6s', 0, 3),
       (GETDATE(), NULL, 'from-seed', NULL, 'Demons', '2m 57s', 0, 3),
       (GETDATE(), NULL, 'from-seed', NULL, 'I Bet My Life', '3m 14s', 0, 4),
       (GETDATE(), NULL, 'from-seed', NULL, 'Shots', '3m 52s', 0, 4),
       (GETDATE(), NULL, 'from-seed', NULL, 'Yellow', '4m 29s', 0, 5),
       (GETDATE(), NULL, 'from-seed', NULL, 'Shiver', '4m 59s', 0, 5),
       (GETDATE(), NULL, 'from-seed', NULL, 'The Scientist', '5m 9s', 0, 6),
       (GETDATE(), NULL, 'from-seed', NULL, 'Clocks', '5m 7s', 0, 6),
       (GETDATE(), NULL, 'from-seed', NULL, 'Something Just Like This', '4m 7s', 0, 7),
       (GETDATE(), NULL, 'from-seed', NULL, 'Paris', '3m 41s', 0, 7),
       (GETDATE(), NULL, 'from-seed', NULL, 'Sick Boy', '3m 13s', 0, 8),
       (GETDATE(), NULL, 'from-seed', NULL, 'Hope', '2m 49s', 0, 8),
       (GETDATE(), NULL, 'from-seed', NULL, 'Harder to Breathe', '2m 55s', 0, 9),
       (GETDATE(), NULL, 'from-seed', NULL, 'This Love', '3m 26s', 0, 9),
       (GETDATE(), NULL, 'from-seed', NULL, 'Maps', '3m 10s', 0, 10),
       (GETDATE(), NULL, 'from-seed', NULL, 'Sugar', '3m 55s', 0, 10),
       (GETDATE(), NULL, 'from-seed', NULL, 'The Hills', '4m 2s', 0, 11),
       (GETDATE(), NULL, 'from-seed', NULL, 'Can''t Feel My Face', '3m 35s', 0, 11),
       (GETDATE(), NULL, 'from-seed', NULL, 'Starboy', '3m 50s', 0, 12),
       (GETDATE(), NULL, 'from-seed', NULL, 'I Feel It Coming', '4m 29s', 0, 12),
       (GETDATE(), NULL, 'from-seed', NULL, 'The A Team', '4m 18s', 0, 13),
       (GETDATE(), NULL, 'from-seed', NULL, 'Lego House', '3m 5s', 0, 13),
       (GETDATE(), NULL, 'from-seed', NULL, 'Sing', '3m 55s', 0, 14),
       (GETDATE(), NULL, 'from-seed', NULL, 'Thinking Out Loud', '4m 41s', 0, 14),
       (GETDATE(), NULL, 'from-seed', NULL, 'Chasing Pavements', '3m 30s', 0, 15),
       (GETDATE(), NULL, 'from-seed', NULL, 'Hometown Glory', '4m 1s', 0, 15),
       (GETDATE(), NULL, 'from-seed', NULL, 'Rolling in the Deep', '3m 48s', 0, 16),
       (GETDATE(), NULL, 'from-seed', NULL, 'Someone Like You', '4m 45s', 0, 16),
       (GETDATE(), NULL, 'from-seed', NULL, 'Over My Dead Body', '4m 32s', 0, 17),
       (GETDATE(), NULL, 'from-seed', NULL, 'Headlines', '3m 56s', 0, 17),
       (GETDATE(), NULL, 'from-seed', NULL, 'One Dance', '2m 54s', 0, 18),
       (GETDATE(), NULL, 'from-seed', NULL, 'Hotline Bling', '4m 27s', 0, 18),
       (GETDATE(), NULL, 'from-seed', NULL, 'Crazy in Love', '3m 56s', 0, 19),
       (GETDATE(), NULL, 'from-seed', NULL, 'Baby Boy', '4m 5s', 0, 19),
       (GETDATE(), NULL, 'from-seed', NULL, 'Formation', '3m 26s', 0, 20),
       (GETDATE(), NULL, 'from-seed', NULL, 'Hold Up', '3m 41s', 0, 20);
GO