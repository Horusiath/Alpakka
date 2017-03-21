using Akka.Configuration;

namespace Akka.Streams.Kafka
{
    public class KafkaConfig
    {
        private KafkaConfig() { }

        public const string BoostrapServers = "bootstrap.servers";
        public const string ClientId = "client.id";
        public const string GroupId = "group.id";

        public static Config Default => 
            ConfigurationFactory.FromResource<KafkaConfig>("Akka.Streams.Kafka.reference.conf");
    }
}