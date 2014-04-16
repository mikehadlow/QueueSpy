#! /bin/bash

# This script is run when the queuespy_app docker container starts

sed -i "s/host=localhost/host=$RABBIT_PORT_5672_TCP_ADDR;username=admin;password=i8rUx_32mn/g" src/*/bin/Debug/*.config

supervisord -n -c src/supervisord.conf
