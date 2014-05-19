
DROP TABLE IF EXISTS "Connection";

CREATE TABLE "Connection" (
	Id				SERIAL PRIMARY KEY,
	BrokerId		INT REFERENCES "Broker"(Id),
	Name			VARCHAR(1024),
	Connected		TIMESTAMP,
	Disconnected	TIMESTAMP,
	IsConnected		BOOLEAN
)