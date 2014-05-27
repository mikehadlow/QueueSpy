DELETE FROM "EventType";

INSERT INTO "EventType" (Id, Description) VALUES 
	(1, 'Broker contact established.'),
	(2, 'Broker contact lost.'),
	(3, 'Connection Established.'),
	(4, 'Connection Lost.'),
	(5, 'Consumer Cancelled'),
	(6, 'New Consumer'),
	(7, 'Queue Created'),
	(8, 'Queue Deleted');