#! /bin/bash

/etc/init.d/postgresql start

psql --command "CREATE USER queuespy WITH SUPERUSER PASSWORD 'queuespy';"

createdb -O queuespy queuespy

for i in /sql/tables/*.sql
do
    psql --file=$i --dbname=queuespy
done

for i in /sql/static_data/*.sql
do
    psql --file=$i --dbname=queuespy
done

/etc/init.d/postgresql stop

/usr/lib/postgresql/9.3/bin/postgres -D /var/lib/postgresql/9.3/main -c config_file=/etc/postgresql/9.3/main/postgresql.conf

