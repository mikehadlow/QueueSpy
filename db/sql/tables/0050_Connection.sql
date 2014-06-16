
DROP TABLE IF EXISTS "Connection";

CREATE TABLE "Connection" (
	Id				SERIAL PRIMARY KEY,
	VHostId 		INT REFERENCES "VHost"(Id),
	Name			VARCHAR(1024),
	Connected		TIMESTAMP,
	Disconnected	TIMESTAMP,
	IsConnected		BOOLEAN
)
