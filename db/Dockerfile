FROM mcr.microsoft.com/mssql/server:2022-latest

COPY ./run.sh /run.sh
COPY ./role-seeds.sql /role-seeds.sql
COPY ./users-seeds.sql /users-seeds.sql
COPY ./artists-seeds.sql /artists-seeds.sql
COPY ./albums-seeds.sql /albums-seeds.sql
COPY ./songs-seeds.sql /songs-seeds.sql

CMD /run.sh