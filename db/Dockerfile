# 
# Creates the QueueSpy Postgress database.
#
#

FROM nornagon/postgres
MAINTAINER mike@suteki.co.uk

ADD sql sql
ADD create_db.sh /src/

USER postgres

CMD /src/create_db.sh

