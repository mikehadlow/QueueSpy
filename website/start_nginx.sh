#! /bin/bash
sed -i "s/app_ip_addr/$APP_PORT_8080_TCP_ADDR/g" /etc/nginx/nginx.conf
service nginx start
