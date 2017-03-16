using Akka.Actor;
using Confluent.Kafka;
using Xunit.Abstractions;

namespace Akka.Streams.Kafka.Tests.Internals
{
    public class ConsumerSpec : Akka.TestKit.Xunit2.TestKit
    {
        private static CommittableMessage<string, string> CreateMessage(int seed, string topic = "topic",
            string group = "group1")
        {
            var offset = new PartitionOffset(new PartitionId(group, topic, 1), seed);
            var record = new Message<string, string>(offset.Key.Topic, offset.Key.Partition, offset.Offset, seed.ToString(), seed.ToString(), default(Timestamp), null);
            return new CommittableMessage<string, string>(offset, record);
        }

        public ConsumerSpec(ITestOutputHelper output) : base(output: output)
        {
        }
    }
}