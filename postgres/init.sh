#!/bin/bash

docker run -d --name teddy_postgres -p 5432:5432 -v $(pwd)/startup:/docker-entrypoint-initdb.d/ -e POSTGRES_USER=teddy -e POSTGRES_PASSWORD=teddy postgres