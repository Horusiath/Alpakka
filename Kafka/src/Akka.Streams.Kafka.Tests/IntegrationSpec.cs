using System;
using Akka.Actor;
using Xunit;
using Xunit.Abstractions;

namespace Akka.Streams.Kafka.Tests
{
    public class IntegrationSpec : Akka.TestKit.Xunit2.TestKit
    {
        private const int Partition = 0;
        private readonly string uid = Guid.NewGuid().ToString("N");
        private readonly ActorMaterializer materializer;
        private readonly ProducerSettings<byte[], byte[]> producerSettings;
        private readonly string bootstrapServers = "localhost:8300";

        public IntegrationSpec(ITestOutputHelper output) : base(output: output)
        {
            materializer = Sys.Materializer();
            producerSettings = ProducerSettings<byte[], byte[]>.Create(Sys);
        }

        [Fact]
        public void Kafka_streams_should_produce_to_standard_sink_and_consume_standard_source()
        {
            throw new NotImplementedException();
        }

        [Fact]
        public void Kafka_streams_should_resume_consumer_from_committed_offset()
        {
            throw new NotImplementedException();
        }

        [Fact]
        public void Kafka_streams_should_handle_commit_without_demand()
        {
            throw new NotImplementedException();
        }

        [Fact]
        public void Kafka_streams_should_consume_and_commit_in_batches()
        {
            throw new NotImplementedException();
        }

        [Fact]
        public void Kafka_streams_should_connect_producet_to_consumer_and_commit_in_batches()
        {
            throw new NotImplementedException();
        }

        private string CreateTopic(int n) => $"topic{n}-{uid}";
        private string CreateGroup(int n) => $"group{n}-{uid}";
    }
}