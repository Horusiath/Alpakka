using System;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace Akka.Streams.Kafka.Tests
{
    public class ProducerSettingsSpec : Akka.TestKit.Xunit2.TestKit
    {
        public ProducerSettingsSpec(ITestOutputHelper output) : base(output: output)
        {
        }

        [Fact]
        public void ProducerSettings_should_have_default_properties_set()
        {
            var settings = ProducerSettings<string, string>.Create(Sys);
            settings.Parallelism.Should().Be(100);
            settings.CloseTimeout.Should().Be(TimeSpan.FromSeconds(60));
            settings.DispatcherId.Should().Be("akka.kafka.default-dispatcher");
        }

        [Fact]
        public void ProducerSettings_should_handle_serializers_defined_in_config()
        {
            var settings = ConsumerSettings<string, string>.Create(Sys);

        }
    }
}