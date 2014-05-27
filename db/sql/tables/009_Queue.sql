DROP TABLE IF EXISTS "Queue";

CREATE TABLE "Queue" (
	Id				SERIAL PRIMARY KEY,
	BrokerId		INT REFERENCES "Broker"(Id),
	Name			VARCHAR(1024),
	Created			TIMESTAMP,
	Deleted			TIMESTAMP,
	IsCurrent		BOOLEAN
)
