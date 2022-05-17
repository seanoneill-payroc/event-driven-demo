# event-driven-demo

```
docker-compose up --build
```

seq log system at http://localhost:881

rabbitmq admin http://localhost:15672 

event producer will generate a random event every 15 seconds

logs will show the webhookmanagement service picking up this event and locating subscribers

# WIP

* webhookmanagement service will drop this notification job into a separate exchange
* webhookmanagement service will monitor event exchange for failed webhook jobs to determine and apply retry/abandon strategy
* webhook processor (n) will pick up notification jobs 
  * explore priority queue/routing - approach would be to have 1 processor only on *.p10, and a second that handles all nature of a message bus is that it is FIFO, but by providing a dedicated processor, in addition to the rest, this should provide reasonable coverage for priorty messages.
* service correlation tracking (leverage rabbit, correlation middleware) headers
* configurable scale (events produced, and consumer)
