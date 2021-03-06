DROP TABLE IF EXISTS "Broker";

CREATE TABLE "Broker" (
	Id				SERIAL PRIMARY KEY,
	UserId			INT REFERENCES "User"(Id),
	Url				VARCHAR(1024),
	Username		VARCHAR(1024),
	Password		VARCHAR(1024),
	Active			BOOLEAN,
    ContactOK       BOOLEAN
);
