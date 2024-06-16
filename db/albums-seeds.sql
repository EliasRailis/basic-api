USE [haefele_db]
GO

INSERT INTO [dbo].[albums]
( [created]
, [last_modified]
, [created_by]
, [last_modified_by]
, [name]
, [year_of_release]
, [duration]
, [number_of_songs]
, [is_deleted]
, [fk_artist_id])
VALUES (GETDATE(), NULL, 'from-seed', NULL, 'Vessel', '2013', '1h 1m 15s', 12, 0, 1),
       (GETDATE(), NULL, 'from-seed', NULL, 'Blurryface', '2015', '1h 4m 20s', 14, 0, 1),
       (GETDATE(), NULL, 'from-seed', NULL, 'Night Visions', '2012', '1h 4m 26s', 13, 0, 2),
       (GETDATE(), NULL, 'from-seed', NULL, 'Smoke + Mirrors', '2015', '1h 6m 35s', 14, 0, 2),
       (GETDATE(), NULL, 'from-seed', NULL, 'Parachutes', '2000', '0h 41m 28s', 10, 0, 3),
       (GETDATE(), NULL, 'from-seed', NULL, 'A Rush of Blood to the Head', '2002', '1h 1m 39s', 11, 0, 3),
       (GETDATE(), NULL, 'from-seed', NULL, 'Memories...Do Not Open', '2017', '1h 1m 24s', 12, 0, 4),
       (GETDATE(), NULL, 'from-seed', NULL, 'Sick Boy', '2018', '0h 36m 15s', 10, 0, 4),
       (GETDATE(), NULL, 'from-seed', NULL, 'Songs About Jane', '2002', '1h 0m 33s', 12, 0, 5),
       (GETDATE(), NULL, 'from-seed', NULL, 'V', '2014', '0h 50m 16s', 11, 0, 5),
       (GETDATE(), NULL, 'from-seed', NULL, 'Beauty Behind the Madness', '2015', '1h 5m 10s', 14, 0, 6),
       (GETDATE(), NULL, 'from-seed', NULL, 'Starboy', '2016', '1h 8m 22s', 18, 0, 6),
       (GETDATE(), NULL, 'from-seed', NULL, '+', '2011', '0h 49m 4s', 12, 0, 7),
       (GETDATE(), NULL, 'from-seed', NULL, 'x', '2014', '0h 49m 47s', 12, 0, 7),
       (GETDATE(), NULL, 'from-seed', NULL, '19', '2008', '0h 43m 41s', 12, 0, 8),
       (GETDATE(), NULL, 'from-seed', NULL, '21', '2011', '0h 48m 12s', 11, 0, 8),
       (GETDATE(), NULL, 'from-seed', NULL, 'Take Care', '2011', '1h 20m 14s', 20, 0, 9),
       (GETDATE(), NULL, 'from-seed', NULL, 'Views', '2016', '1h 21m 13s', 20, 0, 9),
       (GETDATE(), NULL, 'from-seed', NULL, 'Dangerously in Love', '2003', '1h 0m 30s', 15, 0, 10),
       (GETDATE(), NULL, 'from-seed', NULL, 'Lemonade', '2016', '0h 45m 49s', 12, 0, 10);
GO