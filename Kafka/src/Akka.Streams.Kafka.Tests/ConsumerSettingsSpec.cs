using System;
using Akka.Actor;
using Akka.Configuration;
using FluentAssertions;
using Xunit;

namespace Akka.Streams.Kafka.Tests
{
    public class ConsumerSettingsSpec : Akka.TestKit.Xunit2.TestKit
    {
        [Fact]
        public void ConsumerSettings_should_have_default_properties_set()
        {
            using (var system = ActorSystem.Create(nameof(ProducerSettingsSpec), KafkaConfig.Default))
            {
                var settings = ConsumerSettings<string, string>.Create(system);
                settings.CloseTimeout.Should().Be(TimeSpan.FromSeconds(20));
                settings.CommitTimeout.Should().Be(TimeSpan.FromSeconds(15));
                settings.PollInterval.Should().Be(TimeSpan.FromMilliseconds(50));
                settings.PollTimeout.Should().Be(TimeSpan.FromMilliseconds(50));
                settings.StopTimeout.Should().Be(TimeSpan.FromSeconds(30));
                settings.WakeUpTimeout.Should().Be(TimeSpan.FromSeconds(3));
                settings.DispatcherId.Should().Be("akka.kafka.default-dispatcher");
                settings.MaxWakeUps.Should().Be(10);
                settings.KeyDeserializer.Should().NotBeNull();
                settings.ValueDeserializer.Should().NotBeNull();
                settings.Properties.Should().NotBeNull();
            }
        }

        [Fact]
        public void ConsumerSettings_should_handle_nested_kafka_clients_properties()
        {
            var config = ConfigurationFactory.ParseString(@"
                akka.kafka.consumer {
                }")
                .WithFallback(KafkaConfig.Default);
            using (var system = ActorSystem.Create(nameof(ProducerSettingsSpec), config))
            {
                throw new NotImplementedException();
            }
        }

        [Fact]
        public void ConsumerSettings_should_handle_deserializers_defined_in_config()
        {
            var config = ConfigurationFactory.ParseString(@"
                akka.kafka.consumer {
                }")
                .WithFallback(KafkaConfig.Default);
            using (var system = ActorSystem.Create(nameof(ProducerSettingsSpec), config))
            {
                throw new NotImplementedException();
            }
        }
    }
}