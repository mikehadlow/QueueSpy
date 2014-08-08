DROP TABLE IF EXISTS "Alert";

CREATE TABLE "Alert" (
    Id              SERIAL PRIMARY KEY,
    BrokerId        INT REFERENCES "Broker"(Id),
    AlertTypeId     INT REFERENCES "AlertType"(Id),
    Description     VARCHAR (1024),
    DateTimeUTC     TIMESTAMP
);
