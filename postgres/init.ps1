
docker rm -f teddy_postgres;
docker run --name teddy_postgres -p 5432:5432 -v ${pwd}\startup:/docker-entrypoint-initdb.d/ -e POSTGRES_DB=teddy  -e POSTGRES_USER=teddy -e POSTGRES_PASSWORD=teddy -e POSTGRES_DB=teddy postgres 