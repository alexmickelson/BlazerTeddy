#!/bin/bash

cd integrationTests
./startTestDatabase.sh -d

cd ../test
dotnet test

cd ../integrationTests
dotnet test
