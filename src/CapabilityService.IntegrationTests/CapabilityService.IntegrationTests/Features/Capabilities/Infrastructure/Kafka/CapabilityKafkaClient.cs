using System;
using System.Collections.Generic;
using Confluent.Kafka;
using Newtonsoft.Json;

namespace CapabilityService.IntegrationTests.Features.Capabilities.Infrastructure.Kafka
{
    public class CapabilityKafkaClient
    {
        public List<dynamic> GetUntil(
            Func<dynamic, bool> weAreDone, 
            TimeSpan timeOutForNewEvents
        )
        {
            var consumerConfig = new ConsumerConfig
            {
                GroupId = "test-consumer-group-" + Guid.NewGuid(),
                BootstrapServers = "localhost:9092",
                AutoOffsetReset = AutoOffsetReset.Earliest,
            };
            
            using (var consumer = new ConsumerBuilder<Ignore, string>(consumerConfig).Build())
            {
                consumer.Subscribe("build.capabilities");

                try
                {
                    var cutOffTime = DateTime.Now.Add(timeOutForNewEvents);

                    var events = new List<dynamic>();

                    dynamic deserializeObject = null;
                    do
                    {
                        var consumeResult = consumer.Consume(TimeSpan.FromMilliseconds(1));
                        if (consumeResult != null)
                        {
                            deserializeObject = JsonConvert.DeserializeObject<dynamic>(consumeResult.Value);
                            events.Add(deserializeObject);
                            continue;
                        }


                        if (cutOffTime <= DateTime.Now)
                        {
                            throw new Exception(
                                $"No new events has been posted before the deadline of Hour:Minute:Seconds: {timeOutForNewEvents:c} ");
                        }
                    } while (weAreDone(deserializeObject) == false);

                    return events;
                }
                finally
                {
                    consumer.Close();
                }
            }
        }
    }
}
