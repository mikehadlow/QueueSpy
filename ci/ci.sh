#!/bin/bash
#
# This script is run by the CI process after all source has been pulled from GitHub 
# See node-ci.js & ci-bootstrapper.sh
#
# It builds and runs the QueueSpy application's Docker images.
#

echo QueueSpy CI Script

# build application containers
docker build -t queuespy/website --no-cache=true website/
docker build -t queuespy/app --no-cache=true app/
docker build -t queuespy/db --no-cache=true db/

# stop and remove containers in correct dependency order
docker stop queuespy_logs
docker rm queuespy_logs

docker stop queuespy_website
docker rm queuespy_website

docker stop queuespy_app
docker rm queuespy_app

docker stop queuespy_rabbit
docker rm queuespy_rabbit

docker stop queuespy_db
docker rm queuespy_db

# remove all untagged images
docker rmi $( docker images | grep '<none>' | tr -s ' ' | cut -d ' ' -f 3)

# start the log container
docker run -d -v /home/mike/logs:/logs --name queuespy_logs ubuntu echo "starting log container"

# start the Postgres DB container
docker run -d --name queuespy_db queuespy/db

# start the RabbitMQ container
docker run -d -p 15672:15672 --name queuespy_rabbit -e RABBITMQ_PASS="i8rUx_32mn" tutum/rabbitmq

# start the application container
docker run -d --volumes-from queuespy_logs --link queuespy_rabbit:rabbit --link queuespy_db:db --name queuespy_app -p 9001:9001 queuespy/app

# start the website container
docker run -d --volumes-from queuespy_logs --link queuespy_app:app --name queuespy_website -p 80:80 -p 443:443 queuespy/website


