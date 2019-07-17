using System;
using System.Linq;
using System.Text;
using Confluent.Kafka;

namespace DFDS.CapabilityService.WebApi.Infrastructure.Messaging
{
    public class KafkaPublisherFactory
    {
        private readonly KafkaConfiguration _configuration;

        public KafkaPublisherFactory(KafkaConfiguration configuration)
        {
            _configuration = configuration;
        }
        
        private void OnKafkaError(IProducer<string, string> producer, Error error)
        {
            if (error.IsFatal)
                Environment.FailFast($"Fatal error in Kafka producer: {error.Reason}. Shutting down...");
            else
                throw new Exception(error.Reason);
        }

        public IProducer<string, string> Create()
        {
            var config = new ProducerConfig(_configuration.GetProducerConfiguration());
            var builder = new ProducerBuilder<string, string>(config);
            builder.SetErrorHandler(OnKafkaError);
            return builder.Build();
        }

 
    }
}