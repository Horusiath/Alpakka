using System;
using Akka.Serialization;
using Confluent.Kafka.Serialization;

namespace Akka.Streams.Kafka
{
    internal sealed class KafkaSerializer<T> : ISerializer<T>, IDeserializer<T>
    {
        private readonly Serializer inner;

        public KafkaSerializer(Serializer inner)
        {
            if (inner == null) throw new ArgumentNullException(nameof(inner), "Valid serializer for kafka was not found");
            this.inner = inner;
        }

        public byte[] Serialize(T data) => this.inner.ToBinary(data);
        public T Deserialize(byte[] data) => (T)this.inner.FromBinary(data, typeof(T));
    }

    public static class KafkaExtensions
    {
        internal static ISerializer<T> ToKafkaSerializer<T>(this Serializer serializer) => new KafkaSerializer<T>(serializer);
        internal static IDeserializer<T> ToKafkaDeserializer<T>(this Serializer serializer) => new KafkaSerializer<T>(serializer);
    }
}