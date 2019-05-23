using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Confluent.Kafka;
using Confluent.Kafka.Serialization;
using Microsoft.Extensions.Configuration;

namespace DFDS.CapabilityService.WebApi.Infrastructure.Messaging
{
    public class KafkaConsumerFactory
    {
        private readonly KafkaConfiguration _configuration;

        public KafkaConsumerFactory(KafkaConfiguration configuration)
        {
            _configuration = configuration;
        }

        public Consumer<string, string> Create()
        {
            var config = _configuration.AsEnumerable().ToArray();
            
            return new Consumer<string, string>(
                config: config,
                keyDeserializer: new StringDeserializer(Encoding.UTF8),
                valueDeserializer: new StringDeserializer(Encoding.UTF8)
            );
        }
    }
}