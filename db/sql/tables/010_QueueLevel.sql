DROP TABLE IF EXISTS "QueueLevel";

CREATE TABLE "QueueLevel" (
	Id				SERIAL PRIMARY KEY,
	QueueId			INT REFERENCES "Queue"(Id),
	Ready			BIGINT,
	Unacked			BIGINT,
	Total			BIGINT,
	SampledAt		TIMESTAMP
)