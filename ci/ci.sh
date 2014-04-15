#!/bin/bash
echo QueueSpy CI Script

docker build -t queuespy/website website/
docker build -t queuespy/app    app/

docker stop queuespy_rabbit
docker rm queuespy_rabbit
docker run -d 55672:55672 -e --name queuespy_rabbit RABBITMQ_PASS="i8rUx_32mn" tutum/rabbitmq

docker stop queuespy_app
docker rm queuespy_app
docker run -d --link queuespy_rabbit:rabbit --name queuespy_app queuespy/app

docker stop queuespy_website
docker rm queuespy_website
docker run -d --link queuespy_app:app --name queuespy_website -p 80:80 queuespy/website


