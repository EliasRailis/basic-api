﻿USE [haefele_db]
GO

INSERT INTO [dbo].[users]
( [created]
, [last_modified]
, [created_by]
, [last_modified_by]
, [first_name]
, [last_name]
, [email]
, [password_hash]
, [fk_role_id]
, [is_active]
, [is_deleted])
VALUES (GETDATE(),
        GETDATE(),
        'from-seed',
        'from-seed',
        'Elias',
        'Railis',
        'eli@gmail.com',
        '$2a$11$EkHZekDau77vkvzGFocxD.Rx1Z.RMGBC7.SFaSsU9Wf1xHhN3RxEe',
        1,
        1,
        0),
       (GETDATE(),
        GETDATE(),
        'from-seed',
        'from-seed',
        'Joe',
        'Doe',
        'joe@gmail.com',
        '$2a$11$qhMQNeY.tqA1UmjwGkh5ZO0vy8mueObnk4MOlUiFdVaeEuWDaoEZe',
        2,
        1,
        0)
GO