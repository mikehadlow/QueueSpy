# 
# Creates the QueueSpy Postgress database.
#
#

FROM nornagon/postgres
MAINTAINER mike@suteki.co.uk

ADD sql sql
ADD create_db.sh /src/

USER postgres

RUN /src/create_db.sh

CMD ["/usr/lib/postgresql/9.3/bin/postgres", "-D", "/var/lib/postgresql/9.3/main", "-c", "config_file=/etc/postgresql/9.3/main/postgresql.conf"]
