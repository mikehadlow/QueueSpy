DROP TABLE IF EXISTS "BrokerEvent";

CREATE TABLE "BrokerEvent" (
	Id				SERIAL PRIMARY KEY,
	BrokerId		INT REFERENCES "Broker"(Id),
	EventTypeId		INT REFERENCES "EventType"(Id),
	Description		VARCHAR (1024),
	DateTimeUTC		TIMESTAMP
)