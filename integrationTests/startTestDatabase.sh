#!/bin/bash

docker rm -f teddy_postgres_test_db;
docker run -d --name teddy_postgres_test_db -p 5433:5432 -v $(pwd)/../postgres/startup/CreateTables.sql:/docker-entrypoint-initdb.d/CreateTables.sql -e POSTGRES_USER=teddy -e POSTGRES_PASSWORD=teddy postgres