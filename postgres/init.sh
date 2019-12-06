#!/bin/bash

background=''

while getopts "d" OPTION; do
  case "$OPTION" in
    d) background='-d' ;;
  esac
done

docker rm -f teddy_postgres;
docker run --name teddy_postgres -p 5432:5432 $(echo $background) -v $(pwd)/startup:/docker-entrypoint-initdb.d/ -e POSTGRES_USER=teddy -e POSTGRES_PASSWORD=teddy -e POSTGRES_DB=teddy postgres