#!/bin/bash
echo QueueSpy CI Script

docker build -t mikehadlow/website dockerfiles/website/
docker stop nginx_1
docker rm nginx_1
docker run -d --name nginx_1 -p 80:80 mikehadlow/website
