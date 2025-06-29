#!/bin/bash

echo "Running postCreateCommand.sh......................................................................................................."

# Setting variables
dacpac="false"

# Load SA_PASSWORD from .env file
export $(grep -v '^#' .devcontainer/.env | xargs)
SApassword=$MSSQL_SA_PASSWORD

# Parameters
dacpath=$1

# Extract the project directory from the dacpath
projectDir=$(echo $dacpath | cut -d'/' -f1)

echo "SELECT * FROM SYS.DATABASES" | dd of=testsqlconnection.sql
for i in {1..30};
do
    sqlcmd -S localhost -U sa -P $SApassword -d master -i testsqlconnection.sql > /dev/null
    if [ $? -eq 0 ]
    then
        echo "SQL server ready"
        break
    else
        echo "Not ready yet..."
        sleep 1
    fi
done

echo "Running testsqlconnection.sql......................................................................................................."
echo "$dacpath"
rm testsqlconnection.sql

for f in $dacpath/*
do
    if [ $f == $dacpath/*".dacpac" ]
    then
        dacpac="true"
        echo "Found dacpac $f"
    else
        dotnet build /workspace/database/Todo.sqlproj
        dacpac="true"
    fi
done

echo "Dacpac found: $dacpac"

if [ $dacpac == "true" ] 
then
    # Build the SQL Database project
    echo "Run Sqlcmd........................................."
    for f in $dacpath/*
    do
        if [ $f == $dacpath/*".dacpac" ]
        then
            dbname=$(basename $f ".dacpac")
            # Deploy the dacpac
            echo "Deploying dacpac $f"
            /opt/sqlpackage/sqlpackage /Action:Publish /SourceFile:$f /TargetServerName:localhost /TargetDatabaseName:$dbname /TargetUser:sa /TargetPassword:$SApassword /TargetTrustServerCertificate:True

            echo "Running postDeployment.sql..."
            sqlcmd -S localhost -d $dbname -U sa -P $SApassword -i /workspace/database/postDeployment.sql
        fi
    done
fi

dotnet dev-certs https --trust