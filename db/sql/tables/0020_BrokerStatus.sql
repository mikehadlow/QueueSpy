DROP TABLE IF EXISTS "BrokerStatus";

CREATE TABLE "BrokerStatus" (
	Id				SERIAL PRIMARY KEY,
	BrokerId		INT REFERENCES "Broker"(Id),
	ContactOK		BOOLEAN,
	StatusText		VARCHAR (1024)
);
