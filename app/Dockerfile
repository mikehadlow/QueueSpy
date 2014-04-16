#DOCKER-VERSION 0.9.1

FROM    mikehadlow/ubuntu-monoxide-mono-devel
MAINTAINER  Mike Hadlow <mike@suteki.co.uk>

ADD     src src
ADD     docker_start.sh /src/

# import mozilla root certs so that NuGet can perform HTTPS
RUN     mozroots --import --sync

# restore NuGet packages
RUN     mono src/tools/NuGet.exe restore src/QueueSpy.sln

# compile
RUN     xbuild src/QueueSpy.sln

# when container starts run the startup script
EXPOSE  8080
CMD     /src/docker_start.sh
