DROP TABLE IF EXISTS "VHost"; 

CREATE TABLE "VHost" ( 
    Id              SERIAL PRIMARY KEY,
    BrokerId	    INT REFERENCES "Broker"(Id),
    Name            VARCHAR(1024)
);