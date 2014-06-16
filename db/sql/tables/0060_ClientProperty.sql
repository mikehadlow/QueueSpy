
DROP TABLE IF EXISTS "ClientProperty";

CREATE TABLE "ClientProperty" (
	Id				SERIAL PRIMARY KEY,
	ConnectionId	INT REFERENCES "Connection"(Id),
	Key				VARCHAR(1024),
	Value			VARCHAR(1024)
)