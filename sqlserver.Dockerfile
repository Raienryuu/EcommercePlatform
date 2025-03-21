FROM mcr.microsoft.com/mssql/server:2022-CU12-GDR1-ubuntu-22.04

ENV ACCEPT_EULA=Y
ENTRYPOINT ["/opt/mssql/bin/permissions_check.sh"]
COPY ["prepareDb.sql", "prepareDb.sql"]

RUN --mount=type=secret,id=sqlpassword,uid=10001 \
  export MSSQL_SA_PASSWORD=$(cat /run/secrets/sqlpassword) \
  && ("/opt/mssql/bin/sqlservr" &) \
  && echo "Dbpassword $MSSQL_SA_PASSWORD" \
  && sleep 8 \
  &&  /opt/mssql-tools/bin/sqlcmd -U SA -P "$MSSQL_SA_PASSWORD" -i /prepareDb.sql

CMD [ "/opt/mssql/bin/sqlservr" ]
