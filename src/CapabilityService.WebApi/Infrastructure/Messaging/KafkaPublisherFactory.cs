using System.Linq;
using System.Text;
using Confluent.Kafka;
using Confluent.Kafka.Serialization;

namespace DFDS.CapabilityService.WebApi.Infrastructure.Messaging
{
    public class KafkaPublisherFactory
    {
        private readonly KafkaConfiguration _configuration;

        public KafkaPublisherFactory(KafkaConfiguration configuration)
        {
            _configuration = configuration;
        }

        public Producer<string, string> Create()
        {
            var config = _configuration.AsEnumerable().ToArray();
            
            return new Producer<string, string>(
                config: config,
                keySerializer: new StringSerializer(Encoding.UTF8),
                valueSerializer: new StringSerializer(Encoding.UTF8)
            );
        }

 
    }
}