DROP TABLE IF EXISTS "Consumer";

CREATE TABLE "Consumer" (
	Id				SERIAL PRIMARY KEY,
	ConnectionId	INT REFERENCES "Connection"(Id),
	Tag				VARCHAR(1024),
	QueueName		VARCHAR(1024),
	Created			TIMESTAMP,
	Cancelled		TIMESTAMP,
	IsConsuming		BOOLEAN
)