-- set the broker Id that you want to delete here first.
set @brokerId = 10000;

delete from "QueueLevel" where QueueId in (select QueueId from "Queue" where BrokerId = @brokerid);
delete from "Queue" where BrokerId = @brokerId;
delete from "Consumer" where ConnectionId in (select ConnectionId from "Connection" where BrokerId = @brokerId);
delete from "ClientProperty" where ConnectionId in (select ConnectionId from "Connection" where BrokerId = @brokerId);
delete from "Connection" where BrokerId = @brokerId;
delete from "BrokerEvent" where BrokerId = @brokerId;
delete from "BrokerStatus" where BrokerId = @brokerId;
delete from "Broker" where id = @brokerId;