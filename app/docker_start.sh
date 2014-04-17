#! /bin/bash

# This script is run when the queuespy_app docker container starts

sed -i "s/host=localhost/host=$RABBIT_PORT_5672_TCP_ADDR;username=admin;password=i8rUx_32mn/g" src/*/bin/Debug/*.config

sed -i "s/Server=127.0.0.1/Server=$DB_PORT_5432_TCP_ADDR/g" src/*/bin/Debug/*.config

supervisord -n -c src/supervisord.conf
