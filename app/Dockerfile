#DOCKER-VERSION 0.9.1

FROM    mikehadlow/ubuntu-monoxide-mono-devel
MAINTAINER  Mike Hadlow <mike@suteki.co.uk>

RUN echo "deb http://archive.ubuntu.com/ubuntu precise main universe" > /etc/apt/sources.list
RUN apt-get update
RUN DEBIAN_FRONTEND=noninteractive apt-get install -y supervisor
RUN mkdir -p /var/log/supervisor

ADD     src src
ADD     docker_start.sh /src/
ADD     start_app.sh /src/

# import mozilla root certs so that NuGet can perform HTTPS
RUN     mozroots --import --sync

# restore NuGet packages
RUN     mono src/tools/NuGet.exe restore src/QueueSpy.sln

# compile
RUN     xbuild src/QueueSpy.sln

# when container starts run the startup script
EXPOSE  8080
CMD     /src/docker_start.sh
