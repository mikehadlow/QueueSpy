#! /bin/bash

# This script is run when the queuespy_app docker container starts

sed -i "s/host=localhost/host=$RABBIT_PORT_5672_TCP_ADDR/g" src/*/bin/Debug/*.config

mono src/QueueSpy.Api/bin/Debug/QueueSpy.Api.exe
