using System;
using Akka.Actor;
using Akka.Configuration;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace Akka.Streams.Kafka.Tests
{
    public class ProducerSettingsSpec
    {
        public ProducerSettingsSpec(ITestOutputHelper output)
        {
        }

        [Fact]
        public void ProducerSettings_should_have_default_properties_set()
        {
            using (var system = ActorSystem.Create(nameof(ProducerSettingsSpec), KafkaConfig.Default))
            {
                var settings = ProducerSettings<string, string>.Create(system);
                settings.Parallelism.Should().Be(100);
                settings.CloseTimeout.Should().Be(TimeSpan.FromSeconds(60));
                settings.DispatcherId.Should().Be("akka.kafka.default-dispatcher");
                settings.KeySerializer.Should().NotBeNull();
                settings.ValueSerializer.Should().NotBeNull();
                settings.Properties.Should().NotBeNull();
            }
        }

        [Fact]
        public void ProducerSettings_should_handle_serializers_defined_in_config()
        {
            var config = ConfigurationFactory.ParseString(@"
                akka.kafka.producer {
                }")
                .WithFallback(KafkaConfig.Default);
            using (var system = ActorSystem.Create(nameof(ProducerSettingsSpec), config))
            {
                throw new NotImplementedException();
            }
        }
    }
}