# Dockerfile to build Nginx installed containers
# Based on Ubuntu

FROM ubuntu:13.10
MAINTAINER  Mike Hadlow <mike@suteki.co.uk>

# Install Nginx
RUN echo "deb http://archive.ubuntu.com/ubuntu/ raring main universe" >> /etc/apt/sources.list
RUN apt-get update
RUN DEBIAN_FRONTEND=noninteractive apt-get install -y nginx

# Remove the default Nginx configuration file
RUN rm -v /etc/nginx/nginx.conf

# Add the nginx.conf from the current directory
ADD nginx.conf /etc/nginx/

# Append "daemon off;" to the beginning of the configuration. We DO NOT want Nginx to fork and exit.
RUN echo "daemon off;" >> /etc/nginx/nginx.conf

# Copy static content.
ADD sites sites

# Expose ports
EXPOSE  80

# Start Nginx when the container is started
CMD service nginx start
