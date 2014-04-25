#! /bin/bash

psql --command "CREATE USER queuespy WITH SUPERUSER PASSWORD 'queuespy';"

createdb -O queuespy queuespy

for i in sql/tables/*.sql
do
    psql --file=$i --dbname=queuespy
done

for i in sql/static_data/*.sql
do
    psql --file=$i --dbname=queuespy
done
