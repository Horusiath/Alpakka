using System;
using System.Collections.Immutable;
using System.Linq;
using Akka.Actor;
using Akka.Configuration;
using Confluent.Kafka.Serialization;

namespace Akka.Streams.Kafka
{
    public sealed class ProducerSettings<TKey, TValue>
    {
        public static ProducerSettings<TKey, TValue> Create(ActorSystem system)
        {
            if (system == null) throw new ArgumentNullException(nameof(system));

            var keySerializer = system.Serialization.FindSerializerForType(typeof(TKey)).ToKafkaSerializer<TKey>();
            var valSerializer = system.Serialization.FindSerializerForType(typeof(TValue)).ToKafkaSerializer<TValue>();
            var config = system.Settings.Config.GetConfig("akka.kafka.producer");
            return Create(config, keySerializer, valSerializer);
        }

        public static ProducerSettings<TKey, TValue> Create(Config config, ISerializer<TKey> keySerializer,
            ISerializer<TValue> valueSerializer)
        {
            if (config == null) throw new ArgumentNullException(nameof(config), "Kafka config for Akka.NET producer was not provided");

            return new ProducerSettings<TKey, TValue>(
                keySerializer: keySerializer,
                valueSerializer: valueSerializer,
                closeTimeout: config.GetTimeSpan("close-timeout", TimeSpan.FromSeconds(60)),
                parallelism: config.GetInt("parallelism", 100),
                dispatcherId: config.GetString("use-dispatcher", "akka.kafka.default-dispatcher"),
                properties: ImmutableDictionary<string, object>.Empty);
        }

        public object this[string propertyKey] => this.Properties.GetValueOrDefault(propertyKey);

        public ISerializer<TKey> KeySerializer { get; }
        public ISerializer<TValue> ValueSerializer { get; }
        public TimeSpan CloseTimeout { get; }
        public int Parallelism { get; }
        public string DispatcherId { get; }
        public IImmutableDictionary<string, object> Properties { get; }

        public ProducerSettings(ISerializer<TKey> keySerializer, ISerializer<TValue> valueSerializer, TimeSpan closeTimeout, int parallelism, string dispatcherId, IImmutableDictionary<string, object> properties)
        {
            KeySerializer = keySerializer;
            ValueSerializer = valueSerializer;
            CloseTimeout = closeTimeout;
            Parallelism = parallelism;
            DispatcherId = dispatcherId;
            Properties = properties;
        }

        public ProducerSettings<TKey, TValue> WithBootstrapServers(string bootstrapServers) =>
            WithProperty(KafkaConfig.BoostrapServers, bootstrapServers);

        public ProducerSettings<TKey, TValue> WithProperty(string key, object value) =>
            Copy(properties: this.Properties.SetItem(key, value));

        public ProducerSettings<TKey, TValue> WithCloseTimeout(TimeSpan closeTimeout) =>
            Copy(closeTimeout: closeTimeout);

        public ProducerSettings<TKey, TValue> WithParallelism(int parallelism) =>
            Copy(parallelism: parallelism);

        public ProducerSettings<TKey, TValue> WithDispatcher(string dispatcherId) =>
            Copy(dispatcherId: dispatcherId);

        private ProducerSettings<TKey, TValue> Copy(
            ISerializer<TKey> keySerializer = null,
            ISerializer<TValue> valueSerializer = null,
            TimeSpan? closeTimeout = null,
            int? parallelism = null,
            string dispatcherId = null,
            IImmutableDictionary<string, object> properties = null) =>
            new ProducerSettings<TKey, TValue>(
                keySerializer: keySerializer ?? this.KeySerializer,
                valueSerializer: valueSerializer ?? this.ValueSerializer,
                closeTimeout: closeTimeout ?? this.CloseTimeout,
                parallelism: parallelism ?? this.Parallelism,
                dispatcherId: dispatcherId ?? this.DispatcherId,
                properties: properties ?? this.Properties);

        internal Confluent.Kafka.Producer<TKey, TValue> CreateKafkaProducer() => 
            new Confluent.Kafka.Producer<TKey,TValue>(this.Properties, this.KeySerializer, this.ValueSerializer);
    }
}