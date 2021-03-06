#! /bin/bash
# setup local development web server
# assumes you have nginx installed
# assumes it is being run from its location in the root of the QueueSpy repository

# create a new temp location for nginx.conf
mkdir -p /tmp/tmp_nginx_conf

# copy over the local nginx configuration. This location is correct for nginx installed with homebrew on OSX. You may have to change it for your setup.
cp /usr/local/etc/nginx/* /tmp/tmp_nginx_conf/

# copy over the copy of nginx.conf from the repo
cp $(pwd)/website/nginx.dev.conf /tmp/tmp_nginx_conf/nginx.conf

# replace the API placeholder location with localhost. So we can run up the mono API locally and have it proxied successfully via nginx
sed -i '' "s/app_ip_addr/localhost/g" /tmp/tmp_nginx_conf/nginx.conf

# replace the location used in the docker container with the current source repo location
sed -i '' "s#/sites/#$(pwd)/website/sites/#g" /tmp/tmp_nginx_conf/nginx.conf

# make nginx run as a user process
echo 'master_process off;' >> /tmp/tmp_nginx_conf/nginx.conf
echo 'daemon off;' >> /tmp/tmp_nginx_conf/nginx.conf

# run nginx with the updated temp configuration
nginx -c /tmp/tmp_nginx_conf/nginx.conf

