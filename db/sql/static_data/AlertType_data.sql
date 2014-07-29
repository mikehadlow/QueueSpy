﻿DELETE FROM "AlertType";

INSERT INTO "AlertType" (Id, Description) VALUES 
	(1, 'Broker contact established.'),
	(2, 'Broker contact lost.'),
	(3, 'Connection Established.'),
	(4, 'Connection Lost.'),
	(5, 'Consumer Cancelled'),
	(6, 'New Consumer'),
	(7, 'Queue Created'),
	(8, 'Queue Deleted'),
	(9, 'VHost Deleted'),
	(10, 'VHost Created');