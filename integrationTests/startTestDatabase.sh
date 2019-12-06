#!/bin/bash
# docker exec -it teddy_postgres_test_db psql -U teddy

background=''

while getopts "d" OPTION; do
  case "$OPTION" in
    d) background='-d' ;;
  esac
done

docker rm -f teddy_postgres_test_db;
docker run --name teddy_postgres_test_db $background -p 5433:5432 -v $(pwd)/../postgres/startup/CreateTables.sql:/docker-entrypoint-initdb.d/CreateTables.sql -e POSTGRES_USER=teddy -e POSTGRES_PASSWORD=teddy postgres