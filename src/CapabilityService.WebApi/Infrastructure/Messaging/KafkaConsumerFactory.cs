using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Confluent.Kafka;
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

        public IConsumer<string, string> Create()
        {
            var config = new ConsumerConfig(_configuration.GetConsumerConfiguration());
            var builder = new ConsumerBuilder<string, string>(config);
            builder.SetErrorHandler(OnKafkaError);
            return builder.Build();
        }
        
        private void OnKafkaError(IConsumer<string, string> producer, Error error)
        {
            if (error.IsFatal)
                Environment.FailFast($"Fatal error in Kafka producer: {error.Reason}. Shutting down...");
            else
                throw new Exception(error.Reason);
        }
    }
}